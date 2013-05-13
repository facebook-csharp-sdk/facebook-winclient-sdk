namespace Facebook.Client.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Data;

    /// <summary>
    /// Retrieves a BitmapImage object for the specified image source uri.
    /// </summary>
    public class ImageSourceUriConverter : IValueConverter
    {
        /// <summary>
        /// Retrieves a BitmapImage object for the specified image source uri.
        /// </summary>
        /// <remarks>
        /// If the specified source uri is a relative path, the image is loaded as an embedded resource stream.
        /// </remarks>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">Not required.</param>
        /// <param name="culture">The culture to use in the converter (unused).</param>
        /// <returns>A converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Uri sourceUri;

            if (value is string)
            {
                var sourceUrl = (string)value;
                if (!Uri.TryCreate(sourceUrl, UriKind.RelativeOrAbsolute, out sourceUri))
                {
                    return value;
                }
            }
            else if (value is Uri)
            {
                sourceUri = value as Uri;
            }
            else
            {
                return value;
            }

            var bmp = new System.Windows.Media.Imaging.BitmapImage();

            if (sourceUri.IsAbsoluteUri)
            {
                bmp.UriSource = sourceUri;
                return bmp;
            }
            else
            {
                var library = typeof(ImageSourceUriConverter).Assembly;
                var libraryName = library.GetName().Name;
                var resourceName = string.Format(CultureInfo.InvariantCulture, "{0}{1}", libraryName, sourceUri.ToString().Replace("/", "."));

                using (var stream = library.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        bmp.SetSource(stream);
                        return bmp;
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
