using UnityEngine;
using Grpc.Core;
using ChatApp.Shared;

namespace ChatApp.Client.Unity
{
    /// <summary>
    /// Entry point
    /// </summary>
    public class ChatApp : MonoBehaviour
    {
        [SerializeField] ChatUIView _uiView;

        private ChatStreamingClient _client;

        void Start()
        {
            var channel = new Channel("127.0.0.1:5247", ChannelCredentials.Insecure);
            _client = new ChatStreamingClient(channel);

            _client.ConnectAndForget();

            _client.OnResponseEvent += OnResponseEventHandler;

            _uiView.OnClickSendMessage += async() => 
            {
                await _client.SendMessage(_uiView.GroupName, _uiView.Username, _uiView.Message);
            };
        }

        async void OnDestroy()
        {
            _client.OnResponseEvent -= OnResponseEventHandler;
            await _client.DisposeAsync();
            Debug.Log("[ChatApp] Disposed");
        }

        private void OnResponseEventHandler(ChatMessage message)
        {
            Debug.Log($"[ChatApp] Received message - Username: {message.Username}, Message: {message.Message}");
        }
    }
}