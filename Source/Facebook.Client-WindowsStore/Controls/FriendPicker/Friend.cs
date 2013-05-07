namespace Facebook.Client.Controls
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides a strongly-typed representation of a friend object.
    /// </summary>
    public class Friend
    {
        private static List<Friend> data;

        /// <summary>
        /// Gets or sets the Facebook profile ID of the friend.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the friend.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get or ses the URL of the friend's profile picture.
        /// </summary>
        public string PictureUri { get; set; }

        internal static IEnumerable<Friend> DesignData
        {
            get
            {
                if (Friend.data == null)
                {
                    Friend.data = LoadDesignData();
                }

                return Friend.data;
            }
        }

        private static List<Friend> LoadDesignData()
        {
            var data = new List<Friend>();
            
            var friendList = new[] { 
                "Michael Alexander", "Pilar Ackerman", "Yan Li", "Madeleine Kelly", "Peter Connelly", "Alicia Thornber", "Frank Martinez", "Tina Makovec",
                "Lisa Toftemark", "Karen Toh", "Peter Villadsen", "Anna Misiec", "Doris Krieger", "Pat Coleman", "Samantha Smith", "Roman Stachnio",
                "Oliver Lee", "Jacky Chan", "Daniela Guaita", "Nina Vietzen", "Ray Mohrman", "Eduardo Melo", "Jesse Merriam", "Pat Coleman"
            };

            var profilePictureUrl = ProfilePicture.GetBlankProfilePictureUrl(true);

            int id = 1;
            foreach (var name in friendList)
            {
                data.Add(new Friend { Id = id.ToString(), Name = name, PictureUri = profilePictureUrl });
                id++;
            }

            return data;
        }
    }
}
