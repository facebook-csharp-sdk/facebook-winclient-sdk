using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        }

        private void CloseDialogButton_OnClick(object sender, RoutedEventArgs e)
        {
            var task = Task.Run(async () => await AppAuthenticationHelper.GetFacebookConfigValue("Facebook", "AppId"));
            task.Wait();
            
            dialogWebBrowser.Navigate(new Uri(String.Format("https://m.facebook.com/v2.1/dialog/apprequests?sdk=3.17.1&access_token={0}&redirect_uri=fbconnect%3A%2F%2Fsuccess&app_id={1}&message=YOUR_MESSAGE_HERE&display=touch", FacebookSessionClient.CurrentSession.AccessToken, task.Result)));
        }
    }
}
