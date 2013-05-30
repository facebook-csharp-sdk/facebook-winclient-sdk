namespace Facebook.Client.Controls
{
    /// <summary>
    /// Provides data for the LoadFailed event.
    /// </summary>
    public class LoadFailedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the LoadFailedEventArgs class.
        /// </summary>
        /// <param name="description">The description of the error.</param>
        /// <param name="reason">The reason for the error.</param>
        public LoadFailedEventArgs(string description, string reason)
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
