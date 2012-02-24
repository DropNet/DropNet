using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DropNet.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DropNet.Helpers
{
    static class ModelHelper
    {
        /// <summary>
        /// Converts the json string returned from the dropbox beta2_delta api call to a Delta2Page object
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Delta2Page Delta2PageFromJson(string json)
        {
            var o = JObject.Parse(json);

            var page = new Delta2Page();
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

            return page;

        }

    }
}
