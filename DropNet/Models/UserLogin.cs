using System;
namespace DropNet.Models
{
#if !WINDOWS_PHONE
    [Serializable]
#endif
    public class UserLogin
    {
        public string Token { get; set; }
        public string Secret { get; set; }
    }
}
