namespace Facebook.Client.Controls
{
#if NETFX_CORE
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;
#endif
    /// <summary>
    /// Shows the .
    /// </summary>
    [TemplatePart(Name = PartListView, Type = typeof(ListView))]
    public class FriendPicker : Control
    {
        #region Part Definitions

        private const string PartListView = "PART_ListView";

        #endregion Part Definitions

        #region Default Property Values

        private const string DefaultAccessToken = "";
        private const string DefaultDisplayOrdering = "";

        #endregion Default Property Values

        #region Member variables

        private ListView listView;

        #endregion Member variables

        /// <summary>
        /// Initializes a new instance of the FriendPicker class.
        /// </summary>
        public FriendPicker()
        {
            this.DefaultStyleKey = typeof(FriendPicker);
            this.SetValue(ItemsProperty, new ObservableCollection<Friend>()); 
            this.SetValue(SelectedItemsProperty, new ObservableCollection<Friend>()); 
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
            friendPicker.DataContext = await friendPicker.RetrieveData();
        }

        #endregion AccessToken

        #region Items

        /// <summary>
        /// Gets the list of currently selected items for the FriendPicker control.
        /// </summary>
        public ObservableCollection<Friend> Items
        {
            get { return (ObservableCollection<Friend>)this.GetValue(ItemsProperty); }
            private set { this.SetValue(ItemsProperty, value); }
        }

        /// <summary>
        /// Identifies the Items dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ObservableCollection<Friend>), typeof(FriendPicker), null);

        #endregion Items

        #region SelectedItems
        
        /// <summary>
        /// Gets the list of currently selected items for the FriendPicker control.
        /// </summary>
        public ObservableCollection<Friend> SelectedItems
        {
            get { return (ObservableCollection<Friend>)this.GetValue(SelectedItemsProperty); }
            private set { this.SetValue(SelectedItemsProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedItems dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(ObservableCollection<Friend>), typeof(FriendPicker), null);

        #endregion SelectedItems

        #region DisplayOrdering

        public string DisplayOrdering
        {
            get { return (string)GetValue(DisplayOrderingProperty); }
            set { SetValue(DisplayOrderingProperty, value); }
        }

        public static readonly DependencyProperty DisplayOrderingProperty =
            DependencyProperty.Register("DisplayOrdering", typeof(string), typeof(FriendPicker), new PropertyMetadata(DefaultDisplayOrdering));
        
        #endregion DisplayOrdering

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

            this.listView = this.GetTemplateChild(FriendPicker.PartListView) as ListView;
            if (this.listView != null)
            {
                this.listView.SelectionChanged += this.OnSelectionChanged;
            }

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                this.DataContext = this.GroupData(Friend.DesignData);
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.RaiseSelectionChanged(e);
        }

        private CollectionViewSource GroupData(IEnumerable<Friend> friends)
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
                         select new GroupInfoList<Friend>(letter, subfriend)
            };
        }

        private async Task<CollectionViewSource> RetrieveData()
        {
            if (string.IsNullOrEmpty(this.AccessToken))
            {
                return null;
            }

            FacebookClient facebookClient = new FacebookClient(this.AccessToken);

            dynamic friendsTaskResult = await facebookClient.GetTaskAsync("/me/friends");
            var result = (IDictionary<string, object>)friendsTaskResult;
            var data = (IEnumerable<object>)result["data"];
            var friends = new List<Friend>();
            foreach (var item in data)
            {
                var friend = (IDictionary<string, object>)item;

                friends.Add(
                    new Friend
                    {
                        Name = (string)friend["name"],
                        Id = (string)friend["id"],
                        PictureUri = string.Format(
                                        CultureInfo.InvariantCulture,
                                        "https://graph.facebook.com/{0}/picture?type={1}&access_token={2}",
                                        (string)friend["id"],
                                        "square",
                                        this.AccessToken)
                    });
            }

            return this.GroupData(friends);
        }

        private void RaiseSelectionChanged(SelectionChangedEventArgs e)
        {
            EventHandler<SelectionChangedEventArgs> handler = this.SelectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion Implementation
    }
}
