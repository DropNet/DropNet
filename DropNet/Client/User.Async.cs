using DropboxNet.Models;
using RestSharp;
using RestSharp.Authenticators;
using System;

namespace DropNet
{
    public partial class DropNetClient
    {

        public void LoginAsync(string email, string password, Action<RestResponse<UserLogin>> callback)
        {
            _restClient.BaseUrl = DropNet.Resource.SecureLoginBaseUrl;
            
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
            _restClient.BaseUrl = Resource.ApiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/account/info";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            _restClient.ExecuteAsync<AccountInfo>(request, callback);
        }

    }
}
