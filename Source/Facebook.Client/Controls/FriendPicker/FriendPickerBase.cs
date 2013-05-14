namespace Facebook.Client.Controls
{
#if NETFX_CORE
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
#endif
#if WINDOWS_PHONE
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
#endif

    /// <summary>
    /// Base class for showing a user interface that can be used to select Facebook friends.
    /// </summary>
    public abstract class FriendPickerBase : Control
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
        /// Initializes a new instance of the FriendPickerBase class.
        /// </summary>
        public FriendPickerBase()
        {
            this.SetValue(ItemsProperty, new ObservableCollection<GraphUser>());
            this.SetValue(SelectedItemsProperty, new ObservableCollection<GraphUser>());
        }

        #region Events

        /// <summary>
        /// Occurs whenever a new friend is about to be added to the list.
        /// </summary>
        public event EventHandler<DataItemRetrievedEventArgs<GraphUser>> DataItemRetrieved;

        /// <summary>
        /// Occurs when the list of friends has finished loading.
        /// </summary>
        public event EventHandler<DataReadyEventArgs<GraphUser>> LoadCompleted;

        /// <summary>
        /// Occurs whenever an error occurs while loading data.
        /// </summary>
        public event EventHandler<LoadFailedEventArgs> LoadFailed;

        /// <summary>
        /// Occurs when the current selection changes.
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        /// <summary>
        /// Occurs when the display order changes.
        /// </summary>
        internal event EventHandler<DependencyPropertyChangedEventArgs> DisplayOrderChanged;

        #endregion Events

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
            DependencyProperty.Register("AccessToken", typeof(string), typeof(FriendPickerBase), new PropertyMetadata(FriendPickerBase.DefaultAccessToken, OnAccessTokenPropertyChanged));

        private static async void OnAccessTokenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var friendPicker = (FriendPickerBase)d;
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
            DependencyProperty.Register("ProfileId", typeof(string), typeof(FriendPickerBase), new PropertyMetadata(FriendPickerBase.DefaultProfileId, OnProfileIdPropertyChanged));

        private static async void OnProfileIdPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var friendPicker = (FriendPickerBase)d;
            await friendPicker.RefreshData();
        }

        #endregion ProfileId

        #region Items

        /// <summary>
        /// Gets the list of currently selected items for the FriendPicker control.
        /// </summary>
        public ObservableCollection<GraphUser> Items
        {
            get { return (ObservableCollection<GraphUser>)this.GetValue(ItemsProperty); }
            private set { this.SetValue(ItemsProperty, value); }
        }

        /// <summary>
        /// Identifies the Items dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ObservableCollection<GraphUser>), typeof(FriendPickerBase), null);

        #endregion Items

        #region SelectedItems

        /// <summary>
        /// Gets the list of currently selected items for the FriendPicker control.
        /// </summary>
        public ObservableCollection<GraphUser> SelectedItems
        {
            get { return (ObservableCollection<GraphUser>)this.GetValue(SelectedItemsProperty); }
            private set { this.SetValue(SelectedItemsProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedItems dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(ObservableCollection<GraphUser>), typeof(FriendPickerBase), null);

        #endregion SelectedItems

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
            DependencyProperty.Register("DisplayOrder", typeof(FriendPickerDisplayOrder), typeof(FriendPickerBase), new PropertyMetadata(DefaultDisplayOrder, OnDisplayOrderPropertyChanged));

        private static void OnDisplayOrderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var friendPicker = (FriendPickerBase)d;
            if (friendPicker.DisplayOrderChanged != null)
            {
                friendPicker.DisplayOrderChanged(friendPicker, e);
            }
        }

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
            DependencyProperty.Register("SortOrder", typeof(FriendPickerSortOrder), typeof(FriendPickerBase), new PropertyMetadata(DefaultSortOrder));

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
            DependencyProperty.Register("DisplayFields", typeof(string), typeof(FriendPickerBase), new PropertyMetadata(FriendPickerBase.DefaultDisplayFields));

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
            DependencyProperty.Register("DisplayProfilePictures", typeof(bool), typeof(FriendPickerBase), new PropertyMetadata(DefaultDisplayProfilePictures));

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
            DependencyProperty.Register("PictureSize", typeof(Size), typeof(FriendPickerBase), new PropertyMetadata(DefaultPictureSize));

        #endregion PictureSize

        #endregion Properties

        #region Implementation

        private async Task RefreshData()
        {
            this.Items.Clear();
            this.SelectedItems.Clear();
            this.SetDataSource(this.Items);

            if (string.IsNullOrEmpty(this.AccessToken))
            {                
                return;
            }

            try
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
                    if (this.DataItemRetrieved.RaiseEvent(this, new DataItemRetrievedEventArgs<GraphUser>(user), e => e.Exclude))
                    {
                        this.Items.Add(user);
                    }
                }

                this.SetDataSource(this.Items);
                this.LoadCompleted.RaiseEvent(this, new DataReadyEventArgs<GraphUser>(this.Items.ToList()));
            }
            catch (Exception ex)
            {
                // TODO: review the types of exception that can be caught here
                this.LoadFailed.RaiseEvent(this, new LoadFailedEventArgs("Error loading friend data.", ex.Message));
            }
        }

        protected abstract void SetDataSource(IEnumerable<GraphUser> friends);

        protected virtual void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SelectionChanged.RaiseEvent(this, e);
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