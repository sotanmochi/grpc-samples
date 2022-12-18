using UnityEngine;
using Grpc.Core;

namespace BinaryStreaming.Client.Unity
{
    /// <summary>
    /// Entry point
    /// </summary>
    public class BinaryStreamingApp : MonoBehaviour
    {
        [SerializeField] BinaryStreamingUIView _uiView;

        private BinaryStreamingClient _client;

        void Start()
        {
            var channel = new Channel("127.0.0.1:5247", ChannelCredentials.Insecure);
            _client = new BinaryStreamingClient(channel);

            _client.ConnectAndForget();

            _client.OnResponseEvent += OnResponseEventHandler;

            _uiView.OnClickSendMessage += async() => 
            {
                var data = System.Text.Encoding.UTF8.GetBytes(_uiView.Message);
                await _client.SendAsync(data);
            };
        }

        async void OnDestroy()
        {
            _client.OnResponseEvent -= OnResponseEventHandler;
            await _client.DisposeAsync();
            Debug.Log("[BinaryStreaming] Disposed");
        }

        private void OnResponseEventHandler(byte[] data)
        {
            var message = System.Text.Encoding.UTF8.GetString(data);
            _uiView.SetReceivedMessage(message);
            Debug.Log($"[BinaryStreaming] Received message - Message: {message}");
        }
    }
}