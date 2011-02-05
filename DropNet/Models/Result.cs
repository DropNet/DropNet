using System;
using System.Collections.Generic;

namespace DropNet.Models
{
    public class Result
    {
        public readonly System.Net.HttpStatusCode StatusCode;
        
        public static implicit operator bool(Result m)
        {
            // code to convert from Result to Boolean
            return (m.StatusCode == System.Net.HttpStatusCode.OK);
        }

        public static implicit operator System.Net.HttpStatusCode(Result m)
        {
            // code to convert from Result to HttpStatusCode (for convenience and to keep dev code cleaner)
            return m.StatusCode;
        }

        public Result(RestSharp.RestResponse m)
        {
            StatusCode = m.StatusCode;
        }
    }

}
