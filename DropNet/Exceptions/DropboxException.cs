using System;
using RestSharp;
using System.Net;

namespace DropNet.Exceptions
{
    public class DropboxException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        /// The response of the error call (for Debugging use)
        /// </summary>
        public IRestResponse Response { get; private set; }
        private string message;
        override public string Message { get { return message; } }

        public DropboxException()
        {
        }

        public DropboxException(string message)
        {
            this.message = message;
        }

        public DropboxException(IRestResponse r)
        {
            Response = r;
            StatusCode = r.StatusCode;
            if(r.ContentType == "application/json") {
                message = r.StatusCode.ToString();
                RestSharp.JsonObject parsedJson = SimpleJson.DeserializeObject(r.Content) as RestSharp.JsonObject;
                if (parsedJson != null && parsedJson.ContainsKey("error") && parsedJson["error"] != null)
                    message += ": " + parsedJson["error"].ToString();
            }
        }

    }
}
