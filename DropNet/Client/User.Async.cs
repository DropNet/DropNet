using DropNet.Models;
using RestSharp;
using DropNet.Authenticators;
using System;

namespace DropNet
{
    public partial class DropNetClient
    {

        public void LoginAsync(string email, string password, Action<RestResponse<UserLogin>> callback)
        {
            _restClient.BaseUrl = "https://api.getdropbox.com";

            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/token";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            request.AddParameter("oauth_consumer_key", _apiKey);

            request.AddParameter("email", email);
            request.AddParameter("password", password);

            _restClient.ExecuteAsync<UserLogin>(request, (restResponse) =>
            {
                _userLogin = restResponse.Data;
                callback(restResponse);
            });

        }

        public void Account_InfoAsync(Action<RestResponse<AccountInfo>> callback)
        {
            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = "http://api.dropbox.com";
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/account/info";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            _restClient.ExecuteAsync<AccountInfo>(request, callback);
        }

    }
}
