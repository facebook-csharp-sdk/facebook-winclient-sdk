namespace Facebook.Client.Controls
{
    /// <summary>
    /// Provides data for the AuthenticationError event.
    /// </summary>
    public class AuthenticationErrorEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the AuthenticationErrorEventArgs class.
        /// </summary>
        /// <param name="description">The description of the authentication error.</param>
        /// <param name="reason">The reason for the authentication error.</param>
        public AuthenticationErrorEventArgs(string description, string reason)
        {
            this.Description = description;
            this.Reason = reason;
        }

        /// <summary>
        /// Returns a description of the error.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Returns a reason for the error.
        /// </summary>
        public string Reason { get; private set; }
    }
}
