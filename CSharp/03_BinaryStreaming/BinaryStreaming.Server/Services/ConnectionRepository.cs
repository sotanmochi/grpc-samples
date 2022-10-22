using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Grpc.Core;

namespace BinaryStreaming.Server.Services;

public class ConnectionRepository
{
    public int Count => _connections.Count;

    private readonly ConcurrentDictionary<Guid, IServerStreamWriter<byte[]>> _connections = new ConcurrentDictionary<Guid, IServerStreamWriter<byte[]>>();

    public void Add(Guid connectionId, IServerStreamWriter<byte[]> responseStream) => _connections.TryAdd(connectionId, responseStream);
    public void Remove(Guid connectionId) => _connections.TryRemove(connectionId, out _);

    public async Task BroadcastAsync(byte[] data)
    {
        foreach (var connection in _connections)
        {
            var responseStreamWriter = connection.Value;
            await responseStreamWriter.WriteAsync(data);
        }
    }

    public async Task BroadcastExceptAsync(byte[] data, Guid connectionId)
    {
        foreach (var connection in _connections)
        {
            if (connection.Key != connectionId)
            {
                var responseStreamWriter = connection.Value;
                await responseStreamWriter.WriteAsync(data);
            }
        }
    }

    public async Task SendToAsync(byte[] data, Guid connectionId)
    {
        if (_connections.TryGetValue(connectionId, out var responseStreamWriter))
        {
            await responseStreamWriter.WriteAsync(data);
        }
    }
}