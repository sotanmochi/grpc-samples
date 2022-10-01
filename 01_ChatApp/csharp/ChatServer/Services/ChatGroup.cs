using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Grpc.Core;
using ChatApp.Shared;

namespace ChatApp.Server.Services;

public class ChatGroupRepository
{
    private ConcurrentDictionary<string, ChatGroup> _groups = new ConcurrentDictionary<string, ChatGroup>();

    public bool ContainsUserId(Guid userId, string groupName)
    {
        if (_groups.TryGetValue(groupName, out var group))
        {
            return group.Contains(userId);
        }

        return false;
    }

    public ChatGroup GetOrAdd(string groupName)
    {
        return _groups.GetOrAdd(groupName, _ => { return new ChatGroup(groupName); });
    }

    public bool TryRemove(string groupName)
    {
        return _groups.TryRemove(groupName, out _);
    }
}

public class ChatGroup
{
    public string Name { get; }
    public int UserCount => _users.Count;

    private ConcurrentDictionary<Guid, IServerStreamWriter<ChatMessage>> _users = new ConcurrentDictionary<Guid, IServerStreamWriter<ChatMessage>>();

    public ChatGroup(string name)
    {
        Name = name;
    }

    public bool Contains(Guid userId) => _users.ContainsKey(userId);
    public void Add(Guid userId, IServerStreamWriter<ChatMessage> responseStream) => _users.TryAdd(userId, responseStream);
    public void Remove(Guid userId) => _users.TryRemove(userId, out _);

    public async Task BroadcastAsync(ChatMessage message)
    {
        foreach (var user in _users)
        {
            var responseStreamWriter = user.Value;
            await responseStreamWriter.WriteAsync(message);
        }
    }

    public async Task BroadcastExceptAsync(ChatMessage message, Guid userId)
    {
        foreach (var user in _users)
        {
            if (user.Key != userId)
            {
                var responseStreamWriter = user.Value;
                await responseStreamWriter.WriteAsync(message);
            }
        }
    }

    public async Task SendToAsync(ChatMessage message, Guid userId)
    {
        if (_users.TryGetValue(userId, out var responseStreamWriter))
        {
            await responseStreamWriter.WriteAsync(message);
        }
    }
}