using System;
using System.Net;
using DropNet.Authenticators;
using DropNet.Exceptions;
using DropNet.Extensions;
using DropNet.Helpers;
using DropNet.Models;
using RestSharp;
using RestSharp.Deserializers;
using System.Threading.Tasks;

namespace DropNet
{
    public partial class DropNetClient
    {
        private const string ApiBaseUrl = "https://api.dropbox.com";
        private const string ApiContentBaseUrl = "https://api-content.dropbox.com";
        private const string Version = "1";

        private UserLogin _userLogin;

        /// <summary>
        /// Contains the Users Token and Secret
        /// </summary>
        public UserLogin UserLogin
        {
            get { return _userLogin; }
            set
            {
                _userLogin = value;
                SetAuthProviders();
            }
        }
        
        /// <summary>
        /// To use Dropbox API in sandbox mode (app folder access) set to true
        /// </summary>
        public bool UseSandbox { get; set; }

        private const string SandboxRoot = "sandbox";
        private const string DropboxRoot = "dropbox";

        private readonly string _apiKey;
        private readonly string _appsecret;

        private RestClient _restClient;
        private RestClient _restClientContent;
        private RequestHelper _requestHelper;

        /// <summary>
        /// Gets the directory root for the requests (full or sandbox mode)
        /// </summary>
        string Root
        {
            get { return UseSandbox ? SandboxRoot : DropboxRoot; }
        }
        
        /// <summary>
        /// Default Constructor for the DropboxClient
        /// </summary>
        /// <param name="apiKey">The Api Key to use for the Dropbox Requests</param>
        /// <param name="appSecret">The Api Secret to use for the Dropbox Requests</param>
        public DropNetClient(string apiKey, string appSecret)
        {
            _apiKey = apiKey;
            _appsecret = appSecret;

            LoadClient();
        }

        /// <summary>
        /// Creates an instance of the DropNetClient given an API Key/Secret and a User Token/Secret
        /// </summary>
        /// <param name="apiKey">The Api Key to use for the Dropbox Requests</param>
        /// <param name="appSecret">The Api Secret to use for the Dropbox Requests</param>
        /// <param name="userToken">The User authentication token</param>
        /// <param name="userSecret">The Users matching secret</param>
        public DropNetClient(string apiKey, string appSecret, string userToken, string userSecret)
        {
            _apiKey = apiKey;
            _appsecret = appSecret;

            LoadClient();

            UserLogin = new UserLogin { Token = userToken, Secret = userSecret };
        }

        private void LoadClient()
        {
            _restClient = new RestClient(ApiBaseUrl);
            _restClient.ClearHandlers();
            _restClient.AddHandler("*", new JsonDeserializer());

            _restClientContent = new RestClient(ApiContentBaseUrl);
            _restClientContent.ClearHandlers();
            _restClientContent.AddHandler("*", new JsonDeserializer());

            _requestHelper = new RequestHelper(Version);

            //Default to full access
            UseSandbox = false;
        }

        /// <summary>
        /// Helper Method to Build up the Url to authorize a Token/Secret
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public string BuildAuthorizeUrl(string callback = null)
        {
            return BuildAuthorizeUrl(UserLogin, callback);
        }

        /// <summary>
        /// Helper Method to Build up the Url to authorize a Token/Secret
        /// </summary>
        /// <param name="userLogin"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public string BuildAuthorizeUrl(UserLogin userLogin, string callback = null)
        {
            if (userLogin == null)
            {
                throw new ArgumentNullException("userLogin");
            }

            //Go 1-Liner!
            return string.Format("https://www.dropbox.com/1/oauth/authorize?oauth_token={0}{1}", userLogin.Token,
                (string.IsNullOrEmpty(callback) ? string.Empty : "&oauth_callback=" + callback));
        }

#if !WINDOWS_PHONE && !WINRT
        private T Execute<T>(ApiType apiType, IRestRequest request) where T : new()
        {
            IRestResponse<T> response;
            if (apiType == ApiType.Base)
            {
                response = _restClient.Execute<T>(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new DropboxException(response);
                }
            }
            else
            {
                response = _restClientContent.Execute<T>(request);

                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.PartialContent)
                {
                    throw new DropboxException(response);
                }
            }

            return response.Data;
        }

        private IRestResponse Execute(ApiType apiType, IRestRequest request)
        {
            IRestResponse response;
            if (apiType == ApiType.Base)
            {
                response = _restClient.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new DropboxException(response);
                }
            }
            else
            {
                response = _restClientContent.Execute(request);

				if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.PartialContent)
                {
                    throw new DropboxException(response);
                }
            }

            return response;
        }
#endif

        private void ExecuteAsync(ApiType apiType, IRestRequest request, Action<IRestResponse> success, Action<DropboxException> failure)
        {
#if WINDOWS_PHONE
            //check for network connection
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                //do nothing
                failure(new DropboxException
                {
                    StatusCode = System.Net.HttpStatusCode.BadGateway
                });
                return;
            }
#endif
            if (apiType == ApiType.Base)
            {
                _restClient.ExecuteAsync(request, (response, asynchandle) =>
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        failure(new DropboxException(response));
                    }
                    else
                    {
                        success(response);
                    }
                });
            }
            else
            {
                _restClientContent.ExecuteAsync(request, (response, asynchandle) =>
                {
					if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.PartialContent)
                    {
                        failure(new DropboxException(response));
                    }
                    else
                    {
                        success(response);
                    }
                });
            }
        }

        private void ExecuteAsync<T>(ApiType apiType, IRestRequest request, Action<T> success, Action<DropboxException> failure) where T : new()
        {
#if WINDOWS_PHONE
            //check for network connection
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                //do nothing
                failure(new DropboxException
                {
                    StatusCode = System.Net.HttpStatusCode.BadGateway
                });
                return;
            }
#endif
            if (apiType == ApiType.Base)
            {
                _restClient.ExecuteAsync<T>(request, (response, asynchandle) =>
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        failure(new DropboxException(response));
                    }
                    else
                    {
                        success(response.Data);
                    }
                });
            }
            else
            {
                _restClientContent.ExecuteAsync<T>(request, (response, asynchandle) =>
                {
					if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.PartialContent)
                    {
                        failure(new DropboxException(response));
                    }
                    else
                    {
                        success(response.Data);
                    }
                });
            }
        }

#if !WINRT

        private Task<T> ExecuteTask<T>(ApiType apiType, IRestRequest request) where T : new()
        {
            if (apiType == ApiType.Base)
            {
                return _restClient.ExecuteTask<T>(request);
            }
            else
            {
                return _restClientContent.ExecuteTask<T>(request);
            }
        }

        private Task<IRestResponse> ExecuteTask(ApiType apiType, IRestRequest request)
        {
            if (apiType == ApiType.Base)
            {
                return _restClient.ExecuteTask(request);
            }
            else
            {
                return _restClientContent.ExecuteTask(request);
            }
        }

#endif

        private UserLogin GetUserLoginFromParams(string urlParams)
        {
            var userLogin = new UserLogin();

            //TODO - Make this not suck
            var parameters = urlParams.Split('&');

            foreach (var parameter in parameters)
            {
                if (parameter.Split('=')[0] == "oauth_token_secret")
                {
                    userLogin.Secret = parameter.Split('=')[1];
                }
                else if (parameter.Split('=')[0] == "oauth_token")
                {
                    userLogin.Token = parameter.Split('=')[1];
                }
            }

            return userLogin;
        }

        private void SetAuthProviders()
        {
            if (UserLogin != null)
            {
                //Set the OauthAuthenticator only when the UserLogin property changes
                _restClientContent.Authenticator = new OAuthAuthenticator(_restClientContent.BaseUrl, _apiKey, _appsecret, UserLogin.Token, UserLogin.Secret);
                _restClient.Authenticator = new OAuthAuthenticator(_restClient.BaseUrl, _apiKey, _appsecret, UserLogin.Token, UserLogin.Secret);
            }
        }

        enum ApiType
        {
            Base,
            Content
        }
    }
}
