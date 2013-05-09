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
            
            var friendList = new[] { 
                "Michael Alexander", "Pilar Ackerman", "Yan Li", "Madeleine Kelly", "Peter Connelly", "Alicia Thornber", "Frank Martinez", "Tina Makovec",
                "Lisa Toftemark", "Karen Toh", "Peter Villadsen", "Anna Misiec", "Doris Krieger", "Pat Coleman", "Samantha Smith", "Roman Stachnio",
                "Oliver Lee", "Jacky Chan", "Daniela Guaita", "Nina Vietzen", "Ray Mohrman", "Eduardo Melo", "Jesse Merriam", "Pat Coleman"
            };

            var profilePictureUrl = new Uri(ProfilePicture.GetBlankProfilePictureUrl(true));

            int id = 1;
            foreach (var name in friendList)
            {
                data.Add(new GraphUser { Id = id.ToString(), Name = name, ProfilePictureUrl = profilePictureUrl });
                id++;
            }

            return data;
        }
    }
}
