using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using Facebook.Client;

namespace BasicApp_Win81_Universal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.dialogWebView.NavigationStarting += dialogWebView_NavigationStarting;

        }

        void dialogWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (args.Uri.ToString().StartsWith("https://pratapgarh.com"))
            {
                dialogWebView.Visibility = Visibility.Collapsed;
            }
        }

        

        private void showDialogButton_Click(object sender, RoutedEventArgs e)
        {
            this.dialogWebView.Visibility = Visibility.Visible;
            this.dialogWebView.Navigate(new Uri("https://facebook.com/dialog/apprequests?display=popup&app_id=540541885996234&message=YOUR_MESSAGE_HERE!&redirect_uri=https://pratapgarh.com&access_token=" + Session.ActiveSession.CurrentAccessTokenData.AccessToken, UriKind.Absolute));
        }
    }
}
