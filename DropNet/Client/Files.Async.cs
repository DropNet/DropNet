using System.Collections.Generic;
using System.IO;
using DropNet.Models;
using RestSharp;
using System;
using DropNet.Authenticators;
using DropNet.Exceptions;

namespace DropNet
{
    public partial class DropNetClient
    {
        /// <summary>
        /// Gets MetaData for a File or Folder. For a folder this includes its contents. For a file, this includes details such as file size.
        /// </summary>
        /// <param name="path">The path of the file or folder</param>
        /// <param name="success">Success call back</param>
        /// <param name="failure">Failure call back </param>
        public void GetMetaDataAsync(string path, Action<MetaData> success, Action<DropboxException> failure)
        {
            if (!string.IsNullOrEmpty(path) && !path.StartsWith("/"))
            {
                path = "/" + path;
            }

            var request = _requestHelper.CreateMetadataRequest(path, Root);

            ExecuteAsync(ApiType.Base, request, success, failure);
        }

        /// <summary>
        /// Gets MetaData for a File or Folder. For a folder this includes its contents. For a file, this includes details such as file size.
        /// Optional 'hash' param returns HTTP code 304	(Directory contents have not changed) if contents have not changed since the
        /// hash was retrieved on a previous call.
        /// </summary>
        /// <param name="path">The path of the file or folder</param>
        /// <param name="hash">hash - Optional. Listing return values include a hash representing the state of the directory's contents. If you provide this argument to the metadata call, you give the service an opportunity to respond with a "304 Not Modified" status code instead of a full (potentially very large) directory listing. This argument is ignored if the specified path is associated with a file or if list=false.</param>
        /// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        public void GetMetaDataAsync(string path, string hash, Action<MetaData> success, Action<DropboxException> failure)
        {
            if (path != "" && !path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateMetadataRequest(path, Root);

            request.AddParameter("hash", hash);

            ExecuteAsync(ApiType.Base, request, success, failure);
        }

        /// <summary>
        /// Gets list of metadata for search string
        /// </summary>
        /// <param name="searchString">The search string </param>
        /// <param name="success">Success call back</param>
        /// <param name="failure">Failure call back </param>
        public void SearchAsync(string searchString, Action<List<MetaData>> success, Action<DropboxException> failure)
        {
            SearchAsync(searchString, string.Empty, success, failure);
        }

        /// <summary>
        /// Gets list of metadata for search string
        /// </summary>
        /// <param name="searchString">The search string </param>
        /// <param name="path">The path of the file or folder</param>
        /// <param name="success">Success call back</param>
        /// <param name="failure">Failure call back </param>
        public void SearchAsync(string searchString, string path, Action<List<MetaData>> success, Action<DropboxException> failure)
        {
            var request = _requestHelper.CreateSearchRequest(searchString, path, Root);

            ExecuteAsync(ApiType.Base, request, success, failure);
        }


        /// <summary>
        /// Downloads a File from dropbox given the path
        /// </summary>
        /// <param name="path">The path of the file to download</param>
        /// /// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        
        public void GetFileAsync(string path, Action<IRestResponse> success, Action<DropboxException> failure)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateGetFileRequest(path, Root);

            ExecuteAsync(ApiType.Content, request, success, failure);
        }

		/// <summary>
		/// Downloads a part of a File from dropbox given the path
		/// </summary>
		/// <param name="path">The path of the file to download</param>
		/// <param name="startByte">The index of the first byte to get.</param>
		/// <param name="endByte">The index of the last byte to get.</param>
		/// <param name="rev">Revision of the file</param>
		/// <param name="success">Success callback </param>
		/// <param name="failure">Failure callback </param>
		public void GetFileAsync(string path, long startByte, long endByte, string rev, Action<IRestResponse> success, Action<DropboxException> failure)
		{
			if (!path.StartsWith("/")) path = "/" + path;

			var request = _requestHelper.CreateGetFileRequest(path, Root, startByte, endByte, rev);

			ExecuteAsync(ApiType.Content, request, success, failure);
		}

#if !WINDOWS_PHONE && !MONOTOUCH && !WINRT
        /// <summary>
        /// Uploads a File to Dropbox from the local file system to the specified folder
        /// </summary>
        /// <param name="path">The path of the folder to upload to</param>
        /// <param name="localFile">The local file to upload</param>/// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        public void UploadFileAsync(string path, FileInfo localFile, Action<MetaData> success, Action<DropboxException> failure)
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

            UploadFileAsync(path, localFile.Name, bytes, success, failure);
        }
#endif

        /// <summary>
        /// Uploads a File to Dropbox given the raw data.
        /// </summary>
        /// <param name="path">The path of the folder to upload to</param>
        /// <param name="filename">The Name of the file to upload to dropbox</param>
        /// <param name="fileData">The file data</param>
        /// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        public void UploadFileAsync(string path, string filename, byte[] fileData, Action<MetaData> success, Action<DropboxException> failure)
        {
            if (path != "" && !path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateUploadFileRequest(path, filename, fileData, Root);

            ExecuteAsync(ApiType.Content, request, success, failure);
        }

        /// <summary>
        /// Uploads a File to Dropbox given the raw data.
        /// </summary>
        /// <param name="path">The path of the folder to upload to</param>
        /// <param name="filename">The Name of the file to upload to dropbox</param>
        /// <param name="fileStream">The file data</param>
        /// <param name="success">The callback Action to perform on completion</param>
        /// <param name="failure">The callback Action to perform on exception</param>
        public void UploadFileAsync(string path, string filename, Stream fileStream, Action<MetaData> success, Action<DropboxException> failure)
        {
            if (path != "" && !path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateUploadFileRequest(path, filename, fileStream, Root);

            ExecuteAsync(ApiType.Content, request, success, failure);
        }

        /// <summary>
        /// Deletes the file or folder from dropbox with the given path
        /// </summary>
        /// <param name="path">The Path of the file or folder to delete.</param>
        /// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        public void DeleteAsync(string path, Action<IRestResponse> success, Action<DropboxException> failure)
        {
            if (path != "" && !path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateDeleteFileRequest(path, Root);

            ExecuteAsync(ApiType.Base, request, success, failure);
        }

        /// <summary>
        /// Copies a file or folder on Dropbox
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to copy</param>
        /// <param name="toPath">The path to where the file or folder is getting copied</param>
        /// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        public void CopyAsync(string fromPath, string toPath, Action<IRestResponse> success, Action<DropboxException> failure)
        {
            if (!fromPath.StartsWith("/")) fromPath = "/" + fromPath;
            if (!toPath.StartsWith("/")) toPath = "/" + toPath;

            var request = _requestHelper.CreateCopyFileRequest(fromPath, toPath, Root);

            ExecuteAsync(ApiType.Base, request, success, failure);
        }

        /// <summary>
        /// Moves a file or folder on Dropbox
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to move</param>
        /// <param name="toPath">The path to where the file or folder is getting moved</param>
        /// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        public void MoveAsync(string fromPath, string toPath, Action<IRestResponse> success, Action<DropboxException> failure)
        {
            if (!fromPath.StartsWith("/")) fromPath = "/" + fromPath;
            if (!toPath.StartsWith("/")) toPath = "/" + toPath;

            var request = _requestHelper.CreateMoveFileRequest(fromPath, toPath, Root);

            ExecuteAsync(ApiType.Base, request, success, failure);
        }

        /// <summary>
        /// Creates a folder on Dropbox
        /// </summary>
        /// <param name="path">The path to the folder to create</param>
        /// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        public void CreateFolderAsync(string path, Action<MetaData> success, Action<DropboxException> failure)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateCreateFolderRequest(path, Root);

            ExecuteAsync(ApiType.Base, request, success, failure);
        }

        /// <summary>
        /// Creates and returns a shareable link to files or folders.
        /// Note: Links created by the /shares API call expire after thirty days.
        /// </summary>
        /// <param name="path">The path</param>
        /// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        public void GetShareAsync(string path, Action<ShareResponse> success, Action<DropboxException> failure)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateShareRequest(path, Root);

            ExecuteAsync(ApiType.Base, request, success, failure);
        }

        /// <summary>
        /// Returns a link directly to a file.
        /// Similar to /shares. The difference is that this bypasses the Dropbox webserver, used to provide a preview of the file, so that you can effectively stream the contents of your media.
        /// </summary>
        /// <param name="path">The path</param>
        /// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        public void GetMediaAsync(string path, Action<ShareResponse> success, Action<DropboxException> failure)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateMediaRequest(path, Root);

            ExecuteAsync(ApiType.Base, request, success, failure);
        }

        /// <summary>
        /// The beta delta function, gets updates for a given folder
        /// </summary>
        /// <param name="IKnowThisIsBetaOnly"></param>
        /// <param name="path"></param>
        /// <param name="success"></param>
        /// <param name="failure"></param>
        public void GetDeltaAsync(bool IKnowThisIsBetaOnly, string path, Action<DeltaPage> success, Action<DropboxException> failure)
        {
            if (!IKnowThisIsBetaOnly) return;

            if (!path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateDeltaRequest(path);

            ExecuteAsync<DeltaPage>(ApiType.Base, request, success, failure);
        }

        /// <summary>
        /// Gets the thumbnail of an image given its MetaData
        /// </summary>
        /// <param name="file">The MetaData</param>
        /// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        public void GetThumbnailAsync(MetaData file, Action<byte[]> success, Action<DropboxException> failure)
        {
            GetThumbnailAsync(file.Path, ThumbnailSize.Small, success, failure);
        }

        /// <summary>
        /// Gets the thumbnail of an image given its MetaData
        /// </summary>
        /// <param name="file">The metadat file</param>
        /// <param name="size">Thumbnail size</param>
        /// <param name="success">success callback</param>
        /// <param name="failure">Failure callback</param>
        public void GetThumbnailAsync(MetaData file, ThumbnailSize size, Action<byte[]> success, Action<DropboxException> failure)
        {
            GetThumbnailAsync(file.Path, size, success, failure);
        }

        /// <summary>
        /// Gets the thumbnail of an image given its path
        /// </summary>
        /// <param name="path">The path</param>
        /// <param name="success">success callback</param>
        /// <param name="failure">failure callback</param>
        public void GetThumbnailAsync(string path, Action<byte[]> success, Action<DropboxException> failure)
        {
            GetThumbnailAsync(path, ThumbnailSize.Small, success, failure);
        }

        /// <summary>
        /// Gets the thumbnail of an image given its path
        /// </summary>
        /// <param name="path">The path</param>
        /// <param name="size">Thumbnail size</param>
        /// <param name="success">success callback</param>
        /// <param name="failure">failure callback</param>
        public void GetThumbnailAsync(string path, ThumbnailSize size, Action<byte[]> success, Action<DropboxException> failure)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateThumbnailRequest(path, size, Root);

            ExecuteAsync(ApiType.Content, request,
                response => success(response.RawBytes),
                failure);
        }

        /// <summary>
        /// Creates and returns a copy_ref to a file.
        /// 
        /// This reference string can be used to copy that file to another user's Dropbox by passing it in as the from_copy_ref parameter on /fileops/copy.
        /// </summary>
        /// <param name="path">The path</param>
        /// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        public void GetCopyRefAsync(string path, Action<CopyRefResponse> success, Action<DropboxException> failure)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateCopyRefRequest(path, Root);

            ExecuteAsync(ApiType.Base, request, success, failure);
        }

    }
}
