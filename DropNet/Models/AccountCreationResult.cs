using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DropNet.Models
{
    public class AccountCreationResult
    {
        public enum ErrorTypes
        {
            Unknown,
            EmailInUse
        }

        public bool success { get; set; }
        public ErrorTypes errorType { get; set; }
    }
}
