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
            var request = _requestHelper.CreateAccessTokenRequest();
            ExecuteAsync(ApiType.Base, request, response =>
            {
                var userLogin = GetUserLoginFromParams(response.Content);
                UserLogin = userLogin;
                success(userLogin);
            }, failure);
        }


        /// <summary>
        ///     Acquire an OAuth2 bearer token once the user has authorized the app.  This endpoint only applies to apps using the AuthorizationFlow.Code flow.
        /// </summary>
        /// <param name="success">Action to perform with the OAuth2 access token</param>
        /// <param name="failure"></param>
        /// <param name="code">The authorization code provided by Dropbox when the user was redirected back to your site.</param>
        /// <param name="redirectUri">The redirect Uri for your site. This is only used to validate that it matches the original /oauth2/authorize; the user will not be redirected again.</param>
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

        /// <summary>
        /// Gets AccountInfo
        /// </summary>
        /// <param name="success"></param>
        /// <param name="failure"></param>
        public void AccountInfoAsync(Action<AccountInfo> success, Action<DropboxException> failure)
        {
            //This has to be here as Dropbox change their base URL between calls
            var request = _requestHelper.CreateAccountInfoRequest();
            ExecuteAsync(ApiType.Base, request, success, failure);
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