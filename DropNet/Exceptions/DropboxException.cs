using System;

namespace DropNet.Exceptions
{
    public class DropboxException : Exception
    {
        public DropboxException(string message)
            : base(message)
        {
            //Wrapper class for DropNet Exceptions
        }

    }
}
