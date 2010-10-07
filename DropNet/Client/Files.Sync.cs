#if WINDOWS_PHONE
//Exclude
#else
using System.IO;
using DropNet.Code.Responses;
using RestSharp;
using RestSharp.Authenticators;

namespace DropNet
{
    public partial class DropNetClient
    {

        /// <summary>
        /// Gets MetaData for a File or Folder. For a folder this includes its contents. For a file, this includes details such as file size.
        /// </summary>
        /// <param name="path">The path of the file or folder</param>
        /// <returns></returns>
        public MetaData GetMetaData(string path)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = Resource.ApiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/metadata/dropbox{path}";
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);

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

            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/files/dropbox{path}";
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);

            var responseData = _restClient.DownloadData(request);

            return responseData;
        }

        /// <summary>
        /// Uploads a File to Dropbox from the local file system to the specified folder
        /// </summary>
        /// <param name="path">The path of the folder to upload to</param>
        /// <param name="localFile">The local file to upload</param>
        /// <returns>True on success</returns>
        public bool UploadFile(string path, FileInfo localFile)
        {
            //Get the file stream
            byte[] bytes = null;
            FileStream fs = new FileStream(localFile.FullName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = localFile.Length;
            bytes = br.ReadBytes((int)numBytes);

            return UploadFile(path, localFile.Name, bytes);
        }

        /// <summary>
        /// Uploads a File to Dropbox given the raw data.
        /// </summary>
        /// <param name="path">The path of the folder to upload to</param>
        /// <param name="filename">The Name of the file to upload to dropbox</param>
        /// <param name="fileData">The file data</param>
        /// <returns>True on success</returns>
        public bool UploadFile(string path, string filename, byte[] fileData)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = Resource.ApiContentBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = new RestRequest(Method.POST);
            request.Resource = "{version}/files/dropbox{path}";
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);
            //Need to add the "file" parameter with the file name
            request.AddParameter("file", filename);

            request.AddFile(new FileParameter { Data = fileData, FileName = filename, ParameterName = "file" });

            var response = _restClient.Execute(request);

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        /// <summary>
        /// Deletes the file or folder from dropbox with the given path
        /// </summary>
        /// <param name="path">The Path of the file or folder to delete.</param>
        /// <returns></returns>
        public bool Delete(string path)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = Resource.ApiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/fileops/delete";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            request.AddParameter("path", path);
            request.AddParameter("root", "dropbox");

            var response = _restClient.Execute(request);

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        /// <summary>
        /// Copies a file or folder on Dropbox
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to copy</param>
        /// <param name="toPath">The path to where the file or folder is getting copied</param>
        /// <returns>True on success</returns>
        public bool Copy(string fromPath, string toPath)
        {
            if (!fromPath.StartsWith("/")) fromPath = "/" + fromPath;
            if (!toPath.StartsWith("/")) toPath = "/" + toPath;

            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = Resource.ApiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/fileops/copy";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            request.AddParameter("from_path", fromPath);
            request.AddParameter("to_path", toPath);
            request.AddParameter("root", "dropbox");

            var response = _restClient.Execute(request);

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        /// <summary>
        /// Moves a file or folder on Dropbox
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to move</param>
        /// <param name="toPath">The path to where the file or folder is getting moved</param>
        /// <returns>True on success</returns>
        public bool Move(string fromPath, string toPath)
        {
            if (!fromPath.StartsWith("/")) fromPath = "/" + fromPath;
            if (!toPath.StartsWith("/")) toPath = "/" + toPath;

            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = Resource.ApiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/fileops/move";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            request.AddParameter("from_path", fromPath);
            request.AddParameter("to_path", toPath);
            request.AddParameter("root", "dropbox");

            var response = _restClient.Execute(request);

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

    }
}
#endif