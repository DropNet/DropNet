using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DropNet.Models
{
    /// <summary>
    /// </summary>
    internal class OAuth2AccessToken
    {
        /// <summary>
        /// </summary>
        public string Access_Token { get; set; }
        public string Token_Type { get; set; }
        public string Uid { get; set; }
    }
}
