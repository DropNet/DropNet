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
        public RestResponseBase Response { get; private set; }

        public DropboxException()
        {
        }

        public DropboxException(string message)
            : base(message)
        {

        }

        public DropboxException(RestResponseBase r)
        {
            Response = r;
            StatusCode = r.StatusCode;
        }

    }
}
