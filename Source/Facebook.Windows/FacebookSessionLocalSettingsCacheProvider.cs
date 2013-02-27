using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Facebook.Apps
{
    public class FacebookSessionLocalSettingsCacheProvider : FacebookSessionCacheProvider
    {
        const string key = "FACEBOOK_SESSION";

        public override async Task<FacebookSession> GetSessionData()
        {
            var settings = ApplicationData.Current.LocalSettings;
            if (!settings.Values.ContainsKey(key))
            {
                return null;
            }

            var json = settings.Values[key] as string;
            var session = SimpleJson.DeserializeObject<FacebookSession>(json);
            return session;
        }

        public override async Task SaveSessionData(FacebookSession data)
        {
            var json = SimpleJson.SerializeObject(data);
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values[key] = json;
        }

        public override async Task DeleteSessionData()
        {
            var settings = ApplicationData.Current.LocalSettings;
            if (settings.Values.ContainsKey(key))
            {
                settings.Values[key] = null;
            }
        }
    }
}
