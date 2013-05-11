namespace Facebook.Client.Controls
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides design time support for the FriendPicker control.
    /// </summary>
    internal class FriendPickerDesignSupport
    {
        private static List<GraphUser> data;

        public static IEnumerable<GraphUser> DesignData
        {
            get
            {
                if (FriendPickerDesignSupport.data == null)
                {
                    FriendPickerDesignSupport.data = LoadDesignData();
                }

                return FriendPickerDesignSupport.data;
            }
        }

        private static List<GraphUser> LoadDesignData()
        {
            var data = new List<GraphUser>();
            
            var profilePictureUrl = new Uri(ProfilePicture.GetBlankProfilePictureUrl(true), UriKind.RelativeOrAbsolute);

            data.Add(new GraphUser { Id = "1", Name = "Michael Alexander", FirstName = "Michael", LastName = "Alexander", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = "", Name = "Pilar Ackerman", FirstName = "Pilar", LastName = "Ackerman", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = "", Name = "Yan Li", FirstName = "Yan", LastName = "Li", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = "", Name = "Madeleine Kelly", FirstName = "Madeleine", LastName = "Kelly", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = "", Name = "Peter Connelly", FirstName = "Peter", LastName = "Connelly", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = "", Name = "Alicia Thornber", FirstName = "Alicia", LastName = "Thornber", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = "", Name = "Frank Martinez", FirstName = "Frank", LastName = "Martinez", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = "", Name = "Tina Makovec", FirstName = "Tina", LastName = "Makovec", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = "", Name = "Lisa Toftemark", FirstName = "Lisa", LastName = "Toftemark", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = "", Name = "Karen Toh", FirstName = "Karen", LastName = "Toh", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = "", Name = "Peter Villadsen", FirstName = "Peter", LastName = "Villadsen", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = "", Name = "Anna Misiec", FirstName = "Anna", LastName = "Misiec", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = "", Name = "Doris Krieger", FirstName = "Doris", LastName = "Krieger", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = "", Name = "Pat Coleman", FirstName = "Pat", LastName = "Coleman", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = "", Name = "Samantha Smith", FirstName = "Samantha", LastName = "Smith", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = "", Name = "Roman Stachnio", FirstName = "Roman", LastName = "Stachnio", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = "", Name = "Oliver Lee", FirstName = "Oliver", LastName = "Lee", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = "", Name = "Jacky Chan", FirstName = "Jacky", LastName = "Chan", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = "", Name = "Daniela Guaita", FirstName = "Daniela", LastName = "Guaita", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = "", Name = "Nina Vietzen", FirstName = "Nina", LastName = "Vietzen", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = "", Name = "Ray Mohrman", FirstName = "Ray", LastName = "Mohrman", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = "", Name = "Eduardo Melo", FirstName = "Eduardo", LastName = "Melo", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = "", Name = "Jesse Merriam", FirstName = "Jesse", LastName = "Merriam", ProfilePictureUrl = profilePictureUrl });

            return data;
        }
    }
}
