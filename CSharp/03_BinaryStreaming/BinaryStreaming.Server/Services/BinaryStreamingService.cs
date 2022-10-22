using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Grpc.Core;
using BinaryStreaming.Shared;

namespace BinaryStreaming.Server.Services;

public class BinaryStreamingService : BinaryStreamingServiceBase
{
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private readonly ConnectionRepository _connectionRepository;
    private readonly ILogger<BinaryStreamingService> _logger;

    public BinaryStreamingService(ConnectionRepository connectionRepository, ILogger<BinaryStreamingService> logger)
    {
        _connectionRepository = connectionRepository;
        _logger = logger;
    }

    public override async Task Connect
    (
        IAsyncStreamReader<byte[]> requestStream,
        IServerStreamWriter<byte[]> responseStream,
        ServerCallContext context
    )
    {
        // OnConnecting
        var connectionId = Guid.NewGuid();
        _connectionRepository.Add(connectionId, responseStream);

        Log($"OnConnected - ConnectionId: {connectionId}");
        Log($"Connections: {_connectionRepository.Count}");

        try
        {
            // Main loop of streaming service.
            while (await requestStream.MoveNext(_cts.Token))
            {
                Log($"OnRequestEvent - ThreadId: {Environment.CurrentManagedThreadId}, ConnectionId: {connectionId}");
                var data = requestStream.Current;
                await _connectionRepository.BroadcastExceptAsync(data, connectionId);
                // await _connectionRepository.BroadcastAsync(data);
            }
        }
        catch (Exception ex)
        {
            LogError(ex, "Exception@Connect");
        }
        finally
        {
            _connectionRepository.Remove(connectionId);
            Log($"OnDisconnected - ConnectionId: {connectionId}");
            Log($"Connections: {_connectionRepository.Count}");
        }
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