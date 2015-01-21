using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Client
{
    public class AppLinkNavigation
    {
        public AppLink AppLink { get; set; }

        public Dictionary<object, object> AppLinkData { get; set; }

        // getExtra - closure - opaque blob
        public Dictionary<object, object> Extra { get; set; }

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

        async public static Task<bool> NavigateAsync(AppLink appLink)
        {
            string navigationLinkUrl = String.Empty;

            if (appLink != null)
            {
                foreach (var target in appLink.Targets)
                {
#if WP8
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
                // If we found a platform specific app link, launch it
                await Windows.System.Launcher.LaunchUriAsync(new Uri(navigationLinkUrl));
            }
            else
            {
                // no platform specific applink was found, launch the fallback url
                await Windows.System.Launcher.LaunchUriAsync(new Uri(appLink.FallbackUri));
            }

            return false;
        }
    }
}
