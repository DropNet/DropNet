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

            var request = _requestHelper.CreateLoginRequest(_apiKey, email, password);

            var response = _restClient.Execute<UserLogin>(request);

            _userLogin = response.Data;

            return _userLogin;
        }

        public AccountCreationResult CreateAccount(string email, string firstName, string lastName, string password)
        {
            _restClient.BaseUrl = Resource.SecureLoginBaseUrl;

            var request = _requestHelper.CreateNewAccountRequest(_apiKey, email, firstName, lastName, password);

            var response = _restClient.Execute(request);

            var result = new AccountCreationResult();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                result.success = true;
            }
            else
            {
                result.success = false;

                if (response.Content.Contains("This e-mail is already taken"))
                    result.errorType = AccountCreationResult.ErrorTypes.EmailInUse;
                else
                    result.errorType = AccountCreationResult.ErrorTypes.Unknown;
            }

            return result;
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