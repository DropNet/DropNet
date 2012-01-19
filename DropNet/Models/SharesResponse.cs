using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DropNet.Models
{
    public class ShareResponse
    {
        public string Url { get; set; }
        public string Expires { get; set; }
        public DateTime ExpiresDate
        {
            get { return DateTime.Parse(Expires); }
        }
    }
}
