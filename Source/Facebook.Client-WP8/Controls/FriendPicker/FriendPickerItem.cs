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

        public FriendPicker Parent
        {
            get
            {
                return parent;
            }
        }

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

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
