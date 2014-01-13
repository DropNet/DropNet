#if !WINDOWS_PHONE

using DropNet.Models;
using RestSharp;
using DropNet.Authenticators;
using System;

namespace DropNet
{
    public partial class DropNetClient
    {
        public string GetTokenAndBuildUrl(string callback = null)
        {
            GetToken();
            return BuildAuthorizeUrl(callback);
        }

        public UserLogin GetToken()
        {
            var request = _requestHelper.CreateTokenRequest();
            var response = Execute(ApiType.Base, request);
            var userLogin = GetUserLoginFromParams(response.Content);
            UserLogin = userLogin;
            return userLogin;
        }

        public UserLogin GetAccessToken()
        {
            var request = _requestHelper.CreateAccessTokenRequest();
            var response = Execute(ApiType.Base, request);
            var userLogin = GetUserLoginFromParams(response.Content);
            //This is the new Access token we have
            UserLogin = userLogin;
            return userLogin;
        }

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