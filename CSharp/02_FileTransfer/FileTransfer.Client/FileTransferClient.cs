#if ENABLE_MONO || ENABLE_IL2CPP
#define UNITY_ENGINE
#endif

using System;
using System.IO;
using System.IO.Compression;
#if !UNITY_ENGINE
using System.Text.Json;
#else
using Newtonsoft.Json;
using System.Threading;
#endif
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

#if !UNITY_ENGINE
            await using var fileStream = File.Create(binaryFilePath);
            await foreach (var response in streamingCall.ResponseStream.ReadAllAsync())
            {
#else
            using var fileStream = File.Create(binaryFilePath);
            while (await streamingCall.ResponseStream.MoveNext(CancellationToken.None))
            {
                var response = streamingCall.ResponseStream.Current;
#endif
                if (response.HasError && !string.IsNullOrEmpty(response.ResponseMessage))
                {
                    throw new FileTransferRequestException($"'{response.ResponseMessage}' occurred on the server while downloading the file '{fileId}'.");
                }
                
                if (response.Metadata != null)
                {
                    fileName = response.Metadata.FileName;
                    compression = response.Metadata.Compression;
                    
                    Log("Saving metadata to temporary file.");
#if !UNITY_ENGINE
                    var jsonString = JsonSerializer.Serialize(response.Metadata);
                    await File.WriteAllTextAsync(Path.Combine(tempDirectory, "metadata.json"), jsonString);
#else
                    var jsonString = JsonConvert.SerializeObject(response.Metadata);
                    File.WriteAllText(Path.Combine(tempDirectory, "metadata.json"), jsonString);
#endif
                }
                
                if (response.Data != null)
                {
                    var bytes = response.Data.Memory;
                    Log($"Saving {bytes.Length} bytes to temporary file.");
#if !UNITY_ENGINE
                    await fileStream.WriteAsync(bytes);
#else
                    await fileStream.WriteAsync(bytes.ToArray(), 0, bytes.Length, CancellationToken.None);
#endif
                }
            }
            
            var saveFilePath = Path.Combine(downloadsFolder, fileName);
            
            if (compression)
            {
                Log($"Saving decompressed file.");
                fileStream.Position = 0;
#if !UNITY_ENGINE
                await using var decompressedFileStream = File.Create(saveFilePath);
                await using var decompressor = new DeflateStream(fileStream, CompressionMode.Decompress);
#else
                using var decompressedFileStream = File.Create(saveFilePath);
                using var decompressor = new DeflateStream(fileStream, CompressionMode.Decompress);
#endif
                await decompressor.CopyToAsync(decompressedFileStream);
            }
            else
            {
                fileStream.Close();
                Log($"Saving file.");
#if !UNITY_ENGINE
                File.Move(binaryFilePath, saveFilePath, true);
#else
                File.Move(binaryFilePath, saveFilePath);
#endif
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
        
        var streamingCall = _fileTransferClient.UploadFile();

        try
        {
            
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
                
#if !UNITY_ENGINE
                await using var sourceFileStream = File.OpenRead(uploadFilePath);
                await using var compressedFileStream = File.Create(compressedFilePath);
                await using var compressor = new DeflateStream(compressedFileStream, CompressionMode.Compress);
#else
                using var sourceFileStream = File.OpenRead(uploadFilePath);
                using var compressedFileStream = File.Create(compressedFilePath);
                using var compressor = new DeflateStream(compressedFileStream, CompressionMode.Compress);
#endif
                await sourceFileStream.CopyToAsync(compressor);
                
                uploadFilePath = Path.Combine(compressedFilePath);
            }
            
#if !UNITY_ENGINE
            await using (var readStream = File.OpenRead(uploadFilePath))
#else
            using (var readStream = File.OpenRead(uploadFilePath))
#endif
            {
                while (true)
                {
#if !UNITY_ENGINE
                    var count = await readStream.ReadAsync(_buffer);
#else
                    var count = await readStream.ReadAsync(_buffer, 0, _buffer.Length);
#endif
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
            await streamingCall.RequestStream.CompleteAsync();
            Log("Complete request.");
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