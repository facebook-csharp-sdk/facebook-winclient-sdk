using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif
#if WINDOWS_PHONE
using System.Windows.Controls;
using System.Windows;
#endif

namespace Facebook.Client.Controls
{
    /// <summary>
    /// Represents a button control that can log in or log out the user when clicked.
    /// </summary>
    /// <remarks>
    /// The LoginButton keeps track of the authentication status and shows an appropriate label that 
    /// reflects whether the user is currently authenticated. When a user logs in, it can automatically 
    /// retrieve their basic information.
    /// </remarks>
    [TemplatePart(Name = Part_LoginButton, Type = typeof(Button))]
    [TemplatePart(Name = Part_Caption, Type = typeof(TextBlock))]
    public sealed class LoginButton : Control
    {
        private Button loginButton;
        private FacebookSessionClient facebookSessionClient;

        /// <summary>
        /// Initializes a new instance of the LoginButton class. 
        /// </summary>
        public LoginButton()
        {
            this.DefaultStyleKey = typeof(LoginButton);
        }

        #region Part Definitions

        private const string Part_LoginButton = "PART_LoginButton";
        private const string Part_Caption = "PART_Caption";

        #endregion Part Definitions

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
            set { SetValue(ApplicationIdProperty, value); }
        }

        /// <summary>
        /// Identifies the ApplicationId dependency property.
        /// </summary>
        public static readonly DependencyProperty ApplicationIdProperty =
            DependencyProperty.Register("ApplicationId", typeof(string), typeof(LoginButton), new PropertyMetadata(string.Empty, OnApplicationIdPropertyChanged));

        private static void OnApplicationIdPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (LoginButton)d;
            target.facebookSessionClient = new FacebookSessionClient(target.ApplicationId);
        }

        #endregion ApplicationId

        #region DefaultAudience

        /// <summary>
        /// The default audience to use, if publish permissions are requested at login time.
        /// </summary>
        /// <remarks>
        /// Certain operations such as publishing a status or publishing a photo require an audience. When the user grants an application 
        /// permission to perform a publish operation, a default audience is selected as the publication ceiling for the application. This 
        /// enumerated value allows the application to select which audience to ask the user to grant publish permission for.
        /// </remarks>
        public DefaultAudience DefaultAudience
        {
            get { return (DefaultAudience)GetValue(DefaultAudienceProperty); }
            set { SetValue(DefaultAudienceProperty, value); }
        }

        /// <summary>
        /// Identifies the DefaultAudience dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultAudienceProperty =
            DependencyProperty.Register("DefaultAudience", typeof(DefaultAudience), typeof(LoginButton), new PropertyMetadata(DefaultAudience.None));

        #endregion DefaultAudience

        #region ReadPermissions

        /// <summary>
        /// The read permissions to request.
        /// </summary>
        /// <remarks>
        /// Note, that if read permissions are specified, then publish permissions should not be specified.
        /// </remarks>
        public string ReadPermissions
        {
            get { return (string)GetValue(ReadPermissionsProperty); }
            set { SetValue(ReadPermissionsProperty, value); }
        }

        /// <summary>
        /// Identifies the ReadPermissions dependency property.
        /// </summary>
        public static readonly DependencyProperty ReadPermissionsProperty =
            DependencyProperty.Register("ReadPermissions", typeof(string), typeof(LoginButton), new PropertyMetadata(null));
        
        #endregion ReadPermissions

        #region PublishPermissions

        /// <summary>
        /// The publish permissions to request.
        /// </summary>
        /// <remarks>
        /// Note, that a defaultAudience value of OnlyMe, Everyone, or Friends should be set if publish permissions are 
        /// specified. Additionally, when publish permissions are specified, then read should not be specified.
        /// </remarks>
        public string PublishPermissions
        {
            get { return (string)GetValue(PublishPermissionsProperty); }
            set { SetValue(PublishPermissionsProperty, value); }
        }

        /// <summary>
        /// Identifies the PublishPermissions dependency property.
        /// </summary>
        public static readonly DependencyProperty PublishPermissionsProperty =
            DependencyProperty.Register("PublishPermissions", typeof(string), typeof(LoginButton), new PropertyMetadata(null));

        #endregion PublishPermissions

        #region FetchUserInfo

        /// <summary>
        /// Controls whether the user information is fetched when the session is opened. Default is true.
        /// </summary>
        public bool FetchUserInfo
        {
            get { return (bool)GetValue(FetchUserInfoProperty); }
            set { SetValue(FetchUserInfoProperty, value); }
        }

        /// <summary>
        /// Identifies the FetchUserInfo dependency property.
        /// </summary>
        public static readonly DependencyProperty FetchUserInfoProperty =
            DependencyProperty.Register("FetchUserInfo", typeof(bool), typeof(LoginButton), new PropertyMetadata(true));
        
        #endregion FetchUserInfo

        #region CurrentSession

        /// <summary>
        /// Gets the current active session.
        /// </summary>
        public FacebookSession CurrentSession
        {
            get { return (FacebookSession)GetValue(CurrentSessionProperty); }
            private set { SetValue(CurrentSessionProperty, value); }
        }

        /// <summary>
        /// Identifies the CurrentSession dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentSessionProperty =
            DependencyProperty.Register("CurrentSession", typeof(FacebookSession), typeof(LoginButton), new PropertyMetadata(null, OnCurrentSessionPropertyChanged));

        private static void OnCurrentSessionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (LoginButton)d;
            target.UpdateButtonCaption();
        }
        
        #endregion CurrentSession

        #region CornerRadius

        /// <summary>
        /// Gets or sets a value that represents the degree to which the corners of a Border are rounded. 
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Identifies the CornerRadius dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(LoginButton), new PropertyMetadata(new CornerRadius(0)));

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

            this.loginButton = this.GetTemplateChild(Part_LoginButton) as Button;
            if (this.loginButton == null)
            {
                // TODO: throw appropriate exception
                throw new Exception(string.Format("Template element '{0}' is missing.", Part_LoginButton));
            }

            this.loginButton.DataContext = this;
            this.loginButton.Click += OnLoginButtonClicked;
            UpdateButtonCaption();
        }

        async void OnLoginButtonClicked(object sender, RoutedEventArgs e)
        {
            if (this.CurrentSession == null)
            {
                await LogIn();
            }
            else
            {
                LogOut();
            }
        }

        private async Task LogIn()
        {
            try
            {
                RaiseSessionStateChanged(new SessionStateChangedEventArgs(FacebookSessionState.Opening));

                // TODO: using only ReadPermissions for the time being until we decide how 
                // to handle separate ReadPermissions and PublishPermissions
                var session = await this.facebookSessionClient.LoginAsync(this.ReadPermissions);

                // initialize current session
                this.CurrentSession = session;
                RaiseSessionStateChanged(new SessionStateChangedEventArgs(FacebookSessionState.Opened));

                // retrieve information about the current user
                if (this.FetchUserInfo)
                {
                    FacebookClient client = new FacebookClient(session.AccessToken);
                    var parameters = new Dictionary<string, object>();
                    parameters["fields"] = "id,name,username,first_name,middle_name,last_name,birthday,location,link";

                    var result = await client.GetTaskAsync("me", parameters) as IDictionary<string, object>;

                    var userInfo = new UserInfoChangedEventArgs(
                        new FacebookUser()
                        {
                            Id = result.ContainsKey("id") ? (string) result["id"] : string.Empty,
                            Name = result.ContainsKey("name") ? (string) result["name"] : string.Empty,
                            UserName = result.ContainsKey("username") ? (string) result["username"] : string.Empty,
                            FirstName = result.ContainsKey("first_name") ? (string)result["first_name"] : string.Empty,
                            MiddleName = result.ContainsKey("middle_name") ? (string)result["middle_name"] : string.Empty,
                            LastName = result.ContainsKey("last_name") ? (string)result["last_name"] : string.Empty,
                            Birthday = result.ContainsKey("birthday") ? (string)result["birthday"] : string.Empty,
                            //Location = result.ContainsKey("location") ? (string)result["location"] : string.Empty,
                            Link = result.ContainsKey("link") ? (string)result["link"] : string.Empty
                        });

                    RaiseUserInfoChanged(userInfo);
                }
            }
            catch (InvalidOperationException error)
            {
                // TODO: need to obtain richer information than a generic InvalidOperationException
                var authenticationErrorEventArgs =
                    new AuthenticationErrorEventArgs("Login failure.", error.Message);

                RaiseAuthenticationFailure(authenticationErrorEventArgs);
            }
        }

        private void LogOut()
        {
            this.facebookSessionClient.Logout();
            this.CurrentSession = null;
            RaiseSessionStateChanged(new SessionStateChangedEventArgs(FacebookSessionState.Closed));
        }

        private void RaiseSessionStateChanged(SessionStateChangedEventArgs e)
        {
            EventHandler<SessionStateChangedEventArgs> handler = SessionStateChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void RaiseUserInfoChanged(UserInfoChangedEventArgs e)
        {
            EventHandler<UserInfoChangedEventArgs> handler = UserInfoChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void RaiseAuthenticationFailure(AuthenticationErrorEventArgs e)
        {
            EventHandler<AuthenticationErrorEventArgs> handler = AuthenticationError;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void UpdateButtonCaption()
        {
            this.loginButton.Content = this.CurrentSession == null ? "Log In" : "Log Out";
        }

        #endregion Implementation
    }
}
