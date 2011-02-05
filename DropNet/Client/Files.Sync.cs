#if WINDOWS_PHONE
//Exclude
#else
using System.IO;
using DropNet.Models;
using DropNet.Authenticators;
using RestSharp;

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
            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = Resource.ApiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = _requestHelper.CreateMetadataRequest(path);

            var response = _restClient.Execute<MetaData>(request);

            return response.Data;
        }

        //TODO - Make class for this to return (instead of just a byte[])
        /// <summary>
        /// Downloads a File from dropbox given the path
        /// </summary>
        /// <param name="path">The path of the file to download</param>
        /// <returns>The files raw bytes</returns>
        public byte[] GetFile(string path)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = Resource.ApiContentBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = _requestHelper.CreateGetFileRequest(path);

            var responseData = _restClient.DownloadData(request);

            return responseData;
        }

        /// <summary>
        /// Retrieve the content of a file in the local file system
        /// </summary>
        /// <param name="localFile">The local file to upload</param>
        /// <returns>True on success</returns>
        public byte[] GetFileContentFromFS(FileInfo localFile)
        {
            //Get the file stream
            byte[] bytes = null;
            FileStream fs = new FileStream(localFile.FullName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = localFile.Length;
            bytes = br.ReadBytes((int)numBytes);
            fs.Close();

            return bytes;
        }

        /// <summary>
        /// Uploads a File to Dropbox given the raw data.
        /// </summary>
        /// <param name="path">The path of the folder to upload to</param>
        /// <param name="filename">The Name of the file to upload to dropbox</param>
        /// <param name="fileData">The file data</param>
        /// <returns>True on success</returns>
        public Result UploadFile(string path, string filename, byte[] fileData)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = Resource.ApiContentBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = _requestHelper.CreateUploadFileRequest(path, filename, fileData);

            var response = _restClient.Execute(request);

            return new Result(response);
        }

        /// <summary>
        /// Deletes the file or folder from dropbox with the given path
        /// </summary>
        /// <param name="path">The Path of the file or folder to delete.</param>
        /// <returns></returns>
        public MetaData Delete(string path)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = Resource.ApiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = _requestHelper.CreateDeleteFileRequest(path);

            var response = _restClient.Execute<MetaData>(request);

            return response.Data;
        }

        /// <summary>
        /// Copies a file or folder on Dropbox
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to copy</param>
        /// <param name="toPath">The path to where the file or folder is getting copied</param>
        /// <returns>True on success</returns>
        public Result Copy(string fromPath, string toPath)
        {
            if (!fromPath.StartsWith("/")) fromPath = "/" + fromPath;
            if (!toPath.StartsWith("/")) toPath = "/" + toPath;

            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = Resource.ApiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = _requestHelper.CreateCopyFileRequest(fromPath, toPath);

            var response = _restClient.Execute(request);

            return new Result(response);
        }

        /// <summary>
        /// Moves a file or folder on Dropbox
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to move</param>
        /// <param name="toPath">The path to where the file or folder is getting moved</param>
        /// <returns>True on success</returns>
        public Result Move(string fromPath, string toPath)
        {
            if (!fromPath.StartsWith("/")) fromPath = "/" + fromPath;
            if (!toPath.StartsWith("/")) toPath = "/" + toPath;

            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = Resource.ApiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = _requestHelper.CreateMoveFileRequest(fromPath, toPath);

            var response = _restClient.Execute(request);

            return new Result(response);
        }

        /// <summary>
        /// Creates a folder on Dropbox
        /// </summary>
        /// <param name="path">The path to the folder to create</param>
        /// <returns>MetaData of the newly created folder</returns>
        public MetaData CreateFolder(string path)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = Resource.ApiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = _requestHelper.CreateCreateFolderRequest(path);

            var response = _restClient.Execute<MetaData>(request);

            return response.Data;
        }

    }
}
#endif