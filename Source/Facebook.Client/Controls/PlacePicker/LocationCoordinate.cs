namespace Facebook.Client.Controls
{
    /// <summary>
    /// Represents the coordinates of a place.
    /// </summary>
    public class LocationCoordinate
    {
        /// <summary>
        /// Initializes a new instance of the LocationCoordinate class.
        /// </summary>
        /// <param name="latitude">The latitude portion of the coordinate.</param>
        /// <param name="longitude">The longitude portion of the coordinate.</param>
        public LocationCoordinate(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        /// <summary>
        /// Gets the latitude portion of the coordinate.
        /// </summary>
        public double Latitude { get; private set; }

        /// <summary>
        /// Gets the longitude portion of the coordinate.
        /// </summary>
        public double Longitude { get; private set; }

        public override string ToString()
        {
            return this.Latitude + "," + this.Longitude;
        }
    }
}
