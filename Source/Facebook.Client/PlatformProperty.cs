using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Facebook.Client
{
    public class PlatformProperty
    {
        [JsonProperty(PropertyName ="property")]
        public string Property { get; set; }

        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }
    }
}
