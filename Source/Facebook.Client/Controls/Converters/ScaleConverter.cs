namespace Facebook.Client.Controls
{
#if NETFX_CORE
    using System;
    using System.Globalization;
    using Windows.UI.Xaml.Data;
#endif
#if WP8
    using System;
    using System.Globalization;
    using System.Windows.Data;
#endif

    /// <summary>
    /// Scales a value by a given factor.
    /// </summary>
    public class ScaleConverter : IValueConverter
    {
#if NETFX_CORE
        /// <summary>
        /// Scales the value by a given factor.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The factor used to scale the value.</param>
        /// <param name="language">The culture to use in the converter (unused).</param>
        /// <returns>A converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
#endif
#if WP8
        /// <summary>
        /// Scales the value by multiplying by a given factor.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The factor used to scale the value.</param>
        /// <param name="culture">The culture to use in the converter (unused).</param>
        /// <returns>A converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#endif
        {
            return System.Convert.ToDouble(value, CultureInfo.InvariantCulture) * System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
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
