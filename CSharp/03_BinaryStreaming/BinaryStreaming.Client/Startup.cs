using System;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;
using MessagePack;

namespace BinaryStreaming.Client;

public class Startup
{
    private readonly BinaryStreamingClient _client;
    private readonly StringBuilder _inputMessageBuffer = new StringBuilder();

    public Startup(GrpcChannel channel)
    {
        _client = new BinaryStreamingClient(channel);
        _client.OnResponseEvent += OnResponseEventHandler;
        Console.CancelKeyPress += ConsoleCancelEventHandler;
    }

    public void Dispose()
    {
        Console.CancelKeyPress -= ConsoleCancelEventHandler;
        _client.OnResponseEvent -= OnResponseEventHandler;

        _client.DisposeAsync().GetAwaiter().GetResult();

        Console.WriteLine("[Startup] Disposed");
    }

    public async Task StartAsync()
    {
        var streamingMessage = new StreamingMessage();

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

            streamingMessage.TimestampMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            streamingMessage.TextMessage = message;

            var data = MessagePackSerializer.Serialize(streamingMessage);

            // Console.WriteLine($"-------------------------------------------");
            // Console.WriteLine($"SendAsync: {data.Length} [bytes]");
            // Console.WriteLine($"-------------------------------------------");

            await _client.SendAsync(data);
        }
    }

    private void OnResponseEventHandler(byte[] data)
    {
        var streamingMessage = MessagePackSerializer.Deserialize<StreamingMessage>(data);

        var message = streamingMessage.TextMessage;
        var TimestampMilliseconds = streamingMessage.TimestampMilliseconds;

        var localTime = DateTimeOffset.FromUnixTimeMilliseconds(streamingMessage.TimestampMilliseconds).ToLocalTime();

        Console.WriteLine("");
        Console.WriteLine($"-------------------------------------------");
        Console.WriteLine($"Received data size: {data.Length}");
        Console.WriteLine($"-------------------------------------------");
        Console.WriteLine($"Received message: ");
        Console.WriteLine("{");
        Console.WriteLine($"  Timestamp (Milliseconds): {TimestampMilliseconds} ({localTime})");
        Console.WriteLine($"  TextMessage: {message}");
        Console.WriteLine("}");
        Console.WriteLine($"-------------------------------------------");
        Console.Write($"Input message ('q' to quit): {_inputMessageBuffer}");
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