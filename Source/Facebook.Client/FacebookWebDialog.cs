//-----------------------------------------------------------------------
// <copyright file="FacebookWebDialog.cs" company="The Outercurve Foundation">
//    Copyright (c) 2011, The Outercurve Foundation. 
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//      http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>
// <author>Nathan Totten (ntotten.com) and Prabir Shrestha (prabir.me)</author>
// <website>https://github.com/facebook-csharp-sdk/facbook-winclient-sdk</website>
//-----------------------------------------------------------------------

#if WINDOWS_PHONE
using Microsoft.Phone.Controls;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Facebook.Client
{
    public class FacebookWebDialog : FacebookDialog
    {
        private static string responseData = "";
        private static uint responseErrorDetail = 0;
        private static FacebookWebDialogResponseStatus responseStatus = FacebookWebDialogResponseStatus.UserCancel;
        private static AutoResetEvent dialogFinishedEvent = new AutoResetEvent(false);

        internal static bool DialogInProgress { get; private set; }
        internal static Uri EndUri = new Uri("https://www.facebook.com/connect/login_success.html");

        public static Task<FacebookDialogResult> ShowDialogAsync(string dialog, object parameters)
        {
            if (string.IsNullOrEmpty(dialog))
                throw new ArgumentNullException("dialog");
            if (dialog.Equals("oauth", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("OAuth is not supported through this method. Use FacebookSessionClient instead.");
            if (parameters == null)
                throw new ArgumentNullException("parameters");
            
            PhoneApplicationFrame rootFrame = Application.Current.RootVisual as PhoneApplicationFrame;

            if (rootFrame == null)
            {
                throw new InvalidOperationException();
            }


            FacebookWebDialog.DialogInProgress = true;

            var fb = new FacebookClient();
            var dialogUrl = fb.GetDialogUrl(dialog, parameters);
            var encoded = HttpUtility.UrlEncode(dialogUrl.AbsoluteUri);
            
            // Navigate to the login page.
            rootFrame.Navigate(new Uri("/Facebook.Client;component/FacebookDialogPage.xaml?url=" + encoded, UriKind.Relative));

            Task<FacebookDialogResult> task = Task<FacebookDialogResult>.Factory.StartNew(() =>
            {
                dialogFinishedEvent.WaitOne();


                object data = null;

                if (!string.IsNullOrEmpty(responseData))
                {
                    data = fb.ParseDialogCallbackUrl(new Uri(responseData));
                }

                return new FacebookDialogResult(data, responseStatus, responseErrorDetail);
            });

            return task;
        }

        internal static void OnDialogFinished(string data, FacebookWebDialogResponseStatus status, uint error)
        {
            FacebookWebDialog.responseData = data;
            FacebookWebDialog.responseStatus = status;
            FacebookWebDialog.responseErrorDetail = error;

            FacebookWebDialog.DialogInProgress = false;

            // Signal the waiting task that the authentication operation has finished.
            dialogFinishedEvent.Set();
        }

    }
}
