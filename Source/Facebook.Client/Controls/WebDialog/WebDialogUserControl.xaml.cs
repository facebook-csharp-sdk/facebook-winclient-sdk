using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using Windows.System;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Facebook.Client.Controls.WebDialog
{

    public partial class WebDialogUserControl : UserControl
    {
        private WebDialogFinishedDelegate OnDialogFinished;
        public WebDialogUserControl()
        {
            InitializeComponent();

            dialogWebBrowser.Navigating += DialogWebBrowserOnNavigating;
        }

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

        internal Popup ParentControlPopup { get; set; } 

        public void ShowAppRequestsDialog(WebDialogFinishedDelegate callback)
        {
            if (callback != null)
            {
                OnDialogFinished += callback;
            }

            var task = Task.Run(async () => await AppAuthenticationHelper.GetFacebookConfigValue("Facebook", "AppId"));
            task.Wait();
            dialogWebBrowser.Navigate(new Uri(String.Format("https://m.facebook.com/v2.1/dialog/apprequests?access_token={0}&redirect_uri=fbconnect%3A%2F%2Fsuccess&app_id={1}&message=YOUR_MESSAGE_HERE&display=touch", Session.ActiveSession.CurrentAccessTokenData.AccessToken, task.Result)));

            
        }

        public static void ShowAppRequestDialogViaBrowser()
        {
            var task = Task.Run(async () => await AppAuthenticationHelper.GetFacebookConfigValue("Facebook", "AppId"));
            task.Wait();
            Launcher.LaunchUriAsync(new Uri(String.Format("https://m.facebook.com/v2.1/dialog/apprequests?access_token={0}&redirect_uri=fb540541885996234%3A%2F%2Fsuccess&app_id={1}&message=YOUR_MESSAGE_HERE&display=touch", Session.ActiveSession.CurrentAccessTokenData.AccessToken, task.Result)));
        }
        public void ShowFeedDialog()
        {
            var task = Task.Run(async () => await AppAuthenticationHelper.GetFacebookConfigValue("Facebook", "AppId"));
            task.Wait();
            dialogWebBrowser.Navigate(new Uri(String.Format("https://m.facebook.com/v2.1/dialog/feed?access_token={0}&redirect_uri=fbconnect%3A%2F%2Fsuccess&app_id={1}&display=touch", Session.ActiveSession.CurrentAccessTokenData.AccessToken, task.Result)));
        }

        private void CloseDialogButton_OnClick(object sender, RoutedEventArgs e)
        {
            // If the user hits the X at the top, close the Web Dialog without any further ado
            if (ParentControlPopup != null)
            {
                ParentControlPopup.IsOpen = false;
            }
        }

        public void LoginViaWebview(Uri startUri)
        {
            var task = Task.Run(async () => await AppAuthenticationHelper.GetFacebookConfigValue("Facebook", "AppId"));
            task.Wait();
            dialogWebBrowser.Navigate(startUri);
        }
    }
}
