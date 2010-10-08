using DropNet.Models;
using RestSharp;
using RestSharp.Deserializers;

namespace DropNet
{
    public partial class DropNetClient
    {
        private const string _version = "0";

        private UserLogin _userLogin;

        private string _apiKey;
        private string _appsecret;

        private RestClient _restClient;

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

            _restClient = new RestClient(DropNet.Resource.SecureLoginBaseUrl);
            _restClient.ClearHandlers();
            _restClient.AddHandler("*", new JsonDeserializer());
            //probly not needed...
            RequestCount = 0;
            DataCount = 0;
        }

        //TODO - Create Execute and ExecuteAsync Methods to pass through all the RestSharp calls.
        // Maybe even a failure 1?

    }
}
