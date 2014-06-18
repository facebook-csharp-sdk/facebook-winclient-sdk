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

        private static AppLinkResolver _appLinkResolver = new FacebookAppLinkResolver();

        public static AppLinkResolver DefaultResolver
        {
            get { return _appLinkResolver; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("The default resolver cannot be null");
                }

                _appLinkResolver = value;
            }
        }

        // synchronous navigate - no parameters          - instanced
        public void Navigate()
        {
        }

        // sync navigate - that takes a applink parameter           - static, navigate baesd on supplied data

        public static void Navigate(String Url)
        {

        }

        async public static void NavigateAsync(AppLink appLink)
        {
            string navigationLinkUrl = String.Empty;

            if (appLink != null)
            {
                foreach (var target in appLink.Targets)
                {
#if WINDOWS_PHONE
                    if (target.Platform == Platform.WindowsPhone)
                    {
                        navigationLinkUrl = target.Uri;
                        break;
                    }
#else
                    if (target.Platform == Platform.Windows)
                    {
                        navigationLinkUrl = target.Uri;
                        break;
                    }
#endif

                }
            }

            if (!String.IsNullOrEmpty(navigationLinkUrl))
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri(navigationLinkUrl));
            }
            else
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri(appLink.FallbackUri));
            }
        }
    }
}
