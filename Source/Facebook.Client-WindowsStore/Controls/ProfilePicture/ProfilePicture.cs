using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Facebook.Client.Controls
{
    /// <summary>
    /// Shows the profile picture for an object such as a user, place, or event.
    /// </summary>
    [TemplatePart(Name = PartProfilePicture, Type = typeof(Image))]
    public class ProfilePicture : Control
    {
        private Image image;

        #region Part Definitions

        private const string PartProfilePicture = "PART_ProfilePicture";

        #endregion Part Definitions

        /// <summary>
        /// Initializes a new instance of the ProfilePicture class.
        /// </summary>
        public ProfilePicture()
        {
            this.DefaultStyleKey = typeof(ProfilePicture);
        }

        #region AccessToken
        
        /// <summary>
        /// Gets or sets the access token returned by the Facebook Login service.
        /// </summary>
        public string AccessToken
        {
            get { return (string)GetValue(AccessTokenProperty); }
            set { SetValue(AccessTokenProperty, value); }
        }

        /// <summary>
        /// Identifies the AccessToken dependency property.
        /// </summary>
        public static readonly DependencyProperty AccessTokenProperty =
            DependencyProperty.Register("AccessToken", typeof(string), typeof(ProfilePicture), new PropertyMetadata(string.Empty)); 
        
        #endregion AccessToken
        
        #region ProfileId

        /// <summary>
        /// The Facebook ID of the user, place or object for which a picture should be fetched and displayed.
        /// </summary>
        public string ProfileId
        {
            get { return (string)GetValue(ProfileIdProperty); }
            set { SetValue(ProfileIdProperty, value); }
        }

        /// <summary>
        /// Identifies the ProfileId dependency property.
        /// </summary>
        public static readonly DependencyProperty ProfileIdProperty =
            DependencyProperty.Register("ProfileId", typeof(string), typeof(ProfilePicture), new PropertyMetadata(string.Empty, OnProfileIdPropertyChanged));

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
            set { SetValue(CropModeProperty, value); }
        }

        /// <summary>
        /// Identifies the CropMode dependency property.
        /// </summary>
        public static readonly DependencyProperty CropModeProperty =
            DependencyProperty.Register("CropMode", typeof(CropMode), typeof(ProfilePicture), new PropertyMetadata(CropMode.Original, OnCropModePropertyChanged));

        private static void OnCropModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ProfilePicture)d).LoadPicture();
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
#if WINDOWS_PHONE
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            this.image = ((Image)GetTemplateChild(PartProfilePicture));
            this.image.DataContext = this;
            LoadPicture();
        }

        private void LoadPicture()
        {
            const string graphApiUrl = "https://graph.facebook.com";

            if (double.IsNaN(this.Width) ||
                double.IsNaN(this.Height) ||
                string.IsNullOrEmpty(this.ProfileId))
            {
                return;
            }

            // TODO: if Facebook ID is null, force web.application.toolkits1's ID to retrieve 
            // its silhuoutte image, replace later with a suitable image
            var facebookId = string.IsNullOrEmpty(this.ProfileId) ? "100000627996796" : this.ProfileId;

            string profilePictureUrl = graphApiUrl;
            if (this.CropMode == CropMode.Square)
            {
                var size = Math.Min(this.Height, this.Width);
                profilePictureUrl = string.Format(
                    "{0}/{1}/picture?width={2}&height={3}",
                    profilePictureUrl,
                    facebookId,
                    size,
                    size);
            }
            else if (this.CropMode == CropMode.Original)
            {
                profilePictureUrl = string.Format(
                    "{0}/{1}/picture?width={2}&height={3}",
                    profilePictureUrl,
                    facebookId,
                    this.Width,
                    this.Height);
            }

            if (!string.IsNullOrEmpty(this.AccessToken))
            {
                profilePictureUrl += "&access_token=" + this.AccessToken;
            }

            SetValue(ImageSourceProperty, profilePictureUrl);
        }

        private static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(string), typeof(ProfilePicture), new PropertyMetadata(string.Empty));

        #endregion Implementation
    }
}
