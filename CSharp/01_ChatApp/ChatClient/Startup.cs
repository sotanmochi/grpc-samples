using System;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;
using ChatApp.Shared;

namespace ChatApp.Client;

public class Startup
{
    private readonly ChatStreamingClient _client;
    private readonly StringBuilder _inputMessageBuffer = new StringBuilder();

    public Startup(GrpcChannel channel)
    {
        _client = new ChatStreamingClient(channel);
        _client.OnResponseEvent += OnResponseEventHandler;
        Console.CancelKeyPress += ConsoleCancelEventHandler;
    }

    public async Task DisposeAsync()
    {
        Console.CancelKeyPress -= ConsoleCancelEventHandler;
        _client.OnResponseEvent -= OnResponseEventHandler;

        await _client.DisposeAsync();

        Console.WriteLine("[Startup] Disposed");
    }

    public async Task StartAsync(string group, string username)
    {
        _client.ConnectAndForget();

        while (true)
        {
            Console.Write("Input message ('q' to quit): ");
            var message = ReadLine();

            if (message is null || message.ToLower() == "q")
            {
                break;
            }
            else if (message == string.Empty)
            {
                continue;
            }

            await _client.SendMessage(group, username, message);
        }
    }

    private void OnResponseEventHandler(ChatMessage message)
    {
        Console.WriteLine("");
        Console.WriteLine($"-------------------------------------------");
        Console.WriteLine($"Received Message: {message}");
        Console.WriteLine($"-------------------------------------------");
        Console.Write($"Input message ('q' to quit): {_inputMessageBuffer}");
    }

    private void ConsoleCancelEventHandler(object? sender, ConsoleCancelEventArgs e)
    {
        Console.WriteLine();
        Console.WriteLine("Console cancel key has been pressed.");
        DisposeAsync().GetAwaiter().GetResult();
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