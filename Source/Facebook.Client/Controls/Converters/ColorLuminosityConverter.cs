namespace Facebook.Client.Controls
{
    using System;

#if NETFX_CORE
    using System.Globalization;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Media;
#endif
#if WP8
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;
#endif

    /// <summary>
    /// Adjusts the color of a brush changing its luminosity by a given factor.
    /// </summary>
    public class ColorLuminosityConverter : IValueConverter
    {
#if NETFX_CORE
        /// <summary>
        /// Converts the color of the supplied brush changing its luminosity by the given factor.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The factor used to adjust the luminosity (0..1).</param>
        /// <param name="language">The culture to use in the converter (unused).</param>
        /// <returns>A converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
#endif
#if WP8
        /// <summary>
        /// Converts the color of the supplied brush changing its luminosity by the given factor.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The factor used to adjust the luminosity (0..1).</param>
        /// <param name="culture">The culture to use in the converter (unused).</param>
        /// <returns>A converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#endif
        {
            if (value == null) return null;
            if (parameter == null) return null;

            var factor = double.Parse(parameter.ToString(), CultureInfo.InvariantCulture);

            if (value is SolidColorBrush)
            {
                var color = ((SolidColorBrush)value).Color;
                var hlsColor = HlsColor.FromRgb(color);
                hlsColor.L *= factor;
                return new SolidColorBrush(hlsColor.ToRgb());
            }
            else if (value is LinearGradientBrush)
            {
                var gradientStops = new GradientStopCollection();
                foreach (var stop in ((LinearGradientBrush)value).GradientStops)
                {
                    var hlsColor = HlsColor.FromRgb(stop.Color);
                    hlsColor.L *= factor;
                    gradientStops.Add(new GradientStop() { Color = hlsColor.ToRgb(), Offset = stop.Offset });
                }

                var brush = new LinearGradientBrush(gradientStops, 0.0);
                return brush;
            }

            return value;
        }

#if NETFX_CORE
        /// <summary>
        /// Not implemented.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#endif
#if WP8
        /// <summary>
        /// Not implemented.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#endif
        {
            throw new System.NotImplementedException();
        }
    }
}
