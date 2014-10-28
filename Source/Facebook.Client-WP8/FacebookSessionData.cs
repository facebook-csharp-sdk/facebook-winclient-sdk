using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Client
{
    class FacebookSessionData
    {
        public FacebookSessionData()
        {
            this.CurrentPermissions = new List<string>();
        }

        // NOTE: If you add properties to this object, you must update the 
        // cache provider implimentations to ensure all values get persisted.
        public String AppId;
        public string AccessToken { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Issued { get; set; }
        public string FacebookId { get; set; }
        public List<string> CurrentPermissions { get; set; }
        public string State { get; set; }
    }
}
