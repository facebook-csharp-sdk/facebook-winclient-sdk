namespace Facebook.Client.Controls
{
#if NETFX_CORE
    using System;
    using System.Globalization;
    using System.Reflection;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
#endif
#if WP8
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
#endif

    /// <summary>
    /// Shows the profile picture for an object such as a user, place, or event.
    /// </summary>
    [TemplatePart(Name = PartProfilePicture, Type = typeof(Image))]
    public class ProfilePicture : Control
    {
        #region Part Definitions

        private const string PartProfilePicture = "PART_ProfilePicture";

        #endregion Part Definitions

        #region Default Property Values

        private const string DefaultAccessToken = "";
        private const string DefaultProfileId = "";
        private const CropMode DefaultCropMode = CropMode.Original;

        #endregion Default Property Values

        #region Member variables

        private Image image;

        #endregion Member variables

        /// <summary>
        /// Initializes a new instance of the ProfilePicture class.
        /// </summary>
        public ProfilePicture()
        {
            this.DefaultStyleKey = typeof(ProfilePicture);

            this.Loaded += ProfilePicture_Loaded;
        }

        void ProfilePicture_Loaded(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(Session.ActiveSession.CurrentAccessTokenData.AccessToken))
            {
                this.ProfileId = Session.ActiveSession.CurrentAccessTokenData.FacebookId;
            }
        }

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
            DependencyProperty.Register("AccessToken", typeof(string), typeof(ProfilePicture), new PropertyMetadata(ProfilePicture.DefaultAccessToken)); 
        
        #endregion AccessToken
        
        #region ProfileId

        /// <summary>
        /// The Facebook ID of the user, place or object for which a picture should be fetched and displayed.
        /// </summary>
        /// <remarks>
        /// The control displays a blank profile (silhouette) picture if this property is null or empty.
        /// </remarks>
        public string ProfileId
        {
            get { return (string)GetValue(ProfileIdProperty); }
            set { this.SetValue(ProfileIdProperty, value); }
        }

        /// <summary>
        /// Identifies the ProfileId dependency property.
        /// </summary>
        public static readonly DependencyProperty ProfileIdProperty =
            DependencyProperty.Register("ProfileId", typeof(string), typeof(ProfilePicture), new PropertyMetadata(ProfilePicture.DefaultProfileId, OnProfileIdPropertyChanged));

        private static void OnProfileIdPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ProfilePicture)d).LoadPicture();
        }

        #endregion ProfileId

        #region CropMode
        
        /// <summary>
        /// Gets or sets the cropping treatment of the profile picture.
        /// </summary>
        public CropMode CropMode
        {
            get { return (CropMode)GetValue(CropModeProperty); }
            set { this.SetValue(CropModeProperty, value); }
        }

        /// <summary>
        /// Identifies the CropMode dependency property.
        /// </summary>
        public static readonly DependencyProperty CropModeProperty =
            DependencyProperty.Register("CropMode", typeof(CropMode), typeof(ProfilePicture), new PropertyMetadata(ProfilePicture.DefaultCropMode, OnCropModePropertyChanged));

        private static void OnCropModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var profilePicture = (ProfilePicture)d;
            if (profilePicture.image != null)
            {
                profilePicture.image.Stretch = (CropMode)e.NewValue == CropMode.Fill ? Stretch.UniformToFill : Stretch.Uniform;
            }

            profilePicture.LoadPicture();
        }
        
        #endregion CropMode

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

            this.image = (Image)GetTemplateChild(PartProfilePicture);
            this.image.DataContext = this;
            this.image.Stretch = this.CropMode == CropMode.Fill ? Stretch.UniformToFill : Stretch.Uniform;
            this.LoadPicture();
        }

        /// <summary>
        /// Provides the behavior for the "Arrange" pass of layout. Classes can override this method to define their own "Arrange" pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        /// <returns>The actual size that is used after the element is arranged in layout.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.LoadPicture();
            return base.ArrangeOverride(finalSize);
        }

        async private void LoadPicture()
        {
            string profilePictureUrl;

            // TODO: (sanjeevd) A bit of a hack. The picture control should be shown only when the user is logged in.
            // Needs fixing
            if (Session.ActiveSession.CurrentAccessTokenData != null &&
                Session.ActiveSession.CurrentAccessTokenData.AccessToken != null &&
                this.ProfileId == null &&
                Session.ActiveSession.CurrentAccessTokenData.AppId == null)
            {
                FacebookClient client = new FacebookClient(Session.ActiveSession.CurrentAccessTokenData.AccessToken);
                dynamic result = await client.GetTaskAsync("me");
                this.ProfileId = (new GraphUser(result)).Id;
            }
            if (string.IsNullOrEmpty(this.ProfileId))
            {
                profilePictureUrl = ProfilePicture.GetBlankProfilePictureUrl(this.CropMode == CropMode.Square);
            }
            else
            {
                profilePictureUrl = this.GetFacebookProfilePictureUrl();
            }

            var currentprofilePictureUrl = (string)this.GetValue(ImageSourceProperty);
            if (currentprofilePictureUrl != profilePictureUrl)
            {
                this.SetValue(ImageSourceProperty, profilePictureUrl);
            }
        }

        private string GetFacebookProfilePictureUrl()
        {
            string profilePictureUrl;
            const string GraphApiUrl = "https://graph.facebook.com";

            if (this.CropMode == CropMode.Square)
            {
                var size = Math.Max(this.Height, this.Width);
                profilePictureUrl = string.Format(
                                        CultureInfo.InvariantCulture,
                                        "{0}/{1}/picture?width={2}&height={3}",
                                        GraphApiUrl,
                                        this.ProfileId,
                                        size,
                                        size);
            }
            else
            {
                profilePictureUrl = string.Format(
                                        CultureInfo.InvariantCulture,
                                        "{0}/{1}/picture?width={2}&height={3}",
                                        GraphApiUrl,
                                        this.ProfileId,
                                        this.Width,
                                        this.Height);
            }

            if (!string.IsNullOrEmpty(this.AccessToken))
            {
                profilePictureUrl += "&access_token=" + this.AccessToken;
            }

            return profilePictureUrl;
        }

        private static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(string), typeof(ProfilePicture), new PropertyMetadata(string.Empty));

        internal static string GetBlankProfilePictureUrl(bool isSquare)
        {
            const string BlankProfilePictureSquare = "fb_blank_profile_square.png";
            const string BlankProfilePicturePortrait = "fb_blank_profile_portrait.png";

            string imageName = isSquare ? BlankProfilePictureSquare : BlankProfilePicturePortrait;
#if NETFX_CORE
            var libraryName = typeof(ProfilePicture).GetTypeInfo().Assembly.GetName().Name;

            return string.Format(CultureInfo.InvariantCulture, "ms-appx:///{0}/Images/{1}", libraryName, imageName);
#endif
#if WP8
            return string.Format(CultureInfo.InvariantCulture, "/Images/{0}", imageName);
#endif
        }

        #endregion Implementation
    }
}
