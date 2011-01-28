#if WINDOWS_PHONE
//Exclude
#else
using DropNet.Models;
using RestSharp;
using DropNet.Authenticators;
using System.Net;
using DropNet.Helpers;

namespace DropNet
{
    public partial class DropNetClient
    {

        public UserLogin Login(string email, string password)
        {
            _restClient.BaseUrl = Resource.SecureLoginBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, null, null);

            var request = _requestHelper.CreateLoginRequest(_apiKey, email, password);

            var response = _restClient.Execute<UserLogin>(request);

            _userLogin = response.Data;

            return _userLogin;
        }

        public AccountInfo Account_Info()
        {
            //This has to be here as Dropbox change their base URL between calls
            _restClient.BaseUrl = Resource.ApiBaseUrl;
            _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, _userLogin.Token, _userLogin.Secret);

            var request = _requestHelper.CreateAccountInfoRequest();

            var response = _restClient.Execute<AccountInfo>(request);

            return response.Data;
        }

    }
}
#endif