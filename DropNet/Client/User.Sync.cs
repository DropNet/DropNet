#if WINDOWS_PHONE
//Exclude
#else
using DropboxNet.Models;
using RestSharp;
using RestSharp.Authenticators;

namespace DropNet
{
    public partial class DropNetClient
    {

        public UserLogin Login(string email, string password)
        {
            _restClient.BaseUrl = "https://api.getdropbox.com";

            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/token";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            request.AddParameter("oauth_consumer_key", _apiKey);

            request.AddParameter("email", email);
            request.AddParameter("password", password);

            var response = _restClient.Execute<UserLogin>(request);

            _userLogin = response.Data;

            return _userLogin;
        }

        public AccountInfo Account_Info()
        {
            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = "http://api.dropbox.com";
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/account/info";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            var response = _restClient.Execute<AccountInfo>(request);

            return response.Data;
        }

    }
}
#endif