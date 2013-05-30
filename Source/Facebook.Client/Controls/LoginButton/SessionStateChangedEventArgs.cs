namespace Facebook.Client.Controls
{
    /// <summary>
    /// Provides data for the SessionStateChanged event.
    /// </summary>
    public class SessionStateChangedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the SessionStateChangedEventArgs class.
        /// </summary>
        /// <param name="sessionState">The current state of the Facebook session.</param>
        public SessionStateChangedEventArgs(FacebookSessionState sessionState)
        {
            this.SessionState = sessionState;
        }

        /// <summary>
        /// Gets the current state of the Facebook session.
        /// </summary>
        public FacebookSessionState SessionState { get; private set; }
    }
}
