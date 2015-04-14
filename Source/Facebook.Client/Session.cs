//-----------------------------------------------------------------------
// <copyright file="Session.cs" company="The Outercurve Foundation">
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
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

#if WINDOWS80 || WINDOWS_PHONE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
#endif

using Facebook.Client.Controls.WebDialog;
#if NETFX_CORE
using Windows.Security.Authentication.Web;
#endif
using System.Windows;

#if WP8 || WINDOWS_PHONE
using Facebook.Client.Controls.WebDialog;
#endif

#if WINDOWS_UNIVERSAL
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if WP8
using System.Windows.Controls.Primitives;
#endif

using Windows.System;
using Facebook;

using Windows.Graphics.Display;


namespace Facebook.Client
{
    public enum FacebookLoginBehavior
    {
        LoginBehaviorApplicationOnly,
        LoginBehaviorMobileInternetExplorerOnly,
        LoginBehaviorWebViewOnly,
        LoginBehaviorAppwithMobileInternetFallback,
        LoginBehaviorWebAuthenticationBroker
    }

    public enum WebDialogResult
    {
        WebDialogResultDialogCompleted,
        WebDialogResultDialogNotCompleted
    };

    public enum FacebookWebDialog
    {
        AppRequests,
        Feed,
        Pay
    }

    internal enum LoginStatus
    {
        LoggedIn,
        LoggedOut
    }

    public delegate void FacebookAuthenticationDelegate(AccessTokenData session);
    public delegate void FacebookDelegate(FBResult result);

    public delegate void WebDialogFinishedDelegate(WebDialogResult result);

    public delegate void DismissDialogDelegate(Uri uri);

    internal delegate void SessionStateChanged(LoginStatus status);

    public class FBResult
    {
        public string Error { get; set; }
        public string Text { get; set; }
        public JsonObject Json { get; set; }
    }

    public class Session
    {
        public Session()
        {
            // Initialize the ActiveSession etc. right here
        }

        public Session(string appId)
        {
            if (String.IsNullOrEmpty(appId))
            {
                throw new ArgumentNullException("appId");
            }

            // Also save it as part of the static session object
            CurrentAccessTokenData.AppId = appId;
        }

        public static Session ActiveSession = new Session();

        public static FacebookAuthenticationDelegate OnFacebookAuthenticationFinished;
        public static FacebookDelegate OnFacebookAppRequestFinished;
        public static FacebookDelegate OnFacebookFeedFinished;

        internal static SessionStateChanged OnSessionStateChanged;

        /// <summary>
        /// Facebook AppId, obtained from FacebookConfig.xml if it exist.
        /// </summary>
        public static string AppId
        {
            get
            {

                if (string.IsNullOrEmpty(Session.ActiveSession.CurrentAccessTokenData.AppId))
                {
                    var task = Task.Run(async () => await AppAuthenticationHelper.GetFacebookConfigValue("Facebook", "AppId"));
                    task.Wait();
                    Session.ActiveSession.CurrentAccessTokenData.AppId = task.Result;
                }

                return Session.ActiveSession.CurrentAccessTokenData.AppId;
            }
            set
            {
                Session.ActiveSession.CurrentAccessTokenData.AppId = value;
            }
        }

        public static string LoginRedirectUri
        {
            get
            {
                return string.Format("fb{0}%3A%2F%2Fauthorize", AppId);
            }
        }
        public static string AppRequestRedirectUri { get { return string.Format("fb{0}%3A%2F%2Fapprequest", AppId); } }
        public static string FeedRedirectUri { get { return string.Format("fb{0}%3A%2F%2Ffeed", AppId); } }

        public bool LoginInProgress { get; set; }

        internal string RedirectPageOnSuccess = "MainPage.xaml";


        private AccessTokenData _currentAccessTokenData = null;
        private bool firstRun = true;

        /// <summary>
        /// The Access Token information for the last time the app ran
        /// </summary>
        public AccessTokenData CurrentAccessTokenData 
        {
            get
            {
                if (firstRun)
                {
                    AccessTokenData tmpSession = AccessTokenDataCacheProvider.Current.GetSessionData();
                    if (tmpSession == null)
                    {
                        _currentAccessTokenData = new AccessTokenData();
                        _currentAccessTokenData.AccessToken = String.Empty;
                        _currentAccessTokenData.FacebookId = String.Empty;
                    }
                    else
                    {
                        _currentAccessTokenData = tmpSession;
                    }

                    firstRun = false;
                }

                if (_currentAccessTokenData != null)
                {
                    if (!String.IsNullOrEmpty(_currentAccessTokenData.AccessToken)
                        && _currentAccessTokenData.Expires < DateTime.UtcNow)
                    {
                        _currentAccessTokenData.AccessToken = String.Empty;
                        _currentAccessTokenData.CurrentPermissions = new List<string>();
                        _currentAccessTokenData.Expires = DateTime.MinValue;
                        _currentAccessTokenData.FacebookId = String.Empty;
                        _currentAccessTokenData.Issued = DateTime.MinValue;

                    }
                }

                return _currentAccessTokenData;
            }

            set
            {
                _currentAccessTokenData = value;
                if (value == null)
                {
                    AccessTokenDataCacheProvider.Current.DeleteSessionData();
                    if (Session.OnSessionStateChanged != null)
                    {
                        OnSessionStateChanged(LoginStatus.LoggedOut);
                    }

                    return;
                }

                AccessTokenDataCacheProvider.Current.SaveSessionData(_currentAccessTokenData);
            }
        }



#if WP8 || WINDOWS_PHONE
        internal void LoginWithApp()
        {
            LoginWithApp(null);
        }

        internal void LoginWithApp(string permissions)
        {
            LoginWithApp(permissions, null);
        }

        internal void LoginWithApp(string permissions, string state)
        {
            AppAuthenticationHelper.AuthenticateWithApp(Session.AppId, permissions, state);
        }
#endif

//#if WINDOWS_UNIVERSAL
//        async internal Task LoginWithApp()
//        {
//            await LoginWithApp(null);
//        }

//        async internal Task LoginWithApp(string permissions)
//        {
//            await LoginWithApp(permissions, null);
//        }

//        async internal Task LoginWithApp(string permissions, string state)
//        {
//            await AppAuthenticationHelper.AuthenticateWithApp(Session.AppId, permissions, state);
//        }
//#endif



#if !WINDOWS
        public static void ShowAppRequestDialogViaBrowser(string message, string title = "", List<string> idList=null, string data = "")
        {
            WebDialogUserControl.ShowAppRequestDialogViaBrowser(message, title, idList, data);
        }

        public static void ShowFeedDialogViaBrowser(string toId = "", string link = "", string linkName = "", string linkCaption = "", string linkDescription = "", string picture = "")
        {
            WebDialogUserControl.ShowFeedDialogViaBrowser(toId, link, linkName, linkCaption, linkDescription, picture);
        }
#endif
        private static Popup dialogPopup = new Popup();

        public static bool IsDialogOpen
        {
            get { return dialogPopup.IsOpen; }
        }

        public static void CloseWebDialog()
        {
            dialogPopup.IsOpen = false;
        }

        public static void ShowAppRequestsDialog(WebDialogFinishedDelegate callback, String message="Select your friends", String title="", List<string> appIdList=null)
        {
            var webDialog = new WebDialogUserControl();
            
            webDialog.ParentControlPopup = dialogPopup;
            dialogPopup.Child = webDialog;

#if WP8 || WINDOWS_PHONE
            // Set where the popup will show up on the screen.
            dialogPopup.VerticalOffset = 40;
            dialogPopup.HorizontalOffset = 0;
#endif


#if WP8
            dialogPopup.Height = Application.Current.Host.Content.ActualHeight - 40;
            dialogPopup.Width = Application.Current.Host.Content.ActualWidth;
#endif

#if WINDOWS_PHONE
            dialogPopup.Height = Window.Current.Bounds.Height - 40;
            dialogPopup.Width = Window.Current.Bounds.Width;
#endif

#if WINDOWS
            dialogPopup.Height = Window.Current.Bounds.Height;
            dialogPopup.Width = Window.Current.Bounds.Width;
#endif


            webDialog.Height = dialogPopup.Height;
            webDialog.Width = dialogPopup.Width;


            webDialog.ShowAppRequestsDialog(callback, message, title, appIdList);

            // Open the popup.
            dialogPopup.IsOpen = true;
        }

        public static void ShowFeedDialog(string toId = "",string link = "",string linkName = "",string linkCaption = "",string linkDescription = "",string picture = "")
        {
            //dialogPopup.Loaded += dialogPopup_Loaded;

            var webDialog = new WebDialogUserControl();

            webDialog.ParentControlPopup = dialogPopup;
            dialogPopup.Child = webDialog;

#if WP8 || WINDOWS_PHONE
            // Set where the popup will show up on the screen.
            dialogPopup.VerticalOffset = 40;
            dialogPopup.HorizontalOffset = 0;
#endif

#if WP8
            dialogPopup.Height = Application.Current.Host.Content.ActualHeight - 40;
            dialogPopup.Width = Application.Current.Host.Content.ActualWidth;
#endif

#if WINDOWS_PHONE
            dialogPopup.Height = Window.Current.Bounds.Height - 40;
            dialogPopup.Width = Window.Current.Bounds.Width;
#endif

#if WINDOWS
            dialogPopup.Height = Window.Current.Bounds.Height;
            dialogPopup.Width = Window.Current.Bounds.Width;
#endif


            webDialog.Height = dialogPopup.Height;
            webDialog.Width = dialogPopup.Width;


            webDialog.ShowFeedDialog(toId, link, linkName, linkCaption, linkDescription, picture);

            // Open the popup.
            dialogPopup.IsOpen = true;
        }

        internal async Task<AccessTokenData> LoginAsync(FacebookLoginBehavior loginBehavior)
        {
            return await LoginAsync(null, false, loginBehavior);
        }

        internal async Task<AccessTokenData> LoginAsync(string permissions, FacebookLoginBehavior loginBehavior)
        {
            return await LoginAsync(permissions, false, loginBehavior);
        }

        internal async Task<AccessTokenData> LoginAsync(string permissions, bool force, FacebookLoginBehavior loginBehavior)
        {
            if (this.LoginInProgress)
            {
                throw new InvalidOperationException("Login in progress.");
            }

            this.LoginInProgress = true;
            try
            {
                var session = AccessTokenDataCacheProvider.Current.GetSessionData();
                if (session == null)
                {
#if WINDOWS
                    // Authenticate
                    var authResult = await PromptOAuthDialog(permissions, WebAuthenticationOptions.None, loginBehavior);

#else
                    var authResult = await PromptOAuthDialog(permissions, WebAuthenticationOptions.None, loginBehavior);
#endif

                    FacebookClient client = new FacebookClient(authResult.AccessToken);
                    var parameters = new Dictionary<string, object>();
                    parameters["fields"] = "id";

                    var result = await client.GetTaskAsync("me", parameters);
                    var dict = (IDictionary<string, object>)result;

                    session = new AccessTokenData
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
#if WINDOWS
                    // Authenticate
                    var authResult = await PromptOAuthDialog(permissions, WebAuthenticationOptions.None, loginBehavior);

#else
                        var authResult = await PromptOAuthDialog(permissions, WebAuthenticationOptions.None, loginBehavior);
#endif
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
                AccessTokenDataCacheProvider.Current.SaveSessionData(session);
                CurrentAccessTokenData = session;
            }
            finally
            {
                this.LoginInProgress = false;
            }

            return CurrentAccessTokenData;
        }

        async public void LoginWithBehavior(string permissions, FacebookLoginBehavior behavior)
        {
            switch (behavior)
            {
                case FacebookLoginBehavior.LoginBehaviorMobileInternetExplorerOnly:
                {
#if (WP8 || WINDOWS_PHONE)

                    String appId = Session.AppId;
                    Uri uri =
                        new Uri(
                            String.Format(
                                "https://m.facebook.com/v2.1/dialog/oauth?redirect_uri={0}&display=touch&state=%7B%220is_active_session%22%3A1%2C%22is_open_session%22%3A1%2C%22com.facebook.sdk_client_state%22%3A1%2C%223_method%22%3A%22browser_auth%22%7D&scope={2}&type=user_agent&client_id={1}&sdk=ios",
                                LoginRedirectUri, appId, permissions), UriKind.Absolute);
                    
                    Launcher.LaunchUriAsync(uri);
                    break;
                
#else
                    throw new NotImplementedException("Internet explorer based login is not available on Windows");
#endif
                }
                case FacebookLoginBehavior.LoginBehaviorWebViewOnly:
                {
                    String appId = Session.AppId;

#if WP8 || WINDOWS_PHONE
                    Uri uri =
                        new Uri(
                            String.Format(
                                "https://m.facebook.com/v2.1/dialog/oauth?redirect_uri={0}%3A%2F%2Fauthorize&display=touch&state=%7B%220is_active_session%22%3A1%2C%22is_open_session%22%3A1%2C%22com.facebook.sdk_client_state%22%3A1%2C%223_method%22%3A%22browser_auth%22%7D&scope={2}&type=user_agent&client_id={1}&sdk=ios",
                                String.Format("fb{0}", appId), appId, permissions), UriKind.Absolute);
#else
                    Uri uri = await GetLoginUrl(permissions);

#endif
                    WebviewAuthentication.AuthenticateAsync(WebAuthenticationOptions.None, uri, null);
                    break;
                }
                case FacebookLoginBehavior.LoginBehaviorWebAuthenticationBroker:
#if WINDOWS
                {
                    try
                    {
                        // TODO: What to do here? LoginAsync returns inproc. Login with IE returns out of proc?
                        var result = await LoginAsync(permissions, FacebookLoginBehavior.LoginBehaviorWebAuthenticationBroker);
                        // when the results are available, launch the event handler
                        if (OnFacebookAuthenticationFinished != null)
                        {
                            OnFacebookAuthenticationFinished(result);
                        }

                        if (OnSessionStateChanged != null)
                        {
                            OnSessionStateChanged(LoginStatus.LoggedIn);
                        }
                    }
                    catch (FacebookOAuthException e)
                    {
                        if (OnSessionStateChanged != null)
                        {
                            OnSessionStateChanged(LoginStatus.LoggedOut);
                        }
                    }

                    break;
                }
#else
                {
                    throw new NotImplementedException("WebviewAuthentication is not implemented on Windows Phone");
                }
#endif// WINDOWS

                case FacebookLoginBehavior.LoginBehaviorApplicationOnly:
                {
#if WP8 || WINDOWS_PHONE

                    LoginWithApp(permissions);
                    break;
                
#else
                    throw new NotImplementedException("Login via app is not available on Windows");
#endif
                }
            }
        }

#if WP8 || WINDOWS_PHONE
        /// <summary>
        /// TODO: Extend an SSO token daily. This should be an internal method
        /// </summary>
        /// <returns></returns>
        public async static  Task CheckAndExtendTokenIfNeeded()
        {
            // get the existing token
            if (String.IsNullOrEmpty(ActiveSession.CurrentAccessTokenData.AccessToken))
            {
                // If there is no token, do nothing
                return;
            }

            // check if its issue date is over 24 hours and if so, renew it
            if (DateTime.UtcNow - ActiveSession.CurrentAccessTokenData.Issued > TimeSpan.FromHours(24)) // one day 
            {
                var client = new HttpClient();
                String tokenExtendUri = "https://graph.facebook.com/v2.1";
                client.BaseAddress = new Uri(tokenExtendUri);

                var request = new HttpRequestMessage();

                var mfdc = new MultipartFormDataContent();
                var _appId = Session.AppId;

                mfdc.Add(new StringContent(_appId), name: "batch_app_id");

                String extensionString = "[{\"method\":\"GET\",\"relative_url\":\"oauth\\/access_token?grant_type=fb_extend_sso_token&access_token=" + ActiveSession.CurrentAccessTokenData.AccessToken + "\"}]";
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

                    var accessTokenData = new AccessTokenData();
                    // token extension failed...
                    accessTokenData.AccessToken = access_token;

                    // parse out other types
                    long expiresInValue;
                    var now = DateTime.UtcNow;
                    accessTokenData.Expires = now + TimeSpan.FromSeconds(expires_at);
                    accessTokenData.Issued = now - (TimeSpan.FromDays(60) - TimeSpan.FromSeconds(expires_at));
                    accessTokenData.AppId = _appId;

                    // Assign the accessTokenData object over, this saves it to the disk as well.
                    ActiveSession.CurrentAccessTokenData = accessTokenData;
                }
                else
                {
                     // return an error?? Since this is token extension, maybe we should wait until the token is finally expired before throwing an error.
                }
            }


        }

#endif // END TOKEN EXTENSION

        /// <summary>
        /// Log a user out of Facebook.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Logout", Justification = "Logout is preferred by design")]
        public void Logout()
        {
            try
            {
                AccessTokenDataCacheProvider.Current.DeleteSessionData();

            }
            finally
            {
                CurrentAccessTokenData.AccessToken = null;
                CurrentAccessTokenData.CurrentPermissions = null;
                CurrentAccessTokenData.FacebookId = null;

                if (Session.OnSessionStateChanged != null)
                {
                    OnSessionStateChanged(LoginStatus.LoggedOut);
                }
            }
        }

        private async Task<FacebookOAuthResult> PromptOAuthDialog(string permissions, WebAuthenticationOptions options, FacebookLoginBehavior loginBehavior)
        {
#if !WINDOWS
            if (loginBehavior != FacebookLoginBehavior.LoginBehaviorWebViewOnly)
            {
                throw new NotImplementedException("Following login behavior is not supported on Windows Phone: " + loginBehavior.ToString());
            }
#endif
            // Use WebviewAuthentication to launch server side OAuth flow

            Uri startUri = await this.GetLoginUrl(permissions);
            Uri endUri = new Uri("https://www.facebook.com/connect/login_success.html");

            WebAuthenticationResult result = null;
#if WINDOWS
            if (loginBehavior == FacebookLoginBehavior.LoginBehaviorWebAuthenticationBroker)
            {
                result = await WebAuthenticationBroker.AuthenticateAsync(options, startUri, endUri);

                if (result.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                {
                    throw new HttpRequestException("An Http error happened. Error Code: " +
                                                   result.ResponseStatus.ToString());
                }
                else if (result.ResponseStatus == WebAuthenticationStatus.UserCancel)
                {
                    throw new FacebookOAuthException("Facebook.Client: User cancelled login in WebAuthBroker");
                }
            }
#endif
            var client = new FacebookClient();
            var authResult = client.ParseOAuthCallbackUrl(new Uri(result.ResponseData));
            return authResult;
        }

        private async Task<Uri> GetLoginUrl(string permissions)
        {
            var parameters = new Dictionary<string, object>();

            String appId = Session.AppId;
            parameters["client_id"] = appId;
            parameters["redirect_uri"] = "https://www.facebook.com/connect/login_success.html";
            parameters["response_type"] = "token";
#if WP8 || WINDOWS_PHONE
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
