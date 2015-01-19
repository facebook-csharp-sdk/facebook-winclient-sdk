using Facebook.Client;

namespace BasicApp
{
    using System;
    using System.Collections.Generic;
    using Facebook.Client.Controls;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.currentItem = this.welcomeText;
            this.Loaded += MainPage_Loaded;
        }

        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //var appLink = await AppLinkNavigation.DefaultResolver.GetAppLinkAsync("<insert access token here>", "http://pratapgarh.com/fbtest/al.html");
            //await AppLinkNavigation.NavigateAsync(appLink);

        }

        ////private FacebookSession sesion;
        ////private async System.Threading.Tasks.Task Authenticate()
        ////{
        ////    while (sesion == null)
        ////    {
        ////        string message;
        ////        try
        ////        {
        ////            sesion = await App.FacebookSessionClient.LoginAsync("email,publish_checkins,publish_actions,friends_online_presence");
        ////            message = "You are now logged in";
        ////        }
        ////        catch (InvalidOperationException)
        ////        {
        ////            message = "You must log in. Login Required";
        ////        }

        ////        //MessageBox.Show(message);
        ////    }
        ////}

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Session.ActiveSession.CurrentAccessTokenData != null &&
                !String.IsNullOrEmpty(Session.ActiveSession.CurrentAccessTokenData.AccessToken))
            {
                this.friendPicker.AccessToken = Session.ActiveSession.CurrentAccessTokenData.AccessToken;
            }
        }

        private UIElement currentItem;

        private void OnItemSelected(object sender, RoutedEventArgs e)
        {
            var choice = sender as RadioButton;
            if (choice != null)
            {
                this.currentItem.Visibility = Visibility.Collapsed;
                switch (choice.Name)
                {
                    case "user":
                        this.currentItem = this.userInfo; break;
                    case "picture":
                        this.currentItem = this.profilePictureScenario; break;
                    case "friends":
                        this.currentItem = this.friendPickerScenario; break;
                    case "places":
                        this.currentItem = this.placePickerScenario; break;
                }

                this.currentItem.Visibility = Visibility.Visible;
                this.currentItem.Opacity = 100;
            }
        }

        private void OnUserInfoChanged(object sender, Facebook.Client.Controls.UserInfoChangedEventArgs e)
        {
            this.userInfo.DataContext = e.User;
        }

        private void OnSessionStateChanged(object sender, Facebook.Client.Controls.SessionStateChangedEventArgs e)
        {
            this.welcomeText.Opacity = (e.SessionState == FacebookSessionState.Opened) ? 0 : 100;
            this.ContentPanel.Opacity = (e.SessionState == FacebookSessionState.Opened) ? 100 : 0;
            this.userInfo.Visibility = Visibility.Collapsed;
            this.placePickerScenario.Visibility = Visibility.Collapsed;
            this.friendPickerScenario.Visibility = Visibility.Collapsed;
            this.placePickerScenario.Visibility = Visibility.Collapsed;
        }

        private void OnFriendRetrieved(object sender, DataItemRetrievedEventArgs<Facebook.Client.GraphUser> e)
        {
            if (e.Item.LastName.StartsWith("I"))
            {
                e.Exclude = true;
            }
        }

        private async void OnPlacePickerLoadFailed(object sender, LoadFailedEventArgs e)
        {
            var msgbox = new MessageDialog(e.Reason, e.Description);
            await msgbox.ShowAsync();
        }

        private void OnDisplayOrderSelected(object sender, RoutedEventArgs e)
        {
            if (this.friendPicker != null)
            {
                var choice = sender as RadioButton;
                this.friendPicker.DisplayOrder =
                    (Facebook.Client.Controls.FriendPickerDisplayOrder)Enum.Parse(typeof(Facebook.Client.Controls.FriendPickerDisplayOrder), choice.Name);
            }
        }
    }
}
