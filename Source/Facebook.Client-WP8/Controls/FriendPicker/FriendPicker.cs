namespace Facebook.Client.Controls
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Phone.Controls;

    /// <summary>
    /// Shows the.
    /// </summary>
    [TemplatePart(Name = PartLongListSelector, Type = typeof(LongListSelector))]
    public class FriendPicker : FriendPickerBase
    {
        #region Part Definitions

        private const string PartLongListSelector = "PART_LongListSelector";

        #endregion Part Definitions

        #region Default Property Values

        private const FriendPickerSelectionMode DefaultSelectionMode = FriendPickerSelectionMode.Multiple;

        #endregion Default Property Values

        #region Member variables

        private LongListSelector longListSelector;

        #endregion Member variables

        #region Properties

        #region SelectionMode

        /// <summary>
        /// Gets or sets the selection behavior of the control. 
        /// </summary>
        public FriendPickerSelectionMode SelectionMode
        {
            get { return (FriendPickerSelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectionMode dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register("SelectionMode", typeof(FriendPickerSelectionMode), typeof(FriendPicker), new PropertyMetadata(DefaultSelectionMode, OnSelectionModeProperyChanged));

        private async static void OnSelectionModeProperyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var friendPicker = (FriendPicker)d;
            friendPicker.ClearSelection();          
        }

        #endregion SelectionMode

        #endregion Properties

        /// <summary>
        /// Initializes a new instance of the FriendPicker class.
        /// </summary>
        public FriendPicker()
            :base()
        {
            this.DefaultStyleKey = typeof(FriendPicker);
        }

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
                    this.SetDataSource(FriendPickerDesignSupport.DesignData);
                }
            }
        }

        protected override void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectionMode == FriendPickerSelectionMode.None)
            {
                return;
            }

            if (this.longListSelector == null)
            {
                return;
            }

            if (this.longListSelector.SelectedItem == null)
            {
                return;
            }

            SelectionChangedEventArgs selectionChangedEventArgs;

            var selectedItem = this.longListSelector.SelectedItem as FriendPickerItem;

            if (this.SelectionMode == FriendPickerSelectionMode.Single)
            {       
                var unselectedItem = e.RemovedItems[0] as FriendPickerItem;

                selectedItem.IsSelected = true;
                if (unselectedItem != null)
                {
                    unselectedItem.IsSelected = false;
                    this.SelectedItems.Remove(unselectedItem.Item);
                    selectionChangedEventArgs = new SelectionChangedEventArgs(new object[1] { unselectedItem.Item }, new object[1] { selectedItem.Item });
                }
                else
                {
                    selectionChangedEventArgs = new SelectionChangedEventArgs(new object[0], new object[1] { selectedItem.Item });
                }
                
                this.SelectedItems.Add(selectedItem.Item);
            }
            else
            {
                selectedItem.IsSelected = !selectedItem.IsSelected;

                if (selectedItem.IsSelected)
                {
                    this.SelectedItems.Add(selectedItem.Item);
                    selectionChangedEventArgs = new SelectionChangedEventArgs(new object[0], new object[1] { selectedItem.Item });
                }
                else
                {
                    this.SelectedItems.Remove(selectedItem.Item);
                    selectionChangedEventArgs = new SelectionChangedEventArgs(new object[1] { selectedItem.Item }, new object[0]);
                }

                /// Reset selected item to null (no selection)
                this.longListSelector.SelectedItem = null;
            }

            base.OnSelectionChanged(sender, selectionChangedEventArgs);
        }

        protected override void SetDataSource(IEnumerable<GraphUser> friends)
        {
            if (this.longListSelector != null)
            {
                this.longListSelector.ItemsSource = AlphaKeyGroup<FriendPickerItem>.CreateGroups(friends.Select(f => new FriendPickerItem(this) { Item = f }), System.Globalization.CultureInfo.CurrentUICulture, (u) => { return u.Item.Name; }, true);
                this.longListSelector.SelectedItem = null;
            }
        }

        private void ClearSelection()
        {
            this.SelectedItems.Clear();

            if (this.longListSelector != null && this.longListSelector.ItemsSource != null)
            {
                (this.longListSelector.ItemsSource as IEnumerable<AlphaKeyGroup<FriendPickerItem>>)
                    .SelectMany(i => i)
                    .Where(f => f.IsSelected)
                    .ToList()
                    .ForEach(i => i.IsSelected = false);

                this.longListSelector.SelectedItem = null;
            }
        }

        #endregion Implementation
    }
}