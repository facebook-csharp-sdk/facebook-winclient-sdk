using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Client
{
    public delegate void AppLinkObtainedDelegate(AppLink appLink);

    public class WebViewAppLinkResolver : AppLinkResolver
    {
        public AppLinkObtainedDelegate AppLinkObtainedEvent;

        // get applink object from source url in the background
        public void GetAppLink(Uri sourceUri)
        {
        }

        async public Task<AppLink> GetAppLinkAsync(Uri sourceUri)
        {
            return null;
        }

        private readonly string TAG_EXTRACTION_JAVASCRIPT = "function extractTags() {" +
                            "  var metaTags = document.getElementsByTagName('meta');" +
                            "  var results = [];" +
                            "  for (var i = 0; i < metaTags.length; i++) {" +
                            "    var property = metaTags[i].getAttribute('property');" +
                            "    if (property && property.substring(0, 'al:'.length) === 'al:') {" +
                            "      var tag = { \"property\": metaTags[i].getAttribute('property') };" +
                            "      if (metaTags[i].hasAttribute('content')) {" +
                            "        tag['content'] = metaTags[i].getAttribute('content');" +
                            "      }" +
                            "      results.push(tag);" +
                            "    }" +
                            "  }" +
                            "  window.external.notify(JSON.stringify(results));" +
                            "}";




        private WebBrowser appLinkCrawlerBrowser = new WebBrowser();
        public void GetAppLinkFromUrlInBackground(Uri uri)
        {
            appLinkCrawlerBrowser = new WebBrowser();
            appLinkCrawlerBrowser.IsScriptEnabled = true;
            appLinkCrawlerBrowser.ScriptNotify += AppLinkCrawlerBrowserOnScriptNotify;
            appLinkCrawlerBrowser.LoadCompleted += AppLinkCrawlerBrowserOnLoadCompleted;
            appLinkCrawlerBrowser.Navigate(uri);
        }

        private void AppLinkCrawlerBrowserOnLoadCompleted(object sender, NavigationEventArgs navigationEventArgs)
        {
            appLinkCrawlerBrowser.InvokeScript("eval", TAG_EXTRACTION_JAVASCRIPT);
            appLinkCrawlerBrowser.InvokeScript("extractTags");
        }

        private AppLink CreateAppLink(List<PlatformProperty> platforms)
        {
            var platformDictionary = new Dictionary<Platform, Target>();
            platformDictionary[Platform.WindowsPhone] = new Target{Platform = Platform.WindowsPhone};
            platformDictionary[Platform.Windows] = new Target {Platform = Platform.Windows};
            platformDictionary[Platform.Universal] = new Target {Platform = Platform.Universal};

            AppLink resultLink = new AppLink();
            resultLink.Targets = new List<Target>(); ;

            foreach (var platform in platforms)
            {
                //platformDictionary[platform.Property] = platform.Content;
                switch (platform.Property)
                {
                    case "fb:app_id":
                        break;
                    case "al:windows_phone:url":
                        platformDictionary[Platform.WindowsPhone].Uri = platform.Content;
                        break;
                    case "al:windows_phone:app_name":
                        platformDictionary[Platform.WindowsPhone].Name = platform.Content;
                        break;
                    case "al:windows:url":
                        platformDictionary[Platform.Windows].Uri = platform.Content;
                        break;
                    case "al:windows:app_name":
                        platformDictionary[Platform.Windows].Name = platform.Content;
                        break;
                    case "al:windows_universal:url":
                        platformDictionary[Platform.Universal].Uri = platform.Content;
                        break;
                    case "al:windows_universal:app_name":
                        platformDictionary[Platform.Universal].Name = platform.Content;
                        break;
                    default:
                        continue;
                }

                resultLink.Targets.Clear();
                foreach (var key in platformDictionary.Keys)
                {
                    resultLink.Targets.Add(platformDictionary[key]);
                }
            }

            // the order of preference in the app links is:
            // 1. Platform
            // 2. Universal
            // 3. Web
            // 4. return what the user passed in
            return resultLink;
        }

        private void AppLinkCrawlerBrowserOnScriptNotify(object sender, NotifyEventArgs notifyEventArgs)
        {
            if (AppLinkObtainedEvent != null)
            {
                var platformCollections = JsonConvert.DeserializeObject<List<PlatformProperty>>(notifyEventArgs.Value);

                var appLink = CreateAppLink(platformCollections);

                AppLinkObtainedEvent(appLink);
            }
        }
    }
}
