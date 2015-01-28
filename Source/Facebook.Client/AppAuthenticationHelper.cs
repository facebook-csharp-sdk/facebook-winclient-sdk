using System.Xml;
using Windows.Storage;

#if WP8
using Microsoft.Phone.Maps.Services;
#endif

namespace Facebook.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Windows.System;
    using Facebook;

#if WP8
        using Microsoft.Xna.Framework;
#endif
    public static class AppAuthenticationHelper
    {
        #region Constants

        private const int MaxTokenLifeTime = 60;
        /// <summary>
        ///     Defines the key for access token in the query string representation
        /// </summary>
        private const string AccessTokenKey = "access_token";

        /// <summary>
        ///     Defines the key for error code in the query string representation
        /// </summary>
        private const string ErrorCodeKey = "error_code";

        /// <summary>
        ///     Defines the key for error description in the query string representation
        /// </summary>
        private const string ErrorDescriptionKey = "error_description";

        /// <summary>
        ///     Defines the key for error in the query string representation
        /// </summary>
        private const string ErrorKey = "error";

        /// <summary>
        ///     Defines the key for error reason in the query string representation
        /// </summary>
        private const string ErrorReasonKey = "error_reason";

        /// <summary>
        ///     Defines the key for expires_in in the query string representation
        /// </summary>
        private const string ExpiresInKey = "expires_in";

        /// <summary>
        ///     Defines the key for state in the query string representation
        /// </summary>
        private const string StateKey = "state";

        private const string EncodedLaunchUri = "encodedLaunchUri";
        #endregion

        /// <summary>
        /// Determines whether the specified uri is a facebook login response uri
        /// </summary>
        /// <param name="uri">The uri to check</param>
        /// <returns>
        ///   <c>true</c> if the specified uri is a Facebook login uri
        /// </returns>
        async public static Task<bool> IsFacebookLoginResponse(Uri uri)
        {
            var schemeName = await GetFacebookLoginCallbackSchemeName();
            return uri.ToString().ToLowerInvariant().Contains(schemeName.ToLowerInvariant());
        }

        /// <summary>
        /// Initializes a new instance of the AccessTokenData class from a query string with key/value pairs
        /// </summary>
        /// <param name="queryString">
        /// Query to parse
        /// </param>
        public static void ParseQueryString(this AccessTokenData session, string queryString)
        {
            if (string.IsNullOrWhiteSpace(queryString))
            {
                throw new InvalidDataException();
            }

            // parse out errors, if any
            var error = GetQueryStringValueFromUri(queryString, ErrorKey);
            var errorDescription = GetQueryStringValueFromUri(queryString, ErrorDescriptionKey);
            var errorReason = GetQueryStringValueFromUri(queryString, ErrorReasonKey);

            int errorCodeValue = 0;
            int.TryParse(GetQueryStringValueFromUri(queryString, ErrorCodeKey), out errorCodeValue);

            if (string.IsNullOrEmpty(error) && errorCodeValue == 0)
            {
                // parse out string values
                session.AccessToken = GetQueryStringValueFromUri(queryString, AccessTokenKey);
                session.State = GetQueryStringValueFromUri(queryString, StateKey);
                //var encodedLaunchUri = GetQueryStringValueFromUri(queryString, EncodedLaunchUri);
                //if (!String.IsNullOrEmpty(encodedLaunchUri))
                //{
                //    //session.AppId = encodedLaunchUri.Substring(2); // ignore the fb
                //    var index = encodedLaunchUri.IndexOf(":", StringComparison.InvariantCulture);
                //    var appId = encodedLaunchUri.Substring(2, index - 2);
                //    if (!String.IsNullOrEmpty(appId))
                //    {
                //    }
                //}
                
                // parse out other types
                long expiresInValue;
                DateTime now = DateTime.UtcNow;
                if (long.TryParse(GetQueryStringValueFromUri(queryString, ExpiresInKey), out expiresInValue))
                {
                    session.Expires = now + TimeSpan.FromSeconds(expiresInValue);
                    session.Issued = now - (TimeSpan.FromDays(MaxTokenLifeTime) - TimeSpan.FromSeconds(expiresInValue));
                }
            }
            else
            {
                throw new FacebookOAuthException(
                                                    string.Format("{0}: {1}", error, errorDescription),
                                                    errorReason,
                                                    errorCodeValue,
                                                    0);
            }
        }

        /// <summary>
        /// Defines the Facebook Login URI template.
        /// </summary>
        internal const string FacebookConnectUriTemplate = "fbconnect://authorize?client_id={0}&scope={1}&redirect_uri={2}&state={3}";

        /// <summary>
        /// Gets the scheme name registered by the calling app for facebook login
        /// </summary>
        /// <returns>The deep link scheme name for facebook login</returns>
        async internal static Task<string> GetFacebookLoginCallbackSchemeName()
        {
            string result = await GetFilteredManifestAppAttributeValue("Protocol", "Name", "msft-");
            return result;
        }

        /// <summary>
        /// Gets an attribute from the manifest App node, checking for a prefix value
        /// </summary>
        /// <param name="node">
        /// Node name to search under
        /// </param>
        /// <param name="attribute">
        /// Attribute name to search for
        /// </param>
        /// <param name="prefix">
        /// Prefix that the value must start with
        /// </param>
        /// <returns>
        /// Attribute value, or empty string if no match found
        /// </returns>
        internal async static Task<string> GetFilteredManifestAppAttributeValue(string node, string attribute, string prefix)
        {
#if WINDOWS_UNIVERSAL
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///FacebookConfig.xml"));
            using (Stream strm = await file.OpenStreamForReadAsync())
#endif

#if WP8
            using (Stream strm = TitleContainer.OpenStream("WMAppManifest.xml"))
#endif
            {
                var xml = XElement.Load(strm);
                var filteredAttributeValue = (from app in xml.Descendants(node)
                                              let xAttribute = app.Attribute(attribute)
                                              where xAttribute != null
                                              select xAttribute.Value).FirstOrDefault(a => a.StartsWith(prefix));

                if (string.IsNullOrWhiteSpace(filteredAttributeValue))
                {
                    return string.Empty;
                }

                return filteredAttributeValue;
            }
        }


        internal async static Task<string> GetFacebookConfigValue(string node, string attribute)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///FacebookConfig.xml"));
            // TODO: (sanjeevd) throw exception if the file is not found

            using (Stream strm = await file.OpenStreamForReadAsync())
            {
                var xml = XElement.Load(strm);
                var filteredAttributeValue = (from app in xml.Descendants(node)
                                              let xAttribute = app.Attribute(attribute)
                                              where xAttribute != null
                                              select xAttribute.Value).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(filteredAttributeValue))
                {
                    // TODO: (sanjeevd) Throw an exception
                    return string.Empty;
                }

                return filteredAttributeValue;
            }
        }
        /// <summary>
        /// Authenticates via app to app communication with the Facebook app
        /// </summary>
        /// <param name="appId">The application id for which the user needs to be authenticated.</param>
        /// <param name="permissions">List of permissions that are being requested.</param>
        internal static void AuthenticateWithApp(string appId, string permissions)
        {
            AuthenticateWithApp(appId, permissions, null);
        }

        /// <summary>
        /// Authenticates via app to app communication with the Facebook app
        /// </summary>
        /// <param name="appId">The application id for which the user needs to be authenticated.</param>
        /// <param name="permissions">List of permissions that are being requested.</param>
        /// <param name="state">The state, this will be passed back in the response</param>
        internal static async Task AuthenticateWithApp(string appId, string permissions, string state)
        {
            var redirectUri = await GetFacebookLoginCallbackSchemeName() + "://authorize";
            string uriString = string.Format(FacebookConnectUriTemplate, appId, permissions, redirectUri, HttpHelper.UrlEncode(state == null ? string.Empty : state));
            await Launcher.LaunchUriAsync(new Uri(uriString));
        }

        /// <summary>
        /// Gets a query string value from a full URI
        /// </summary>
        /// <param name="uri">
        /// URI to parse
        /// </param>
        /// <param name="key">
        /// QueryString key
        /// </param>
        /// <returns>
        /// QueryString value
        /// </returns>
        private static string GetQueryStringValueFromUri(string uri, string key)
        {
            int questionMarkIndex = uri.LastIndexOf("?", StringComparison.Ordinal);
            if (questionMarkIndex == -1)
            {
                // if no questionmark found, this is just a queryString
                questionMarkIndex = 0;
            }

            string queryString = uri.Substring(questionMarkIndex, uri.Length - questionMarkIndex);
            queryString = queryString.Replace("#", string.Empty).Replace("?", string.Empty);

            string[] keyValuePairs = queryString.Split(new[] { '&' });
            for (int i = 0; i < keyValuePairs.Length; i++)
            {
                string[] pair = keyValuePairs[i].Split(new[] { '=' });
                if (pair[0].ToLowerInvariant() == key.ToLowerInvariant())
                {
                    return HttpHelper.UrlDecode(pair[1]);
                }
            }

            return string.Empty;
        }
    }
}
