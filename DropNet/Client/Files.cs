using System.IO;
using DropNet.Code.Responses;
using RestSharp;
using RestSharp.Authenticators;

namespace DropNet
{
    public partial class DropNetClient
    {

        public MetaData GetMetaData(string path)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = "http://api.dropbox.com";
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/metadata/dropbox{path}";
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);

            var response = _restClient.Execute<MetaData>(request);

            return response.Data;
        }

        //TODO - Make class for this to return (instead of just a byte[])
        public byte[] GetFile(string path)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = "http://api-content.dropbox.com";
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/files/dropbox{path}";
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);

            var responseData = _restClient.DownloadData(request);

            return responseData;
        }

        public bool UploadFile(string path, FileInfo localFile)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            //Get the file stream
            byte[] bytes = null;
            FileStream fs = new FileStream(localFile.FullName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = localFile.Length;
            bytes = br.ReadBytes((int)numBytes);

            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = "http://api-content.dropbox.com";
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = new RestRequest(Method.POST);
            request.Resource = "{version}/files/dropbox{path}";
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);
            //Need to add the "file" parameter with the file name
            request.AddParameter("file", localFile.Name);

            request.AddFile(new FileParameter { Data = bytes, FileName = localFile.Name, ParameterName = "file" });

            var response = _restClient.Execute(request);

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public bool DeleteFile(string path)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = "http://api.dropbox.com";
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/fileops/delete";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            request.AddParameter("path", path);
            request.AddParameter("root", "dropbox");

            var response = _restClient.Execute(request);

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

    }
}
