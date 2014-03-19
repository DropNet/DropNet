using System;
namespace DropNet.Models
{
    [Serializable]
    public class UserLogin
    {
        public string Token { get; set; }
        public string Secret { get; set; }
    }
}
