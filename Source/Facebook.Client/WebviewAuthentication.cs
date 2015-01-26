// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
#if WP8
using Microsoft.Phone.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using Windows.System;
#endif

#if WINDOWS
using Windows.Security.Authentication.Web;
#endif

#if WINDOWS_UNIVERSAL
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
#endif

using Facebook.Client.Controls.WebDialog;

namespace Facebook.Client
{

#if WP8 || WINDOWS_PHONE
    /// <summary>
    /// This class mimics the functionality provided by WebAuthenticationOptions available in Win8.
    /// </summary>
    internal enum WebAuthenticationOptions
    {
        None,
        SilentMode
    }

    /// <summary>
    /// This class mimics the functionality provided by WebAuthenticationStatus available in Win8.
    /// </summary>
    internal enum WebAuthenticationStatus
    {
        Success = 0,

        UserCancel = 1,

        ErrorHttp = 2
    }

    /// <summary>
    /// This class mimics the functionality provided by WebAuthenticationResult available in Win8.
    /// </summary>
    internal sealed class WebAuthenticationResult
    {
        public string ResponseData { get; private set; }

        public WebAuthenticationStatus ResponseStatus { get; private set; }

        public uint ResponseErrorDetail { get; private set; }

        public WebAuthenticationResult(string data, WebAuthenticationStatus status, uint error)
        {
            ResponseData = data;
            ResponseStatus = status;
            ResponseErrorDetail = error;
        }
    }
#endif

    /// <summary>
    /// This class mimics the functionality provided by WebviewAuthentication available in Win8.
    /// </summary>
    internal sealed class WebviewAuthentication
    {
        private static string responseData = "";
        private static uint responseErrorDetail = 0;
        private static WebAuthenticationStatus responseStatus = WebAuthenticationStatus.UserCancel;
        private static AutoResetEvent authenticateFinishedEvent = new AutoResetEvent(false);

        static public bool AuthenticationInProgress { get; private set; }
        static public Uri StartUri { get; private set; }
        static public Uri EndUri { get; private set; }

        /// <summary>
        /// Mimics the WebviewAuthentication's AuthenticateAsync method.
        /// </summary>
        public static void AuthenticateAsync(WebAuthenticationOptions options, Uri startUri, Uri endUri)
        {
            if (options != WebAuthenticationOptions.None)
            {
                throw new NotImplementedException("This method does not support authentication options other than 'None'.");
            }
            Popup dialogPopup = new Popup();

            var webDialog = new WebDialogUserControl();

            webDialog.ParentControlPopup = dialogPopup;
            dialogPopup.Child = webDialog;

#if WP8 || WINDOWS_PHONE
            // Set where the popup will show up on the screen.
            dialogPopup.VerticalOffset = 40;
            dialogPopup.HorizontalOffset = 0;
#endif

#if WP8
            dialogPopup.Height = Application.Current.Host.Content.ActualHeight - 40;
            dialogPopup.Width = Application.Current.Host.Content.ActualWidth;
#endif

#if WINDOWS_PHONE
            dialogPopup.Height = Window.Current.Bounds.Height - 40;
            dialogPopup.Width = Window.Current.Bounds.Width;
#endif

#if WINDOWS
            dialogPopup.Height = Window.Current.Bounds.Height;
            dialogPopup.Width = Window.Current.Bounds.Width;
#endif


            webDialog.Height = dialogPopup.Height;
            webDialog.Width = dialogPopup.Width;

            webDialog.LoginViaWebview(startUri);

            // Open the popup.
            dialogPopup.IsOpen = true;
        }

        public static void OnAuthenticationFinished(string data, WebAuthenticationStatus status, uint error)
        {
            WebviewAuthentication.responseData = data;
            WebviewAuthentication.responseStatus = status;
            WebviewAuthentication.responseErrorDetail = error;

            WebviewAuthentication.AuthenticationInProgress = false;

            // Signal the waiting task that the authentication operation has finished.
            authenticateFinishedEvent.Set();
        }
    }
}
