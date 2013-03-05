using System;

namespace DropNet.Models
{
    public class CopyRefResponse
    {
        public string Copy_Ref { get; set; }
        public string Expires { get; set; }
        public DateTime ExpiresDate
        {
            get { return DateTime.Parse(Expires); }
        }
    }
}
