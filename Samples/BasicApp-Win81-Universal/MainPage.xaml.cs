using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using Facebook.Client;

namespace BasicApp_Win81_Universal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();


        }


        private void showRequestsDialogButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> friendsList = new List<String>();
            friendsList.Add("9074");
            friendsList.Add("535949260");
            Session.ShowAppRequestsDialog(null, "What's up", null);
        }

        private void showDialogButton_Click(object sender, RoutedEventArgs e)
        {
            Session.ShowFeedDialog();
        }

        private void loginViaWebviewButton_Click(object sender, RoutedEventArgs e)
        {
           Session.ActiveSession.LoginWithBehavior("email,public_profile,user_friends", FacebookLoginBehavior.LoginBehaviorWebViewOnly);
        }
    }
}
