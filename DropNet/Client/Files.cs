using System.IO;
using System.Text;
using DropNet.Code.Responses;
using DropNet.Exceptions;
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

        //TODO - Fix "Forbidden" messages
        public Stream GetFile(string path)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = "http://api-content.dropbox.com";
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/files/dropbox{path}";
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);

            var response = _restClient.Execute(request);
            byte[] byteArray = Encoding.ASCII.GetBytes(response.Content);
            MemoryStream stream = new MemoryStream(byteArray);

            return stream;
        }

        public void DeleteFile(string path)
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

            if (response.ResponseStatus == ResponseStatus.Error)
            {
                throw new DropboxException(string.Format("{0}: {1}", response.StatusDescription, response.Content));
            }
        }

    }
}
