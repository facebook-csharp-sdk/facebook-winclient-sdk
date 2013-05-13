namespace Facebook.Client.Controls
{
    using System.ComponentModel;

    internal class FriendPickerItem : INotifyPropertyChanged
    {
        private FriendPicker parent;

        internal FriendPickerItem(FriendPicker parent)
        {
            this.parent = parent;
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

        #endregion

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

        #endregion

        #region Item

        private GraphUser item = null;

        public GraphUser Item
        {
            get
            {
                return this.item;
            }

            set
            {
                if (this.item == null || value.Id != this.item.Id)
                {
                    this.item = value;
                    this.NotifyPropertyChanged("Item");
                }
            }
        }

        #endregion

        #endregion Properties

        #region implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    #endregion
}
