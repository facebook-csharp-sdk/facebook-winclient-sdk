using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Facebook.Client;

namespace BasicApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        private bool isAuthenticated = false;
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            this.Loaded += MainPage_Loaded;
        }

        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isAuthenticated)
            {
                isAuthenticated = true;
                await Authenticate();
            }
        }

        private FacebookSession session;
        private async System.Threading.Tasks.Task Authenticate()
        {
            while (session == null)
            {
                string message;
                try
                {
                    session = await App.FacebookSessionClient.LoginAsync("publish_checkins,manage_notifications");
                    message = "You are now logged in";
                }
                catch (InvalidOperationException)
                {
                    message = "You must log in. Login Required";
                }

                MessageBox.Show(message);
            }
        }
    }
}