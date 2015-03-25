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
using OAuth2Authenticator = DropNet.Authenticators.OAuth2Authenticator;

namespace DropNet
{
    public partial class DropNetClient : IDropNetClient
    {
        private const string MainServerBaseUrl = "https://www.dropbox.com";
        private const string ApiBaseUrl = "https://api.dropbox.com";
        private const string ApiContentBaseUrl = "https://api-content.dropbox.com";
        private const string ApiNotifyUrl = "https://api-notify.dropbox.com";
        private const string Version = "1";

        private UserLogin _userLogin;

        public UserLogin UserLogin
        {
            get { return _userLogin; }
            set
            {
                _userLogin = value;
                SetAuthProviders();
            }
        }

        public TimeSpan Timeout
        {
            get { return TimeSpan.FromMilliseconds(_restClient.Timeout); } 
            set { _restClient.Timeout = value.Milliseconds; }
        }

        public int TimeoutMS
        {
            get { return _restClient.Timeout; }
            set { _restClient.Timeout = value; }
        }

        /// <summary>
        /// To use Dropbox API in sandbox mode (app folder access) set to true
        /// </summary>
        public bool UseSandbox { get; set; }

        private const string SandboxRoot = "sandbox";
        private const string DropboxRoot = "dropbox";

        private readonly string _apiKey;
        private readonly string _appsecret;
        private readonly AuthenticationMethod _authenticationMethod;

        private RestClient _restClientMainServer;
        private RestClient _restClient;
        private RestClient _restClientContent;
        private RestClient _restClientNotify;
        private RequestHelper _requestHelper;

#if !WINDOWS_PHONE
        public IWebProxy Proxy { get; set; }
#endif

        /// <summary>
        /// Gets the directory root for the requests (full or sandbox mode)
        /// </summary>
        string Root
        {
            get { return UseSandbox ? SandboxRoot : DropboxRoot; }
        }

#if WINDOWS_PHONE
        /// <summary>
        /// Default Constructor for the DropboxClient
        /// </summary>
        /// <param name="apiKey">The Api Key to use for the Dropbox Requests</param>
        /// <param name="appSecret">The Api Secret to use for the Dropbox Requests</param>
        /// <param name="proxy">The proxy to use for web requests</param>
        /// <param name="authenticationMethod">The authentication method to use.</param>
        public DropNetClient(string apiKey, string appSecret, AuthenticationMethod authenticationMethod = AuthenticationMethod.OAuth1)
        {
            LoadClient();
            _apiKey = apiKey;
            _appsecret = appSecret;
            _authenticationMethod = authenticationMethod;
            UserLogin = null;
        }

        public DropNetClient(string apiKey, string appSecret, string accessToken)
            : this(apiKey, appSecret, AuthenticationMethod.OAuth2)
        {
            UserLogin = new UserLogin { Token = accessToken };
        }

        public DropNetClient(string apiKey, string appSecret, string userToken, string userSecret)
            : this(apiKey, appSecret)
        {
            UserLogin = new UserLogin { Token = userToken, Secret = userSecret };
        }
#else
        /// <summary>
        /// Default Constructor for the DropboxClient
        /// </summary>
        /// <param name="apiKey">The Api Key to use for the Dropbox Requests</param>
        /// <param name="appSecret">The Api Secret to use for the Dropbox Requests</param>
        /// <param name="proxy">The proxy to use for web requests</param>
        /// <param name="authenticationMethod">The authentication method to use.</param>
        public DropNetClient(string apiKey, string appSecret, IWebProxy proxy = null, AuthenticationMethod authenticationMethod = AuthenticationMethod.OAuth1)
        {
            Proxy = proxy;
            LoadClient();
            _apiKey = apiKey;
            _appsecret = appSecret;
            _authenticationMethod = authenticationMethod;
            UserLogin = null;
        }

        /// <summary>
        /// Creates an instance of the DropNetClient given an API Key/Secret and an OAuth2 Access Token
        /// </summary>
        /// <param name="apiKey">The Api Key to use for the Dropbox Requests</param>
        /// <param name="appSecret">The Api Secret to use for the Dropbox Requests</param>
        /// <param name="accessToken">The OAuth2 access token</param>
        /// <param name="proxy">The proxy to use for web requests</param>
        public DropNetClient(string apiKey, string appSecret, string accessToken, IWebProxy proxy = null)
            : this(apiKey, appSecret, proxy, AuthenticationMethod.OAuth2)
        {
            UserLogin = new UserLogin { Token = accessToken };
        }

        /// <summary>
        /// Creates an instance of the DropNetClient given an API Key/Secret and an OAuth1 User Token/Secret
        /// </summary>
        /// <param name="apiKey">The Api Key to use for the Dropbox Requests</param>
        /// <param name="appSecret">The Api Secret to use for the Dropbox Requests</param>
        /// <param name="userToken">The OAuth1 User authentication token</param>
        /// <param name="userSecret">The OAuth1 Users matching secret</param>
        /// <param name="proxy">The proxy to use for web requests</param>
        public DropNetClient(string apiKey, string appSecret, string userToken, string userSecret, IWebProxy proxy = null)
            : this(apiKey, appSecret, proxy)
        {
            UserLogin = new UserLogin {Token = userToken, Secret = userSecret};
        }
#endif

        private void LoadClient()
        {
            _restClientMainServer = new RestClient(MainServerBaseUrl);

#if !WINDOWS_PHONE
            _restClientMainServer.Proxy = Proxy;
#endif

            _restClient = new RestClient(ApiBaseUrl);

#if !WINDOWS_PHONE
            _restClient.Proxy = Proxy;
#endif

            _restClient.ClearHandlers();
            _restClient.AddHandler("*", new JsonDeserializer());

            _restClientContent = new RestClient(ApiContentBaseUrl);

#if !WINDOWS_PHONE
            _restClientContent.Proxy = Proxy;
#endif

            _restClientContent.ClearHandlers();
            _restClientContent.AddHandler("*", new JsonDeserializer());

            _restClientNotify = new RestClient(ApiNotifyUrl);

#if !WINDOWS_PHONE
            _restClientNotify.Proxy = Proxy;
#endif

            _restClientNotify.ClearHandlers();
            _restClientNotify.AddHandler("*", new JsonDeserializer());

            _requestHelper = new RequestHelper(Version);

            //Default to full access
            UseSandbox = false;
        }

        public string BuildAuthorizeUrl(string callback = null)
        {
            return BuildAuthorizeUrl(UserLogin, callback);
        }

        public string BuildAuthorizeUrl(UserLogin userLogin, string callback = null)
        {
            if (userLogin == null)
            {
                throw new ArgumentNullException("userLogin");
            }
            RestRequest request = _requestHelper.BuildAuthorizeUrl(userLogin.Token, callback);
            return _restClient.BuildUri(request).ToString();
        }

        public string BuildAuthorizeUrl(OAuth2AuthorizationFlow oAuth2AuthorizationFlow, string redirectUri, string state = null)
        {
            if (string.IsNullOrWhiteSpace(redirectUri))
            {
                throw new ArgumentNullException("redirectUri");
            }
            RestRequest request = _requestHelper.BuildOAuth2AuthorizeUrl(oAuth2AuthorizationFlow, _apiKey, redirectUri, state);
            return _restClientMainServer.BuildUri(request).ToString();
        }

#if !WINDOWS_PHONE
        private T Execute<T>(ApiType apiType, IRestRequest request) where T : new()
        {
            IRestResponse<T> response;
            if (apiType == ApiType.Base)
            {
                response = _restClient.Execute<T>(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new DropboxRestException(response, HttpStatusCode.OK);
                }
            }
            else if (apiType == ApiType.Content)
            {
                response = _restClientContent.Execute<T>(request);

                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.PartialContent)
                {
                    throw new DropboxRestException(response, HttpStatusCode.OK, HttpStatusCode.PartialContent);
                }
            }
            else
            {
                response = _restClientNotify.Execute<T>(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new DropboxRestException(response);
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
                    throw new DropboxRestException(response, HttpStatusCode.OK);
                }
            }
            else if (apiType == ApiType.Content)
            {
                response = _restClientContent.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.PartialContent)
                {
                    throw new DropboxRestException(response, HttpStatusCode.OK, HttpStatusCode.PartialContent);
                }
            }
            else
            {
                response = _restClientNotify.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new DropboxRestException(response);
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
                failure(new DropboxRestException
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
                        failure(new DropboxRestException(response, HttpStatusCode.OK));
                    }
                    else
                    {
                        success(response);
                    }
                });
            }
            else if (apiType == ApiType.Content)
            {
                _restClientContent.ExecuteAsync(request, (response, asynchandle) =>
                {
                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.PartialContent)
                    {
                        failure(new DropboxRestException(response, HttpStatusCode.OK, HttpStatusCode.PartialContent));
                    }
                    else
                    {
                        success(response);
                    }
                });
            }
            else
            {
                _restClientNotify.ExecuteAsync(request, (response, asynchandle) =>
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        failure(new DropboxRestException(response));
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
                failure(new DropboxRestException
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
                        failure(new DropboxRestException(response, HttpStatusCode.OK));
                    }
                    else
                    {
                        success(response.Data);
                    }
                });
            }
            else if (apiType == ApiType.Content)
            {
                _restClientContent.ExecuteAsync<T>(request, (response, asynchandle) =>
                {
                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.PartialContent)
                    {
                        failure(new DropboxRestException(response, HttpStatusCode.OK, HttpStatusCode.PartialContent));
                    }
                    else
                    {
                        success(response.Data);
                    }
                });
            }
            else
            {
                _restClientNotify.ExecuteAsync<T>(request, (response, asynchandle) =>
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        failure(new DropboxRestException(response));
                    }
                    else
                    {
                        success(response.Data);
                    }
                });
            }
        }

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
            _restClientContent.Authenticator = GetAuthenticator(_restClientContent.BaseUrl.ToString());
            _restClient.Authenticator = GetAuthenticator(_restClient.BaseUrl.ToString());
        }

        private IAuthenticator GetAuthenticator(string baseUrl)
        {
            var userToken = UserLogin == null ? null : UserLogin.Token;
            var userSecret = UserLogin == null ? null : UserLogin.Secret;
            return _authenticationMethod.Equals(AuthenticationMethod.OAuth1)
                       ? (IAuthenticator)(new OAuthAuthenticator(baseUrl, _apiKey, _appsecret, userToken, userSecret))
                       : new OAuth2Authenticator(userToken);
        }

        enum ApiType
        {
            Base,
            Content,
            Notify
        }

        /// <summary>
        /// Dropbox supports versions 1 and 2 of the OAuth spec.
        /// </summary>
        public enum AuthenticationMethod
        {
            /// <summary>
            /// OAuth1 is the 'standard' authentication mode for Dropbox. For more information see https://www.dropbox.com/developers/core/docs#request-token
            /// </summary>
            OAuth1,

            /// <summary>
            /// OAuth2 support in Dropbox was implemented in 2013. For more information see https://www.dropbox.com/developers/core/docs#oa2-authorize
            /// </summary>
            OAuth2
        }

    }
}
