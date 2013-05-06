namespace Facebook.Client.Controls
{
#if NETFX_CORE
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Threading.Tasks;
    using Windows.ApplicationModel.Resources;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
#endif
#if WINDOWS_PHONE
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows;
    using Facebook.Client.Resources;
#endif

    /// <summary>
    /// Represents a button control that can log in or log out the user when clicked.
    /// </summary>
    /// <remarks>
    /// The LoginButton control keeps track of the authentication status and shows an appropriate label 
    /// that reflects whether the user is currently authenticated. When a user logs in, it can automatically 
    /// retrieve their basic information.
    /// </remarks>
    [TemplatePart(Name = PartLoginButton, Type = typeof(Button))]
    public sealed class LoginButton : Control
    {
        #region Part Definitions

        private const string PartLoginButton = "PART_LoginButton";

        #endregion Part Definitions

        #region Default Property Values

        private const string DefaultApplicationId = "";
        private const string DefaultAccessToken = "";
        private const string DefaultProfileId = "";
        private const Audience DefaultDefaultAudience = Audience.None;
        private const string DefaultPermissions = "";
        private const bool DefaultFetchUserInfo = true;
        private const FacebookSession DefaultCurrentSession = null;
        private const FacebookSession DefaultCurrentUser = null;
        private static readonly CornerRadius DefaultCornerRadius = new CornerRadius(0);

        #endregion Default Property Values

        #region Member variables

        private Button loginButton;
        private FacebookSessionClient facebookSessionClient;

        #endregion Member variables

        /// <summary>
        /// Initializes a new instance of the LoginButton class. 
        /// </summary>
        public LoginButton()
        {
            this.DefaultStyleKey = typeof(LoginButton);
        }

        #region Events

        /// <summary>
        /// Occurs whenever the status of the session associated with this control changes.
        /// </summary>
        public event EventHandler<SessionStateChangedEventArgs> SessionStateChanged;

        /// <summary>
        /// Occurs whenever a communication or authentication error occurs while logging in.
        /// </summary>
        public event EventHandler<AuthenticationErrorEventArgs> AuthenticationError;

        /// <summary>
        /// Occurs whenever the current user changes.
        /// </summary>
        /// <remarks>
        /// To retrieve the current user information, the FetchUserInfo property must be set to true.
        /// </remarks>
        public event EventHandler<UserInfoChangedEventArgs> UserInfoChanged;

        #endregion Events

        #region Properties

        #region ApplicationId

        /// <summary>
        /// Gets or sets the application ID to be used to open the session.
        /// </summary>
        public string ApplicationId
        {
            get { return (string)GetValue(ApplicationIdProperty); }
            set { this.SetValue(ApplicationIdProperty, value); }
        }

        /// <summary>
        /// Identifies the ApplicationId dependency property.
        /// </summary>
        public static readonly DependencyProperty ApplicationIdProperty =
            DependencyProperty.Register("ApplicationId", typeof(string), typeof(LoginButton), new PropertyMetadata(LoginButton.DefaultApplicationId, OnApplicationIdPropertyChanged));

        private static void OnApplicationIdPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (LoginButton)d;
            var applicationId = (string)e.NewValue;
            if (!string.IsNullOrWhiteSpace(applicationId))
            {
                target.facebookSessionClient = new FacebookSessionClient(applicationId);
            }
            else
            {
                target.facebookSessionClient = null;
            }
        }

        #endregion ApplicationId

        #region AccessToken

        /// <summary>
        /// Gets or sets the access token returned by the Facebook Login service.
        /// </summary>
        public string AccessToken
        {
            get { return (string)GetValue(AccessTokenProperty); }
            set { this.SetValue(AccessTokenProperty, value); }
        }

        /// <summary>
        /// Identifies the AccessToken dependency property.
        /// </summary>
        public static readonly DependencyProperty AccessTokenProperty =
            DependencyProperty.Register("AccessToken", typeof(string), typeof(LoginButton), new PropertyMetadata(LoginButton.DefaultAccessToken));

        #endregion AccessToken

        #region ProfileId

        /// <summary>
        /// The Facebook ID of the logged in user.
        /// </summary>
        public string ProfileId
        {
            get { return (string)GetValue(ProfileIdProperty); }
            set { this.SetValue(ProfileIdProperty, value); }
        }

        /// <summary>
        /// Identifies the ProfileId dependency property.
        /// </summary>
        public static readonly DependencyProperty ProfileIdProperty =
            DependencyProperty.Register("ProfileId", typeof(string), typeof(LoginButton), new PropertyMetadata(LoginButton.DefaultProfileId));
        
        #endregion ProfileId

        #region DefaultAudience

        /// <summary>
        /// The default audience to use, if publish permissions are requested at login time.
        /// </summary>
        /// <remarks>
        /// Certain operations such as publishing a status or publishing a photo require an audience. When the user grants an application 
        /// permission to perform a publish operation, a default audience is selected as the publication ceiling for the application. This 
        /// enumerated value allows the application to select which audience to ask the user to grant publish permission for.
        /// </remarks>
        public Audience DefaultAudience
        {
            get { return (Audience)GetValue(DefaultAudienceProperty); }
            set { this.SetValue(DefaultAudienceProperty, value); }
        }

        /// <summary>
        /// Identifies the DefaultAudience dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultAudienceProperty =
            DependencyProperty.Register("DefaultAudience", typeof(Audience), typeof(LoginButton), new PropertyMetadata(LoginButton.DefaultDefaultAudience));

        #endregion DefaultAudience

        #region Permissions

        /// <summary>
        /// The permissions to request.
        /// </summary>
        public string Permissions
        {
            get { return (string)GetValue(PermissionsProperty); }
            set { this.SetValue(PermissionsProperty, value); }
        }

        /// <summary>
        /// Identifies the Permissions dependency property.
        /// </summary>
        public static readonly DependencyProperty PermissionsProperty =
            DependencyProperty.Register("Permissions", typeof(string), typeof(LoginButton), new PropertyMetadata(LoginButton.DefaultPermissions));
        
        #endregion Permissions

        #region FetchUserInfo

        /// <summary>
        /// Controls whether the user information is fetched when the session is opened. Default is true.
        /// </summary>
        public bool FetchUserInfo
        {
            get { return (bool)GetValue(FetchUserInfoProperty); }
            set { this.SetValue(FetchUserInfoProperty, value); }
        }

        /// <summary>
        /// Identifies the FetchUserInfo dependency property.
        /// </summary>
        public static readonly DependencyProperty FetchUserInfoProperty =
            DependencyProperty.Register("FetchUserInfo", typeof(bool), typeof(LoginButton), new PropertyMetadata(LoginButton.DefaultFetchUserInfo));
        
        #endregion FetchUserInfo

        #region CurrentSession

        /// <summary>
        /// Gets the current active session.
        /// </summary>
        public FacebookSession CurrentSession
        {
            get { return (FacebookSession)GetValue(CurrentSessionProperty); }
            private set { this.SetValue(CurrentSessionProperty, value); }
        }

        /// <summary>
        /// Identifies the CurrentSession dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentSessionProperty =
            DependencyProperty.Register("CurrentSession", typeof(FacebookSession), typeof(LoginButton), new PropertyMetadata(LoginButton.DefaultCurrentSession, OnCurrentSessionPropertyChanged));

        private static void OnCurrentSessionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (LoginButton)d;
            target.UpdateSession();
        }
        
        #endregion CurrentSession

        #region CurrentUser

        /// <summary>
        /// Gets the current logged in user.
        /// </summary>
        public GraphUser CurrentUser
        {
            get { return (GraphUser)GetValue(CurrentUserProperty); }
            private set { this.SetValue(CurrentUserProperty, value); }
        }

        /// <summary>
        /// Identifies the CurrentUser dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentUserProperty =
            DependencyProperty.Register("CurrentUser", typeof(GraphUser), typeof(LoginButton), new PropertyMetadata(LoginButton.DefaultCurrentUser));

        #endregion CurrentUser

        #region CornerRadius

        /// <summary>
        /// Gets or sets a value that represents the degree to which the corners of a Border are rounded. 
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { this.SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Identifies the CornerRadius dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(LoginButton), new PropertyMetadata(LoginButton.DefaultCornerRadius));

        #endregion CornerRadius

        #endregion Properties

        #region Implementation

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) call ApplyTemplate. In simplest 
        /// terms, this means the method is called just before a UI element displays in your app. Override this method to influence the 
        /// default post-template logic of a class. 
        /// </summary>
#if NETFX_CORE
        protected override void OnApplyTemplate()
#endif
#if WINDOWS_PHONE
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            if (this.loginButton != null)
            {
                this.loginButton.Click -= this.OnLoginButtonClicked;
            }

            this.loginButton = this.GetTemplateChild(PartLoginButton) as Button;
            if (this.loginButton != null)
            {
                this.loginButton.Click += this.OnLoginButtonClicked;
                this.loginButton.DataContext = this;
            }

            this.UpdateButtonCaption();
        }

        private async void OnLoginButtonClicked(object sender, RoutedEventArgs e)
        {
            if (this.CurrentSession == null)
            {
                await this.LogIn();
            }
            else
            {
                this.LogOut();
            }
        }

        private async Task LogIn()
        {
            try
            {
                this.RaiseSessionStateChanged(new SessionStateChangedEventArgs(FacebookSessionState.Opening));

                // TODO: using Permissions for the time being until we decide how 
                // to handle separate ReadPermissions and PublishPermissions
                var session = await this.facebookSessionClient.LoginAsync(this.Permissions);

                // initialize current session
                this.CurrentSession = session;
                this.RaiseSessionStateChanged(new SessionStateChangedEventArgs(FacebookSessionState.Opened));

                // retrieve information about the current user
                if (this.FetchUserInfo)
                {
                    FacebookClient client = new FacebookClient(session.AccessToken);
                    var parameters = new Dictionary<string, object>();
                    parameters["fields"] = "id,name,username,first_name,middle_name,last_name,birthday,location,link";

                    dynamic result = await client.GetTaskAsync("me", parameters);
                    dynamic location = result.location;
                    this.CurrentUser = new GraphUser()
                    {
                        Id = result.id,
                        Name = result.name,
                        UserName = result.username,
                        FirstName = result.first_name,
                        MiddleName = result.middle_name,
                        LastName = result.last_name,
                        Birthday = result.birthday,
                        Location = new GraphLocation 
                        {
                            ////Street = location.street,
                            City = (location != null) ? location.name : null,
                            ////State = location.state,
                            ////Zip = location.zip,
                            ////Country = location.country,
                            ////Latitude = location.latitude ?? 0.0,
                            ////Longitude = location.longitude ?? 0.0
                        },
                        Link = result.link
                    };

                    var userInfo = new UserInfoChangedEventArgs(this.CurrentUser);
                    this.RaiseUserInfoChanged(userInfo);
                }
            }
            catch (ArgumentNullException error)
            {
                // TODO: remove when bug in SDK is fixed (the bug happens when you cancel the facebook login dialog)
                var authenticationErrorEventArgs =
                    new AuthenticationErrorEventArgs("Login failure.", error.Message);

                this.RaiseAuthenticationFailure(authenticationErrorEventArgs);
            }
            catch (InvalidOperationException error)
            {
                // TODO: need to obtain richer information than a generic InvalidOperationException
                var authenticationErrorEventArgs =
                    new AuthenticationErrorEventArgs("Login failure.", error.Message);

                this.RaiseAuthenticationFailure(authenticationErrorEventArgs);
            }
        }

        private void LogOut()
        {
            this.facebookSessionClient.Logout();
            this.CurrentSession = null;
            this.CurrentUser = null;
            this.RaiseSessionStateChanged(new SessionStateChangedEventArgs(FacebookSessionState.Closed));
        }

        private void RaiseSessionStateChanged(SessionStateChangedEventArgs e)
        {
            EventHandler<SessionStateChangedEventArgs> handler = this.SessionStateChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void RaiseUserInfoChanged(UserInfoChangedEventArgs e)
        {
            EventHandler<UserInfoChangedEventArgs> handler = this.UserInfoChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void RaiseAuthenticationFailure(AuthenticationErrorEventArgs e)
        {
            EventHandler<AuthenticationErrorEventArgs> handler = this.AuthenticationError;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(string), typeof(LoginButton), new PropertyMetadata(string.Empty));

        private void UpdateSession()
        {
            this.AccessToken = this.CurrentSession != null ? this.CurrentSession.AccessToken : string.Empty;
            this.ProfileId = this.CurrentSession != null ? this.CurrentSession.FacebookId : string.Empty;
            this.UpdateButtonCaption();
        }

        private void UpdateButtonCaption()
        {
#if NETFX_CORE
            var libraryName = typeof(LoginButton).GetTypeInfo().Assembly.GetName().Name;
            var name = string.Format(CultureInfo.InvariantCulture, "{0}/Resources/LoginButton", libraryName);
            var loader = new ResourceLoader(name);
            var resourceName = this.CurrentSession == null ? "Caption_OpenSession" : "Caption_CloseSession";
            var caption = loader.GetString(resourceName);
#endif
#if WINDOWS_PHONE
            var caption = this.CurrentSession == null ? AppResources.LoginButtonCaptionOpenSession : AppResources.LoginButtonCaptionCloseSession;
#endif
            this.SetValue(CaptionProperty, caption);
        }

        #endregion Implementation
    }
}
