// BUG!!!!!!
// If you are zoomed out in a semantic zoom control, and the last group is empty, if you tap it, 
// you get a catastrophic failure:
// See http://social.msdn.microsoft.com/Forums/en-US/winappswithcsharp/thread/6535656e-3293-4e0d-93b5-453864b95601
// See http://stackoverflow.com/questions/9952574/semantic-zoom-control-throwing-exception-when-groups-are-empty?rq=1
// See http://social.msdn.microsoft.com/Forums/en-US/winappswithcsharp/thread/6535656e-3293-4e0d-93b5-453864b95601
// See http://stackoverflow.com/questions/14423536/semantic-zoom-catastrophic-failure-on-empty-group
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
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Input;
#endif
    /// <summary>
    /// Shows a user interface that can be used to select Facebook friends.
    /// </summary>
    [TemplatePart(Name = PartSemanticZoom, Type = typeof(SemanticZoom))]
    public class FriendPicker : Control
    {
        #region Part Definitions

        private const string PartSemanticZoom = "PART_SemanticZoom";

        #endregion Part Definitions

        #region Default Property Values

        private const string DefaultAccessToken = "";
        private const string DefaultProfileId = "me";
        private const string DefaultDisplayFields = "id,name,first_name,middle_name,last_name,picture";
        private const string DefaultDisplayOrdering = "";
        private const string DefaultSortOrder = "";
        private const ListViewSelectionMode DefaultSelectionMode = ListViewSelectionMode.Multiple;
        private const bool DefaultDisplayProfilePictures = true;
        private static readonly Size DefaultPictureSize = new Size(50, 50);

        #endregion Default Property Values

        #region Member variables

        private bool isZoomedOut = false;
        private SemanticZoom semanticZoom;

        #endregion Member variables

        /// <summary>
        /// Initializes a new instance of the FriendPicker class.
        /// </summary>
        public FriendPicker()
        {
            this.DefaultStyleKey = typeof(FriendPicker);
            this.SetValue(ItemsProperty, new ObservableCollection<GraphUser>());
            this.SetValue(SelectedItemsProperty, new ObservableCollection<GraphUser>()); 
        }

        #region Events

        /// <summary>
        /// Occurs whenever a new friend is about to be added to the list.
        /// </summary>
        public event EventHandler<FriendRetrievedEventArgs> FriendRetrieved;

        /// <summary>
        /// Occurs when the list of friends has finished loading.
        /// </summary>
        public event EventHandler<DataReadyEventArgs> LoadCompleted;

        /// <summary>
        /// Occurs whenever an error occurs while loading data.
        /// </summary>
        public event EventHandler<LoadFailedEventArgs> LoadFailed;

        /// <summary>
        /// Occurs when the current selection changes.
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

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
            DependencyProperty.Register("AccessToken", typeof(string), typeof(FriendPicker), new PropertyMetadata(FriendPicker.DefaultAccessToken, OnAccessTokenPropertyChanged));

        private async static void OnAccessTokenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
            DependencyProperty.Register("ProfileId", typeof(string), typeof(FriendPicker), new PropertyMetadata(FriendPicker.DefaultProfileId, OnProfileIdPropertyChanged));

        private async static void OnProfileIdPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var friendPicker = (FriendPicker)d;
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
            DependencyProperty.Register("Items", typeof(ObservableCollection<GraphUser>), typeof(FriendPicker), null);

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
            DependencyProperty.Register("SelectedItems", typeof(ObservableCollection<GraphUser>), typeof(FriendPicker), null);

        #endregion SelectedItems

        #region DisplayOrdering

        // TODO: this is not currently functional. The property is not a string either.

        /// <summary>
        /// Controls whether to display first name or last name first.
        /// </summary>
        public string DisplayOrdering
        {
            get { return (string)GetValue(DisplayOrderingProperty); }
            set { SetValue(DisplayOrderingProperty, value); }
        }

        /// <summary>
        /// Identifies the DisplayOrdering dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayOrderingProperty =
            DependencyProperty.Register("DisplayOrdering", typeof(string), typeof(FriendPicker), new PropertyMetadata(DefaultDisplayOrdering));
        
        #endregion DisplayOrdering

        #region SortOrder

        // TODO: this is not currently functional. The property is not a string either.

        /// <summary>
        /// Controls the order in which friends are listed in the friend picker.
        /// </summary>
        public string SortOrder
        {
            get { return (string)GetValue(SortOrderProperty); }
            set { SetValue(SortOrderProperty, value); }
        }

        /// <summary>
        /// Identifies the SortOrder dependency property.
        /// </summary>
        public static readonly DependencyProperty SortOrderProperty =
            DependencyProperty.Register("SortOrder", typeof(string), typeof(FriendPicker), new PropertyMetadata(DefaultSortOrder));

        #endregion SortOrder

        #region SelectionMode

        /// <summary>
        /// Gets or sets the selection behavior of the control. 
        /// </summary>
        public ListViewSelectionMode SelectionMode
        {
            get { return (ListViewSelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectionMode dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register("SelectionMode", typeof(ListViewSelectionMode), typeof(FriendPicker), new PropertyMetadata(DefaultSelectionMode));

        #endregion SelectionMode

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
            set { SetValue(DisplayFieldsProperty, value); }
        }

        /// <summary>
        /// Identifies the DisplayFields dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayFieldsProperty =
            DependencyProperty.Register("DisplayFields", typeof(string), typeof(FriendPicker), new PropertyMetadata(FriendPicker.DefaultDisplayFields));

        #endregion DisplayFields

        #region DisplayProfilePictures

        /// <summary>
        /// Specifies whether profile pictures are displayed.
        /// </summary>
        public bool DisplayProfilePictures
        {
            get { return (bool)GetValue(DisplayProfilePicturesProperty); }
            set { SetValue(DisplayProfilePicturesProperty, value); }
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
            set { SetValue(PictureSizeProperty, value); }
        }

        /// <summary>
        /// Identifies the PictureSize dependency property.
        /// </summary>
        public static readonly DependencyProperty PictureSizeProperty =
            DependencyProperty.Register("PictureSize", typeof(Size), typeof(FriendPicker), new PropertyMetadata(DefaultPictureSize));
        
        #endregion PictureSize

        #endregion Properties

        #region Implementation

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) call ApplyTemplate. In simplest 
        /// terms, this means the method is called just before a UI element displays in your app. Override this method to influence the 
        /// default post-template logic of a class. 
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.semanticZoom != null)
            {
                var view = this.semanticZoom.ZoomedInView as ListViewBase;
                view.SelectionChanged -= this.OnSelectionChanged;
                this.semanticZoom.ViewChangeCompleted -= this.OnSemanticZoomViewChangeCompleted;
                this.semanticZoom.Tapped -= this.OnSemanticZoomTapped;
                (this.semanticZoom.ZoomedOutView as Control).Tapped -= OnSemanticZoomOutViewTapped;
            }

            this.semanticZoom = this.GetTemplateChild(FriendPicker.PartSemanticZoom) as SemanticZoom;
            if (this.semanticZoom != null)
            {
                var view = this.semanticZoom.ZoomedInView as ListViewBase;
                view.SelectionChanged += this.OnSelectionChanged;
                this.semanticZoom.IsZoomOutButtonEnabled = false;
                this.semanticZoom.ViewChangeCompleted += this.OnSemanticZoomViewChangeCompleted;
                this.semanticZoom.Tapped += this.OnSemanticZoomTapped;
                this.semanticZoom.Tag = this;
                (this.semanticZoom.ZoomedOutView as Control).Tapped += OnSemanticZoomOutViewTapped;
            }

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                this.semanticZoom.DataContext = this.GroupData(FriendPickerDesignSupport.DesignData);
            }
        }

        // TODO: this is a hack to prevent switching views whenever an empty group is clicked. 
        // It is not completely effective. For example, it fails if you double-click a group, or click outside a group's area.
        // Must find a better alternative.
        void OnSemanticZoomOutViewTapped(object sender, TappedRoutedEventArgs e)
        {
            var group = (e.OriginalSource as FrameworkElement).DataContext as GroupInfoList<GraphUser>;
            if (group != null && !group.Any())
            {
                this.semanticZoom.IsZoomedInViewActive = false;
                e.Handled = true;
            }
        }

        private void OnSemanticZoomTapped(object sender, TappedRoutedEventArgs e)
        {
            if (!this.isZoomedOut && ((e.OriginalSource as FrameworkElement).DataContext is GroupInfoList<GraphUser>))
            {
                this.isZoomedOut = true;
                this.semanticZoom.ToggleActiveView();
                e.Handled = true;
            }
        }

        private void OnSemanticZoomViewChangeCompleted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            if (!e.IsSourceZoomedInView)
            {
                this.isZoomedOut = false;
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.RemovedItems)
            {
                this.SelectedItems.Remove((GraphUser)item);
            }

            foreach (var item in e.AddedItems)
            {
                this.SelectedItems.Add((GraphUser) item);
            }

            this.RaiseSelectionChanged(e);
        }

        private CollectionViewSource GroupData(IEnumerable<GraphUser> friends)
        {
            var groups = from f in friends
                         group f by f.Name.Substring(0, 1) into grp
                         orderby grp.Key
                         select grp;

            // TODO: this assumes 26 letters only, may vary in other cultures
            var alphabet = Enumerable.Range('A', 26).Select(c => ((char)c).ToString());

            return new CollectionViewSource()
            {
                IsSourceGrouped = true,
                Source = from letter in alphabet
                         join grp in groups on letter equals grp.Key into gj
                         from subfriend in gj.DefaultIfEmpty()
                         select new GroupInfoList<GraphUser>(letter, subfriend)
            };
        }

        private async Task RefreshData()
        {
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
                var friends = new List<GraphUser>();
                foreach (dynamic friend in data)
                {
                    var user = new GraphUser(friend);
                    if (RaiseFriendRetrieved(new FriendRetrievedEventArgs(user)))
                    {
                        friends.Add(user);
                    }
                }

                this.semanticZoom.DataContext = this.GroupData(friends);
                // TODO: populate the Items collection here
                // see http://blogs.msdn.com/b/nathannesbit/archive/2009/04/20/addrange-and-observablecollection.aspx
                this.RaiseLoadCompleted(new DataReadyEventArgs((friends)));
            }
            // TODO: review the types of exception that can be caught here
            catch (Exception ex)
            {
                RaiseLoadFailed(new LoadFailedEventArgs("Error loading friend data.", ex.Message));
            }
        }

        private void RaiseSelectionChanged(SelectionChangedEventArgs e)
        {
            EventHandler<SelectionChangedEventArgs> handler = this.SelectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void RaiseLoadCompleted(DataReadyEventArgs e)
        {
            EventHandler<DataReadyEventArgs> handler = this.LoadCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void RaiseLoadFailed(LoadFailedEventArgs e)
        {
            EventHandler<LoadFailedEventArgs> handler = this.LoadFailed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private bool RaiseFriendRetrieved(FriendRetrievedEventArgs e)
        {
            EventHandler<FriendRetrievedEventArgs> handler = this.FriendRetrieved;
            if (handler != null)
            {
                foreach (EventHandler<FriendRetrievedEventArgs> item in handler.GetInvocationList())
                {
                    item(this, e);
                    if (e.Exclude)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion Implementation
    }
}
