namespace Facebook.Client
{
    using System;
    using System.Collections.Generic;
    using Facebook.Client.Controls;

    /// <summary>
    /// Provides a strongly-typed representation of a Facebook User as defined by the Graph API.
    /// </summary>
    /// <remarks>
    /// The GraphUser class represents the most commonly used properties of a Facebook User object.
    /// </remarks>
    public class GraphUser : GraphObject
    {
        private Uri profilePictureUrl;

        /// <summary>
        /// Initializes a new instance of the GraphUser class.
        /// </summary>
        public GraphUser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the GraphUser class from a dynamic object returned by the Facebook API.
        /// </summary>
        /// <param name="user">The dynamic object representing the Facebook user.</param>
        public GraphUser(dynamic user)
            : base((IDictionary<string, object>)user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var tmpUser = user as IDictionary<string, object>;
            //TODO: (sanjeevd) Runtimebinder exception here. Way too many of them.
            this.Id = tmpUser.ContainsKey("id")? (string)tmpUser["id"]:String.Empty;
            this.Name = tmpUser.ContainsKey("name")? (string)tmpUser["name"]:String.Empty;// user["name"];
            this.UserName = tmpUser.ContainsKey("username") ? (string)tmpUser["username"] : String.Empty; //user["username"];
            this.FirstName = tmpUser.ContainsKey("first_name") ? (string)tmpUser["first_name"] : String.Empty; //user["first_name"];
            this.MiddleName = tmpUser.ContainsKey("middle_name") ? (string)tmpUser["middle_name"] : String.Empty; //user["middle_name"];
            this.LastName = tmpUser.ContainsKey("last_name") ? (string)tmpUser["last_name"] : String.Empty; //user["last_name"];
            this.Birthday = tmpUser.ContainsKey("birthday") ? (string)tmpUser["birthday"] : String.Empty; //user["birthday"];
            var location = tmpUser.ContainsKey("location") ? (IDictionary<string, object>)tmpUser["location"] : null; //user["location"];
            this.Location = location != null ? new GraphLocation(location) : null;
            this.Link = tmpUser.ContainsKey("link") ? (string)tmpUser["link"] : String.Empty;// user["link"];
            var picture = tmpUser.ContainsKey("picture") ? user["picture"] : null; //user["picture"];
            if (picture != null)
            {
                if (picture["data"] != null)
                {
                    if (!String.IsNullOrEmpty(picture["data"]["url"]))
                        Uri.TryCreate(picture["data"]["url"], UriKind.Absolute, out this.profilePictureUrl);
                }
            }
        }

        /// <summary>
        /// Gets or sets the ID of the user.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Facebook username of the user.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the middle name of the user.
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the birthday of the user.
        /// </summary>
        public string Birthday { get; set; }

        /// <summary>
        /// Gets or sets the Facebook URL of the user.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Gets or sets the current city of the user.
        /// </summary>
        public GraphLocation Location { get; set; }

        /// <summary>
        /// Gets or sets the URL of the user's profile picture.
        /// </summary>
        public Uri ProfilePictureUrl
        {
            get
            {
                if (this.profilePictureUrl == null)
                {
                    this.profilePictureUrl = new Uri(ProfilePicture.GetBlankProfilePictureUrl(true), UriKind.RelativeOrAbsolute);
                }

                return this.profilePictureUrl;
            }

            set
            {
                this.profilePictureUrl = value;
            }
        }
    }
}
