using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Client
{
    public abstract class AccessTokenDataCacheProvider
    {

        private static readonly object _fileLock = new object();

        private static AccessTokenDataCacheProvider current;

        public static AccessTokenDataCacheProvider Current
        {
            get
            {
                // even though the internal variable is static, we are not really initializing it 
                // till the first access. This can potentially lead to a race condition. Lock to avoid that
                lock (_fileLock)
                {
                    if (current == null)
                    {
#if WP8
                        current = new AccessTokenDataIsolatedStorageCacheProvider();
#else
                        current = new FacebookSessionLocalSettingsCacheProvider();
#endif
                    }

                    return current;
                }
            }
        }

        public static void SetCacheProvider(AccessTokenDataCacheProvider provider)
        {
            current = provider;
        }

        public abstract AccessTokenData GetSessionData();

        public abstract void SaveSessionData(AccessTokenData data);

        public abstract void DeleteSessionData();

    }
}
