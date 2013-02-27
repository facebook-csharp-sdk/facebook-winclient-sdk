using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NETFX_CORE
using Windows.Security.Authentication.Web;
#endif

namespace Facebook.Apps
{
    public class FacebookAppClient : FacebookClient
    {

        public bool LoginInProgress { get; set; }
        public FacebookUser CurrentUser { get; set; }

        public FacebookAppClient(string appId) : base()
        {
            if (String.IsNullOrEmpty(appId))
            {
                throw new ArgumentNullException("appId");
            }

            this.AppId = appId;
        }

        public async Task<FacebookUser> LoginAsync()
        {
            return await LoginAsync(null);
        }

        public async Task<FacebookUser> LoginAsync(string permissions)
        {
            if (this.LoginInProgress)
            {
                throw new InvalidOperationException("Login in progress.");
            }

            this.LoginInProgress = true;
            try
            {

                // Use WebAuthenticationBroker to launch server side OAuth flow

                Uri startUri = this.GetLoginUrl(permissions);
                Uri endUri = new Uri("https://www.facebook.com/connect/login_success.html");

                var result = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, startUri, endUri);

                if (result.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                {
                    throw new InvalidOperationException();
                }
                else if (result.ResponseStatus == WebAuthenticationStatus.UserCancel)
                {
                    throw new InvalidOperationException();
                }

                var authResult = this.ParseOAuthCallbackUrl(new Uri(result.ResponseData));

                this.AccessToken = authResult.AccessToken;
                this.CurrentUser = new FacebookUser
                {
                    AccessToken = authResult.AccessToken
                };
            }
            finally
            {
                this.LoginInProgress = false;
            }

            return this.CurrentUser;
        }


        /// <summary>
        /// Log a user out of Facebook.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Logout", Justification = "Logout is preferred by design")]
        public void Logout()
        {
            this.AccessToken = null;
        }

        private Uri GetLoginUrl(string permissions)
        {
            var parameters = new Dictionary<string, object>();
            parameters["client_id"] = this.AppId;
            parameters["redirect_uri"] = "https://www.facebook.com/connect/login_success.html";
            parameters["response_type"] = "token";
#if WINDOWS_PHONE
            parameters["display"] = "touch";
#else
            parameters["display"] = "popup";
#endif


            // add the 'scope' only if we have extendedPermissions.
            if (!string.IsNullOrEmpty(permissions))
            {
                // A comma-delimited list of permissions
                parameters["scope"] = permissions;
            }

            return this.GetLoginUrl(parameters);
        }

    }
}
