using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Client
{
    public class FacebookAppLinkResolver : AppLinkResolver
    {
        async public Task<AppLink> GetAppLinkAsync(string accessToken, string sourceUrl)
        {
            FacebookClient _client = new FacebookClient(accessToken);
            
            String finalUrl = String.Format("https://graph.facebook.com/v2.0/?ids={0}&type=al&fields=windows,windows_phone,windows_universal", sourceUrl);
            dynamic appLinkData = await _client.GetTaskAsync(finalUrl);
            List<Target> targets = new List<Target>();

            var outerShell = (IDictionary<string, object>)appLinkData[sourceUrl];
            if (outerShell.ContainsKey("windows"))
            {
                var windowsArray = (IEnumerable<object>)outerShell["windows"];
                var windowsObject = (IDictionary<string, object>)(windowsArray.First());
                targets.Add(new Target { Uri = (string)windowsObject["url"], Platform = Platform.Windows, Name = (string)windowsObject["app_name"] });
            }

            if (outerShell.ContainsKey("windows_phone"))
            {
                var windowsPhoneArray = (IEnumerable<object>)outerShell["windows_phone"];
                var windowsPhoneObject = (IDictionary<string, object>)(windowsPhoneArray.First());
                targets.Add(new Target { Uri = (string)windowsPhoneObject["url"], Platform = Platform.WindowsPhone, Name = (string)windowsPhoneObject["app_name"] });
            }

            if (outerShell.ContainsKey("windows_universal"))
            {
                var universalArray = (IEnumerable<object>)outerShell["windows_universal"];
                var universalObject = (IDictionary<string, object>)(universalArray.First());
                targets.Add(new Target { Uri = (string)universalObject["url"], Platform = Platform.Universal, Name = (string)universalObject["app_name"] });
            }

            return new AppLink {SourceUri =  sourceUrl, FallbackUri = sourceUrl, Targets = targets};
        }
    }



    public class PlatformInfo
    {
        public string AppName { get; set; }

        public string Url { get; set; }
    }

    public class AppLinkObject
    {
        public List<PlatformInfo> Windows { get; set; }

        public List<PlatformInfo> WindowsPhone { get; set; }

        public List<PlatformInfo> WindowsUniversal { get; set; }

        public String Id { get; set; }
    }
}
