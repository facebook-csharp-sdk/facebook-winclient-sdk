using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Facebook.Client
{
    public class FacebookAppLinkResolver
    {
        public Task<AppLink> GetAppLinkAsync(string accessToken, string sourceUrl)
        {
            var tcs = new TaskCompletionSource<AppLink>();

            String finalUrl = String.Format("https://graph.facebook.com/v2.0/?ids={0}&type=al&access_token={1}&fields=windows,windows_phone,windows_universal", sourceUrl, accessToken);

            var client = new WebClient();
            client.DownloadStringCompleted += (s, e) =>
            {
                if (e.Error == null)
                {
                    //tcs.SetResult(JsonConvert.DeserializeObject<AppLink>(e.Result));
                    AppLinkObject appLinkObject = ParseAppLinkData(e.Result);
                    List<Target> targets = new List<Target> ();
                    targets.Add(new Target { Uri = appLinkObject.Windows[0].Url, Platform = Platform.Windows, Name = appLinkObject.Windows[0].AppName });
                    targets.Add(new Target { Uri = appLinkObject.WindowsPhone[0].Url, Platform = Platform.WindowsPhone, Name = appLinkObject.WindowsPhone[0].AppName });
                    targets.Add(new Target { Uri = appLinkObject.WindowsUniversal[0].Url, Platform = Platform.Universal, Name = appLinkObject.WindowsUniversal[0].AppName });

                    tcs.SetResult(new AppLink {SourceUri =  appLinkObject.Id, FallbackUri = appLinkObject.Id, Targets = targets});
                }
                else
                {
                    tcs.SetException(e.Error);
                }
            };

            client.DownloadStringAsync(new Uri(finalUrl));

            return tcs.Task;
        }

        public AppLinkObject ParseAppLinkData(string jsonString)
        {
            var o = JObject.Parse(jsonString);

            //var x = o.SelectToken("windows");

            foreach (JToken child in o.Children())
            {
                foreach (JToken grandChild in child)
                {
                    //foreach (JToken grandGrandChild in grandChild)
                    //{
                    //    var property = grandGrandChild as JProperty;
                    //    if (property != null)
                    //    {
                    //        Console.WriteLine(property.Name + property.Value);
                    //    }
                    //}
                    AppLinkObject obj = JsonConvert.DeserializeObject<AppLinkObject>(grandChild.ToString());

                    return obj;
                }
            }

            return null;
        }
    }



    public class PlatformInfo
    {
        [JsonProperty(PropertyName = "app_name")]
        public string AppName { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
    }

    public class AppLinkObject
    {
        [JsonProperty(PropertyName = "windows")]
        public List<PlatformInfo> Windows { get; set; }

        [JsonProperty(PropertyName = "windows_phone")]
        public List<PlatformInfo> WindowsPhone { get; set; }

        [JsonProperty(PropertyName = "windows_universal")]
        public List<PlatformInfo> WindowsUniversal { get; set; }

        [JsonProperty (PropertyName = "id")]
        public String Id { get; set; }
    }
}
