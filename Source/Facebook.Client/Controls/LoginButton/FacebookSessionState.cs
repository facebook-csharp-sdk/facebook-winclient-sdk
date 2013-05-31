namespace Facebook.Client.Controls
{
    /// <summary>
    /// Identifies the states of a Facebook session.
    /// </summary>
    public enum FacebookSessionState
    {
        /// <summary>
        /// Indicates that the session has not yet been opened and has no cached token.
        /// </summary>
        Created,

        /// <summary>
        /// Indicates that the session has not yet been opened and has a cached token.
        /// </summary>
        CreatedTokenLoaded,

        /// <summary>
        /// Indicates that the session is in the process of opening.
        /// </summary>
        Opening,

        /// <summary>
        /// Indicates that the session is opened.
        /// </summary>
        Opened,

        /// <summary>
        /// Indicates that the session is opened and that the token has changed.
        /// </summary>
        OpenedTokenUpdated,

        /// <summary>
        /// Indicates that the session is closed, and that it was not closed normally.
        /// </summary>
        ClosedLoginFailed,

        /// <summary>
        /// Indicates that the session was closed normally.
        /// </summary>
        Closed
    }
}
