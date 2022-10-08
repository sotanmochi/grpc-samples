using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Grpc.Core;
using Google.Protobuf;
using FileTransferApp.Shared;

namespace FileTransferApp.Server.Services;

public class FileTransferService : FileTransfer.FileTransferBase
{
    private const int ChunkSize = 1024 * 32;
    
    private readonly IConfiguration _config;
    private readonly ILogger<FileTransferService> _logger;
    private readonly string _uploadedFilesDirectory;
    private readonly byte[] _buffer;
    
    public FileTransferService(IConfiguration config, ILogger<FileTransferService> logger)
    {
        _config = config;
        _logger = logger;
        _uploadedFilesDirectory = _config.GetValue<string>("UploadedFilesDirectory");
        _buffer = new byte[ChunkSize];
    }
    
    public override async Task DownloadFile
    (
        DownloadRequest request,
        IServerStreamWriter<DownloadResponse> responseStream,
        ServerCallContext context
    )
    {
        var fileId = request.FileId;

        try
        {
            var jsonString = await File.ReadAllTextAsync(Path.Combine(_uploadedFilesDirectory, fileId, "metadata.json"));
            var metaData = JsonSerializer.Deserialize<MetaData>(jsonString);
            
            await using var fileStream = File.OpenRead(Path.Combine(_uploadedFilesDirectory, fileId, "data.bin"));
            
            await responseStream.WriteAsync(new DownloadResponse
            {
                Metadata = new MetaData { FileName = metaData.FileName, Compression = metaData.Compression }
            });
            
            while (true)
            {
                var numBytesRead = await fileStream.ReadAsync(_buffer);
                if (numBytesRead == 0)
                {
                    break;
                }
                
                _logger.LogInformation($"Sending data chunk of {numBytesRead} bytes.", numBytesRead);
                
                await responseStream.WriteAsync(new DownloadResponse 
                {
                    Data = UnsafeByteOperations.UnsafeWrap(_buffer.AsMemory(0, numBytesRead))
                });
            }
        }
        catch (Exception exception)
        {
            await responseStream.WriteAsync(new DownloadResponse
            {
                HasError = true,
                ResponseMessage = exception.GetType().ToString(),
                Metadata = new MetaData { FileName = fileId }
            });
            
            _logger.LogError(exception.ToString());
        }
    }
    
    public override async Task<UploadResponse> UploadFile
    (
        IAsyncStreamReader<UploadRequest> requestStream,
        ServerCallContext context
    )
    {
        var response = new UploadResponse();
        
        var tempDirectory = Path.Combine(_uploadedFilesDirectory, Path.GetRandomFileName());
        var tempDirectoryInfo = Directory.CreateDirectory(tempDirectory);
        
        try
        {
            var fileHashString = string.Empty;
            
            await using (var fileStream = File.Create(Path.Combine(tempDirectory, "data.bin")))
            {
                await foreach (var message in requestStream.ReadAllAsync())
                {
                    if (message.Metadata != null)
                    {
                        var jsonString = JsonSerializer.Serialize(message.Metadata);
                        await File.WriteAllTextAsync(Path.Combine(tempDirectory, "metadata.json"), jsonString);
                    }
                    if (message.Data != null)
                    {
                        await fileStream.WriteAsync(message.Data.Memory);
                    }
                }
                
                fileStream.Position = 0;
                var hashValue = SHA1.Create().ComputeHash(fileStream);
                
                fileHashString = BitConverter.ToString(hashValue).Replace("-", string.Empty);
            }
            
            if (!string.IsNullOrEmpty(fileHashString))
            {
                var saveDirectory = Path.Combine(_uploadedFilesDirectory, fileHashString);
                Directory.CreateDirectory(saveDirectory);
                
                foreach (var file in tempDirectoryInfo.GetFiles())
                {
                    file.MoveTo(Path.Combine(saveDirectory, file.Name), true);
                }

                response.FileId = fileHashString;
                _logger.LogInformation($"Uploaded file id: '{fileHashString}'.");
            }
        }
        catch (Exception exception)
        {
            response.HasError = true;
            response.ResponseMessage = exception.GetType().ToString();
            _logger.LogError(exception.ToString());
        }
        finally
        {
            Directory.Delete(tempDirectory, true);
        }
        
        return response;
    }
    
    private void Log(string message)
    {
        _logger.LogInformation(message);
    }
    
    private void LogError(Exception e, string message)
    {
        _logger.LogError(e, message);
    }
}