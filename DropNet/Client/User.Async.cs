using DropNet.Models;
using RestSharp;
using System;
using DropNet.Authenticators;
using DropNet.Exceptions;

namespace DropNet
{
    public partial class DropNetClient
    {
        //TODO - Fix this for Async?
        ///// <summary>
        ///// Shorthand method to get a token from Dropbox and build the Url to authorize it.
        ///// </summary>
        ///// <returns></returns>
        //public string GetTokenAndBuildUrlAsync(Action<UserLogin> success, Action<DropboxException> failure, string callback = null)
        //{
        //    GetTokenAsync(success, failure);
        //    return BuildAutorizeUrl(callback);
        //}

        /// <summary>
        /// Gets a token from the almightly dropbox.com (Token cant be used until authorized!)
        /// </summary>
        /// <returns></returns>
        public void GetTokenAsync(Action<UserLogin> success, Action<DropboxException> failure)
        {
            _restClient.BaseUrl = _apiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret);

            var request = _requestHelper.CreateTokenRequest();

            ExecuteAsync(request, response =>
            {
                var userLogin = GetUserLoginFromParams(response.Content);
                this.UserLogin = userLogin;
                success(userLogin);
            }, failure);
        }

        //TODO - Method descriptions
        public void GetAccessTokenAsync(Action<UserLogin> success, Action<DropboxException> failure)
        {
            _restClient.BaseUrl = _apiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, UserLogin.Token, UserLogin.Secret);

            var request = _requestHelper.CreateAccessTokenRequest();

            ExecuteAsync(request, response =>
            {
                var userLogin = GetUserLoginFromParams(response.Content);
                this.UserLogin = userLogin;
                success(userLogin);
            }, failure);
        }

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