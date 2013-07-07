using System;
using System.Collections.Generic;

namespace DropNet.Models
{
    public class ChunkedUpload
    {
        public string UploadId { get; set; }
        public long Offset { get; set; }
        public string Expires { get; set; }
        public DateTime ExpiresDate
        {
            get
            {
                return Expires == null ? DateTime.MinValue : DateTime.Parse(Expires);
            }
        }
    }
}
