using DropNet.Models;
using RestSharp;
using System;
using DropNet.Authenticators;
using DropNet.Exceptions;

namespace DropNet
{
    public partial class DropNetClient
    {
        public void GetTokenAsync(Action<UserLogin> success, Action<DropboxException> failure)
        {
            var request = _requestHelper.CreateTokenRequest();
            ExecuteAsync(ApiType.Base, request, response =>
            {
                var userLogin = GetUserLoginFromParams(response.Content);
                UserLogin = userLogin;
                success(userLogin);
            }, failure);
        }

        public void GetAccessTokenAsync(Action<UserLogin> success, Action<DropboxException> failure)
        {
            var request = _requestHelper.CreateAccessTokenRequest();
            ExecuteAsync(ApiType.Base, request, response =>
            {
                var userLogin = GetUserLoginFromParams(response.Content);
                UserLogin = userLogin;
                success(userLogin);
            }, failure);
        }

        public void GetAccessTokenAsync(Action<UserLogin> success, Action<DropboxException> failure, string code, string redirectUri)
        {
            RestRequest request = _requestHelper.CreateOAuth2AccessTokenRequest(code, redirectUri, _apiKey, _appsecret);
            ExecuteAsync<OAuth2AccessToken>(ApiType.Base, request, response =>
            {
                var userLogin = new UserLogin { Token = response.Access_Token };
                UserLogin = userLogin;
                success(userLogin);
            }, failure);
        }

        public void AccountInfoAsync(Action<AccountInfo> success, Action<DropboxException> failure)
        {
            //This has to be here as Dropbox change their base URL between calls
            var request = _requestHelper.CreateAccountInfoRequest();
            ExecuteAsync(ApiType.Base, request, success, failure);
        }

        /// <summary>
        /// Disables the current access token.
        /// </summary>
        public void DisableAccessTokenAsync(Action success, Action<DropboxException> failure)
        {
            var request = _requestHelper.CreateDisableAccessTokenRequest();
            ExecuteAsync(ApiType.Base, request, _ => success(), failure);
        }

        [Obsolete("No longer supported by Dropbox")]
        public void CreateAccountAsync(string email, string firstName, string lastName, string password, Action<RestResponse> success, Action<DropboxException> failure)
        {
            //This has to be here as Dropbox change their base URL between calls
            var request = _requestHelper.CreateNewAccountRequest(_apiKey, email, firstName, lastName, password);
            ExecuteAsync(ApiType.Base, request, success, failure);
        }
    }
}