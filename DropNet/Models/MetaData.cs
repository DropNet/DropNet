using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DropNet.Models
{
    public class MetaData
    {
        public string Hash { get; set; }
        public bool Thumb_Exists { get; set; }
        public long Bytes { get; set; }
        public string Modified { get; set; }
        public string Path { get; set; }
        public bool Is_Dir { get; set; }
        public bool Is_Deleted { get; set; }
        public string Size { get; set; }
        public string Root { get; set; }
        public string Icon { get; set; }
        public int Revision { get; set; }
        public List<MetaData> Contents { get; set; }

        public System.DateTime ModifiedDate
        {
            get
            {
                //cast to datetime and return
                return DateTime.Parse(Modified); //RFC1123 format date codes are returned by API
            }
        }

		 public DateTime UTCDateModified
        {
            get
            {
                string str = Modified;
                if (str.EndsWith(" +0000")) str = str.Substring(0, str.Length - 6);
                if (!str.EndsWith(" UTC")) str += " UTC";
                return DateTime.ParseExact(str, "ddd, d MMM yyyy HH:mm:ss UTC", System.Globalization.CultureInfo.InvariantCulture);
            }
            set
            {
                Modified = value.ToString("ddd, d MMM yyyy HH:mm:ss UTC");
            }
        }

		
		
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(Path)) return string.Empty;
                if (Path.LastIndexOf("/") == -1)
                {
                    return string.Empty;
                }
                else
                {
                    return string.IsNullOrEmpty(Path) ? "root" : Path.Substring(Path.LastIndexOf("/") + 1);
                }
            }
        }
		
        public string Extension
        {
            get
            {
                if (string.IsNullOrEmpty(Path)) return string.Empty;
                if (Path.LastIndexOf(".") == -1)
                {
                    return string.Empty;
                }
                else
                {
                    return Is_Dir ? string.Empty : Path.Substring(Path.LastIndexOf("."));
                }
            }
        }
    }

}
