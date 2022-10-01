using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Grpc.Core;
using ChatApp.Shared;

namespace ChatApp.Server.Services;

public class ChatService : Chat.ChatBase
{
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private readonly ChatGroupRepository _chatGroupRepository;
    private readonly ILogger<ChatService> _logger;

    public ChatService(ChatGroupRepository chatGroupRepository, ILogger<ChatService> logger)
    {
        _chatGroupRepository = chatGroupRepository;
        _logger = logger;
    }

    public override async Task Connect
    (
        IAsyncStreamReader<ChatMessage> requestStream,
        IServerStreamWriter<ChatMessage> responseStream,
        ServerCallContext context
    )
    {
        // OnConnecting
        var clientId = Guid.NewGuid();
        var chatGroups = new HashSet<ChatGroup>();

        Log($"OnConnecting - ClientId: {clientId}");

        try
        {
            // Main loop of streaming service.
            while (await requestStream.MoveNext(_cts.Token))
            {
                var message = requestStream.Current;
                Log($"OnRequestEvent - ThreadId: {Environment.CurrentManagedThreadId}, ClientId: {clientId}, Message: {message}");

                var chatGroup = _chatGroupRepository.GetOrAdd(message.GroupName);
                if (!chatGroup.Contains(clientId))
                {
                    chatGroup.Add(clientId, responseStream);
                    chatGroups.Add(chatGroup);
                    Log($"OnJoinedChatGroup - Group: {chatGroup.Name}, UserCount: {chatGroup.UserCount}");
                }

                await chatGroup.BroadcastAsync(message);
            }
        }
        catch (Exception ex)
        {
            LogError(ex, "Exception@Connect");
        }
        finally
        {
            Log($"OnDisconnected - ClientId: {clientId}");

            // OnDisconnected
            foreach (var chatGroup in chatGroups)
            {
                chatGroup.Remove(clientId);

                if (chatGroup.UserCount <= 0)
                {
                    _chatGroupRepository.TryRemove(chatGroup.Name);
                    Log($"Chat group '{chatGroup.Name}' has been removed.");
                }
                else
                {
                    Log($"ChatGroup: {chatGroup.Name}, UserCount: {chatGroup.UserCount}");
                }
            }
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