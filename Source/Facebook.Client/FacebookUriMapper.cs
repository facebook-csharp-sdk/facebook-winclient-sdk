using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
#if WP8
using System.Windows.Navigation;
#endif

namespace Facebook.Client
{
#if WP8
    public class FacebookUriMapper : UriMapperBase
#else
    public class FacebookUriMapper
#endif
    {
        private enum FacebookUriType
        {
            Login,
            AppRequest,
            Feed
        }

#if WP8
        public override Uri MapUri(Uri uri)
#else
        public Uri MapUri(Uri uri)
#endif
        {
            var tempUri = uri.ToString();
            FacebookUriType uriType = FacebookUriType.Login;
#if WP8
            if (tempUri.StartsWith(string.Format("/Protocol?encodedLaunchUri={0}", Session.LoginRedirectUri)))
                uriType = FacebookUriType.Login;
            else if (tempUri.StartsWith(string.Format("/Protocol?encodedLaunchUri={0}", Session.AppRequestRedirectUri)))
                uriType = FacebookUriType.AppRequest;
            else if (tempUri.StartsWith(string.Format("/Protocol?encodedLaunchUri={0}", Session.FeedRedirectUri)))
                uriType = FacebookUriType.Feed;
#else
            if (tempUri.StartsWith(WebUtility.UrlDecode(Session.LoginRedirectUri)))
                uriType = FacebookUriType.Login;
            else if (tempUri.StartsWith(WebUtility.UrlDecode(Session.AppRequestRedirectUri)))
                uriType = FacebookUriType.AppRequest;
            else if (tempUri.StartsWith(WebUtility.UrlDecode(Session.FeedRedirectUri)))
                uriType = FacebookUriType.Feed;
#endif

            if (uriType == FacebookUriType.Login)
            {
                try
                {
                    AccessTokenData session = new AccessTokenData();
                    session.ParseQueryString(WebUtility.UrlDecode(uri.ToString()));
                    if (!String.IsNullOrEmpty(session.AccessToken))
                    {
                        session.AppId = Session.AppId;
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
                    // fire the authentication finished handler with the exception 
                    if (Session.OnFacebookAuthenticationFinished != null)
                    {
                        Session.OnFacebookAuthenticationFinished(null);
                    }
                }
            }
            else if (uriType == FacebookUriType.AppRequest)
            {
                // parsing query string to get request id and facebook ids of the people the request has been sent to
                // or error code and error messages
                FBResult fbResult = new FBResult();

                try
                {
                    fbResult.Json = new JsonObject();

#if WP8
                    string queryString = GetQueryStringFromUri(HttpUtility.UrlDecode(tempUri));
#else
                    string queryString = GetQueryStringFromUri("/Protocol?encodedLaunchUri=" + WebUtility.UrlDecode(tempUri));
#endif

                    string[] queries = queryString.Split('&');
                    if (queries.Length > 0)
                    {
                        string request = string.Empty;
                        List<string> toList = new List<string>();

                        foreach (string query in queries)
                        {
                            string[] keyValue = query.Split('=');
                            if (keyValue.Length >= 2)
                            {
                                if (keyValue[0].Contains("request"))
                                    request = keyValue[1];
                                else if (keyValue[0].Contains("to"))
                                    toList.Add(keyValue[1]);
                                else if (keyValue[0].Contains("error_code"))
                                    fbResult.Error = keyValue[1];
                                else if (keyValue[0].Contains("error_message"))
                                    fbResult.Text = keyValue[1].Replace('+', ' ');
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(request))
                        {
                            fbResult.Json.Add(new KeyValuePair<string, object>("request", request));
                            fbResult.Json.Add(new KeyValuePair<string, object>("to", toList));
                        }

                        // If there's no error, assign the success text
                        if (string.IsNullOrWhiteSpace(fbResult.Text))
                            fbResult.Text = "Success";
                    }
                }
                catch
                {
                    fbResult.Error = "Failure";
                    fbResult.Text = "AppRequest cancelled or ended with exceptional state";
                }

                // trigger the event handler with the session
                if (Session.OnFacebookAppRequestFinished != null)
                {
                    Session.OnFacebookAppRequestFinished(fbResult);
                }
            }
            else if (uriType == FacebookUriType.Feed)
            {
                // parsing query string to get request id and facebook ids of the people the request has been sent to
                // or error code and error messages
                FBResult fbResult = new FBResult();

                try
                {
                    // parsing query string to get request id and facebook ids of the people the request has been sent to
                    // or error code and error messages
                    fbResult.Json = new JsonObject();

#if WP8
                    string queryString = GetQueryStringFromUri(HttpUtility.UrlDecode(tempUri));
#else
                    string queryString = GetQueryStringFromUri("/Protocol?encodedLaunchUri=" + WebUtility.UrlDecode(tempUri));
#endif

                    string[] queries = queryString.Split('&');
                    if (queries.Length > 0)
                    {
                        string postId = string.Empty;
                        List<string> toList = new List<string>();

                        foreach (string query in queries)
                        {
                            string[] keyValue = query.Split('=');
                            if (keyValue.Length >= 2)
                            {
                                if (keyValue[0].Contains("post_id"))
                                    postId = keyValue[1];
                                else if (keyValue[0].Contains("error_code"))
                                    fbResult.Error = keyValue[1];
                                else if (keyValue[0].Contains("error_msg"))
                                    fbResult.Text = keyValue[1].Replace('+', ' ');
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(postId))
                        {
                            fbResult.Json.Add(new KeyValuePair<string, object>("post_id", postId));
                        }
                    }

                    // If there's no error, assign the success text
                    if (string.IsNullOrWhiteSpace(fbResult.Text))
                        fbResult.Text = "Success";

                }
                catch
                {
                    fbResult.Error = "Failure";
                    fbResult.Text = "Feed cancelled or ended with exceptional state";
                }

                // trigger the event handler with the session
                if (Session.OnFacebookFeedFinished != null)
                {
                    Session.OnFacebookFeedFinished(fbResult);
                }
            }

            if (uri.ToString().StartsWith("/Protocol"))
            {
                // Read which page to redirect to when redirecting from the Facebook authentication.
                var RedirectPageNameTask =
                    Task.Run(async () => await AppAuthenticationHelper.GetFacebookConfigValue("RedirectPage", "Name"));
                RedirectPageNameTask.Wait();
                Session.ActiveSession.RedirectPageOnSuccess = String.IsNullOrEmpty(RedirectPageNameTask.Result)
                    ? "MainPage.xaml"
                    : RedirectPageNameTask.Result;

                return new Uri("/" + Session.ActiveSession.RedirectPageOnSuccess, UriKind.Relative);
            }

            // Otherwise perform normal launch.
            return uri;
        }

        private static string GetQueryStringFromUri(string uri)
        {
            int questionMarkIndex = uri.LastIndexOf("?", StringComparison.Ordinal);
            if (questionMarkIndex == -1)
            {
                // if no questionmark found, this is just a queryString
                return uri;
            }

            string queryString = string.Empty;

            int endIndex = uri.LastIndexOf("#_=_");
            if (endIndex == -1)
                queryString = uri.Substring(questionMarkIndex, uri.Length - questionMarkIndex);
            else
                queryString = uri.Substring(questionMarkIndex, endIndex - questionMarkIndex);
            
            queryString = queryString.Replace("#", string.Empty).Replace("?", string.Empty);
            return queryString;
        }
    }
}
