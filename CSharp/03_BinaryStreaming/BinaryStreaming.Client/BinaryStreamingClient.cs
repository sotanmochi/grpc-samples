#if ENABLE_MONO || ENABLE_IL2CPP
#define UNITY_ENGINE
#endif

using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using BinaryStreaming.Shared;
    
namespace BinaryStreaming.Client
{
public class BinaryStreamingClient
{
    public event Action<byte[]> OnResponseEvent;
    
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    
    private readonly string _host;
    private readonly CallOptions _options;
    private readonly CallInvoker _callInvoker;
    
    private AsyncDuplexStreamingCall<byte[], byte[]> _streamingCall;
    
    public BinaryStreamingClient(ChannelBase channel, string host = null, CallOptions options = default(CallOptions))
    {
        _host = host;
        _options = options;
        _callInvoker = channel.CreateCallInvoker();
    }

    public async Task DisposeAsync()
    {
        Log("Disposing");

        try
        {
            if (_streamingCall != null)
            {
                await _streamingCall.RequestStream.CompleteAsync().ConfigureAwait(false);
                Log("RequestStream.CompleteAsync");
            }
        }
        finally
        {
            _cts.Cancel();
            _cts.Dispose();
            Log("Finally");
        }
        
        Log("Disposed");
    }

    public async void ConnectAndForget()
    {
        try
        {
            await ConnectAsync().ConfigureAwait(false);
        }
        catch (Exception e)
        {
             const string message = "An error occurred while connecting to the server.";
            LogError($"{message}\n{e}");
        }
    }

    public async Task ConnectAsync()
    {
        var syncContext = SynchronizationContext.Current; // Capture SynchronizationContext.
        
        try
        {
            using (_streamingCall = _callInvoker.AsyncDuplexStreamingCall<byte[], byte[]>(BinaryStreamingGrpc.ConnectMethod, _host, _options))
            {
                var streamReader = _streamingCall.ResponseStream;
                while (await streamReader.MoveNext(_cts.Token).ConfigureAwait(false))
                {
                    try
                    {
                        ConsumeData(syncContext, streamReader.Current);
                    }
                    catch (Exception e)
                    {
                        const string message = "An error occurred when consuming a received message, but the subscription is still alive.";
                        LogError($"{message}\n{e}");
                    }
                }
            }
        }
        catch (Exception e)
        {
            _streamingCall = null;
            const string message = "An error occurred while subscribing to messages.";
            LogError($"{message}\n{e}");
        }
        finally
        {
            Log("OnDisconnected");
            await DisposeAsync().ConfigureAwait(false);
        }
    }

    public async Task SendAsync(byte[] data)
    {
        await _streamingCall.RequestStream.WriteAsync(data);
    }

    private void ConsumeData(SynchronizationContext syncContext, byte[] data)
    {
        if (syncContext != null)
        {
            syncContext.Post(_ => OnResponseEvent?.Invoke(data), null);
        }
        else
        {
            OnResponseEvent?.Invoke(data);
        }
    }

    [
        System.Diagnostics.Conditional("DEVELOPMENT_BUILD"), 
        System.Diagnostics.Conditional("UNITY_EDITOR"),
    ]
    private void Log(object message)
    {
#if UNITY_ENGINE
        UnityEngine.Debug.Log($"[BinaryStreamingClient] {message}");
#else
        Console.WriteLine($"[BinaryStreamingClient] {message}");
#endif
    }

    private void LogError(object message)
    {
#if UNITY_ENGINE
        UnityEngine.Debug.LogError($"[BinaryStreamingClient] {message}");
#else
        Console.WriteLine($"[ERROR][BinaryStreamingClient] {message}");
#endif
    }
}
}