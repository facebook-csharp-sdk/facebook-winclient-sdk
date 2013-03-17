using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneApp1.Resources;
using Facebook.Client;
using Facebook.Client.Tasks;

namespace PhoneApp1
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.Loaded += MainPage_Loaded;

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await Authenticate();
        }

        private FacebookSession session;
        private async System.Threading.Tasks.Task Authenticate()
        {
            while (session == null)
            {
                string message;
                try
                {
                    session = await App.FacebookSessionClient.LoginAsync("email,publish_checkins");
                    message = "You are now logged in";
                }
                catch (InvalidOperationException)
                {
                    message = "You must log in. Login Required";
                }

                MessageBox.Show(message);
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //var parameters = new Dictionary<string, object>();
            //parameters["name"] = "My Dialog";
            //parameters["redirect_uri"] = "https://www.facebook.com/connect/login_success.html";
            //parameters["display"] = "touch";
            //parameters["app_id"] = App.FacebookSessionClient.AppId;
            //var result = await FacebookWebDialog.ShowDialogAsync("feed", parameters);

            var task = new FacebookFeedTask();
            task.AppId = App.FacebookSessionClient.AppId;
            task.Name = "My Dialog";
            var result = await task.Show();

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
    }
}