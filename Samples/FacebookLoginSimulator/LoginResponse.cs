// <copyright file="LoginResponse.cs" company="Microsoft Corporation.">
//     Copyright (c) 2013 Microsoft Corporation. All rights reserved.
// </copyright>
namespace FacebookLoginSimulator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;

    /// <summary>
    /// LoginResponse represents data returned from a Facebook Login request
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Defines the format for a key value pair in the querystring representation
        /// </summary>
        private const string pairFormat = "{0}={1}";

        /// <summary>
        /// Defines the key for access token in the query string representation
        /// </summary>
        private const string accessTokenKey = "access_token";

        /// <summary>
        /// Defines the key for expires_in in the query string representation
        /// </summary>
        private const string expiresInKey = "expires_in";

        /// <summary>
        /// Defines the key for error in the query string representation
        /// </summary>
        private const string errorKey = "error";

        /// <summary>
        /// Defines the key for error code in the query string representation
        /// </summary>
        private const string errorCodeKey = "error_code";

        /// <summary>
        /// Defines the key for error description in the query string representation
        /// </summary>
        private const string errorDescriptionKey = "error_description";

        /// <summary>
        /// Defines the key for error reason in the query string representation
        /// </summary>
        private const string errorReasonKey = "error_reason";

        /// <summary>
        /// Defines the key for state in the query string representation
        /// </summary>
        private const string stateKey = "state";

        /// <summary>
        /// Gets or sets the access token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets a value indicating when this token will expire
        /// </summary>
        /// <value>
        /// The number of seconds till expiration
        /// </value>
        public string ExpiresIn { get; set; }

        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets the error code
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the error description
        /// </summary>
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Gets or sets the error reason
        /// </summary>
        public string ErrorReason { get; set; }

        /// <summary>
        /// Gets or sets the state value
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets a value indicating whether the login response was successful and without any errors.
        /// </summary>
        public bool IsSuccess
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.AccessToken) && !string.IsNullOrWhiteSpace(this.ExpiresIn) && string.IsNullOrEmpty(this.Error);
            }
        }

        /// <summary>
        /// Initializes a new instance of the LoginResponse class from a querystring with key/value pairs
        /// </summary>
        /// <param name="queryString">Query to parse</param>
        public LoginResponse(string queryString)
        {
            if (string.IsNullOrWhiteSpace(queryString))
            {
                throw new InvalidDataException();
            }
            else
            {
                this.AccessToken = GetQueryStringValueFromUri(queryString, accessTokenKey);
                this.ExpiresIn = GetQueryStringValueFromUri(queryString, expiresInKey);
                this.Error = GetQueryStringValueFromUri(queryString, errorKey);
                this.ErrorCode = GetQueryStringValueFromUri(queryString, errorCodeKey);
                this.ErrorDescription = GetQueryStringValueFromUri(queryString, errorDescriptionKey);
                this.ErrorReason = GetQueryStringValueFromUri(queryString, errorReasonKey);
                this.State = GetQueryStringValueFromUri(queryString, stateKey);
            }
        }

        /// <summary>
        /// Initializes a new instance of the LoginResponse class
        /// </summary>
        public LoginResponse()
        {
        }

        /// <summary>
        /// Converts the LoginResponse to a string representation
        /// </summary>
        /// <returns>String representation in querystring format</returns>
        public override string ToString()
        {
            List<string> keyValuePairs;

            if (this.IsSuccess)
            {
                keyValuePairs = new List<string>()
                {
                    BuildKeyValuePair(accessTokenKey, this.AccessToken),
                    BuildKeyValuePair(expiresInKey, this.ExpiresIn)
                };
            }
            else
            {
                keyValuePairs = new List<string>()
                {
                    BuildKeyValuePair(errorKey, this.Error),
                    BuildKeyValuePair(errorCodeKey, this.ErrorCode),
                    BuildKeyValuePair(errorDescriptionKey, this.ErrorDescription),
                    BuildKeyValuePair(errorReasonKey, this.ErrorReason),
                    BuildKeyValuePair(stateKey, this.State),
                };
            }

            return null != keyValuePairs ? string.Join("&", keyValuePairs) : string.Empty;
        }

        /// <summary>
        /// Builds a key value pair for serialization to a querystring
        /// </summary>
        /// <param name="key">Key in pair</param>
        /// <param name="value">Value in pair</param>
        /// <returns>Key value pair</returns>
        private static string BuildKeyValuePair(string key, string value)
        {
            return string.Format(pairFormat, HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value));
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
        /// <param name="separator">
        /// Query string separator
        /// </param>
        /// <returns>
        /// QueryString value
        /// </returns>
        private string GetQueryStringValueFromUri(string uri, string key)
        {
            int questionMarkIndex = uri.LastIndexOf("?", StringComparison.Ordinal);
            if (questionMarkIndex == -1)
            {
                // if no questionmark found, this is just a queryString
                questionMarkIndex = 0;
            }

            string queryString = uri.Substring(questionMarkIndex, uri.Length - questionMarkIndex);
            queryString = queryString.Replace("#", string.Empty).Replace("?", string.Empty);

            string[] keyValuePairs = queryString.Split(
                new[]
                {
                    '&'
                });
            for (int i = 0; i < keyValuePairs.Length; i++)
            {
                string[] pair = keyValuePairs[i].Split(
                    new[]
                    {
                        '='
                    });
                if (pair[0].ToLowerInvariant() == key.ToLowerInvariant())
                {
                    return HttpUtility.UrlDecode(pair[1]);
                }
            }

            return string.Empty;
        }
    }
}
