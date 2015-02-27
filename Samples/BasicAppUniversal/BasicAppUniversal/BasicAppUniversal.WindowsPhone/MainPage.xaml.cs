using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
using Facebook;
using Facebook.Client;
using Facebook.Client.Controls.WebDialog;

namespace BasicAppUniversal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

         private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            //Uri uri =
            //    new Uri(
            //        "https://m.facebook.com/v1.0/dialog/oauth?redirect_uri=fb540541885996234%3A%2F%2Fauthorize&display=touch&state=%7B%220is_active_session%22%3A1%2C%22is_open_session%22%3A1%2C%22com.facebook.sdk_client_state%22%3A1%2C%223_method%22%3A%22browser_auth%22%7D&scope=email%2Cbasic_info&type=user_agent&client_id=540541885996234&ret=login&sdk=ios&ext=1413580961&hash=Aeb0Q3uVJ6pgMh4C&refsrc=https%3A%2F%2Fm.facebook.com%2Flogin.php&refid=9&_rdr",
            //        UriKind.RelativeOrAbsolute);
            //Launcher.LaunchUriAsync(uri);
            var client = new Session();
            client.LoginWithBehavior("email,public_profile,user_friends", FacebookLoginBehavior.LoginBehaviorMobileInternetExplorerOnly);
        }

        async private void extendTokenButton_Click(object sender, RoutedEventArgs e)
        {
           await Session.CheckAndExtendTokenIfNeeded();
            

        }

        async private void graphCallButton_Click(object sender, RoutedEventArgs e)
        {
            FacebookClient fb = new FacebookClient(Session.ActiveSession.CurrentAccessTokenData.AccessToken);

            dynamic friendsTaskResult = await fb.GetTaskAsync("/me/friends");
        }

        private void showDialogButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> friendsList = new List<String>();
            friendsList.Add("9074");
            friendsList.Add("535949260");
            Session.ShowAppRequestsDialog(null, "What's up", "random title", friendsList);
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
            List<string> friendsList = new List<String>();
            friendsList.Add("9074");
            friendsList.Add("535949260");
            Session.ShowAppRequestDialogViaBrowser("What's up", "random title", friendsList);
        }
    
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private void ShowUiContrrolsButton_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (ControlsTest));
        }

        private void LoginViaWebViewButton_Click(object sender, RoutedEventArgs e)
        {
            Session.ActiveSession.LoginWithBehavior("email,public_profile,user_friends", FacebookLoginBehavior.LoginBehaviorWebViewOnly);
        }
    }
}
