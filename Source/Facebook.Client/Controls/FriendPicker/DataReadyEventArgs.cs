namespace Facebook.Client.Controls
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides data for the LoadCompleted event.
    /// </summary>
    public class DataReadyEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the DataReadyEventArgs class.
        /// </summary>
        /// <param name="friends">The list of Facebook friends.</param>
        public DataReadyEventArgs(IEnumerable<GraphUser> friends)
        {
            this.Friends = friends;
        }

        /// <summary>
        /// Gets the list of Facebook friends retrieved.
        /// </summary>
        public IEnumerable<GraphUser> Friends { get; private set; }
    }
}
