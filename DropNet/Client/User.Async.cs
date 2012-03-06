using DropNet.Models;
using RestSharp;
using System;
using DropNet.Authenticators;
using DropNet.Exceptions;

namespace DropNet
{
    public partial class DropNetClient
    {
        /// <summary>
        /// Gets a token from the almightly dropbox.com (Token cant be used until authorized!)
        /// </summary>
        public void GetTokenAsync(Action<UserLogin> success, Action<DropboxException> failure)
        {
            _restClient.BaseUrl = ApiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret);

            var request = _requestHelper.CreateTokenRequest();

            ExecuteAsync(ApiType.Base, request, response =>
            {
                var userLogin = GetUserLoginFromParams(response.Content);
                UserLogin = userLogin;
                success(userLogin);
            }, failure);
        }

        /// <summary>
        /// Converts a request token into an Access token after the user has authorized access via dropbox.com
        /// </summary>
        /// <param name="success"></param>
        /// <param name="failure"></param>
        public void GetAccessTokenAsync(Action<UserLogin> success, Action<DropboxException> failure)
        {
            _restClient.BaseUrl = ApiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, UserLogin.Token, UserLogin.Secret);

            var request = _requestHelper.CreateAccessTokenRequest();

            ExecuteAsync(ApiType.Base, request, response =>
            {
                var userLogin = GetUserLoginFromParams(response.Content);
                UserLogin = userLogin;
                success(userLogin);
            }, failure);
        }

        /// <summary>
        /// Gets AccountInfo
        /// </summary>
        /// <param name="success"></param>
        /// <param name="failure"></param>
        public void AccountInfoAsync(Action<AccountInfo> success, Action<DropboxException> failure)
        {
            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = ApiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, UserLogin.Token, UserLogin.Secret);

            var request = _requestHelper.CreateAccountInfoRequest();

            ExecuteAsync(ApiType.Base, request, success, failure);
        }


        [Obsolete("No longer supported by Dropbox")]
        public void CreateAccountAsync(string email, string firstName, string lastName, string password, Action<RestResponse> success, Action<DropboxException> failure)
        {
            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = ApiBaseUrl;

            var request = _requestHelper.CreateNewAccountRequest(_apiKey, email, firstName, lastName, password);

            ExecuteAsync(ApiType.Base, request, success, failure);
        }
    }
}