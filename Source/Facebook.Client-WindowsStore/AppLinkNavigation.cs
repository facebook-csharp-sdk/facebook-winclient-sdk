using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Client
{
    public class AppLinkNavigation
    {
        // private static defaultResolver - you set this to what you want - by default the WebViewResolver

        // enum - navigationResult {failed, web, app}

        // roughly equivalent to http request i.e. URL + header + query parameters + body (referer)
        // getAppLink - get the app link object association with this request
        public AppLink AppLink { get; set; }

        // getAppLinkData - the raw JSON data
        public Dictionary<object, object> AppLinkData { get; set; }

        // getExtra - closure - opaque blob
        public Dictionary<object, object> Extra { get; set; }

        // setDEfaultResolver
        // getDefaultResolver
        public AppLinkResolver DefaultResolver { get; set; }

        // synchronous navigate - no parameters          - instanced
        public void Navigate()
        {
        }

        // sync navigate - that takes a applink parameter           - static, navigate baesd on supplied data

        public static void Navigate(String Url)
        {

        }

        public static void Navigate(AppLink appLink)
        {

        }

        // static method - navigate in background 1. takes applink, 2. takes source url and resolver,  3. sring url and resolver 4. navigate in bkground with default resolver and URL 5. navigate in background with default resolver and string 
        public static void Navigate(String sourceUrl, AppLinkResolver resolver)
        {

        }

        public static void Navigate(Uri sourceUrl, AppLinkResolver resolver)
        {

        }

        public async static void NavigateAsync(String sourceUrl, AppLinkResolver resolver)
        {

        }

        public async static void NavigateAsync(Uri sourceUrl, AppLinkResolver resolver)
        {

        }
    }
}
