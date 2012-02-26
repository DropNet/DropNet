using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp.Deserializers;
using DropNet.Models;

namespace DropNet.DeSerializers
{
    class CustomJsonDeserializer : IDeserializer
    {
        public string DateFormat { get; set; }
        public string Namespace { get; set; }
        public string RootElement { get; set; }

        public T Deserialize<T>(RestSharp.RestResponse response) where T : new()
        {
            if (typeof(T) == typeof(Delta2Page))
                return new Delta2PageJsonDeserializer().Deserialize<T>(response);

            return new JsonDeserializer().Deserialize<T>(response);
        }       
    }
}
