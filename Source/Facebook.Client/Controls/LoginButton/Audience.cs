namespace Facebook.Client.Controls
{
    /// <summary>
    /// Identifies the default audience to use for sessions that post data to Facebook.
    /// </summary>
    public enum Audience
    {
        /// <summary>
        /// No audience needed; this value is useful for cases where data will only be read from Facebook.
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates that only the user is able to see posts made by the application.
        /// </summary>
        OnlyMe = 10,

        /// <summary>
        /// Indicates that the user's friends are able to see posts made by the application.
        /// </summary>
        Friends = 20,

        /// <summary>
        /// Indicates that all Facebook users are able to see posts made by the application.
        /// </summary>
        Everyone = 30
    }
}
