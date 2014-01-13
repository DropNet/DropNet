using System.Collections.Generic;
using System.IO;
using DropNet.Models;
using RestSharp;
using System;
using DropNet.Exceptions;

namespace DropNet
{
    public partial class DropNetClient
    {
        public void GetMetaDataAsync(string path, Action<MetaData> success, Action<DropboxException> failure)
        {
            if (!string.IsNullOrEmpty(path) && !path.StartsWith("/"))
            {
                path = "/" + path;
            }

            var request = _requestHelper.CreateMetadataRequest(path, Root);

            ExecuteAsync(ApiType.Base, request, success, failure);
        }

        public void GetMetaDataAsync(string path, string hash, Action<MetaData> success, Action<DropboxException> failure)
        {
            if (path != "" && !path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateMetadataRequest(path, Root);

            request.AddParameter("hash", hash);

            ExecuteAsync(ApiType.Base, request, success, failure);
        }

        public void SearchAsync(string searchString, Action<List<MetaData>> success, Action<DropboxException> failure)
        {
            SearchAsync(searchString, string.Empty, success, failure);
        }

        public void SearchAsync(string searchString, string path, Action<List<MetaData>> success, Action<DropboxException> failure)
        {
            var request = _requestHelper.CreateSearchRequest(searchString, path, Root);

            ExecuteAsync(ApiType.Base, request, success, failure);
        }
        
        public void GetFileAsync(string path, Action<IRestResponse> success, Action<DropboxException> failure)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateGetFileRequest(path, Root);

            ExecuteAsync(ApiType.Content, request, success, failure);
        }

		public void GetFileAsync(string path, long startByte, long endByte, string rev, Action<IRestResponse> success, Action<DropboxException> failure)
		{
			if (!path.StartsWith("/")) path = "/" + path;

			var request = _requestHelper.CreateGetFileRequest(path, Root, startByte, endByte, rev);

			ExecuteAsync(ApiType.Content, request, success, failure);
		}

#if !WINDOWS_PHONE && !MONOTOUCH && !WINRT
        public void UploadFileAsync(string path, FileInfo localFile, Action<MetaData> success, Action<DropboxException> failure, bool overwrite = true, string parentRevision = null)
        {
            //Get the file stream
            byte[] bytes;
            using (var fs = new FileStream(localFile.FullName, FileMode.Open, FileAccess.Read))
            {
                using (var br = new BinaryReader(fs))
                {
                    long numBytes = localFile.Length;
                    bytes = br.ReadBytes((int)numBytes);
                }
            }

            UploadFileAsync(path, localFile.Name, bytes, success, failure, overwrite, parentRevision);
        }
#endif

        public void UploadFileAsync(string path, string filename, byte[] fileData, Action<MetaData> success, Action<DropboxException> failure, bool overwrite = true, string parentRevision = null)
        {
            if (path != "" && !path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateUploadFileRequest(path, filename, fileData, Root, overwrite, parentRevision);

            ExecuteAsync(ApiType.Content, request, success, failure);
        }

        public void UploadFileAsync(string path, string filename, Stream fileStream, Action<MetaData> success, Action<DropboxException> failure, bool overwrite = true, string parentRevision = null)
        {
            if (path != "" && !path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateUploadFileRequest(path, filename, fileStream, Root, overwrite, parentRevision);

            ExecuteAsync(ApiType.Content, request, success, failure);
        }

        public void UploadChunkedFileAsync(Func<long, byte[]> chunkNeeded, string path, Action<MetaData> success, Action<DropboxException> failure, Action<ChunkedUploadProgress> progress = null, bool overwrite = true, string parentRevision = null, long? fileSize = null, long? maxRetries = null)
        {
            var chunkedUploader = new DropNet.Helpers.ChunkedUploadHelper(this, chunkNeeded, path, success, failure, progress, overwrite, parentRevision, fileSize, maxRetries);
            chunkedUploader.Start();
        }

        public void StartChunkedUploadAsync(byte[] fileData, Action<ChunkedUpload> success, Action<DropboxException> failure)
        {
            var request = _requestHelper.CreateChunkedUploadRequest(fileData);

            ExecuteAsync(ApiType.Content, request, success, failure);
        }

        public void  AppendChunkedUploadAsync(ChunkedUpload upload, byte[] fileData, Action<ChunkedUpload> success, Action<DropboxException> failure)
        {
            var request = _requestHelper.CreateAppendChunkedUploadRequest(upload, fileData);

            ExecuteAsync(ApiType.Content, request, success, failure);
        }

        public void CommitChunkedUploadAsync(ChunkedUpload upload, string path, Action<MetaData> success, Action<DropboxException> failure, bool overwrite = true, string parentRevision = null)
        {
            var request = _requestHelper.CreateCommitChunkedUploadRequest(upload, path, Root, overwrite, parentRevision);

            ExecuteAsync(ApiType.Content, request, success, failure);
        }

        public void DeleteAsync(string path, Action<IRestResponse> success, Action<DropboxException> failure)
        {
            if (path != "" && !path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateDeleteFileRequest(path, Root);

            ExecuteAsync(ApiType.Base, request, success, failure);
        }

        public void CopyAsync(string fromPath, string toPath, Action<IRestResponse> success, Action<DropboxException> failure)
        {
            if (!fromPath.StartsWith("/")) fromPath = "/" + fromPath;
            if (!toPath.StartsWith("/")) toPath = "/" + toPath;

            var request = _requestHelper.CreateCopyFileRequest(fromPath, toPath, Root);

            ExecuteAsync(ApiType.Base, request, success, failure);
        }

        public void CopyFromCopyRefAsync(string fromCopyRef, string toPath, Action<IRestResponse> success, Action<DropboxException> failure)
        {
            if (!toPath.StartsWith("/")) toPath = "/" + toPath;

            var request = _requestHelper.CreateCopyFileFromCopyRefRequest(fromCopyRef, toPath, Root);

            ExecuteAsync(ApiType.Base, request, success, failure);
        }

        public void MoveAsync(string fromPath, string toPath, Action<IRestResponse> success, Action<DropboxException> failure)
        {
            if (!fromPath.StartsWith("/")) fromPath = "/" + fromPath;
            if (!toPath.StartsWith("/")) toPath = "/" + toPath;

            var request = _requestHelper.CreateMoveFileRequest(fromPath, toPath, Root);

            ExecuteAsync(ApiType.Base, request, success, failure);
        }

        public void CreateFolderAsync(string path, Action<MetaData> success, Action<DropboxException> failure)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateCreateFolderRequest(path, Root);

            ExecuteAsync(ApiType.Base, request, success, failure);
        }

        public void GetShareAsync(string path, Action<ShareResponse> success, Action<DropboxException> failure)
        {
            GetShareAsync(path, true, success, failure);
        }

        public void GetShareAsync(string path, bool shortUrl, Action<ShareResponse> success, Action<DropboxException> failure)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateShareRequest(path, Root, shortUrl);

            ExecuteAsync(ApiType.Base, request, success, failure);
        }

        public void GetMediaAsync(string path, Action<ShareResponse> success, Action<DropboxException> failure)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateMediaRequest(path, Root);

            ExecuteAsync(ApiType.Base, request, success, failure);
        }

        public void GetDeltaAsync(bool IKnowThisIsBetaOnly, string path, Action<DeltaPage> success, Action<DropboxException> failure)
        {
            if (!IKnowThisIsBetaOnly) return;

            if (!path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateDeltaRequest(path);

            ExecuteAsync<DeltaPage>(ApiType.Base, request, success, failure);
        }

        public void GetThumbnailAsync(MetaData file, Action<byte[]> success, Action<DropboxException> failure)
        {
            GetThumbnailAsync(file.Path, ThumbnailSize.Small, success, failure);
        }

        public void GetThumbnailAsync(MetaData file, ThumbnailSize size, Action<byte[]> success, Action<DropboxException> failure)
        {
            GetThumbnailAsync(file.Path, size, success, failure);
        }

        public void GetThumbnailAsync(string path, Action<byte[]> success, Action<DropboxException> failure)
        {
            GetThumbnailAsync(path, ThumbnailSize.Small, success, failure);
        }

        public void GetThumbnailAsync(string path, ThumbnailSize size, Action<byte[]> success, Action<DropboxException> failure)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateThumbnailRequest(path, size, Root);

            ExecuteAsync(ApiType.Content, request,
                response => success(response.RawBytes),
                failure);
        }

        public void GetCopyRefAsync(string path, Action<CopyRefResponse> success, Action<DropboxException> failure)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateCopyRefRequest(path, Root);

            ExecuteAsync(ApiType.Base, request, success, failure);
        }

    }
}
