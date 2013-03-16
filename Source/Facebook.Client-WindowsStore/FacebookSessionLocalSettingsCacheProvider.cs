using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;

namespace Facebook.Client
{
    public class FacebookSessionLocalSettingsCacheProvider : FacebookSessionCacheProvider
    {
        const string key = "FACEBOOK_SESSION";

        public override FacebookSession GetSessionDataAsync()
        {
            var settings = ApplicationData.Current.LocalSettings;
            if (!settings.Values.ContainsKey(key))
            {
                return null;
            }

            FacebookSession session;
            var json = settings.Values[key] as string;
            var serializer = new XmlSerializer(typeof(FacebookSession));
            using (var reader = new StringReader(json))
            {
                session = serializer.Deserialize(reader) as FacebookSession;
            }
            return session;
        }

        public override void SaveSessionDataAsync(FacebookSession data)
        {
            string json;
            var serializer = new XmlSerializer(typeof(FacebookSession));
            using (StringWriter writer = new StringWriter()) {
                serializer.Serialize(writer, data);
                json = writer.ToString();
            }
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values[key] = json;
        }

        public override void DeleteSessionDataAsync()
        {
            var settings = ApplicationData.Current.LocalSettings;
            if (settings.Values.ContainsKey(key))
            {
                settings.Values[key] = null;
            }
        }
    }
}
