using DropNet.Models;
using RestSharp;
using RestSharp.Deserializers;
using DropNet.Helpers;

namespace DropNet
{
    public partial class DropNetClient
    {
        private const string _version = "0";

        private UserLogin _userLogin;

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
        public DropNetClient(string apiKey, string appSecret)
        {
            _apiKey = apiKey;
            _appsecret = appSecret;

            LoadClient();
        }

        public DropNetClient(string apiKey, string appSecret, string userToken, string userSecret)
        {
            _apiKey = apiKey;
            _appsecret = appSecret;

            _userLogin = new UserLogin { Token = userToken, Secret = userSecret };

            LoadClient();
        }

        private void LoadClient()
        {
            _restClient = new RestClient(DropNet.Resource.SecureLoginBaseUrl);
            _restClient.ClearHandlers();
            _restClient.AddHandler("*", new JsonDeserializer());
            
            //probly not needed...
            RequestCount = 0;
            DataCount = 0;
            _requestHelper = new RequestHelper(_version);
        }

        //TODO - Create Execute and ExecuteAsync Methods to pass through all the RestSharp calls.
        // Maybe even a failure 1?

    }
}
