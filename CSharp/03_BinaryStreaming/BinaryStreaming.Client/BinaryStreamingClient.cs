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
                await _streamingCall.RequestStream.CompleteAsync();
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
            using (_streamingCall = _callInvoker.AsyncDuplexStreamingCall<byte[], byte[]>(BinaryStreamingGrpc.ConnectMethod, _host, _options))
            {
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

    public async Task SendAsync(byte[] data)
    {
        await _streamingCall.RequestStream.WriteAsync(data);
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
}
}