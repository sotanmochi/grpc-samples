using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using ChatApp.Shared;

namespace ChatApp.Client;

public class ChatStreamingClient
{
    public Action<ChatMessage> OnResponseEvent;

    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private readonly Chat.ChatClient _chatClient;
    private AsyncDuplexStreamingCall<ChatMessage, ChatMessage> _streamingCall;

    public ChatStreamingClient(GrpcChannel channel)
    {
        _chatClient = new Chat.ChatClient(channel);
    }

    public async Task SendMessage(string groupName, string username, string message)
    {
        var chatMessage = new ChatMessage()
        {
            GroupName = groupName,
            Username = username,
            Message = message,
        };

        await _streamingCall.RequestStream.WriteAsync(chatMessage);
    }

    public async void ConnectAndForget()
    {
        try
        {
            await Connect().ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Log($"An error occurred while connecting to the server.\n {e}");
        }
    }

    public async Task Connect()
    {
        try
        {
            using (_streamingCall = _chatClient.Connect())
            {
                // Read messages from the response stream
                var streamReader = _streamingCall.ResponseStream;
                while (await streamReader.MoveNext(_cts.Token).ConfigureAwait(false))
                {
                    OnResponseEvent?.Invoke(streamReader.Current);
                }
            }
        }
        catch (RpcException)
        {
            _streamingCall = null;
            throw;
        }
        finally
        {
            Log("OnDisconnected");
            await DisposeAsync();
        }
    }

    public async Task DisposeAsync()
    {
        Log("Disposing");
        try
        {
            if (_streamingCall != null)
            {
                await _streamingCall.RequestStream.CompleteAsync();
                Log("RequestStream.CompleteAsync");
            }
        }
        catch {}
        finally
        {
            _cts.Cancel();
            _cts.Dispose();
        }
        Log("Disposed");
    }

    private void Log(object message)
    {
        Console.WriteLine($"[StreamingClient] {message}");
    }
}