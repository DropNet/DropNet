using DropNet.Models;
using RestSharp;
using System;
using DropNet.Authenticators;
using DropNet.Exceptions;

namespace DropNet
{
    public partial class DropNetClient
    {

        public void Account_InfoAsync(Action<AccountInfo> success, Action<DropboxException> failure)
        {
            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = _apiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, UserLogin.Token, UserLogin.Secret);

            var request = _requestHelper.CreateAccountInfoRequest();

            ExecuteAsync<AccountInfo>(request, success, failure);
        }

        public void CreateAccountAsync(string email, string firstName, string lastName, string password, Action<RestResponse> success, Action<DropboxException> failure)
        {
            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = _apiBaseUrl;

            var request = _requestHelper.CreateNewAccountRequest(_apiKey, email, firstName, lastName, password);

            ExecuteAsync(request, success, failure);
        }
    }
}