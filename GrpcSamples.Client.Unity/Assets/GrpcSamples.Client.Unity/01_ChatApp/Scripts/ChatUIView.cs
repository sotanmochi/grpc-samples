using System;
using UnityEngine;
using UnityEngine.UI;

namespace ChatApp.Client.Unity
{
    public class ChatUIView : MonoBehaviour
    {
        [SerializeField] InputField _groupName;
        [SerializeField] InputField _username;
        [SerializeField] InputField _message;
        [SerializeField] Button _sendMessageButton;

        public Action OnClickSendMessage;

        public string GroupName => _groupName.text;
        public string Username => _username.text;
        public string Message => _message.text;

        void Awake()
        {
            _sendMessageButton.onClick.AddListener(() => OnClickSendMessage?.Invoke());
        }
    }
}