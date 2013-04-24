using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Facebook.Client.Controls
{
    /// <summary>
    /// Adjusts the color of a SolidColorBrush changing its luminosity by a given factor.
    /// </summary>
    internal class ColorLuminosityConverter : IValueConverter
    {
        /// <summary>
        /// Converts the color of the supplied brush changing its luminosity by the given factor.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The factor used to adjust the luminosity (0..1).</param>
        /// <param name="language">The culture to use in the converter (unused).</param>
        /// <returns>A converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            if (parameter == null) return null;

            var brush = (SolidColorBrush)value;

            var hlsColor = HlsColor.FromRgb(brush.Color);
            var factor = Double.Parse((parameter.ToString()));
            hlsColor.L *= factor;

            return new SolidColorBrush(hlsColor.ToRgb());
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
