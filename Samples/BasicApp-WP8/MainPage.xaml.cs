using Facebook.Client;

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

            this.Loaded += MainPage_Loaded;
        }

        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //WebViewAppLinkResolver resolver = new WebViewAppLinkResolver();
            //resolver.AppLinkObtainedEvent += AppLinkObtainedEvent;
            //resolver.GetAppLinkFromUrlInBackground(new Uri("http://pratapgarh.com/fbtest/al.html"));

            var appLink = await (new FacebookAppLinkResolver()).GetAppLinkAsync("CAACEdEose0cBAHFYX1jgGzeBZBLSNNO6ZBkUsFJrtnvAOC5k5r0FBfG6uqKztVgydqQWSSCYK8cGx8qpInuAZBfDhh2fjh0rGPWNtMVSZBEgn02p5n0MIgN4nvZAUXG1ehdyC955OVLeYvb3XbzDmZBT2ZBRXUnaHWtsHvEpqnodlMNocU9efy2oLLaUndH7dEZD", "http://pratapgarh.com/fbtest/al.html");

            AppLinkNavigation.Navigate(appLink);
        }

        private void AppLinkObtainedEvent(AppLink appLink)
        {
            // do something with applink
            int x = 10;
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