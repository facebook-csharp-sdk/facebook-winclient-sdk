using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Apps
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
                    current = new FacebookSessionLocalSettingsCacheProvider();
#endif
                }
                return current;
            }
        }

        public static void SetCacheProvider(FacebookSessionCacheProvider provider)
        {
            current = provider;
        }

        public abstract Task<FacebookSession> GetSessionData();

        public abstract Task SaveSessionData(FacebookSession data);

        public abstract Task DeleteSessionData();

    }
}
