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

        public DropboxException()
        {
        }

        public DropboxException(string message)
            : base(message)
        {

        }

        public DropboxException(IRestResponse r)
        {
            Response = r;
            StatusCode = r.StatusCode;
        }

    }
}
