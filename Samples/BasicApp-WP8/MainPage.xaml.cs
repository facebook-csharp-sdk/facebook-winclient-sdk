namespace PhoneApp2
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Facebook.Client.Controls;
    using Microsoft.Phone.Controls;

    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            this.InitializeComponent();            
        }

        private void OnSessionStateChanged(object sender, Facebook.Client.Controls.SessionStateChangedEventArgs e)
        {
            this.welcomeText.Opacity = (e.SessionState == FacebookSessionState.Opened) ? 0 : 100;
            this.contentPanel.Opacity = (e.SessionState == FacebookSessionState.Opened) ? 100 : 0;
        }

        private void OnDisplayOrderSelected(object sender, RoutedEventArgs e)
        {
            if (this.friendPicker != null)
            {
                var choice = sender as RadioButton;
                this.friendPicker.DisplayOrder =
                    (Facebook.Client.Controls.FriendPickerDisplayOrder)Enum.Parse(typeof(Facebook.Client.Controls.FriendPickerDisplayOrder), choice.Name);
            }
        }

        private void OnPlacePickerLoadFailed(object sender, Facebook.Client.Controls.LoadFailedEventArgs e)
        {
            MessageBox.Show(e.Description, e.Reason, MessageBoxButton.OK);
        }
    }
}