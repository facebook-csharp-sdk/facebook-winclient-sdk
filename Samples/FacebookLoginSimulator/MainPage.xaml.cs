// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Microsoft">
//   2013
// </copyright>
// <summary>
//   The main page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FacebookLoginSimulator
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Windows;
    using System.Windows.Navigation;
    using Facebook.Client;
    using Windows.System;

    /// <summary>
    /// The main page
    /// </summary>
    public partial class MainPage
    {
        /// <summary>
        /// The callback URI format
        /// </summary>
        private const string CallbackUriFormat = "{0}?{1}";

        /// <summary>
        /// The redirect uri.
        /// </summary>
        private string redirectUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage" /> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            
            // Enter your access token for development purposes below.
            // You can grab one from the graph api explorer: https://developers.facebook.com/tools/explorer

        //#error missing access token
            this.AccessTokenBox.Text = "CAACEdEose0cBAJZC4W2BBCkZCEXs6Hi2tt5wFa78IZBaddAJ3zYl96kpO5niyLZA84p1FI8D75xEpDZByh6uqsvJuP8y3u1rF3G75rjd1HZC2PpIwXKtlM6uzEOprDET0D92f42P0fNZC16FjzjcTfn2qVddR1PZBapuoOxULCydV2Foc4Mmj0FgeLgSpEpnBacZD";
        }

        /// <summary>
        /// Called when a page becomes the active page in a frame.
        /// </summary>
        /// <param name="e">
        /// An object that contains the event data.
        /// </param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (this.NavigationContext.QueryString.TryGetValue("redirect_uri", out this.redirectUri))
            {
                if (!this.redirectUri.ToLower().StartsWith("msft-"))
                {
                    var loginResponse = new LoginResponse
                                        {
                                            ErrorCode = "2001", 
                                            ErrorReason = "Calling app's URI is invalid", 
                                            Error = "Invalid calling app URI",
                                            ErrorDescription = "Calling app's URI needs to start 'msft-'"
                                        };

                    this.LaunchCallingApp(loginResponse);
                }
            }

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// The access denied button_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AccessDeniedButton_OnClick(object sender, RoutedEventArgs e)
        {
            var loginResponse = new LoginResponse
                                {
                                    ErrorCode = "200", 
                                    Error = "access_denied", 
                                    ErrorDescription = "Permissions error", 
                                    ErrorReason = "user_denied"
                                };

            this.LaunchCallingApp(loginResponse);
        }

        /// <summary>
        /// Launches the calling app.
        /// </summary>
        /// <param name="loginResponse">
        /// The login response.
        /// </param>
        private async void LaunchCallingApp(LoginResponse loginResponse)
        {
            if (string.IsNullOrEmpty(this.redirectUri))
            {
                MessageBox.Show("No redirect uri provided");
                return;
            }

            string uri = string.Format(CallbackUriFormat, this.redirectUri, loginResponse.ToString());
            Debug.WriteLine("Final Redirect URI: " + uri);
            await Launcher.LaunchUriAsync(new Uri(uri, UriKind.Absolute));
            Application.Current.Terminate();
        }

        /// <summary>
        /// The no network button_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void NoNetworkButton_OnClick(object sender, RoutedEventArgs e)
        {
            var loginResponse = new LoginResponse
                                {
                                    ErrorCode = "2002", 
                                    ErrorReason = "No connection to facebook", 
                                    Error = "Connection failed", 
                                    ErrorDescription = "Unable to communicate with facebook in order to get the access token"
                                };

            this.LaunchCallingApp(loginResponse);
        }

        /// <summary>
        /// No access token response.
        /// </summary>
        private void NoAccessTokenResponse()
        {
            var loginResponse = new LoginResponse
                           {
                               ErrorCode = "2003",
                               ErrorDescription = "No access token provided in the login simulator",
                               Error = "No access token provided in the login simulator",
                               ErrorReason = "No access token provided in the login simulator"
                           };

            this.LaunchCallingApp(loginResponse);
        }

        /// <summary>
        /// The success button_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SuccessButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.AccessTokenBox.Text))
            {
                this.NoAccessTokenResponse();
                return;
            }

            var loginResponse = new LoginResponse
                                {
                                    AccessToken = this.AccessTokenBox.Text,
                                    ExpiresIn = this.AccessTokenExpiresInBox.Text
                                };

            this.LaunchCallingApp(loginResponse);
        }
    }
}