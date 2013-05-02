namespace Facebook.Client.Controls
{
    /// <summary>
    /// Provides data for the UserInfoChanged event.
    /// </summary>
    public class UserInfoChangedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the UserInfoChangedEventArgs class.
        /// </summary>
        /// <param name="user">The current user.</param>
        public UserInfoChangedEventArgs(GraphUser user)
        {
            this.User = user;
        }

        /// <summary>
        /// Gets the current user, or null if there is no user.
        /// </summary>
        public GraphUser User { get; private set; }
    }
}
