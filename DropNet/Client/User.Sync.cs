#if !WINDOWS_PHONE

using DropNet.Models;
using RestSharp;
using DropNet.Authenticators;
using System;

namespace DropNet
{
    public partial class DropNetClient
    {
        /// <summary>
        /// Shorthand method to get an OAuth1 token from Dropbox and build the Url to authorize it.
        /// </summary>
        /// <returns></returns>
        public string GetTokenAndBuildUrl(string callback = null)
        {
            GetToken();
            return BuildAuthorizeUrl(callback);
        }

        /// <summary>
        ///     Provisions an OAuth1 token from the almightly dropbox.com (Token cant be used until authorized!)
        /// </summary>
        /// <returns></returns>
        public UserLogin GetToken()
        {
            var request = _requestHelper.CreateTokenRequest();
            var response = Execute(ApiType.Base, request);
            var userLogin = GetUserLoginFromParams(response.Content);
            UserLogin = userLogin;
            return userLogin;
        }

        /// <summary>
        ///     Authorizes the previously-requested OAuth1 token
        /// </summary>
        /// <returns></returns>
        public UserLogin GetAccessToken()
        {
            var request = _requestHelper.CreateAccessTokenRequest();
            var response = Execute(ApiType.Base, request);
            var userLogin = GetUserLoginFromParams(response.Content);
            //This is the new Access token we have
            UserLogin = userLogin;
            return userLogin;
        }

        /// <summary>
        ///     Acquire an OAuth2 bearer token once the user has authorized the app.  This endpoint only applies to apps using the AuthorizationFlow.Code flow.
        /// </summary>
        /// <param name="code">The authorization code provided by Dropbox when the user was redirected back to your site.</param>
        /// <param name="redirectUri">The redirect Uri for your site. This is only used to validate that it matches the original /oauth2/authorize; the user will not be redirected again.</param>
        /// <returns>An OAuth2 bearer token.</returns>
        public UserLogin GetAccessToken(string code, string redirectUri)
        {
            RestRequest request = _requestHelper.CreateOAuth2AccessTokenRequest(code, redirectUri, _apiKey, _appsecret);
            var response = Execute<OAuth2AccessToken>(ApiType.Base, request);
            var userLogin = new UserLogin { Token = response.Access_Token };
            UserLogin = userLogin;
            return userLogin;
        }

        public AccountInfo AccountInfo()
        {
            var request = _requestHelper.CreateAccountInfoRequest();
            return Execute<AccountInfo>(ApiType.Base, request);
        }

    }
}
#endif