using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using System.Net;
using Facebook;

namespace Facebook.Client
{
    class LifecycleHelper
    {
#if WINDOWS_UNIVERSAL
        public static void FacebookAuthenticationReceived(ProtocolActivatedEventArgs protocolArgs)
        {
            if (protocolArgs == null)
            {
                throw new ArgumentNullException("protocolArgs");
            }

            // parse and fill out the token data
            try
            {
                AccessTokenData session = new AccessTokenData();
                session.ParseQueryString(Facebook.HttpHelper.UrlDecode(protocolArgs.Uri.ToString()));
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
                }
            }
            catch (Facebook.FacebookOAuthException exc)
            {

            }
        }
#endif
    }
}
