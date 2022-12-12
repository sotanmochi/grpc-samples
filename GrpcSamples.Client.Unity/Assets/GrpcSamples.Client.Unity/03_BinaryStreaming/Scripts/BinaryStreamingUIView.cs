using System;
using UnityEngine;
using UnityEngine.UI;

namespace BinaryStreaming.Client.Unity
{
    public class BinaryStreamingUIView : MonoBehaviour
    {
        [SerializeField] InputField _message;
        [SerializeField] Button _sendMessageButton;
        [SerializeField] Text _receivedMessage;

        public event Action OnClickSendMessage;

        public string Message => _message.text;

        void Awake()
        {
            _sendMessageButton.onClick.AddListener(() => OnClickSendMessage?.Invoke());
        }

        public void SetReceivedMessage(string message)
        {
            _receivedMessage.text = message;
        }
    }
}