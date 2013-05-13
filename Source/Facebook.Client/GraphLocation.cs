namespace Facebook.Client
{
    using System;

    /// <summary>
    /// Provides a strongly-typed representation of a Facebook Location as defined by the Graph API.
    /// </summary>
    /// <remarks>
    /// The GraphLocation class represents the most commonly used properties of a Facebook Location object.
    /// </remarks>
    public class GraphLocation
    {
        /// <summary>
        /// Initializes a new instance of the GraphLocation class.
        /// </summary>
        public GraphLocation()
        {
        }

        /// <summary>
        /// Initializes a new instance of the GraphLocation class  from a dynamic object returned by the Facebook API.
        /// </summary>
        /// <param name="location">The dynamic object representing the Facebook location.</param>
        public GraphLocation(dynamic location)
        {
            if (location == null)
            {
                throw new ArgumentNullException("location");
            }

            this.Street = location.street;
            this.City = location.name;
            this.State = location.state;
            this.Zip = location.zip;
            this.Country = location.country;
            this.Latitude = location.latitude ?? 0.0;
            this.Longitude = location.longitude ?? 0.0;
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
