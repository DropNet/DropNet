using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DropNet.Authenticators;
using DropNet.Models;
using DropNet.Exceptions;
using DropNet.Extensions;

namespace DropNet
{
    public partial class DropNetClient
    {
        /// <summary>
        /// Gets the Authentication Token/Secret for use with the Web API (Must be called before PostWebAuth)
        /// </summary>
        /// <param name="callbackUrl"></param>
        /// <returns></returns>
        public string WebAuthUrl(string callbackUrl)
        {
            _restClient.BaseUrl = _apiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret);

            var request = _requestHelper.CreateWebAuthRequest();

            var response = _restClient.Execute(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new DropboxException(response);
            }
            //TODO, something better with the response here?
            var webAuthString = response.Content;

            var callbackEncode = callbackUrl.UrlEncode();

            string url = "https://www.dropbox.com/0/oauth/authorize?" + webAuthString;
            if (callbackUrl != null)
            {
                url += "&oauth_callback=" + callbackUrl.UrlEncode();
            }
            return url;
        }

        public UserLogin PostWebAuth(string token)
        {
            _restClient.BaseUrl = _apiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, token, "");

            var request = _requestHelper.CreateAccessToken();

            var response = _restClient.Execute(request);

            int idx, end;
            String KeySecret = "oauth_token_secret=";
            String KeyToken = "oauth_token=";

            UserLogin = new UserLogin();

            // Get secret
            idx = response.Content.LastIndexOf(KeySecret);
            if (idx == -1)
                return null;
            idx += KeySecret.Length;
            end = response.Content.IndexOf('&', idx);
            if (end == -1)
                end = response.Content.Length;
            UserLogin.Secret = response.Content.Substring(idx, end - idx);

            // Get token
            idx = response.Content.LastIndexOf(KeyToken);
            if (idx == -1)
                return null;
            idx += KeyToken.Length;
            end = response.Content.IndexOf('&', idx);
            if (end == -1)
                end = response.Content.Length;
            UserLogin.Token = response.Content.Substring(idx, end - idx);

            return UserLogin;
        }
    }
}
