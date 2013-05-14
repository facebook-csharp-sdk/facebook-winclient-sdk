namespace Facebook.Client.Controls
{
    using System.ComponentModel;
using System.Windows.Data;

    internal class FriendPickerItem : INotifyPropertyChanged
    {
        private FriendPicker parent;

        internal FriendPickerItem(FriendPicker parent, GraphUser item)
        {
            this.parent = parent;
            this.item = item;

            this.SetDisplayName();

            //// TODO: Review if there is a better approach to listen for changes in DisplayOrder dependency property
            parent.DisplayOrderChanged += this.OnDisplayOrderChanged;
        }

        #region Properties

        #region Parent

        public FriendPicker Parent
        {
            get
            {
                return this.parent;
            }
        }

        #endregion Parent

        #region Item

        private GraphUser item = null;

        public GraphUser Item
        {
            get
            {
                return this.item;
            }
        }

        #endregion Item

        #region IsSelected

        private bool isSelected = false;

        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                if (value != this.isSelected)
                {
                    this.isSelected = value;
                    this.NotifyPropertyChanged("IsSelected");
                }
            }
        }

        #endregion IsSelected

        #region DisplayName

        private string displayName = string.Empty;

        public string DisplayName
        {
            get
            {
                return this.displayName;
            }

            private set
            {
                if (!this.displayName.Equals(value))
                {
                    this.displayName = value;
                    this.NotifyPropertyChanged("DisplayName");
                }
            }
        }

        #endregion DisplayName

        #endregion Properties

        #region Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnDisplayOrderChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            this.SetDisplayName();
        }

        private void SetDisplayName()
        {
            this.DisplayName = FriendPickerBase.FormatDisplayName(this.Item, this.parent.DisplayOrder);
        }
    }

    #endregion Implementation
}
