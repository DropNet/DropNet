using DropNet.Models;
using RestSharp;
using RestSharp.Deserializers;
using DropNet.Helpers;
using System;
using DropNet.Exceptions;
using System.Net;

namespace DropNet
{
    public partial class DropNetClient
    {
        private const string _apiBaseUrl = "https://api.dropbox.com";
        private const string _apiContentBaseUrl = "https://api-content.dropbox.com";
        private const string _version = "0";

        /// <summary>
        /// Contains the Users Token and Secret
        /// </summary>
        public UserLogin UserLogin { get; set; }

        private string _apiKey;
        private string _appsecret;

        private RestClient _restClient;
        private RequestHelper _requestHelper;
        /// <summary>
        /// The number of requests that have been made by the current Client instance
        /// </summary>
        public int RequestCount { get; set; }
        /// <summary>
        /// The total Bytes returned from the requests made by the current Client instance
        /// </summary>
        public long DataCount { get; set; }

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

            UserLogin = new UserLogin { Token = userToken, Secret = userSecret };

            LoadClient();
        }

        private void LoadClient()
        {
            _restClient = new RestClient(_apiBaseUrl);
            _restClient.ClearHandlers();
            _restClient.AddHandler("*", new JsonDeserializer());

            _requestHelper = new RequestHelper(_version);

            //probably not needed...
            RequestCount = 0;
            DataCount = 0;
        }

#if !WINDOWS_PHONE
        private T Execute<T>(RestRequest request) where T : new()
        {
            RequestCount++;

            var response = _restClient.Execute<T>(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new DropboxException(response);
            }

            return response.Data;
        }
#endif

        private void ExecuteAsync(RestRequest request, Action<RestResponse> success, Action<DropboxException> failure)
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
            RequestCount++;

            _restClient.ExecuteAsync(request, (response) =>
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

        private void ExecuteAsync<T>(RestRequest request, Action<T> success, Action<DropboxException> failure) where T : new()
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
            RequestCount++;

            _restClient.ExecuteAsync<T>(request, (response) =>
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

    }
}
