namespace PhoneApp2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Navigation;
    using Facebook;
    using Facebook.Client;
    using Facebook.Client.Controls;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using PhoneApp2.Resources;

    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            this.InitializeComponent();
            ////this.Loaded += MainPage_Loaded;

            //// Sample code to localize the ApplicationBar
            ////BuildLocalizedApplicationBar();
        }

        ////async void MainPage_Loaded(object sender, RoutedEventArgs e)
        ////{
        ////    await Authenticate();
        ////}

        ////private FacebookSession session;
        ////private async System.Threading.Tasks.Task Authenticate()
        ////{
        ////    while (session == null)
        ////    {
        ////        string message;
        ////        try
        ////        {
        ////            session = await App.FacebookSessionClient.LoginAsync("email,publish_checkins");
        ////            message = "You are now logged in";
        ////        }
        ////        catch (InvalidOperationException)
        ////        {
        ////            message = "You must log in. Login Required";
        ////        }

        ////        MessageBox.Show(message);
        ////    }
        ////}

        //// Sample code for building a localized ApplicationBar
        ////private void BuildLocalizedApplicationBar()
        ////{
        ////    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        ////    ApplicationBar = new ApplicationBar();

        ////    // Create a new button and set the text value to the localized string from AppResources.
        ////    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        ////    appBarButton.Text = AppResources.AppBarButtonText;
        ////    ApplicationBar.Buttons.Add(appBarButton);

        ////    // Create a new menu item with the localized string from AppResources.
        ////    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        ////    ApplicationBar.MenuItems.Add(appBarMenuItem);
        ////}

        private void OnUserInfoChanged(object sender, Facebook.Client.Controls.UserInfoChangedEventArgs e)
        {
            this.userInfo.DataContext = e.User;
        }

        private void OnSessionStateChanged(object sender, Facebook.Client.Controls.SessionStateChangedEventArgs e)
        {
            this.welcomeText.Opacity = (e.SessionState == FacebookSessionState.Opened) ? 0 : 100;
            this.contentPanel.Opacity = (e.SessionState == FacebookSessionState.Opened) ? 100 : 0;
        }
    }
}