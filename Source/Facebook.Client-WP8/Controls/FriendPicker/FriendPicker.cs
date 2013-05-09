namespace Facebook.Client.Controls
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Phone.Controls;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Shows the.
    /// </summary>
    [TemplatePart(Name = PartLongListSelector, Type = typeof(LongListSelector))]
    public class FriendPicker : Control
    {
        #region Part Definitions

        private const string PartLongListSelector = "PART_LongListSelector";

        #endregion Part Definitions

        #region Member variables

        private LongListSelector longListSelector;

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

        #region Properties

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
            DependencyProperty.Register("AccessToken", typeof(string), typeof(FriendPicker), new PropertyMetadata(string.Empty, OnAccessTokenChanged));

        private static async void OnAccessTokenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var friendPicker = (FriendPicker)d;
            await friendPicker.RetrieveData();
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

        #endregion Properties

        #region Implementation

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) call ApplyTemplate. In simplest 
        /// terms, this means the method is called just before a UI element displays in your app. Override this method to influence the 
        /// default post-template logic of a class. 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.longListSelector = (LongListSelector)GetTemplateChild(FriendPicker.PartLongListSelector);
            if (this.longListSelector != null)
            {
                this.longListSelector.SelectionChanged += this.OnSelectionChanged;

                if (System.ComponentModel.DesignerProperties.IsInDesignTool)
                {
                    this.longListSelector.ItemsSource = AlphaKeyGroup<FriendPickerItem>.CreateGroups(Friend.DesignData.Select(f => new FriendPickerItem { Item = f }), System.Globalization.CultureInfo.CurrentUICulture, (u) => { return u.Item.Name; }, true);
                }
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.longListSelector == null)
            {
                return;
            }

            if (this.longListSelector.SelectedItem == null)
            {
                return;
            }

            var selectedItem = this.longListSelector.SelectedItem as FriendPickerItem;
            selectedItem.IsSelected = !selectedItem.IsSelected;

            if (selectedItem.IsSelected)
            {
                this.SelectedItems.Add(selectedItem.Item);
            }
            else
            {
                this.SelectedItems.Remove(selectedItem.Item);
            }

            /// Reset selected item to null (no selection)
            this.longListSelector.SelectedItem = null;
        }

        private async Task RetrieveData()
        {
            this.Items.Clear();
            this.SelectedItems.Clear();

            if (this.longListSelector == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(this.AccessToken))
            {
                return;
            }

            FacebookClient facebookClient = new FacebookClient(this.AccessToken);

            dynamic friendsTaskResult = await facebookClient.GetTaskAsync("/me/friends");
            var result = (IDictionary<string, object>)friendsTaskResult;
            var data = (IEnumerable<object>)result["data"];
            foreach (var item in data)
            {
                var friend = (IDictionary<string, object>)item;

                this.Items.Add(
                    new Friend
                        {
                            Id = (string)friend["id"],
                            Name = (string)friend["name"],
                            PictureUri = string.Format(
                                    "https://graph.facebook.com/{0}/picture?type={1}&access_token={2}",
                                    (string)friend["id"],
                                    "square",
                                    this.AccessToken)
                        });
            }

            this.longListSelector.ItemsSource = AlphaKeyGroup<FriendPickerItem>.CreateGroups(this.Items.Select(f => new FriendPickerItem { Item = f }), System.Globalization.CultureInfo.CurrentUICulture, (u) => { return u.Item.Name; }, true);
        }

        #endregion Implementation
    }
}