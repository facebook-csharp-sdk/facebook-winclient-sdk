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
#if WP8
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
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
        private const Audience DefaultDefaultAudience = Audience.None;
        private const string DefaultPermissions = "";
        private const bool DefaultFetchUserInfo = true;
        private const AccessTokenData DefaultCurrentSession = null;
        private const AccessTokenData DefaultCurrentUser = null;
        private static readonly CornerRadius DefaultCornerRadius = new CornerRadius(0);

        #endregion Default Property Values

        #region Member variables

        private Button loginButton;

        #endregion Member variables

        /// <summary>
        /// Initializes a new instance of the LoginButton class. 
        /// </summary>
        public LoginButton()
        {
            this.DefaultStyleKey = typeof(LoginButton);

            this.Loaded += LoginButton_Loaded;
        }

        void LoginButton_Loaded(object sender, RoutedEventArgs e)
        {
            //await  PreloadUserInformation();
            UpdateButtonCaption(Session.ActiveSession.CurrentAccessTokenData != null && !String.IsNullOrEmpty(Session.ActiveSession.CurrentAccessTokenData.AccessToken) ? LoginStatus.LoggedIn : LoginStatus.LoggedOut); ;
            Session.OnSessionStateChanged += UpdateButtonCaption;
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


        #region DefaultAudience

        /// <summary>
        /// Gets or sets the default audience to use, if publish permissions are requested at login time.
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
        /// Gets or sets the permissions to request.
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

        #region LoginBehavior

        /// <summary>
        /// Gets or sets the permissions to request.
        /// </summary>
        public FacebookLoginBehavior LoginBehavior
        {
            get { return (FacebookLoginBehavior)GetValue(LoginBehaviorProperty); }
            set { this.SetValue(LoginBehaviorProperty, value); }
        }

        /// <summary>
        /// Identifies the Permissions dependency property.
        /// </summary>
        public static readonly DependencyProperty LoginBehaviorProperty =
            DependencyProperty.Register("LoginBehavior", typeof(FacebookLoginBehavior), typeof(LoginButton), 
#if WINDOWS
            new PropertyMetadata(FacebookLoginBehavior.LoginBehaviorWebViewOnly));
#else
            new PropertyMetadata(FacebookLoginBehavior.LoginBehaviorMobileInternetExplorerOnly));
#endif

        #endregion LoginBehavior

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

        #region UpdateCaption
        /// <summary>
        /// Identifies the CurrentSession dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentSessionProperty =
            DependencyProperty.Register("CurrentSession", typeof(AccessTokenData), typeof(LoginButton), new PropertyMetadata(LoginButton.DefaultCurrentSession, OnCurrentSessionPropertyChanged));

        private static void OnCurrentSessionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (LoginButton)d;
            target.UpdateSession();
        }

        #endregion UpdateCaption

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

        #region Methods
        
        /// <summary>
        /// Requests new permissions for the current Facebook session.
        /// </summary>
        /// <param name="permissions">The permissions to request.</param>
        public async Task RequestNewPermissions(string permissions)
        {
            await this.LogIn(permissions);
        }

        #endregion Methods

        #region Implementation

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) call ApplyTemplate. In simplest 
        /// terms, this means the method is called just before a UI element displays in your app. Override this method to influence the 
        /// default post-template logic of a class. 
        /// </summary>
#if NETFX_CORE
        protected override void OnApplyTemplate()
#endif
#if WP8
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            if (this.loginButton != null)
            {
                this.loginButton.Click -= this.OnLoginButtonClicked;
            }

            //if (String.IsNullOrEmpty(Session.ActiveSession.CurrentAccessTokenData.AccessToken))
            //{
            //    PreloadUserInformation();
            //}

            this.loginButton = this.GetTemplateChild(PartLoginButton) as Button;
            if (this.loginButton != null)
            {
                this.loginButton.Click += this.OnLoginButtonClicked;
                
                this.loginButton.DataContext = this;
            }


        }

        private async void OnLoginButtonClicked(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(Session.ActiveSession.CurrentAccessTokenData.AccessToken))
            {
                await this.LogIn();
            }
            else
            {
                this.LogOut();
                this.SessionStateChanged.RaiseEvent(this, new SessionStateChangedEventArgs(FacebookSessionState.Closed));
            }
        }

        //internal async Task PreloadUserInformation()
        //{
        //    if (!String.IsNullOrEmpty(Session.ActiveSession.CurrentAccessTokenData.AccessToken))
        //    {
        //        FacebookClient client = new FacebookClient(Session.ActiveSession.CurrentAccessTokenData.AccessToken);
        //        dynamic result = await client.GetTaskAsync("me");
        //        this.CurrentUser = new GraphUser(result);
        //        Session.ActiveSession.CurrentAccessTokenData.FacebookId = this.CurrentUser.Id;
        //        AccessTokenDataCacheProvider.Current.SaveSessionData(Session.ActiveSession.CurrentAccessTokenData);
        //        var userInfo = new UserInfoChangedEventArgs(this.CurrentUser);
        //        this.UserInfoChanged.RaiseEvent(this, userInfo);

        //        this.SessionStateChanged.RaiseEvent(this, new SessionStateChangedEventArgs(FacebookSessionState.Opened));
        //    }
        //}

        private async Task LogIn(string permissions = null)
        {
            try
            {
                this.SessionStateChanged.RaiseEvent(this, new SessionStateChangedEventArgs(FacebookSessionState.Opening));

                Session.ActiveSession.LoginWithBehavior(permissions ?? this.Permissions,
                        this.LoginBehavior);
            }
            catch (ArgumentNullException error)
            {
                // TODO: remove when bug in SDK is fixed (the bug happens when you cancel the facebook login dialog)
                var authenticationErrorEventArgs =
                    new AuthenticationErrorEventArgs("Login failure.", error.Message);

                this.AuthenticationError.RaiseEvent(this, authenticationErrorEventArgs);
            }
            catch (InvalidOperationException error)
            {
                // TODO: need to obtain richer information than a generic InvalidOperationException
                var authenticationErrorEventArgs =
                    new AuthenticationErrorEventArgs("Login failure.", error.Message);

                this.AuthenticationError.RaiseEvent(this, authenticationErrorEventArgs);
            }
            catch (FacebookOAuthException error)
            {
                var authenticationErrorEventArgs =
                    new AuthenticationErrorEventArgs("Login failure.", error.Message);

                this.AuthenticationError.RaiseEvent(this, authenticationErrorEventArgs);
            }
        }

        private void LogOut()
        {
            Session.ActiveSession.Logout();
            this.CurrentUser = null;
            this.SessionStateChanged.RaiseEvent(this, new SessionStateChangedEventArgs(FacebookSessionState.Closed));
        }

        private static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(string), typeof(LoginButton), new PropertyMetadata("Login"));

        private void UpdateSession()
        {
            UpdateButtonCaption(Session.ActiveSession.CurrentAccessTokenData != null && !String.IsNullOrEmpty(Session.ActiveSession.CurrentAccessTokenData.AccessToken) ? LoginStatus.LoggedIn : LoginStatus.LoggedOut); ;
        }

        private void UpdateButtonCaption(LoginStatus status)
        {
#if NETFX_CORE
            var caption = status == LoginStatus.LoggedIn ? "LogOut" : "Login";
            if (Session.ActiveSession.CurrentAccessTokenData != null && !String.IsNullOrEmpty(Session.ActiveSession.CurrentAccessTokenData.AccessToken))
            {               
                this.SessionStateChanged.RaiseEvent(this, new SessionStateChangedEventArgs(FacebookSessionState.Opened));
                
            }
#endif
#if WP8
            var caption = String.IsNullOrEmpty(Session.ActiveSession.CurrentAccessTokenData.AccessToken) ? AppResources.LoginButtonCaptionOpenSession : AppResources.LoginButtonCaptionCloseSession;
#endif
            this.SetValue(CaptionProperty, caption);
        }

        #endregion Implementation
    }
}
