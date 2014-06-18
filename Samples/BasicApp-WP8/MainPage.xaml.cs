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

            var appLink = await AppLinkNavigation.DefaultResolver.GetAppLinkAsync("CAACEdEose0cBAIGx6bM7QtKqnpjapVkCXFVbAsIgCoZAQ7ENWi2RJrZCLmbH8yvXKoQ3pLtjHG9mlgiRkot75Mi9RU3h5o7vGVsFbdUbqRxL8kkXvJZAxonALl1IXITDJyWZAkCY0epbah7Agdb5LPTT81Re5bga2X9oZByqAMutELpflNZB8Nt0oVjYHv3T2xBy80UZCTSYgZDZD", "http://pratapgarh.com/fbtest/al.html");

            AppLinkNavigation.NavigateAsync(appLink);
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