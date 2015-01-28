namespace Facebook.Client.Controls
{
#if NETFX_CORE
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
#endif
#if WP8
    using System;
    using System.Collections.Generic;
    using System.Globalization;
#endif

    /// <summary>
    /// Provides design time support for the FriendPicker control.
    /// </summary>
    internal class PlacePickerDesignSupport
    {
        private static List<GraphPlace> data;

        public static IEnumerable<GraphPlace> DesignData
        {
            get
            {
                if (PlacePickerDesignSupport.data == null)
                {
                    PlacePickerDesignSupport.data = LoadDesignData();
                }

                return PlacePickerDesignSupport.data;
            }
        }

        private static List<GraphPlace> LoadDesignData()
        {
            const string BlankProfilePictureSquare = "fb_blank_place_profile_square.png";

#if NETFX_CORE
            var libraryName = typeof(PlacePicker).GetTypeInfo().Assembly.GetName().Name;
            var blankProfilePictureUrl = string.Format(CultureInfo.InvariantCulture, "ms-appx:///{0}/Images/{1}", libraryName, BlankProfilePictureSquare);
#endif
#if WP8
            var blankProfilePictureUrl = string.Format(CultureInfo.InvariantCulture, "/Images/{0}", BlankProfilePictureSquare);
#endif

            var data = new List<GraphPlace>();

            var profilePictureUrl = new Uri(blankProfilePictureUrl, UriKind.RelativeOrAbsolute);

            data.Add(new GraphPlace { Id = string.Empty, Name = "Contoso Bank", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphPlace { Id = string.Empty, Name = "National Museum of Art", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphPlace { Id = string.Empty, Name = "The Pet Shop", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphPlace { Id = string.Empty, Name = "Ten Forks Restaurant", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphPlace { Id = string.Empty, Name = "Adventure Works Store", ProfilePictureUrl = profilePictureUrl });
            data.Add(new GraphPlace { Id = string.Empty, Name = "After Office Pub", ProfilePictureUrl = profilePictureUrl });
             
            return data;
        }
    }
}
