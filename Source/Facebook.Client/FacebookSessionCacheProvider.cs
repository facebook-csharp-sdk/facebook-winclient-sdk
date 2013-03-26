using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Client
{
    public abstract class FacebookSessionCacheProvider
    {

        private static FacebookSessionCacheProvider current;

        public static FacebookSessionCacheProvider Current
        {
            get
            {
                if (current == null)
                {
#if WINDOWS_PHONE
                    current = new FacebookSessionIsolatedStorageCacheProvider();
#else
                    current = new FacebookSessionRoamingSettingsCacheProvider();
#endif
                }
                return current;
            }
        }

        public static void SetCacheProvider(FacebookSessionCacheProvider provider)
        {
            current = provider;
        }

        public abstract FacebookSession GetSessionData();

        public abstract void SaveSessionData(FacebookSession data);

        public abstract void DeleteSessionData();

    }
}
