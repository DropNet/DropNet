using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp.Deserializers;
using Newtonsoft.Json.Linq;
using DropNet.Models;
using Newtonsoft.Json;

namespace DropNet.DeSerializers
{
    class Delta2PageJsonDeserializer : IDeserializer
    {
        public string DateFormat
        {
            get;
            set;
        }

        public T Deserialize<T>(RestSharp.RestResponse response) where T : new ()
        {
            if (typeof(T) != typeof(Delta2Page))
                throw new Exception("This deserializer only works for deserializing to a Delta2Page");

            var o = JObject.Parse(response.Content);

            var page = new T() as Delta2Page;
            page.Reset = o["reset"].Value<bool>();
            page.Cursor = o["cursor"].Value<string>();
            page.Reset = o["has_more"].Value<bool>();

            JArray entries = (JArray)o["entries"];

            foreach (var fields in entries)
            {
                var entry = new Delta2Entry();

                int i = 0;

                foreach (var field in fields)
                {
                    if (i == 0) //First element is path
                        entry.Path = field.Value<string>();

                    if (i == 1) //Second element can be null or contain a MetaDataObject
                    {
                        if (field.GetType() == typeof(JValue))
                            entry.MetaData = null;

                        if (field.GetType() == typeof(JObject))
                            entry.MetaData = new JsonSerializer().Deserialize(new JTokenReader(field), typeof(MetaData)) as MetaData;
                    }

                    i++;
                }

                page.Entries.Add(entry);
            }

            return (T)Convert.ChangeType(page, typeof(T));                 
        }

        public string Namespace
        {
            get;
            set;
        }

        public string RootElement
        {
            get;
            set;

        }        
    }
}
