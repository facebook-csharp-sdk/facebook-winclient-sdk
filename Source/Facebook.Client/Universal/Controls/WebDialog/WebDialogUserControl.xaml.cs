using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Facebook.Client.Controls.WebDialog
{

    public sealed partial class WebDialogUserControl : UserControl
    {
        private WebDialogFinishedDelegate OnDialogFinished;

        public WebDialogUserControl()
        {
            this.InitializeComponent();
        }

        private void DismissDialogWhenDone(Uri navigationUri)
        {
            if (ParentControlPopup != null)
            {
                var task = Task.Run(async () => await AppAuthenticationHelper.GetFacebookConfigValue("Facebook", "AppId"));
                task.Wait();
                // cancel the navigation when we successfully hit the send
                if (navigationUri.ToString().StartsWith(String.Format("fb{0}", task.Result)))
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

        internal Popup ParentControlPopup { get; set; }

        public void ShowAppRequestsDialog(WebDialogFinishedDelegate callback)
        {
            if (callback != null)
            {
                OnDialogFinished += callback;
            }

            // Remove all dialog dismiss delegates since only one dialog can be active at one point of time.
            LifecycleHelper.OnDialogDismissed = null;
            LifecycleHelper.OnDialogDismissed += DismissDialogWhenDone;

            var task = Task.Run(async () => await AppAuthenticationHelper.GetFacebookConfigValue("Facebook", "AppId"));
            task.Wait();
            //Uri uri =
            //    new Uri(
            //        String.Format(
            //            "https://m.facebook.com/v2.1/dialog/apprequests?access_token={0}&redirect_uri=fbconnect%3A%2F%2Fsuccess&app_id={1}&message=YOUR_MESSAGE_HERE&display=touch",
            //            Session.ActiveSession.CurrentAccessTokenData.AccessToken, task.Result));
            dialogWebBrowser.Navigate(new Uri(String.Format("https://m.facebook.com/v2.1/dialog/apprequests?access_token={0}&redirect_uri=fb{2}%3A%2F%2Fsuccess&app_id={1}&message=YOUR_MESSAGE_HERE&display=touch", Session.ActiveSession.CurrentAccessTokenData.AccessToken, task.Result, task.Result)));
        }


        public static void ShowAppRequestDialogViaBrowser()
        {
            var task = Task.Run(async () => await AppAuthenticationHelper.GetFacebookConfigValue("Facebook", "AppId"));
            task.Wait();
            Launcher.LaunchUriAsync(new Uri(String.Format("https://m.facebook.com/v2.1/dialog/apprequests?access_token={0}&redirect_uri=fb{2}%3A%2F%2Fsuccess&app_id={1}&message=YOUR_MESSAGE_HERE&display=touch", Session.ActiveSession.CurrentAccessTokenData.AccessToken, task.Result, task.Result)));
        }
        public void ShowFeedDialog()
        {
            LifecycleHelper.OnDialogDismissed = null;
            LifecycleHelper.OnDialogDismissed += DismissDialogWhenDone;

            var task = Task.Run(async () => await AppAuthenticationHelper.GetFacebookConfigValue("Facebook", "AppId"));
            task.Wait();
            dialogWebBrowser.Navigate(new Uri(String.Format("https://m.facebook.com/v2.1/dialog/feed?access_token={0}&redirect_uri=fb{2}%3A%2F%2Fsuccess&app_id={1}&display=touch", Session.ActiveSession.CurrentAccessTokenData.AccessToken, task.Result, task.Result)));
        }

        private void CloseDialogButton_OnClick(object sender, RoutedEventArgs e)
        {
            // If the user hits the X at the top, close the Web Dialog without any further ado
            if (ParentControlPopup != null)
            {
                ParentControlPopup.IsOpen = false;
            }
        }
    }
}
