//-----------------------------------------------------------------------
// <copyright file="FacebookSessionClient.cs" company="The Outercurve Foundation">
//    Copyright (c) 2011, The Outercurve Foundation. 
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//      http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>
// <author>Sanjeev Dwivedi (sanjeev.dwivedi.net), Prabir Shrestha (prabir.me) and Nathan Totten (ntotten.com)</author>
// <website>https://github.com/facebook-csharp-sdk/facbook-winclient-sdk</website>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#if NETFX_CORE
using Windows.Security.Authentication.Web;
#endif
using System.Windows;
using System.Windows.Controls.Primitives;
using Windows.System;
using Facebook;
using Facebook.Client.Controls.WebDialog;

namespace Facebook.Client
{
    public enum FacebookLoginBehavior
    {
        LoginBehaviorApplicationOnly,
        LoginBehaviorMobileInternetExplorerOnly,
        LoginBehaviorWebViewOnly,
#if WINDOWS_UNIVERSAL
        LoginBehaviorAppwithMobileInternetFallback
#endif
    }

    public enum FacebookWebDialog
    {
        AppRequests,
        Feed,
        Pay
    }

    public delegate void FacebookAuthenticationDelegate(FacebookSession session);

    public class FacebookSessionClient
    {
        // don't want this class instantiated without an app ID
        public FacebookSessionClient()
        {
           
        }


        public static FacebookAuthenticationDelegate OnFacebookAuthenticationFinished;

        public static string AppId { get; private set; }
        public bool LoginInProgress { get; set; }

        private static FacebookSession _currentSession = null;
        private static bool firstRun = true;
        public static FacebookSession CurrentSession 
        {
            get
            {
                if (firstRun)
                {
                    FacebookSession tmpSession = FacebookSessionCacheProvider.Current.GetSessionData();
                    if (tmpSession == null)
                    {
                        _currentSession = new FacebookSession();
                    }
                    else
                    {
                        _currentSession = tmpSession;
                    }

                    firstRun = false;
                }

                return _currentSession;
            }

            set
            {
                _currentSession = value;
                FacebookSessionCacheProvider.Current.SaveSessionData(_currentSession);
            }
        }

        public FacebookSessionClient(string appId)
        {
            if (String.IsNullOrEmpty(appId))
            {
                throw new ArgumentNullException("appId");
            }
            AppId = appId;

            // Also save it as part of the static session object
            CurrentSession.AppId = appId;
        }

#if WP8
        public void LoginWithApp()
        {
            LoginWithApp(null);
        }

        public void LoginWithApp(string permissions)
        {
            LoginWithApp(permissions, null);
        }

        public void LoginWithApp(string permissions, string state)
        {
            AppAuthenticationHelper.AuthenticateWithApp(AppId, permissions, state);
        }
#endif

#if WINDOWS_UNIVERSAL
        async public Task LoginWithApp()
        {
            await LoginWithApp(null);
        }

        async public Task LoginWithApp(string permissions)
        {
            await LoginWithApp(permissions, null);
        }

        async public Task LoginWithApp(string permissions, string state)
        {
            await AppAuthenticationHelper.AuthenticateWithApp(this.AppId, permissions, state);
        }
#endif

        public static void ShowAppRequestsDialog()
        {
            Popup dialogPopup = new Popup();

            var webDialog = new WebDialogUserControl();
            
            webDialog.ParentControlPopup = dialogPopup;
            dialogPopup.Child = webDialog;

            // Set where the popup will show up on the screen.
            dialogPopup.VerticalOffset = 40;
            dialogPopup.HorizontalOffset = 0;

            dialogPopup.Height = Application.Current.Host.Content.ActualHeight - 40;
            dialogPopup.Width = Application.Current.Host.Content.ActualWidth;

            webDialog.Height = dialogPopup.Height;
            webDialog.Width = dialogPopup.Width;


            webDialog.ShowAppRequestsDialog();

            // Open the popup.
            dialogPopup.IsOpen = true;
        }

        public static void ShowFeedDialog()
        {
            Popup dialogPopup = new Popup();

            var webDialog = new WebDialogUserControl();

            webDialog.ParentControlPopup = dialogPopup;
            dialogPopup.Child = webDialog;

            // Set where the popup will show up on the screen.
            dialogPopup.VerticalOffset = 40;
            dialogPopup.HorizontalOffset = 0;

            dialogPopup.Height = Application.Current.Host.Content.ActualHeight - 40;
            dialogPopup.Width = Application.Current.Host.Content.ActualWidth;

            webDialog.Height = dialogPopup.Height;
            webDialog.Width = dialogPopup.Width;


            webDialog.ShowFeedDialog();

            // Open the popup.
            dialogPopup.IsOpen = true;
        }

        public async Task<FacebookSession> LoginAsync()
        {
            return await LoginAsync(null, false);
        }

        public async Task<FacebookSession> LoginAsync(string permissions)
        {
            return await LoginAsync(permissions, false);
        }

        async public void LoginWithBehavior(string permissions, FacebookLoginBehavior behavior)
        {
            switch (behavior)
            {
                case FacebookLoginBehavior.LoginBehaviorMobileInternetExplorerOnly:
                {
                    String appId = await AppAuthenticationHelper.GetFacebookConfigValue("Facebook", "AppId");
                    Uri uri =
                        new Uri(
                            String.Format(
                                "https://m.facebook.com/v1.0/dialog/oauth?redirect_uri={0}%3A%2F%2Fauthorize&display=touch&state=%7B%220is_active_session%22%3A1%2C%22is_open_session%22%3A1%2C%22com.facebook.sdk_client_state%22%3A1%2C%223_method%22%3A%22browser_auth%22%7D&scope=email%2Cbasic_info&type=user_agent&client_id={1}&ret=login&sdk=ios&ext=1413580961&hash=Aeb0Q3uVJ6pgMh4C&refsrc=https%3A%2F%2Fm.facebook.com%2Flogin.php&refid=9&_rdr",
                                String.Format("fb{0}", appId), appId), UriKind.Absolute);
                    Launcher.LaunchUriAsync(uri);
                    break;
                }
                case FacebookLoginBehavior.LoginBehaviorWebViewOnly:
                {
                    // TODO: What to do here? LoginAsync returns inproc. Login with IE returns out of proc?
                    // LoginAsync()
                    break;
                }
                case FacebookLoginBehavior.LoginBehaviorApplicationOnly:
                {
                    // LoginWithApp
                    break;
                }
            }
        }

        /*
         * Token Extension trace from Facebook
            User-Agent: FacebookiOSSDK.3.17.1
            Content-Type: multipart/form-data; boundary=3i2ndDfv2rTHiSisAbouNdArYfORhtTPEefj3q2f
        

            --3i2ndDfv2rTHiSisAbouNdArYfORhtTPEefj3q2f
            Content-Disposition: form-data; name="batch_app_id"

            540541885996234
            --3i2ndDfv2rTHiSisAbouNdArYfORhtTPEefj3q2f
            Content-Disposition: form-data; name="batch"

           [{"method":"GET","relative_url":"oauth\/access_token?sdk=ios&grant_type=fb_extend_sso_token&access_token=CAAHrnrcZA3MoBALWrjPGIniC0SyBnLr6KWjFgZA4ZC0W5X6CRLVipoH5ZCQo62F1jTcjOwmJlrSW3gjFQsWMzJzHafaROj2cZAw9l26FDmqgKTG5hetZA2QZAmETsXZA90qKXWXvOBUToC7KDFIOZAlndcJpARDb7ZAhZAMbr3QzZAGgbq6CwqRllLAc4nZCZC9a8qAbIZD&sdk=ios"}]

           --3i2ndDfv2rTHiSisAbouNdArYfORhtTPEefj3q2
        */

        public async static  Task CheckAndExtendTokenIfNeeded()
        {
            // get the existing token
            if (String.IsNullOrEmpty(CurrentSession.AccessToken))
            {
                // If there is no token, do nothing
                return;
            }

            // check if its issue date is over 24 hours and if so, renew it
            if (DateTime.UtcNow - CurrentSession.Issued > TimeSpan.FromHours(24)) // one day 
            {
                var client = new HttpClient();
                String tokenExtendUri = "https://graph.facebook.com/v2.1";
                client.BaseAddress = new Uri(tokenExtendUri);

                var request = new HttpRequestMessage();

                var mfdc = new MultipartFormDataContent();
                mfdc.Add(new StringContent("540541885996234"), name: "batch_app_id");

                String extensionString = "[{\"method\":\"GET\",\"relative_url\":\"oauth\\/access_token?grant_type=fb_extend_sso_token&access_token=" + CurrentSession.AccessToken + "\"}]";
                mfdc.Add(new StringContent(extensionString), name: "batch");

                HttpResponseMessage response = await client.PostAsync(tokenExtendUri, mfdc);
                String resultContent = await response.Content.ReadAsStringAsync();

                var result = SimpleJson.DeserializeObject(resultContent);

                // extract the access token and save it in the session
                var data = (List<object>)result;

                var dictionary = (IDictionary<string, object>)data[0];
                var code = (long)dictionary["code"];
                if (code == 200)
                {
                    // the API succeeded
                    var body = (IDictionary<string, object>) SimpleJson.DeserializeObject((string) dictionary["body"]);
                    var access_token = (string) body["access_token"];
                    var expires_at = (long) body["expires_at"];

                    var session = new FacebookSession();
                    // token extension failed...
                    session.AccessToken = access_token;

                    // parse out other types
                    long expiresInValue;
                    var now = DateTime.UtcNow;
                    session.Expires = now + TimeSpan.FromSeconds(expires_at);
                    session.Issued = now - (TimeSpan.FromDays(60) - TimeSpan.FromSeconds(expires_at));
                    session.AppId = AppId;

                    // Assign the session object over, this saves it to the disk as well.
                    CurrentSession = session;
                }
                else
                {

                }
            }


        }

        internal async Task<FacebookSession> LoginAsync(string permissions, bool force)
        {
            if (this.LoginInProgress)
            {
                throw new InvalidOperationException("Login in progress.");
            }

            this.LoginInProgress = true;
            try
            {
                var session = FacebookSessionCacheProvider.Current.GetSessionData();
                if (session == null)
                {
                    // Authenticate
                    var authResult = await PromptOAuthDialog(permissions, WebAuthenticationOptions.None);

                    FacebookClient client = new FacebookClient(authResult.AccessToken);
                    var parameters = new Dictionary<string, object>();
                    parameters["fields"] = "id";

                    var result = await client.GetTaskAsync("me", parameters);
                    var dict = (IDictionary<string, object>)result;

                    session = new FacebookSession
                    {
                        AccessToken = authResult.AccessToken,
                        Expires = authResult.Expires,
                        FacebookId = (string)dict["id"],
                    };
                  
                }
                else
                {
                    // Check if we are requesting new permissions
                    bool newPermissions = false;
                    if (!string.IsNullOrEmpty(permissions))
                    {
                        var p = permissions.Split(',');
                        newPermissions = session.CurrentPermissions.Join(p, s1 => s1, s2 => s2, (s1, s2) => s1).Count() != p.Length;
                    }

                    // Prompt OAuth dialog if force renew is true or
                    // if new permissions are requested or 
                    // if the access token is expired.
                    if (force || newPermissions || session.Expires <= DateTime.UtcNow)
                    {
                        var authResult = await PromptOAuthDialog(permissions, WebAuthenticationOptions.None);
                        if (authResult != null)
                        {
                            session.AccessToken = authResult.AccessToken;
                            session.Expires = authResult.Expires;
                        }
                    }
                }

                // Set the current known permissions
                if (!string.IsNullOrEmpty(permissions))
                {
                    var p = permissions.Split(',');
                    session.CurrentPermissions = session.CurrentPermissions.Union(p).ToList();
                }

                // Save session data
                FacebookSessionCacheProvider.Current.SaveSessionData(session);
                CurrentSession = session;
            }
            finally
            {
                this.LoginInProgress = false;
            }

            return CurrentSession;
        }

        /// <summary>
        /// Log a user out of Facebook.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Logout", Justification = "Logout is preferred by design")]
        public void Logout()
        {
            try
            {
                FacebookSessionCacheProvider.Current.DeleteSessionData();
            }
            finally
            {
                CurrentSession = null;
            }
        }

        private async Task<FacebookOAuthResult> PromptOAuthDialog(string permissions, WebAuthenticationOptions options)
        {
            // Use WebAuthenticationBroker to launch server side OAuth flow

            Uri startUri = this.GetLoginUrl(permissions);
            Uri endUri = new Uri("https://www.facebook.com/connect/login_success.html");

            var result = await WebAuthenticationBroker.AuthenticateAsync(options, startUri, endUri);


            if (result.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
            {
                throw new InvalidOperationException();
            }
            else if (result.ResponseStatus == WebAuthenticationStatus.UserCancel)
            {
                throw new InvalidOperationException();
            }

            var client = new FacebookClient();
            var authResult = client.ParseOAuthCallbackUrl(new Uri(result.ResponseData));
            return authResult;
        }

        private Uri GetLoginUrl(string permissions)
        {
            var parameters = new Dictionary<string, object>();
            parameters["client_id"] = AppId;
            parameters["redirect_uri"] = "https://www.facebook.com/connect/login_success.html";
            parameters["response_type"] = "token";
#if WINDOWS_PHONE
            parameters["display"] = "touch";
            parameters["mobile"] = true;
#else
            parameters["display"] = "popup";
#endif

            // add the 'scope' only if we have extendedPermissions.
            if (!string.IsNullOrEmpty(permissions))
            {
                // A comma-delimited list of permissions
                parameters["scope"] = permissions;
            }

            var client = new FacebookClient();
            return client.GetLoginUrl(parameters);
        }
    }
}
