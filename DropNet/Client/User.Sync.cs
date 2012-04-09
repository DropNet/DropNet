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
        /// Shorthand method to get a token from Dropbox and build the Url to authorize it.
        /// </summary>
        /// <returns></returns>
        public string GetTokenAndBuildUrl(string callback = null)
        {
            GetToken();
            return BuildAuthorizeUrl(callback);
        }

        /// <summary>
        /// Gets a token from the almightly dropbox.com (Token cant be used until authorized!)
        /// </summary>
        /// <returns></returns>
        public UserLogin GetToken()
        {
            _restClient.BaseUrl = ApiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret);

            var request = _requestHelper.CreateTokenRequest();

            var response = Execute(ApiType.Base, request);

            var userLogin = GetUserLoginFromParams(response.Content);

            UserLogin = userLogin;

            return userLogin;
        }

        public UserLogin GetAccessToken()
        {
            _restClient.BaseUrl = ApiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, UserLogin.Token, UserLogin.Secret);

            var request = _requestHelper.CreateAccessTokenRequest();

            var response = Execute(ApiType.Base, request);

            var userLogin = GetUserLoginFromParams(response.Content);

            //This is the new Access token we have
            UserLogin = userLogin;

            return userLogin;
        }

        [Obsolete("No longer supported by Dropbox!")]
        public RestResponse CreateAccount(string email, string firstName, string lastName, string password)
        {
            var request = _requestHelper.CreateNewAccountRequest(_apiKey, email, firstName, lastName, password);

            return Execute(ApiType.Base, request);
        }

        public AccountInfo AccountInfo()
        {
            _restClient.BaseUrl = ApiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, UserLogin.Token, UserLogin.Secret);

            var request = _requestHelper.CreateAccountInfoRequest();

            return Execute<AccountInfo>(ApiType.Base, request);
        }

    }
}
#endif