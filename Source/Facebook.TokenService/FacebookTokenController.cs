using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Facebook.TokenService
{
    public class FacebookTokenController : ApiController
    {


        public async Task<object> ExtendToken(string accessToken)
        {
            var appId = "";
            var appSecret = "";
            var url = "https://graph.facebook.com/oauth/access_token?" + 
                        "grant_type=fb_exchange_token" +          
                        "&client_id=" + appId +
                        "&client_secret=" + appSecret +
                        "&fb_exchange_token=" + accessToken;
        

            var client = new HttpClient();
            var json = await client.GetStringAsync(url);
            dynamic result = await JsonConvert.DeserializeObjectAsync(json);
            string longLivedAccessToken = result.something;
            return longLivedAccessToken;
        }


    }
}
