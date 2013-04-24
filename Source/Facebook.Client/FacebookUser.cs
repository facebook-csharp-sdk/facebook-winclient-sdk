namespace Facebook.Client
{
    /// <summary>
    /// Provides a strongly-typed representation of a Facebook User as defined by the Graph API.
    /// </summary>
    public class FacebookUser
    {
        /// <summary>
        /// Gets the ID of the user.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the Facebook username of the user.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets the first name of the user.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets the middle name of the user.
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Gets the last name of the user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets the birthday of the user.
        /// </summary>
        public string Birthday { get; set; }

        /// <summary>
        /// Gets the Facebook URL of the user.
        /// </summary>
        public string Link { get; set; }

        ///// <summary>
        ///// Gets the current city of the user.
        ///// </summary>
        //public Location Location { get; set; }
    }
}
