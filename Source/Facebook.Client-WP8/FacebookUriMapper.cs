using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using System.Net;
using Microsoft.Phone.Controls;

namespace Facebook.Client
{
    public class FacebookUriMapper : UriMapperBase
    {
        public override Uri MapUri(Uri uri)
        {
            var tempUri = System.Net.HttpUtility.UrlDecode(uri.ToString());

            try
            {
                AccessTokenData session = new AccessTokenData();
                session.ParseQueryString(HttpUtility.UrlDecode(uri.ToString()));
                if (!String.IsNullOrEmpty(session.AccessToken))
                {
                    var task = Task.Run(async () => await AppAuthenticationHelper.GetFacebookConfigValue("Facebook", "AppId"));
                    task.Wait();
                    session.AppId = task.Result;
                    Session.ActiveSession.CurrentAccessTokenData = session;

                    // trigger the event handler with the session
                    if (Session.OnFacebookAuthenticationFinished != null)
                    {
                        Session.OnFacebookAuthenticationFinished(session);
                    }

                    if (Session.OnSessionStateChanged != null)
                    {
                        Session.OnSessionStateChanged(LoginStatus.LoggedIn);
                    }
                }
            }
            catch (Facebook.FacebookOAuthException exc)
            {

            }
            
            return new Uri("/" + Session.ActiveSession.RedirectPageOnSuccess, UriKind.Relative);
        }
    }
}
