// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacebookUriMapper.cs" company="Microsoft">
//   2013
// </copyright>
// <summary>
//   Uri Mapper for Facebook URI schemes
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FacebookLoginSimulator
{
    using System;
    using System.Net;
    using System.Windows.Navigation;

    /// <summary>
    /// Uri Mapper for Facebook URI schemes
    /// </summary>
    public class FacebookUriMapper : UriMapperBase
    {
        #region Constants

        /// <summary>
        ///     Uri Key for client ID
        /// </summary>
        public const string ClientIdKey = "client_id=";

        /// <summary>
        ///     Uri Key for redirect Uri
        /// </summary>
        public const string RedirectUriKey = "redirect_uri=";
        
        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Maps a scheme Uri to a Uri within this app
        /// </summary>
        /// <param name="uri">
        /// Scheme Uri used for invocation
        /// </param>
        /// <returns>
        /// The <see cref="Uri"/>.
        /// </returns>
        public override Uri MapUri(Uri uri)
        {
            string tempUri = HttpUtility.UrlDecode(uri.ToString());

            if (this.IsSchemeNavigation(tempUri))
            {
                return this.GetSchemeDeeplinkUri(tempUri);
            }

            // Unknown mapping... just navigate to what user has requested.
            return uri;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get scheme deep link uri.
        /// </summary>
        /// <param name="uri">
        /// The uri.
        /// </param>
        /// <returns>
        /// The <see cref="Uri"/>.
        /// </returns>
        private Uri GetSchemeDeeplinkUri(string uri)
        {
            int startIndex = 0;
            if (uri.Contains("?"))
            {
                startIndex = uri.IndexOf("?", StringComparison.Ordinal) + 1;
            }

            string queryString = uri.ToLower().Substring(startIndex);

            string uriString = "/MainPage.xaml?" + queryString;

            return new Uri(uriString, UriKind.Relative);
        }

        /// <summary>
        /// The is scheme navigation.
        /// </summary>
        /// <param name="uri">
        /// The uri.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsSchemeNavigation(string uri)
        {
            return uri.ToLower().Contains(Constants.FacebookConnectSchemeName);
        }

        #endregion
    }
}