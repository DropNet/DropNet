#if !WINDOWS_PHONE

using DropNet.Models;
using RestSharp;
using System.Net;
using DropNet.Helpers;
using DropNet.Authenticators;

namespace DropNet
{
    public partial class DropNetClient
    {

        public UserLogin Login(string email, string password)
        {
            _restClient.BaseUrl = _apiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret);

            var request = _requestHelper.CreateLoginRequest(_apiKey, email, password);

            var userlogin = Execute<UserLogin>(request);

            UserLogin = userlogin;

            return UserLogin;
        }

        public RestResponse CreateAccount(string email, string firstName, string lastName, string password)
        {
            _restClient.BaseUrl = _apiBaseUrl;

            var request = _requestHelper.CreateNewAccountRequest(_apiKey, email, firstName, lastName, password);

            return _restClient.Execute(request);
        }

        public AccountInfo Account_Info()
        {
            _restClient.BaseUrl = _apiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, UserLogin.Token, UserLogin.Secret);

            var request = _requestHelper.CreateAccountInfoRequest();

            return Execute<AccountInfo>(request);
        }

    }
}
#endif