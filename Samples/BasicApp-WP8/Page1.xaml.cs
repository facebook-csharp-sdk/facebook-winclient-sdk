using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Facebook.Client;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace PhoneApp2
{
    public partial class Page1 : PhoneApplicationPage
    {
        public Page1()
        {
            InitializeComponent();
            
        }

        private void testButton_Click(object sender, RoutedEventArgs e)
        {
            var x = Session.ActiveSession.CurrentAccessTokenData;
        }
    }
}