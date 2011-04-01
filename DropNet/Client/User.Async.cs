using DropNet.Models;
using RestSharp;
using System;
using DropNet.Authenticators;

namespace DropNet
{
    public partial class DropNetClient
    {

        public void LoginAsync(string email, string password, Action<RestResponse<UserLogin>> callback)
        {
            _restClient.BaseUrl = _apiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret);

            var request = _requestHelper.CreateLoginRequest(_apiKey, email, password);

            _restClient.ExecuteAsync<UserLogin>(request, (restResponse) =>
            {
                _userLogin = restResponse.Data;
                callback(restResponse);
			});

        }

        public void Account_InfoAsync(Action<RestResponse<AccountInfo>> callback)
        {
            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = _apiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = _requestHelper.CreateAccountInfoRequest();

            _restClient.ExecuteAsync<AccountInfo>(request, callback);
        }

        public void CreateAccountAsync(string email, string firstName, string lastName, string password, Action<RestResponse> callback)
        {
            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = _apiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = _requestHelper.CreateNewAccountRequest(_apiKey, email, firstName, lastName, password);

            _restClient.ExecuteAsync(request, callback);
        }
    }
}