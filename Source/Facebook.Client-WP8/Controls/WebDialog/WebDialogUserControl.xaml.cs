using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Facebook.Client.Controls.WebDialog
{
    public partial class WebDialogUserControl : UserControl
    {
        public WebDialogUserControl()
        {
            InitializeComponent();

            dialogWebBrowser.Navigating += DialogWebBrowserOnNavigating;
            
        }

        private void DialogWebBrowserOnNavigating(object sender, NavigatingEventArgs navigatingEventArgs)
        {
            if (ParentControlPopup != null)
            {
                // cancel the navigation when we successfully hit the send
                if (navigatingEventArgs.Uri.ToString().StartsWith("fbconnect"))
                {
                    navigatingEventArgs.Cancel = true;
                    ParentControlPopup.IsOpen = false;
                }
            }
        }

        internal Popup ParentControlPopup { get; set; } 

        public void ShowAppRequestsDialog()
        {
            var task = Task.Run(async () => await AppAuthenticationHelper.GetFacebookConfigValue("Facebook", "AppId"));
            task.Wait();
            dialogWebBrowser.Navigate(new Uri(String.Format("https://m.facebook.com/v2.1/dialog/apprequests?access_token={0}&redirect_uri=fbconnect%3A%2F%2Fsuccess&app_id={1}&message=YOUR_MESSAGE_HERE&display=touch", FacebookSessionClient.CurrentSession.AccessToken, task.Result)));
        }

        public void ShowFeedDialog()
        {
            var task = Task.Run(async () => await AppAuthenticationHelper.GetFacebookConfigValue("Facebook", "AppId"));
            task.Wait();
            dialogWebBrowser.Navigate(new Uri(String.Format("https://m.facebook.com/v2.1/dialog/feed?access_token={0}&redirect_uri=fbconnect%3A%2F%2Fsuccess&app_id={1}&display=touch", FacebookSessionClient.CurrentSession.AccessToken, task.Result)));
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
