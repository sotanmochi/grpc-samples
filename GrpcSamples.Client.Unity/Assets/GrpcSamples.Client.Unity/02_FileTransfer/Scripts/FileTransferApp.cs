using System.IO;
using UnityEngine;
using Grpc.Core;

namespace FileTransferApp.Client.Unity
{
    /// <summary>
    /// Entry point
    /// </summary>
    public class FileTransferApp : MonoBehaviour
    {
        [SerializeField] FileTransferUIView _uiView;

        private FileTransferClient _client;

        void Start()
        {
            var channel = new Channel("127.0.0.1:5247", ChannelCredentials.Insecure);
            _client = new FileTransferClient(channel);

            _uiView.OnClickUpload += async() => 
            {
                var uploadFilePath = Path.Combine(Application.streamingAssetsPath, _uiView.UploadFileName);
                var uploadedFileId = await _client.UploadFile(uploadFilePath, true);
                _uiView.SetUploadedFileId(uploadedFileId);
            };

            _uiView.OnClickDownload += async() => 
            {
                var downloadsFolder = Path.Combine(Application.streamingAssetsPath, "Downloads");
                var fileInfo = await _client.DownloadFile(_uiView.DownloadFileId, downloadsFolder);
                Debug.Log($"Downloaded file: {fileInfo.FullName}");
            };
        }
    }
}