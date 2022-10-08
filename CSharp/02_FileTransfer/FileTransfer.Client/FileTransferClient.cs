#if ENABLE_MONO || ENABLE_IL2CPP
#define UNITY_ENGINE
#endif

using System;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using FileTransferApp.Shared;

namespace FileTransferApp.Client
{
public class FileTransferClient
{
    private const int ChunkSize = 1024 * 32;
    private readonly FileTransfer.FileTransferClient _fileTransferClient;
    private readonly byte[] _buffer;

    public FileTransferClient(ChannelBase channel)
    {
        _fileTransferClient = new FileTransfer.FileTransferClient(channel);
        _buffer = new byte[ChunkSize];
    }

    public async Task<FileInfo> DownloadFile(string fileId, string downloadsFolder)
    {
        FileInfo downloadedFileInfo = null;
        
        var tempDirectory = Path.Combine(downloadsFolder, $"{fileId}.tmp");
        Directory.CreateDirectory(tempDirectory);
        
        try
        {
            var fileName = fileId;
            var compression = false;
            var binaryFilePath = Path.Combine(tempDirectory, "data.bin");
            
            using var streamingCall = _fileTransferClient.DownloadFile(new DownloadRequest()
            {
                FileId = fileId,
            });
            
            await using var fileStream = File.Create(binaryFilePath);
            
            await foreach (var message in streamingCall.ResponseStream.ReadAllAsync())
            {
                if (message.HasError && !string.IsNullOrEmpty(message.ResponseMessage))
                {
                    throw new FileTransferRequestException($"'{message.ResponseMessage}' occurred on the server while downloading the file '{fileId}'.");
                }
                
                if (message.Metadata != null)
                {
                    fileName = message.Metadata.FileName;
                    compression = message.Metadata.Compression;
                    
                    Log("Saving metadata to temporary file.");
                    var jsonString = JsonSerializer.Serialize(message.Metadata);
                    await File.WriteAllTextAsync(Path.Combine(tempDirectory, "metadata.json"), jsonString);
                }
                
                if (message.Data != null)
                {
                    var bytes = message.Data.Memory;
                    Log($"Saving {bytes.Length} bytes to temporary file.");
                    await fileStream.WriteAsync(bytes);
                }
            }
            
            var saveFilePath = Path.Combine(downloadsFolder, fileName);
            
            if (compression)
            {
                Log($"Saving decompressed file.");
                fileStream.Position = 0;
                await using var decompressedFileStream = File.Create(saveFilePath);
                await using var decompressor = new DeflateStream(fileStream, CompressionMode.Decompress);
                await decompressor.CopyToAsync(decompressedFileStream);
            }
            else
            {
                fileStream.Close();
                Log($"Saving file.");
                File.Move(binaryFilePath, saveFilePath, true);
            }
            
            downloadedFileInfo = new FileInfo(saveFilePath);
            Log($"Downloaded file: '{saveFilePath}'.");
        }
        catch (Exception exception)
        {
            LogError($"{exception}\n");
        }
        finally
        {
            Directory.Delete(tempDirectory, true);            
        }

        return downloadedFileInfo;
    }
    
    public async Task<string> UploadFile(string uploadFilePath, bool compression)
    {
        var fileId = string.Empty;

        var tempDirectory = Path.Combine(Path.GetDirectoryName(uploadFilePath), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDirectory);
        
        try
        {
            var streamingCall = _fileTransferClient.UploadFile();
            
            var metadata = new MetaData()
            {
                FileName = Path.GetFileName(uploadFilePath),
                Compression = compression,
            };
            
            Log("Sending file metadata.");
            await streamingCall.RequestStream.WriteAsync(new UploadRequest(){ Metadata = metadata });
            
            if (compression)
            {
                var compressedFilePath = Path.Combine(tempDirectory, "compressed.bin");
                
                await using var sourceFileStream = File.OpenRead(uploadFilePath);
                await using var compressedFileStream = File.Create(compressedFilePath);
                await using var compressor = new DeflateStream(compressedFileStream, CompressionMode.Compress);
                await sourceFileStream.CopyToAsync(compressor);
                
                uploadFilePath = Path.Combine(compressedFilePath);
            }
            
            await using (var readStream = File.OpenRead(uploadFilePath))
            {
                while (true)
                {
                    var count = await readStream.ReadAsync(_buffer);
                    if (count == 0)
                    {
                        break;
                    }
                    
                    Log($"Sending file data chunk of length {count}.");
                    await streamingCall.RequestStream.WriteAsync(new UploadRequest()
                    {
                        Data = UnsafeByteOperations.UnsafeWrap(_buffer.AsMemory(0, count))
                    });
                }
            }
            
            await streamingCall.RequestStream.CompleteAsync();
            Log("Complete request.");
            
            var response = await streamingCall;
            if (response.HasError && !string.IsNullOrEmpty(response.ResponseMessage))
            {
                throw new FileTransferRequestException($"'{response.ResponseMessage}' occurred on the server while uploading a file.");
            }

            fileId = response.FileId;
            Log($"Uploaded file id: '{fileId}'.");
        }
        catch (Exception exception)
        {
            fileId = null;
            LogError($"{exception}\n");
        }
        finally
        {
            Directory.Delete(tempDirectory, true);
        }
        
        return fileId;
    }
    
    [
        System.Diagnostics.Conditional("DEVELOPMENT_BUILD"), 
        System.Diagnostics.Conditional("UNITY_EDITOR"),
    ]
    private void Log(object message)
    {
#if UNITY_ENGINE
        UnityEngine.Debug.Log($"[FileTransferClient] {message}");
#else
        Console.WriteLine($"[FileTransferClient] {message}");
#endif
    }
    
    private void LogError(object message)
    {
#if UNITY_ENGINE
        UnityEngine.Debug.LogError($"[FileTransferClient] {message}");
#else
        Console.WriteLine($"[ERROR][FileTransferClient] {message}");
#endif
    }
}

public class FileTransferRequestException : Exception
{
    public FileTransferRequestException(){}
    public FileTransferRequestException(string message) : base(message){}
    public FileTransferRequestException(string message, Exception inner) : base(message, inner){}
}
}