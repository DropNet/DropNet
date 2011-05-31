using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DropNet.Extensions
{
    public static class StringExtensions
    {
        public static string UrlEncode(this string value)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char ch in value)
            {
                if ("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~".IndexOf(ch) != -1)
                {
                    builder.Append(ch);
                }
                else
                {
                    builder.Append('%' + string.Format("{0:X2}", (int)ch));
                }
            }
            return builder.ToString();
        }
    }
}
