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
        /// Gets the Authentication Token/Secret for use with the Web API
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
            var webAuthString = response.Content;

            var callbackEncode = callbackUrl.UrlEncode();

            return "https://www.dropbox.com/0/oauth/authorize?" + webAuthString + "&oauth_callback=" + callbackEncode;
        }

        public void DoWebAuth()
        {
            //something does here...?
        }
    }
}
