using System;
using System.Collections.Generic;

namespace DropNet.Models
{
    public class DropNetResult
    {
        public readonly System.Net.HttpStatusCode StatusCode;
        
        public static implicit operator bool(DropNetResult m)
        {
            // code to convert from Result to Boolean
            return (m.StatusCode == System.Net.HttpStatusCode.OK);
        }

        public static implicit operator System.Net.HttpStatusCode(DropNetResult m)
        {
            // code to convert from Result to HttpStatusCode (for convenience and to keep dev code cleaner)
            return m.StatusCode;
        }

        public DropNetResult(RestSharp.RestResponse m)
        {
            StatusCode = m.StatusCode;
        }
    }

}
