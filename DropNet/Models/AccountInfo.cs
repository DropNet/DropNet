using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DropNet.Models
{
    public class AccountInfo
    {
        public string Country { get; set; }
        public string Display_Name { get; set; }
        public QuotaInfo Quota_Info { get; set; }
        public string Uid { get; set; }
    }

    public class QuotaInfo
    {
        public string Shared { get; set; }
        public string Quota { get; set; }
        public string Normal { get; set; }
    }
}
