namespace BasicApp
{
    using Facebook.Client.Controls;
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
            ////this.Loaded += MainPage_Loaded;
        }

        ////async void MainPage_Loaded(object sender, RoutedEventArgs e)
        ////{
        ////    await Authenticate();
        ////}

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
        }

        private void OnUserInfoChanged(object sender, Facebook.Client.Controls.UserInfoChangedEventArgs e)
        {
            this.userInfo.DataContext = e.User;
        }

        private void OnSessionStateChanged(object sender, Facebook.Client.Controls.SessionStateChangedEventArgs e)
        {
            this.welcomeText.Opacity = (e.SessionState == FacebookSessionState.Opened) ? 0 : 100;
            //this.userInfo.Opacity = (e.SessionState == FacebookSessionState.Opened) ? 100 : 0;
            this.friendPicker.Opacity = (e.SessionState == FacebookSessionState.Opened) ? 100 : 0;
        }

        private void OnFriendRetrieved(object sender, FriendRetrievedEventArgs e)
        {
            if (e.Friend.LastName.StartsWith("I"))
            {
                e.Exclude = true;
            }
        }
    }
}
