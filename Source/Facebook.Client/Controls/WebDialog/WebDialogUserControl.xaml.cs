using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.System;
#if WP8
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
#endif

#if WINDOWS_UNIVERSAL
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
#endif

#if WINDOWS
using Windows.UI.Xaml.Navigation;
#endif
namespace Facebook.Client.Controls.WebDialog
{

    public partial class WebDialogUserControl : UserControl
    {
        private WebDialogFinishedDelegate OnDialogFinished;
        public WebDialogUserControl()
        {
            InitializeComponent();
#if WP8
            dialogWebBrowser.Navigating += DialogWebBrowserOnNavigating;
#endif
#if WINDOWS_UNIVERSAL && !WINDOWS80
            dialogWebBrowser.NavigationStarting += dialogWebBrowser_NavigationStarting;
#endif

#if WINDOWS80
            dialogWebBrowser.LoadCompleted += DialogWebBrowserOnLoadCompleted;
#endif
        }





#if !WP8
#if WINDOWS_UNIVERSAL && !WINDOWS80
        async void dialogWebBrowser_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
#endif
#if WINDOWS80
        async private void DialogWebBrowserOnLoadCompleted(object sender, NavigationEventArgs args)
#endif
        {
            if (args.Uri.ToString().StartsWith("https://www.facebook.com/connect/login_success.html"))
            {
                if (ParentControlPopup != null)
                {
                    ParentControlPopup.IsOpen = false;
                }

                if (!args.Uri.Fragment.Contains("access_token"))
                {
                    // this callback is in return for the dialog, so just cancel it.

                    if (OnDialogFinished != null)
                    {
                        OnDialogFinished(WebDialogResult.WebDialogResultDialogCompleted);
                    }

                    return;
                }

                try
                {
                    var client = new FacebookClient();
                    var authResult = client.ParseOAuthCallbackUrl(args.Uri);

                    client = new FacebookClient(authResult.AccessToken);
                    var parameters = new Dictionary<string, object>();
                    parameters["fields"] = "id";

                    var result = await client.GetTaskAsync("me", parameters);
                    var dict = (IDictionary<string, object>)result;

                    Session.ActiveSession.CurrentAccessTokenData = new AccessTokenData
                    {
                        AccessToken = authResult.AccessToken,
                        Expires = authResult.Expires,
                        FacebookId = (string)dict["id"],
                        AppId = Session.AppId
                    };

                    if (Session.OnFacebookAuthenticationFinished != null)
                    {
                        Session.OnFacebookAuthenticationFinished(Session.ActiveSession.CurrentAccessTokenData);
                    }

                    if (Session.OnSessionStateChanged != null)
                    {
                        Session.OnSessionStateChanged(LoginStatus.LoggedIn);
                    }
                }
                catch (Facebook.FacebookOAuthException exc)
                {
                    // TODO: (sanjeevd) catch appropriately
                }
            }
        }
#endif

#if WP8
        private void DialogWebBrowserOnNavigating(object sender, NavigatingEventArgs navigatingEventArgs)
        {
            if (ParentControlPopup != null)
            {
                // cancel the navigation when we successfully hit the send or when the login dialog finishes with a login
                if (navigatingEventArgs.Uri.ToString().StartsWith("fbconnect"))
                {
                    navigatingEventArgs.Cancel = true;
                    ParentControlPopup.IsOpen = false;
                }

                // Parse the Uri string and based on the results, invoke the callback
                if (OnDialogFinished != null)
                {
                    // TODO: Fix this and send the correct results back based on the results returned by the dialog
                    OnDialogFinished(WebDialogResult.WebDialogResultDialogCompleted);
                }
            }
        }
#else

        private void DismissDialogWhenDone(Uri navigationUri)
        {
            if (ParentControlPopup != null)
            {
                // cancel the navigation when we successfully hit the send
                if (navigationUri.ToString().StartsWith(String.Format("fb{0}", Session.AppId)))
                {
                    ParentControlPopup.IsOpen = false;
                }

                // Parse the Uri string and based on the results, invoke the callback
                if (OnDialogFinished != null)
                {
                    // TODO: Fix this and send the correct results back based on the results returned by the dialog
                    OnDialogFinished(WebDialogResult.WebDialogResultDialogCompleted);
                }
            }
        }

#endif

        internal Popup ParentControlPopup { get; set; } 

        public void ShowAppRequestsDialog(WebDialogFinishedDelegate callback, string message, string title, List<string> idList)
        {
            if (callback != null)
            {
                OnDialogFinished += callback;
            }

#if WINDOWS || WINDOWS_UNIVERSAL
            LifecycleHelper.OnDialogDismissed = null;
            LifecycleHelper.OnDialogDismissed += DismissDialogWhenDone;
#endif
            var idBuilder = new StringBuilder("&to=");
            if (idList != null)
            {
                foreach (var id in idList)
                {
                    idBuilder.Append(id+",");
                }
            }
            //idBuilder.Length > 4? idBuilder.ToString() : String.Empty;
#if WINDOWS
            dialogWebBrowser.Navigate(new Uri(String.Format("https://facebook.com/dialog/apprequests?display=popup&app_id={0}&message={1}&redirect_uri=https://www.facebook.com/connect/login_success.html{2}&title={3}", Session.AppId, message, idBuilder.Length > 4? idBuilder.ToString() : String.Empty, title), UriKind.Absolute));
#endif

#if WINDOWS_PHONE
            dialogWebBrowser.Navigate(new Uri(String.Format("https://m.facebook.com/v2.1/dialog/apprequests?access_token={0}&redirect_uri=fb{2}%3A%2F%2Fsuccess&app_id={1}&message={3}&display=touch{4}&title={5}", Session.ActiveSession.CurrentAccessTokenData.AccessToken, Session.AppId, Session.AppId, message, idBuilder.Length > 4 ? idBuilder.ToString() : String.Empty, title)));
#endif

#if WP8

            dialogWebBrowser.Navigate(new Uri(String.Format("https://m.facebook.com/v2.1/dialog/apprequests?access_token={0}&redirect_uri=fbconnect%3A%2F%2Fsuccess&app_id={1}&message={2}&display=touch{3}&title={4}", Session.ActiveSession.CurrentAccessTokenData.AccessToken, Session.AppId, message, idBuilder.Length > 4 ? idBuilder.ToString() : String.Empty, title)));
#endif
        }


        internal static void ShowAppRequestDialogViaBrowser(string message, string title, List<string> idList, string data)
        {
            var idBuilder = new StringBuilder("&to=");
            if (idList != null)
            {
                foreach (var id in idList)
                {
                    idBuilder.Append(id + ",");
                }
            }

            Launcher.LaunchUriAsync(new Uri(String.Format("https://m.facebook.com/v2.1/dialog/apprequests?access_token={0}&redirect_uri={2}&app_id={1}&message={5}&display=touch{3}&title={4}&data={6}", Session.ActiveSession.CurrentAccessTokenData.AccessToken, Session.AppId, Session.AppRequestRedirectUri, idBuilder.Length > 4 ? idBuilder.ToString() : String.Empty, title, message, data)));
        }

        internal static void ShowFeedDialogViaBrowser(string toId = "", string link = "", string linkName = "", string linkCaption = "", string linkDescription = "", string picture = "")
        {
            Launcher.LaunchUriAsync(new Uri(String.Format("https://m.facebook.com/v2.1/dialog/feed?access_token={0}&redirect_uri={2}&app_id={1}&display=touch&to={3}&link={4}&name={5}&caption={6}&description={7}&picture={8}", Session.ActiveSession.CurrentAccessTokenData.AccessToken, Session.AppId, Session.FeedRedirectUri, toId, link, linkName, linkCaption, linkDescription, picture)));
        }


        public void ShowFeedDialog(string toId = "", string link = "", string linkName = "", string linkCaption = "", string linkDescription = "", string picture = "")
        {
#if WINDOWS || WINDOWS_UNIVERSAL
            LifecycleHelper.OnDialogDismissed = null;
            LifecycleHelper.OnDialogDismissed += DismissDialogWhenDone;
#endif

#if WINDOWS
            dialogWebBrowser.Navigate(new Uri(String.Format("https://facebook.com/dialog/feed?display=popup&app_id={0}&redirect_uri=https://www.facebook.com/connect/login_success.html&to={1}&link={2}&name={3}&caption={4}&description={5}&picture={6}", Session.AppId, toId, link, linkName, linkCaption, linkDescription, picture), UriKind.Absolute));
#endif

#if WP8
            dialogWebBrowser.Navigate(new Uri(String.Format("https://m.facebook.com/v2.1/dialog/feed?access_token={0}&redirect_uri=fbconnect%3A%2F%2Fsuccess&app_id={1}&display=touch&to={2}&link={3}&name={4}&caption={5}&description={6}&picture={7}", Session.ActiveSession.CurrentAccessTokenData.AccessToken, Session.AppId, toId, link, linkName, linkCaption, linkDescription, picture)));
#endif

#if WINDOWS_PHONE
            dialogWebBrowser.Navigate(new Uri(String.Format("https://m.facebook.com/v2.1/dialog/feed?access_token={0}&redirect_uri=fb{2}%3A%2F%2Fsuccess&app_id={1}&display=touch&to={3}&link={4}&name={5}&caption={6}&description={7}&picture={8}", Session.ActiveSession.CurrentAccessTokenData.AccessToken, Session.AppId, Session.AppId, toId, link, linkName, linkCaption, linkDescription, picture)));

#endif

        }

        private void CloseDialogButton_OnClick(object sender, RoutedEventArgs e)
        {
            // If the user hits the X at the top, close the Web Dialog without any further ado
            if (ParentControlPopup != null)
            {
                ParentControlPopup.IsOpen = false;
            }

            if (Session.OnFacebookAuthenticationFinished != null)
            {
                Session.OnFacebookAuthenticationFinished(null);
            }
        }

        public void LoginViaWebview(Uri startUri)
        {
#if WINDOWS || WINDOWS_UNIVERSAL
            LifecycleHelper.OnDialogDismissed = null;
            LifecycleHelper.OnDialogDismissed += DismissDialogWhenDone;
#endif
            dialogWebBrowser.Navigate(startUri);
        }
    }
}
