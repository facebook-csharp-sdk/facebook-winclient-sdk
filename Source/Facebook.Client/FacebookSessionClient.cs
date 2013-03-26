//-----------------------------------------------------------------------
// <copyright file="FacebookSessionClient.cs" company="The Outercurve Foundation">
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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NETFX_CORE
using Windows.Security.Authentication.Web;
#endif

namespace Facebook.Client
{
    public class FacebookSessionClient
    {
        public string AppId { get; set; }
        public bool LoginInProgress { get; set; }
        public FacebookSession CurrentSession { get; private set; }

        public FacebookSessionClient(string appId)
        {
            if (String.IsNullOrEmpty(appId))
            {
                throw new ArgumentNullException("appId");
            }
            this.AppId = appId;
        }

        public async Task<FacebookSession> LoginAsync()
        {
            return await LoginAsync(null, false);
        }

        public async Task<FacebookSession> LoginAsync(string permissions)
        {
            return await LoginAsync(permissions, false);
        }

        internal async Task<FacebookSession> LoginAsync(string permissions, bool force)
        {
            if (this.LoginInProgress)
            {
                throw new InvalidOperationException("Login in progress.");
            }

            this.LoginInProgress = true;
            try
            {
                var session = FacebookSessionCacheProvider.Current.GetSessionData();
                if (session == null)
                {
                    // Authenticate
                    var authResult = await PromptOAuthDialog(permissions, WebAuthenticationOptions.None);

                    FacebookClient client = new FacebookClient(authResult.AccessToken);
                    var parameters = new Dictionary<string, object>();
                    parameters["fields"] = "id";

                    var result = await client.GetTaskAsync("me", parameters);
                    var dict = (IDictionary<string, object>)result;

                    session = new FacebookSession
                    {
                        AccessToken = authResult.AccessToken,
                        Expires = authResult.Expires,
                        FacebookId = (string)dict["id"],
                    };
                  
                }
                else
                {
                    // Check if we are requesting new permissions
                    bool newPermissions = false;
                    if (!string.IsNullOrEmpty(permissions))
                    {
                        var p = permissions.Split(',');
                        newPermissions = session.CurrentPermissions.Join(p, s1 => s1, s2 => s2, (s1, s2) => s1).Count() != p.Length;
                    }

                    // Prompt OAuth dialog if force renew is true or
                    // if new permissions are requested or 
                    // if the access token is expired.
                    if (force || newPermissions || session.Expires > DateTime.UtcNow)
                    {
                        var authResult = await PromptOAuthDialog(permissions, WebAuthenticationOptions.None);
                        if (authResult != null)
                        {
                            session.AccessToken = authResult.AccessToken;
                            session.Expires = authResult.Expires;
                        }
                    }
                }

                // Set the current known permissions
                if (!string.IsNullOrEmpty(permissions))
                {
                    var p = permissions.Split(',');
                    session.CurrentPermissions = session.CurrentPermissions.Union(p).ToList();
                }

                // Save session data
                FacebookSessionCacheProvider.Current.SaveSessionData(session);
                this.CurrentSession = session;
            }
            finally
            {
                this.LoginInProgress = false;
            }

            return this.CurrentSession;
        }

        /// <summary>
        /// Log a user out of Facebook.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Logout", Justification = "Logout is preferred by design")]
        public void Logout()
        {
            try
            {
                FacebookSessionCacheProvider.Current.DeleteSessionData();
            }
            finally
            {
                this.CurrentSession = null;
            }
        }

        private async Task<FacebookOAuthResult> PromptOAuthDialog(string permissions, WebAuthenticationOptions options)
        {
            // Use WebAuthenticationBroker to launch server side OAuth flow

            Uri startUri = this.GetLoginUrl(permissions);
            Uri endUri = new Uri("https://www.facebook.com/connect/login_success.html");

            var result = await WebAuthenticationBroker.AuthenticateAsync(options, startUri, endUri);


            if (result.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
            {
                throw new InvalidOperationException();
            }
            else if (result.ResponseStatus == WebAuthenticationStatus.UserCancel)
            {
                throw new InvalidOperationException();
            }

            var client = new FacebookClient();
            var authResult = client.ParseOAuthCallbackUrl(new Uri(result.ResponseData));
            return authResult;
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

            var client = new FacebookClient();
            return client.GetLoginUrl(parameters);
        }



    }
}
