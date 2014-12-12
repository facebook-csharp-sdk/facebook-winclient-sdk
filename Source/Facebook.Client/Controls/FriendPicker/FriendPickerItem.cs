namespace Facebook.Client.Controls
{
#if NETFX_CORE
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;
#endif
#if WP8
    using System.Windows;
    using System.Windows.Data;
#endif

    internal class FriendPickerItem : PickerItem<GraphUser>
    {
        internal FriendPickerItem(FriendPicker parent, GraphUser item)
            : base(parent, item)
        {
            this.SetValue(DisplayOrderProperty, parent.DisplayOrder);

            var binding = new Binding
            {
                // TODO: change property path to a string constant?
                Path = new PropertyPath("DisplayOrder"),
                Source = parent,
                Mode = BindingMode.OneWay
            };

            BindingOperations.SetBinding(this, DisplayOrderProperty, binding);
        }

        #region Properties

        #region DisplayOrder

        public static readonly DependencyProperty DisplayOrderProperty =
            DependencyProperty.Register("DisplayOrder", typeof(FriendPickerDisplayOrder), typeof(FriendPickerItem), new PropertyMetadata(FriendPickerDisplayOrder.DisplayFirstNameFirst, OnDisplayOrderPropertyChanged));

        private static void OnDisplayOrderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pickerItem = (FriendPickerItem)d;
            pickerItem.NotifyPropertyChanged("DisplayName");
        }

        #endregion DisplayOrder

        #region DisplayName

        public string DisplayName
        {
            get
            {
                return FriendPicker.FormatDisplayName(
                    this.Item, 
                    (FriendPickerDisplayOrder)this.GetValue(DisplayOrderProperty));
            }
        }

        #endregion DisplayName

        #endregion Properties
    }
}
