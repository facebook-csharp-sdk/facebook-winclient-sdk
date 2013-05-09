namespace Facebook.Client.Controls
{
    /// <summary>
    /// Provides data for the FriendRetrieved event.
    /// </summary>
    public class FriendRetrievedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the FriendRetrievedEventArgs class.
        /// </summary>
        /// <param name="friend">The Facebook friend.</param>
        public FriendRetrievedEventArgs(GraphUser friend)
        {
            this.Friend = friend;
        }

        /// <summary>
        /// The Facebook friend retrieved.
        /// </summary>
        public GraphUser Friend { get; private set; }

        /// <summary>
        /// Controls whether the friend is included in the list.
        /// </summary>
        public bool Exclude { get; set; }
    }
}
