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
            _restClient.BaseUrl = DropNet.Resource.SecureLoginBaseUrl;

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
            _restClient.BaseUrl = Resource.ApiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = _requestHelper.CreateAccountInfoRequest();

            _restClient.ExecuteAsync<AccountInfo>(request, callback);
        }

    }
}