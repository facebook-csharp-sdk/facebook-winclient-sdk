using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BasicAppAuthentication
{
    public partial class Dialog : PhoneApplicationPage
    {
        public Dialog()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var session = SessionStorage.Load();
            Uri dialogUri = new Uri(String.Format("https://m.facebook.com/v2.1/dialog/pay?access_token={0}&redirect_uri=fbconnect%3A%2F%2Fsuccess&app_id={1}&message=YOUR_MESSAGE_HERE&display=touch", session.AccessToken, MainPage.AppId));
            //Uri dialogUri = new Uri(String.Format("https://m.facebook.com/dialog/apprequests?&access_token=CAAHrnrcZA3MoBAKuBxKqFyOANlecvU0Myc4j8HXZCyVtE3cexnITp7wZCpGk9Lyufyq0uyfThIrC0L245s4M63oFASld9MbnReKddLo8gWrjqbpVzbvdB6zG5W7sC4pkSZBhelNGOUoQjQkdb5pjwj6eygfk7XkMv7mAivCCgqR0bIx8t8rpC1wJIDJXP1xyNjUmOYf7XT60wifHnndZB&redirect_uri=https%3A%2F%2Fpratapgarh.com&app_id=540541885996234&message=YOUR_MESSAGE_HERE&display=touch", session.AccessToken));
            this.MyWebBrowser.Navigate(dialogUri);
        }

        private void MyWebBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
        }

        private void tokenExtendButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}