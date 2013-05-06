namespace Facebook.Client.Controls
{
    /// <summary>
    /// Specifies the cropping treatment of the profile picture.
    /// </summary>
    public enum CropMode
    {
        /// <summary>
        /// Specifies the square version of the picture that the Facebook user defined.
        /// </summary>
        Square = 0,

        /// <summary>
        /// Specifies the original profile picture, as uploaded by the user.
        /// </summary>
        Original = 1,

        /// <summary>
        /// Specifies the picture is resized to fit the control's dimensions while preserving its native aspect ratio.
        /// </summary>
        Fill = 2
    }
}
