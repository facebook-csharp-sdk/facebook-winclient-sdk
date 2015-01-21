using System.Windows;

namespace Facebook.Client.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Windows.Foundation;

#if WINDOWS_PHONE || WINDOWS
    using Windows.UI.Xaml;
#endif

    /// <summary>
    /// Shows a user interface that can be used to select Facebook friends.
    /// </summary>
    public class FriendPicker : Picker<GraphUser>
    {
        #region Default Property Values

        private const string DefaultAccessToken = "";
        private const string DefaultProfileId = "me";
        private const string DefaultDisplayFields = "id,name,first_name,middle_name,last_name,picture";
        private const FriendPickerDisplayOrder DefaultDisplayOrder = FriendPickerDisplayOrder.DisplayLastNameFirst;
        private const FriendPickerSortOrder DefaultSortOrder = FriendPickerSortOrder.SortByLastName;
        private const bool DefaultDisplayProfilePictures = true;
        private static readonly Size DefaultPictureSize = new Size(50, 50);

        #endregion Default Property Values

        /// <summary>
        /// Initializes a new instance of the FriendPicker class.
        /// </summary>
        public FriendPicker()
            : base()
        {
            this.DefaultStyleKey = typeof(FriendPicker);

            this.Loaded += FriendPicker_Loaded;
        }

        void FriendPicker_Loaded(object sender, RoutedEventArgs e)
        {
            Session.OnSessionStateChanged += UpdateWithLoginStatus;
            if (Session.ActiveSession.CurrentAccessTokenData.AccessToken != null)
            {
                AccessToken = Session.ActiveSession.CurrentAccessTokenData.AccessToken;
            }
        }

        internal void UpdateWithLoginStatus(LoginStatus status)
        {
            if (status == LoginStatus.LoggedIn)
            {
                AccessToken = Session.ActiveSession.CurrentAccessTokenData.AccessToken;
            }
            else
            {
                AccessToken = null;
            }
        }

        #region Properties

        #region AccessToken

        /// <summary>
        /// Gets or sets the access token returned by the Facebook Login service.
        /// </summary>
        public string AccessToken
        {
            get { return (string)this.GetValue(AccessTokenProperty); }
            set { this.SetValue(AccessTokenProperty, value); }
        }

        /// <summary>
        /// Identifies the AccessToken dependency property.
        /// </summary>
        public static readonly DependencyProperty AccessTokenProperty =
            DependencyProperty.Register("AccessToken", typeof(string), typeof(FriendPicker), new PropertyMetadata(DefaultAccessToken, OnAccessTokenPropertyChanged));

        private static async void OnAccessTokenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var friendPicker = (FriendPicker)d;
            await friendPicker.RefreshData();
        }

        #endregion AccessToken

        #region ProfileId

        /// <summary>
        /// The profile ID of the user for which to retrieve a list of friends.
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
            DependencyProperty.Register("ProfileId", typeof(string), typeof(FriendPicker), new PropertyMetadata(DefaultProfileId, OnProfileIdPropertyChanged));

        private static async void OnProfileIdPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var friendPicker = (FriendPicker)d;
            await friendPicker.RefreshData();
        }

        #endregion ProfileId

        #region DisplayOrder

        /// <summary>
        /// Controls whether to display first name or last name first.
        /// </summary>
        public FriendPickerDisplayOrder DisplayOrder
        {
            get { return (FriendPickerDisplayOrder)GetValue(DisplayOrderProperty); }
            set { this.SetValue(DisplayOrderProperty, value); }
        }

        /// <summary>
        /// Identifies the DisplayOrder dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayOrderProperty =
            DependencyProperty.Register("DisplayOrder", typeof(FriendPickerDisplayOrder), typeof(FriendPicker), new PropertyMetadata(DefaultDisplayOrder));

        #endregion DisplayOrder

        #region SortOrder

        /// <summary>
        /// Controls the order in which friends are listed in the friend picker.
        /// </summary>
        public FriendPickerSortOrder SortOrder
        {
            get { return (FriendPickerSortOrder)GetValue(SortOrderProperty); }
            set { this.SetValue(SortOrderProperty, value); }
        }

        /// <summary>
        /// Identifies the SortOrder dependency property.
        /// </summary>
        public static readonly DependencyProperty SortOrderProperty =
            DependencyProperty.Register("SortOrder", typeof(FriendPickerSortOrder), typeof(FriendPicker), new PropertyMetadata(DefaultSortOrder));

        #endregion SortOrder

        #region DisplayFields

        /// <summary>
        /// Gets or sets additional fields to fetch when requesting friend data.
        /// </summary>
        /// <remarks>
        /// By default, the following data is retrieved for each friend: id, name, first_name, middle_name, last_name and picture.
        /// </remarks>
        public string DisplayFields
        {
            get { return (string)GetValue(DisplayFieldsProperty); }
            set { this.SetValue(DisplayFieldsProperty, value); }
        }

        /// <summary>
        /// Identifies the DisplayFields dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayFieldsProperty =
            DependencyProperty.Register("DisplayFields", typeof(string), typeof(FriendPicker), new PropertyMetadata(DefaultDisplayFields));

        #endregion DisplayFields

        #region DisplayProfilePictures

        /// <summary>
        /// Specifies whether profile pictures are displayed.
        /// </summary>
        public bool DisplayProfilePictures
        {
            get { return (bool)GetValue(DisplayProfilePicturesProperty); }
            set { this.SetValue(DisplayProfilePicturesProperty, value); }
        }

        /// <summary>
        /// Identifies the DisplayProfilePictures dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayProfilePicturesProperty =
            DependencyProperty.Register("DisplayProfilePictures", typeof(bool), typeof(FriendPicker), new PropertyMetadata(DefaultDisplayProfilePictures));

        #endregion DisplayProfilePictures

        #region PictureSize
        /// <summary>
        /// Gets or sets the size of the profile pictures displayed.
        /// </summary>
        public Size PictureSize
        {
            get { return (Size)GetValue(PictureSizeProperty); }
            set { this.SetValue(PictureSizeProperty, value); }
        }

        /// <summary>
        /// Identifies the PictureSize dependency property.
        /// </summary>
        public static readonly DependencyProperty PictureSizeProperty =
            DependencyProperty.Register("PictureSize", typeof(Size), typeof(FriendPicker), new PropertyMetadata(DefaultPictureSize));

        #endregion PictureSize

        #endregion Properties

        #region Implementation

        protected override async Task LoadData()
        {
            if (!string.IsNullOrEmpty(this.AccessToken))
            {
                FacebookClient facebookClient = new FacebookClient(this.AccessToken);

                string graphUrl = string.Format(
                                        CultureInfo.InvariantCulture,
                                        "/{0}/friends?fields={1}",
                                        this.ProfileId,
                                        this.DisplayFields);
                dynamic friendsTaskResult = await facebookClient.GetTaskAsync(graphUrl);
                var result = (IDictionary<string, object>)friendsTaskResult;
                var data = (IEnumerable<object>)result["data"];

                foreach (dynamic friend in data)
                {
                    var user = new GraphUser(friend);
                    if (this.OnDataItemRetrieved(new DataItemRetrievedEventArgs<GraphUser>(user), e => e.Exclude))
                    {
                        this.Items.Add(user);
                    }
                }
            }
        }

        protected override IList GetData(IEnumerable<GraphUser> items)
        {
            return AlphaKeyGroup<PickerItem<GraphUser>>.CreateGroups(
                            items.Select(item => new FriendPickerItem((FriendPicker)this, item)),
                            System.Globalization.CultureInfo.CurrentUICulture,
                            u => u.Item.Name,
                            true);
        }

        protected override IEnumerable<GraphUser> GetDesignTimeData()
        {
            return FriendPickerDesignSupport.DesignData;
        }

        internal static string FormatDisplayName(GraphUser user, FriendPickerDisplayOrder displayOrder)
        {
            bool hasFirstName = !string.IsNullOrWhiteSpace(user.FirstName);
            bool hasLastName = !string.IsNullOrWhiteSpace(user.LastName);
            bool hasFirstNameAndLastName = hasFirstName && hasLastName;

            if (hasFirstName || hasLastName)
            {
                switch (displayOrder)
                {
                    case FriendPickerDisplayOrder.DisplayFirstNameFirst:
                        return user.FirstName + (hasFirstNameAndLastName ? " " : null) + user.LastName;
                    case FriendPickerDisplayOrder.DisplayLastNameFirst:
                        return user.LastName + (hasFirstNameAndLastName ? ", " : null) + user.FirstName;
                }
            }

            return user.Name;
        }

        #endregion Implementation
    }
}
