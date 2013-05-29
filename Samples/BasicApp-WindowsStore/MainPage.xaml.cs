using Facebook.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BasicApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }
		
		private bool isAuthenticated = false;
        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
			if(!isAuthenticated)
			{
				isAuthenticated = true;
				await Authenticate();
			}
        }

        private FacebookSession sesion;
        private async System.Threading.Tasks.Task Authenticate()
        {
            while (sesion == null)
            {
                string message;
                try
                {
                    sesion = await App.FacebookSessionClient.LoginAsync("email,publish_checkins,publish_actions,friends_online_presence");
                    message = "You are now logged in";
                }
                catch (InvalidOperationException)
                {
                    message = "You must log in. Login Required";
                }

                //MessageDialog dialog = new MessageDialog(message);
                //dialog.showAsync();
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
