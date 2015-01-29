using System;
using System.Collections.Generic;

namespace DropNet.Models
{
    public class MetaData
    {
        public string Hash { get; set; }
        public bool Thumb_Exists { get; set; }
        public long Bytes { get; set; }
        public string Modified { get; set; }
        public string Client_Mtime { get; set; }
        public string Path { get; set; }
        public bool Is_Dir { get; set; }
        public bool Is_Deleted { get; set; }
        public string Size { get; set; }
        public string Root { get; set; }
        public string Icon { get; set; }
        public int Revision { get; set; }
		public string Rev { get; set; }
        public List<MetaData> Contents { get; set; }
        public String Mime_Type { get; set; }

        public DateTime ModifiedDate
        {
            get
            {
                return GetDateTimeFromString(Modified);
            }
        }

		 public DateTime UTCDateModified
        {
            get
            {
                return GetUTCDateTimeFromString(Modified);
            }
            set
            {
                Modified = GetStringFromDateTime(value);
            }
        }

        public DateTime Client_MtimeDate
        {
            get
            {
                return GetDateTimeFromString(Client_Mtime);
            }
        }

        public DateTime UTCDateClient_Mtime
        {
            get
            {
                return GetUTCDateTimeFromString(Client_Mtime);
            }
            set
            {
                Client_Mtime = GetStringFromDateTime(value);
            }
        }

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(Path))
                {
                    return string.Empty;
                }
                
                if (Path.LastIndexOf("/") == -1)
                {
                    return string.Empty;
                }

                return string.IsNullOrEmpty(Path) ? "root" : Path.Substring(Path.LastIndexOf("/") + 1);
            }
        }
		
        public string Extension
        {
            get
            {
                if (string.IsNullOrEmpty(Path))
                {
                    return string.Empty;
                }
                
                if (Path.LastIndexOf(".") == -1)
                {
                    return string.Empty;
                }

                return Is_Dir ? string.Empty : Path.Substring(Path.LastIndexOf("."));
            }
        }

        private static DateTime GetDateTimeFromString(string dateTimeStr)
        {
            //cast to datetime and return
            return dateTimeStr == null ? DateTime.MinValue : DateTime.Parse(dateTimeStr); //RFC1123 format date codes are returned by API
        }

        private static DateTime GetUTCDateTimeFromString(string dateTimeStr)
        {
            string str = dateTimeStr;
            if (str == null)
                return DateTime.MinValue;
            if (str.EndsWith(" +0000")) str = str.Substring(0, str.Length - 6);
            if (!str.EndsWith(" UTC")) str += " UTC";
            return DateTime.ParseExact(str, "ddd, d MMM yyyy HH:mm:ss UTC", System.Globalization.CultureInfo.InvariantCulture);
        }

        private static string GetStringFromDateTime(DateTime dateTime)
        {
            return dateTime.ToString("ddd, d MMM yyyy HH:mm:ss UTC");
        }
    }

}
