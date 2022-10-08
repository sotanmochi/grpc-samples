using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace FileTransferApp.Client;

public class Startup
{
    private readonly FileTransferClient _client;
    private readonly StringBuilder _inputMessageBuffer = new StringBuilder();

    public Startup(GrpcChannel channel)
    {
        _client = new FileTransferClient(channel);
        Console.CancelKeyPress += ConsoleCancelEventHandler;
    }

    public void Dispose()
    {
        Console.CancelKeyPress -= ConsoleCancelEventHandler;
        Console.WriteLine("[Startup] Disposed");
    }

    public async Task StartAsync(string downloadsFolder)
    {
        while (true)
        {
            Console.Write("Input command ('u' to upload, 'd' to download, 'q' to quit): ");
            var command = ReadLine().ToLower();

            if (command == "q")
            {
                break;
            }
            else if (command != "d" && command != "u")
            {
                continue;
            }

            if (command == "u")
            {
                Console.Write("Input file path: ");
                var filepath = ReadLine();
                
                if (!File.Exists(filepath))
                {
                    Console.WriteLine($"File Not Found: '{filepath}'");
                    continue;
                }
                
                Console.Write("Compress the file? (y/n): ");
                var answer = ReadLine();
                var compression = (answer is "y" or "Y");
                
                var fileId = await _client.UploadFile(filepath, compression);
                Console.WriteLine($"Uploaded file id: '{fileId}'.");
            }
            else if (command == "d")
            {
                Console.Write("Input file id: ");
                var fileId = ReadLine();
                
                var fileInfo = await _client.DownloadFile(fileId, downloadsFolder);
                if (fileInfo != null)
                {
                    Console.WriteLine($"Downloaded file: '{fileInfo.FullName}'.");
                }
            }
        }
    }

    private void ConsoleCancelEventHandler(object? sender, ConsoleCancelEventArgs e)
    {
        Console.WriteLine();
        Console.WriteLine("Console cancel key has been pressed.");
        Dispose();
    }

    private string ReadLine()
    {
        _inputMessageBuffer.Clear();

        while (true)
        {
            var keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.Escape:
                    Console.WriteLine();
                    return string.Empty;

                case ConsoleKey.Enter:
                    Console.WriteLine();
                    return _inputMessageBuffer.ToString();

                case ConsoleKey.Backspace:
                    if(_inputMessageBuffer.Length > 0)
                    {
                        _inputMessageBuffer.Remove(_inputMessageBuffer.Length - 1, 1);
                        Console.Write("\b \b");
                    }
                    break;

                default:
                    if(keyInfo.KeyChar != '\0')
                        _inputMessageBuffer.Append(keyInfo.KeyChar);
                    Console.Write(keyInfo.KeyChar);
                    break;
            }
        }
    }
}