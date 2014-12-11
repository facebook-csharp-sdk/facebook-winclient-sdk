using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Client
{
    public class WebViewAppLinkResolver : AppLinkResolver
    {
        // get applink object from source url in the background
        public AppLink GetAppLink(Uri sourceUri)
        {
            return null;
        }

        async public Task<AppLink> GetAppLinkAsync(Uri sourceUri)
        {
            return null;
        }
    }
}
