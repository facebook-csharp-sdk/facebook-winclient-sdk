// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Microsoft">
//   2013
// </copyright>
// <summary>
//   Implements the MainPage class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BasicAppAuthentication
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows;
    using Facebook;
    using Facebook.Client;

    /// <summary>
    ///     Implements the MainPage class
    /// </summary>
    public partial class MainPage : INotifyPropertyChanged
    {
        #region Constants

        /// <summary>
        ///     The facebook app id (this is the ID given by facebook in the developer portal for your app)
        /// </summary>
        private const string AppId = "540541885996234";
        //public const string AppId = "186169374816846";

        #endregion

        #region Fields

        /// <summary>
        ///     The current user
        /// </summary>
        private GraphUser currentUser;

        /// <summary>
        /// The progress is visible.
        /// </summary>
        private bool progressIsVisible;

        /// <summary>
        /// The progress text.
        /// </summary>
        private string progressText;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the MainPage class
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += this.MainPageLoaded;
            this.DataContext = this;
        }

        #endregion

        #region Public Events

        /// <summary>
        ///     Event raised when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the current user.
        /// </summary>
        public GraphUser CurrentUser
        {
            get
            {
                return this.currentUser;
            }

            set
            {
                if (value != this.currentUser)
                {
                    this.currentUser = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether progress is visible.
        /// </summary>
        public bool ProgressIsVisible
        {
            get
            {
                return this.progressIsVisible;
            }

            set
            {
                if (value != this.progressIsVisible)
                {
                    this.progressIsVisible = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the progress text.
        /// </summary>
        public string ProgressText
        {
            get
            {
                return this.progressText;
            }

            set
            {
                if (value != this.progressText)
                {
                    this.progressText = value;
                    this.OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">
        /// Name of the property.
        /// </param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Handles the event for user tapping on the login button
        /// </summary>
        /// <param name="sender">
        /// Sender object
        /// </param>
        /// <param name="e">
        /// Event args
        /// </param>
        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            SessionStorage.Remove();
            
            FacebookSessionClient fb = new FacebookSessionClient(AppId);
            fb.LoginWithApp("basic_info,publish_actions,read_stream", "custom_state_string");
        }

        /// <summary>
        /// Logouts the click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="RoutedEventArgs"/> instance containing the event data.
        /// </param>
        private void LogoutClick(object sender, RoutedEventArgs e)
        {
            //SessionStorage.Remove();
            //this.CurrentUser = null;
            //var session = SessionStorage.Load();
            NavigationService.Navigate(new Uri("/Dialog.xaml", UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Handles the page load event
        /// </summary>
        /// <param name="sender">
        /// Sender object
        /// </param>
        /// <param name="e">
        /// Event args
        /// </param>
        private async void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            var session = SessionStorage.Load();

            if (null != session)
            {
                this.ExpiryText.Text = string.Format("Login expires on: {0}", session.Expires.ToString());

                this.ProgressText = "Fetching details from Facebook...";
                this.ProgressIsVisible = true;

                try
                {
                    var fb = new FacebookClient(session.AccessToken);

                    dynamic result = await fb.GetTaskAsync("me");
                    var user = new GraphUser(result);
                    user.ProfilePictureUrl = new Uri(string.Format("https://graph.facebook.com/{0}/picture?access_token={1}", user.Id, session.AccessToken));

                    this.CurrentUser = user;

                    await this.GetUserStatus(fb);
                }
                catch (FacebookOAuthException exception)
                {
                    MessageBox.Show("Error fetching user data: " + exception.Message);
                }

                this.ProgressText = string.Empty;
                this.ProgressIsVisible = false;
            }
        }

        /// <summary>
        /// Gets the user status.
        /// </summary>
        /// <param name="fb">The facebook client.</param>
        /// <returns>
        /// a task
        /// </returns>
        private async Task GetUserStatus(FacebookClient fb)
        {
            dynamic statusResult = await fb.GetTaskAsync("me?fields=statuses.limit(1).fields(message)");

            this.StatusText.Text = statusResult.statuses.data[0].message;
        }

        #endregion

        /// <summary>
        /// Handles the OnClick event of the UpdateStatusButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void UpdateStatusButton_OnClick(object sender, RoutedEventArgs e)
        {
            var session = SessionStorage.Load();
            if (null == session)
            {
                return;
            }

            this.ProgressText = "Updating status...";
            this.ProgressIsVisible = true;

            this.UpdateStatusButton.IsEnabled = false;

            try
            {
                var fb = new FacebookClient(session.AccessToken);

                await fb.PostTaskAsync(string.Format("me/feed?message={0}", this.UpdateStatusBox.Text), null);

                await this.GetUserStatus(fb);

                this.UpdateStatusBox.Text = string.Empty;
            }
            catch (FacebookOAuthException exception)
            {
                MessageBox.Show("Error fetching user data: " + exception.Message);
            }

            this.ProgressText = string.Empty;
            this.ProgressIsVisible = false;
            this.UpdateStatusButton.IsEnabled = true;
        }
    }
}