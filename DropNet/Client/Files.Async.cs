using System.IO;
using DropNet.Models;
using RestSharp;
using System;
using DropNet.Authenticators;

namespace DropNet
{
    public partial class DropNetClient
    {

        /// <summary>
        /// Gets MetaData for a File or Folder. For a folder this includes its contents. For a file, this includes details such as file size.
        /// </summary>
        /// <param name="path">The path of the file or folder</param>
        /// <param name="callback">The callback Action to perform on completion</param>
        public void GetMetaDataAsync(string path, Action<RestResponse<MetaData>> callback)
        {
            if (path!="" && !path.StartsWith("/")) path = "/" + path;

            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = Resource.ApiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = _requestHelper.CreateMetadataRequest(path);

            _restClient.ExecuteAsync<MetaData>(request, callback);
        }

		/// <summary>
		/// Gets MetaData for a File or Folder. For a folder this includes its contents. For a file, this includes details such as file size.
		/// Optional 'hash' param returns HTTP code 304	(Directory contents have not changed) if contents have not changed since the
		/// hash was retrieved on a previous call.
		/// </summary>
		/// <param name="path">The path of the file or folder</param>
        /// <param name="hash">hash - Optional. Listing return values include a hash representing the state of the directory's contents. If you provide this argument to the metadata call, you give the service an opportunity to respond with a "304 Not Modified" status code instead of a full (potentially very large) directory listing. This argument is ignored if the specified path is associated with a file or if list=false.</param>
        /// <param name="callback">The callback Action to perform on completion</param>
        public void GetMetaDataAsync(string path, string hash, Action<RestResponse<MetaData>> callback)
        {
            if (path != "" && !path.StartsWith("/")) path = "/" + path;

            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = Resource.ApiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = _requestHelper.CreateMetadataRequest(path);

            request.AddParameter("hash", hash);

            _restClient.ExecuteAsync<MetaData>(request, callback);
        }


        //TODO - Make class for this to return (instead of just a byte[])
        /// <summary>
        /// Downloads a File from dropbox given the path
        /// </summary>
        /// <param name="path">The path of the file to download</param>
        /// <param name="callback">The callback Action to perform on completion</param>
        public void GetFile(string path, Action<RestResponse> callback)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = Resource.ApiContentBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = _requestHelper.CreateGetFileRequest(path);

            _restClient.ExecuteAsync(request, callback);
        }

#if WINDOWS_PHONE || MONOTOUCH
        //Exclude for now...
#else

        /// <summary>
        /// Uploads a File to Dropbox from the local file system to the specified folder
        /// </summary>
        /// <param name="path">The path of the folder to upload to</param>
        /// <param name="localFile">The local file to upload</param>
        /// <param name="callback">The callback Action to perform on completion</param>
        public void UploadFileAsync(string path, FileInfo localFile, Action<RestResponse> callback)
        {
            //Get the file stream
            byte[] bytes = null;
            FileStream fs = new FileStream(localFile.FullName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = localFile.Length;
            bytes = br.ReadBytes((int)numBytes);

            UploadFileAsync(path, localFile.Name, bytes, callback);
        }
#endif

        /// <summary>
        /// Uploads a File to Dropbox given the raw data.
        /// </summary>
        /// <param name="path">The path of the folder to upload to</param>
        /// <param name="filename">The Name of the file to upload to dropbox</param>
        /// <param name="fileData">The file data</param>
        /// <param name="callback">The callback Action to perform on completion</param>
        public void UploadFileAsync(string path, string filename, byte[] fileData, Action<RestResponse> callback)
        {
            if (path != "" && !path.StartsWith("/")) path = "/" + path;

            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = Resource.ApiContentBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = _requestHelper.CreateUploadFileRequest(path, filename, fileData);

            _restClient.ExecuteAsync(request, callback);
        }

        /// <summary>
        /// Deletes the file or folder from dropbox with the given path
        /// </summary>
        /// <param name="path">The Path of the file or folder to delete.</param>
        /// <param name="callback">The callback Action to perform on completion</param>
        public void DeleteAsync(string path, Action<RestResponse> callback)
        {
            if (path != "" && !path.StartsWith("/")) path = "/" + path;

            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = Resource.ApiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = _requestHelper.CreateDeleteFileRequest(path);

            _restClient.ExecuteAsync(request, callback);
        }

        /// <summary>
        /// Copies a file or folder on Dropbox
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to copy</param>
        /// <param name="toPath">The path to where the file or folder is getting copied</param>
        /// <param name="callback">The callback Action to perform on completion</param>
        public void Copy(string fromPath, string toPath, Action<RestResponse> callback)
        {
            if (!fromPath.StartsWith("/")) fromPath = "/" + fromPath;
            if (!toPath.StartsWith("/")) toPath = "/" + toPath;

            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = Resource.ApiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = _requestHelper.CreateCopyFileRequest(fromPath, toPath);

            _restClient.ExecuteAsync(request, callback);
        }

        /// <summary>
        /// Moves a file or folder on Dropbox
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to move</param>
        /// <param name="toPath">The path to where the file or folder is getting moved</param>
        /// <param name="callback">The callback Action to perform on completion</param>
        public void Move(string fromPath, string toPath, Action<RestResponse> callback)
        {
            if (!fromPath.StartsWith("/")) fromPath = "/" + fromPath;
            if (!toPath.StartsWith("/")) toPath = "/" + toPath;

            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = Resource.ApiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = _requestHelper.CreateMoveFileRequest(fromPath, toPath);

            _restClient.ExecuteAsync(request, callback);
        }

        /// <summary>
        /// Creates a folder on Dropbox
        /// </summary>
        /// <param name="path">The path to the folder to create</param>
        /// <param name="callback">The callback Action to perform on completion</param>
        public void CreateFolder(string path, Action<RestResponse<MetaData>> callback)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = Resource.ApiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = _requestHelper.CreateCreateFolderRequest(path);

            _restClient.ExecuteAsync<MetaData>(request, callback);
        }

    }
}
