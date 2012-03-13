#if !WINDOWS_PHONE

using System.Collections.Generic;
using System.IO;
using DropNet.Models;
using DropNet.Authenticators;
using System.Net;
using DropNet.Exceptions;

namespace DropNet
{
    public partial class DropNetClient
    {

        /// <summary>
        /// Gets MetaData for the root folder.
        /// </summary>
        /// <returns></returns>
        public MetaData GetMetaData()
        {
            return GetMetaData(string.Empty);
        }

        /// <summary>
        /// Gets MetaData for a File or Folder. For a folder this includes its contents. For a file, this includes details such as file size.
        /// </summary>
        /// <param name="path">The path of the file or folder</param>
        /// <returns></returns>
        public MetaData GetMetaData(string path)
        {
            var request = _requestHelper.CreateMetadataRequest(path, Root);

            return Execute<MetaData>(ApiType.Base, request);
        }

        /// <summary>
        /// Gets list of metadata for search string
        /// </summary>
        /// <param name="searchString">The search string </param>
        public List<MetaData> Search(string searchString)
        {
            return Search(searchString, string.Empty);
        }

        /// <summary>
        /// Gets list of metadata for search string
        /// </summary>
        /// <param name="searchString">The search string </param>
        /// <param name="path">The path of the file or folder</param>
        public List<MetaData> Search(string searchString, string path)
        {
            var request = _requestHelper.CreateSearchRequest(searchString, path, DropboxRoot);

            return Execute<List<MetaData>>(ApiType.Base, request);
        }

        //TODO - Make class for this to return (instead of just a byte[])
        /// <summary>
        /// Downloads a File from dropbox given the path
        /// </summary>
        /// <param name="path">The path of the file to download</param>
        /// <returns>The files raw bytes</returns>
        public byte[] GetFile(string path)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }

            var request = _requestHelper.CreateGetFileRequest(path, Root);
            var response = Execute(ApiType.Content, request);

            return response.RawBytes;
        }

        /// <summary>
        /// Retrieve the content of a file in the local file system
        /// </summary>
        /// <param name="localFile">The local file to upload</param>
        /// <returns>True on success</returns>
        public byte[] GetFileContentFromFS(FileInfo localFile)
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

                fs.Close();
            }

            return bytes;
        }

        /// <summary>
        /// NOTE: DOES NOT WORK, Use UploadFile
        /// Uploads a File to Dropbox given the raw data. 
        /// </summary>
        /// <param name="path">The path of the folder to upload to</param>
        /// <param name="filename">The Name of the file to upload to dropbox</param>
        /// <param name="fileData">The file data</param>
        /// <returns>True on success</returns>
        public MetaData UploadFilePUT(string path, string filename, byte[] fileData)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            var request = _requestHelper.CreateUploadFilePutRequest(path, filename, fileData, Root);
            var response = _restClientContent.Execute<MetaData>(request);

            //TODO - Return something better here?
            return response.Data;
        }

        /// <summary>
        /// Uploads a File to Dropbox given the raw data.
        /// </summary>
        /// <param name="path">The path of the folder to upload to</param>
        /// <param name="filename">The Name of the file to upload to dropbox</param>
        /// <param name="fileData">The file data</param>
        /// <returns>True on success</returns>
        public MetaData UploadFile(string path, string filename, byte[] fileData)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            var request = _requestHelper.CreateUploadFileRequest(path, filename, fileData, Root);
            var response = _restClientContent.Execute<MetaData>(request);

            //TODO - Return something better here?
            return response.Data;
        }

        /// <summary>
        /// Deletes the file or folder from dropbox with the given path
        /// </summary>
        /// <param name="path">The Path of the file or folder to delete.</param>
        /// <returns></returns>
        public MetaData Delete(string path)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            var request = _requestHelper.CreateDeleteFileRequest(path, Root);
            return Execute<MetaData>(ApiType.Base, request);
        }

        /// <summary>
        /// Copies a file or folder on Dropbox
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to copy</param>
        /// <param name="toPath">The path to where the file or folder is getting copied</param>
        /// <returns>True on success</returns>
        public MetaData Copy(string fromPath, string toPath)
        {
            if (!fromPath.StartsWith("/"))
            {
                fromPath = "/" + fromPath;
            }

            if (!toPath.StartsWith("/"))
            {
                toPath = "/" + toPath;
            }
            var request = _requestHelper.CreateCopyFileRequest(fromPath, toPath, Root);

            return Execute<MetaData>(ApiType.Base, request);
        }

        /// <summary>
        /// Moves a file or folder on Dropbox
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to move</param>
        /// <param name="toPath">The path to where the file or folder is getting moved</param>
        /// <returns>True on success</returns>
        public MetaData Move(string fromPath, string toPath)
        {
            if (!fromPath.StartsWith("/"))
            {
                fromPath = "/" + fromPath;
            }

            if (!toPath.StartsWith("/"))
            {
                toPath = "/" + toPath;
            }
            var request = _requestHelper.CreateMoveFileRequest(fromPath, toPath, Root);

            return Execute<MetaData>(ApiType.Base, request);
        }

        /// <summary>
        /// Creates a folder on Dropbox
        /// </summary>
        /// <param name="path">The path to the folder to create</param>
        /// <returns>MetaData of the newly created folder</returns>
        public MetaData CreateFolder(string path)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            var request = _requestHelper.CreateCreateFolderRequest(path, Root);

            return Execute<MetaData>(ApiType.Base, request);
        }

        /// <summary>
        /// Creates and returns a shareable link to files or folders.
        /// Note: Links created by the /shares API call expire after thirty days.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ShareResponse GetShare(string path)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }

            var request = _requestHelper.CreateShareRequest(path, Root);

            return Execute<ShareResponse>(ApiType.Base, request);
        }

        /// <summary>
        /// Returns a link directly to a file.
        /// Similar to /shares. The difference is that this bypasses the Dropbox webserver, used to provide a preview of the file, so that you can effectively stream the contents of your media.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ShareResponse GetMedia(string path)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            var request = _requestHelper.CreateMediaRequest(path, Root);

            return Execute<ShareResponse>(ApiType.Base, request);
        }

        /// <summary>
        /// Gets the thumbnail of an image given its MetaData
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public byte[] GetThumbnail(MetaData file)
        {
            return GetThumbnail(file.Path, ThumbnailSize.Small);
        }

        /// <summary>
        /// Gets the thumbnail of an image given its MetaData
        /// </summary>
        /// <param name="file"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public byte[] GetThumbnail(MetaData file, ThumbnailSize size)
        {
            return GetThumbnail(file.Path, size);
        }

        /// <summary>
        /// Gets the thumbnail of an image given its path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public byte[] GetThumbnail(string path)
        {
            return GetThumbnail(path, ThumbnailSize.Small);
        }

        /// <summary>
        /// Gets the thumbnail of an image given its path
        /// </summary>
        /// <param name="path">The path to the picture</param>
        /// <param name="size">The size to return the thumbnail</param>
        /// <returns></returns>
        public byte[] GetThumbnail(string path, ThumbnailSize size)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            var request = _requestHelper.CreateThumbnailRequest(path, size, Root);

            var response = Execute(ApiType.Content, request);

            return response.RawBytes;
        }

        /// <summary>
        /// The beta delta function, gets updates for a given folder
        /// </summary>
        /// <param name="IKnowThisIsBetaOnly"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public DeltaPage GetDelta(bool IKnowThisIsBetaOnly, string path)
        {
            if (!IKnowThisIsBetaOnly) return null;

            if (!path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateDeltaRequest(path);

            return Execute<DeltaPage>(ApiType.Base, request);
        }

    }
}
#endif