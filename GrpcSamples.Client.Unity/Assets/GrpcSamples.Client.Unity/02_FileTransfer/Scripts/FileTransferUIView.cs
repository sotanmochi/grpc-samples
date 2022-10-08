using System;
using UnityEngine;
using UnityEngine.UI;

namespace FileTransferApp.Client.Unity
{
    public class FileTransferUIView : MonoBehaviour
    {
        [SerializeField] InputField _uploadFileName;
        [SerializeField] Button _uploadButton;
        [SerializeField] Text _uploadedFileId;
        [SerializeField] InputField _downloadFileId;
        [SerializeField] Button _downloadButton;

        public Action OnClickUpload;
        public Action OnClickDownload;

        public string UploadFileName => _uploadFileName.text;
        public string DownloadFileId => _downloadFileId.text;

        void Awake()
        {
            _uploadButton.onClick.AddListener(() => OnClickUpload?.Invoke());
            _downloadButton.onClick.AddListener(() => OnClickDownload?.Invoke());
        }

        public void SetUploadedFileId(string fileId)
        {
            _uploadedFileId.text = $"Uploaded File ID: {fileId}";
        }
    }
}