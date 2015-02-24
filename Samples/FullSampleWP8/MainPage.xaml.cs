using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Windows.System;
using Facebook;
using Facebook.Client;
using Facebook.Client.Controls.WebDialog;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FullSampleWP8.Resources;
using System.Windows.Controls.Primitives;

namespace FullSampleWP8
{
    public partial class MainPage : PhoneApplicationPage
    {
        public static string AccessToken { get; set;  }

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            //Uri uri =
            //    new Uri(
            //        "https://m.facebook.com/v1.0/dialog/oauth?redirect_uri=fb540541885996234%3A%2F%2Fauthorize&display=touch&state=%7B%220is_active_session%22%3A1%2C%22is_open_session%22%3A1%2C%22com.facebook.sdk_client_state%22%3A1%2C%223_method%22%3A%22browser_auth%22%7D&scope=email%2Cbasic_info&type=user_agent&client_id=540541885996234&ret=login&sdk=ios&ext=1413580961&hash=Aeb0Q3uVJ6pgMh4C&refsrc=https%3A%2F%2Fm.facebook.com%2Flogin.php&refid=9&_rdr",
            //        UriKind.RelativeOrAbsolute);
            //Launcher.LaunchUriAsync(uri);
            //var client = new Session("540541885996234");
            Session.ActiveSession.LoginWithBehavior("email,public_profile,user_friends", FacebookLoginBehavior.LoginBehaviorMobileInternetExplorerOnly);
        }

        async private void extendTokenButton_Click(object sender, RoutedEventArgs e)
        {
           await Session.CheckAndExtendTokenIfNeeded();
            MessageBox.Show("Token extended. New expiry at: " +
                            Session.ActiveSession.CurrentAccessTokenData.Expires.ToString());
        }

        async private void graphCallButton_Click(object sender, RoutedEventArgs e)
        {
            FacebookClient fb = new FacebookClient(Session.ActiveSession.CurrentAccessTokenData.AccessToken);

            dynamic friendsTaskResult = await fb.GetTaskAsync("/me/friends");

            MessageBox.Show("Graph Request for /me/friends: " +
                friendsTaskResult.ToString());
        }

        private void showDialogButton_Click(object sender, RoutedEventArgs e)
        {
            Session.ShowAppRequestsDialog(null);
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
        private void ShowFeedDialogButton_OnClick(object sender, RoutedEventArgs e)
        {
            Session.ShowFeedDialog();
        }

        private void ShowRequestWithBrowserButton_Click(object sender, RoutedEventArgs e)
        {
            Session.ShowAppRequestDialogViaBrowser("Test Message", "test title");
        }


        private void LoginViaWebView_OnClick(object sender, RoutedEventArgs e)
        {
            Session.ActiveSession.LoginWithBehavior("email,public_profile,user_friends", FacebookLoginBehavior.LoginBehaviorWebViewOnly);
        }
    }
}