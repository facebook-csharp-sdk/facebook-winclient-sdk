namespace Facebook.Client.Controls
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides design time support for the FriendPicker control.
    /// </summary>
    public class FriendPickerDesignSupport
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
            data.Add(new GraphUser { Id = string.Empty, Name = "Pilar Ackerman", FirstName = "Pilar", LastName = "Ackerman", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = string.Empty, Name = "Yan Li", FirstName = "Yan", LastName = "Li", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = string.Empty, Name = "Madeleine Kelly", FirstName = "Madeleine", LastName = "Kelly", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = string.Empty, Name = "Peter Connelly", FirstName = "Peter", LastName = "Connelly", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = string.Empty, Name = "Alicia Thornber", FirstName = "Alicia", LastName = "Thornber", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = string.Empty, Name = "Frank Martinez", FirstName = "Frank", LastName = "Martinez", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = string.Empty, Name = "Tina Makovec", FirstName = "Tina", LastName = "Makovec", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = string.Empty, Name = "Lisa Toftemark", FirstName = "Lisa", LastName = "Toftemark", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = string.Empty, Name = "Karen Toh", FirstName = "Karen", LastName = "Toh", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = string.Empty, Name = "Peter Villadsen", FirstName = "Peter", LastName = "Villadsen", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = string.Empty, Name = "Anna Misiec", FirstName = "Anna", LastName = "Misiec", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = string.Empty, Name = "Doris Krieger", FirstName = "Doris", LastName = "Krieger", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = string.Empty, Name = "Pat Coleman", FirstName = "Pat", LastName = "Coleman", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = string.Empty, Name = "Samantha Smith", FirstName = "Samantha", LastName = "Smith", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = string.Empty, Name = "Roman Stachnio", FirstName = "Roman", LastName = "Stachnio", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = string.Empty, Name = "Oliver Lee", FirstName = "Oliver", LastName = "Lee", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = string.Empty, Name = "Jacky Chan", FirstName = "Jacky", LastName = "Chan", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = string.Empty, Name = "Daniela Guaita", FirstName = "Daniela", LastName = "Guaita", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = string.Empty, Name = "Nina Vietzen", FirstName = "Nina", LastName = "Vietzen", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = string.Empty, Name = "Ray Mohrman", FirstName = "Ray", LastName = "Mohrman", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = string.Empty, Name = "Eduardo Melo", FirstName = "Eduardo", LastName = "Melo", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphUser { Id = string.Empty, Name = "Jesse Merriam", FirstName = "Jesse", LastName = "Merriam", ProfilePictureUrl = profilePictureUrl });

            return data;
        }
    }
}
