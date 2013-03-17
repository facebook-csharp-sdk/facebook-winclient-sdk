using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Client.Tasks
{
    public abstract class FacebookTask
    {

        /// <summary>
        /// Your application's identifier. Required.
        /// </summary>
        [DataMember(Name = "app_id")]
        public string AppId { get; set; }

        /// <summary>
        /// The URL to redirect to after the user clicks a button on the dialog. Required, but automatically specified by most SDKs.
        /// </summary>
        [DataMember(Name = "redirect_uri")]
        public Uri RedirectUri { get; set; }

        /// <summary>	
        /// The display mode in which to render the dialog. The default is page on the www subdomain. display=wap is currently not supported. This is automatically specified by most SDKs.
        /// </summary>
        [DataMember(Name = "display")]
        public string Display { get; set; }

        public FacebookTask()
        {
            this.RedirectUri = FacebookWebDialog.EndUri;
            this.Display = "touch";
        }

        protected IDictionary<string, object> BuildParameters()
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var propertyInfo in this.GetType().GetProperties())
            {
                if (!propertyInfo.CanRead) continue;
                var attributes = propertyInfo.GetCustomAttributes(typeof(DataMemberAttribute), false);
                var propertyName = propertyInfo.Name;
                if (attributes.Length == 1)
                {
                    var attribute = attributes[0] as DataMemberAttribute;
                    if (!string.IsNullOrEmpty(attribute.Name))
                    {
                        propertyName = attribute.Name;
                    }
                }
                var value = propertyInfo.GetValue(this, null);

                if (value != null)
                {
                    dictionary[propertyName] = value;
                }
            }
            return dictionary;
        }


    }
}
