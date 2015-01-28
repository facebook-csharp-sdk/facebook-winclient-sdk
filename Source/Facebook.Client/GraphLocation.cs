namespace Facebook.Client
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides a strongly-typed representation of a Facebook Location as defined by the Graph API.
    /// </summary>
    /// <remarks>
    /// The GraphLocation class represents the most commonly used properties of a Facebook Location object.
    /// </remarks>
    public class GraphLocation : GraphObject
    {
        /// <summary>
        /// Initializes a new instance of the GraphLocation class.
        /// </summary>
        public GraphLocation()
        {
        }

        /// <summary>
        /// Initializes a new instance of the GraphLocation class from a dynamic object returned by the Facebook API.
        /// </summary>
        /// <param name="location">The dynamic object representing the Facebook location.</param>
        public GraphLocation(dynamic location)
            : base((IDictionary<string, object>)location)
        {
            if (location == null)
            {
                throw new ArgumentNullException("location");
            }

            var tmpLocation = location as IDictionary<string, object>;
            this.Street = tmpLocation.ContainsKey("street") ? (string)tmpLocation["street"] : string.Empty; //location.street;
            this.City = tmpLocation.ContainsKey("name") ? (string)tmpLocation["name"] : string.Empty; //location.name;
            this.State = tmpLocation.ContainsKey("state") ? (string)tmpLocation["state"] : string.Empty; //location.state;
            this.Zip = tmpLocation.ContainsKey("zip") ? (string)tmpLocation["zip"] : string.Empty; //location.zip;
            this.Country = tmpLocation.ContainsKey("country") ? (string)tmpLocation["country"] : string.Empty; //location.country;
            this.Latitude = tmpLocation.ContainsKey("latitude") ? (double)tmpLocation["latitude"] : 0.0; //location.latitude ?? 0.0;
            this.Longitude = tmpLocation.ContainsKey("longitude") ? (double)tmpLocation["longitude"] : 0.0; //location.longitude ?? 0.0;
        }

        /// <summary>
        /// Gets or sets the street component of the location.
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Gets or sets the city component of the location.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the state component of the location.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the country component of the location.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the postal code component of the location.
        /// </summary>
        public string Zip { get; set; }

        /// <summary>
        /// Gets or sets the latitude component of the location.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude component of the location.
        /// </summary>
        public double Longitude { get; set; }
    }
}
