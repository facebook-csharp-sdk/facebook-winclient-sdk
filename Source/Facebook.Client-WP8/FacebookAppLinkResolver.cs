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
        //public Task<AppLink> _GetAppLinkAsync(string accessToken, string sourceUrl)
        //{
        //    var tcs = new TaskCompletionSource<AppLink>();

        //    String finalUrl = String.Format("https://graph.facebook.com/v2.0/?ids={0}&type=al&access_token={1}&fields=windows,windows_phone,windows_universal", sourceUrl, accessToken);

        //    var client = new WebClient();
        //    client.DownloadStringCompleted += (s, e) =>
        //    {
        //        if (e.Error == null)
        //        {
        //            //tcs.SetResult(JsonConvert.DeserializeObject<AppLink>(e.Result));
        //            AppLinkObject appLinkObject = ParseAppLinkData(e.Result);
        //            List<Target> targets = new List<Target> ();
        //            targets.Add(new Target { Uri = appLinkObject.Windows[0].Url, Platform = Platform.Windows, Name = appLinkObject.Windows[0].AppName });
        //            targets.Add(new Target { Uri = appLinkObject.WindowsPhone[0].Url, Platform = Platform.WindowsPhone, Name = appLinkObject.WindowsPhone[0].AppName });
        //            targets.Add(new Target { Uri = appLinkObject.WindowsUniversal[0].Url, Platform = Platform.Universal, Name = appLinkObject.WindowsUniversal[0].AppName });

        //            tcs.SetResult(new AppLink {SourceUri =  appLinkObject.Id, FallbackUri = appLinkObject.Id, Targets = targets});
        //        }
        //        else
        //        {
        //            tcs.SetException(e.Error);
        //        }
        //    };

        //    client.DownloadStringAsync(new Uri(finalUrl));

        //    return tcs.Task;
        //}


        async public Task<AppLink> GetAppLinkAsync(string accessToken, string sourceUrl)
        {
            FacebookClient _client = new FacebookClient(accessToken);

            
            String finalUrl = String.Format("https://graph.facebook.com/v2.0/?ids={0}&type=al&fields=windows,windows_phone,windows_universal", sourceUrl);
            dynamic appLinkData = await _client.GetTaskAsync(finalUrl);

            var outerShell = appLinkData[sourceUrl];
            var windowsObject = outerShell["windows"];
            var windowsPhoneObject = outerShell["windows_phone"];
            var universalProject = outerShell["windows_universal"];

            List<Target> targets = new List<Target>();
            targets.Add(new Target { Uri = windowsObject[0]["url"], Platform = Platform.Windows, Name = windowsObject[0]["app_name"] });
            targets.Add(new Target { Uri = windowsPhoneObject[0]["url"], Platform = Platform.WindowsPhone, Name = windowsPhoneObject[0]["app_name"] });
            targets.Add(new Target { Uri = universalProject[0]["url"], Platform = Platform.Universal, Name = universalProject[0]["app_name"] });

            return new AppLink {SourceUri =  sourceUrl, FallbackUri = sourceUrl, Targets = targets};
        }


    //    public AppLinkObject ParseAppLinkData(string jsonString)
    //    {
    //        var o = JObject.Parse(jsonString);

    //        //var x = o.SelectToken("windows");

    //        foreach (JToken child in o.Children())
    //        {
    //            foreach (JToken grandChild in child)
    //            {
    //                //foreach (JToken grandGrandChild in grandChild)
    //                //{
    //                //    var property = grandGrandChild as JProperty;
    //                //    if (property != null)
    //                //    {
    //                //        Console.WriteLine(property.Name + property.Value);
    //                //    }
    //                //}
    //                AppLinkObject obj = JsonConvert.DeserializeObject<AppLinkObject>(grandChild.ToString());

    //                return obj;
    //            }
    //        }

    //        return null;
    //    }
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
