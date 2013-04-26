using System;

#if NETFX_CORE
using Windows.UI.Xaml.Data;
#endif
#if WINDOWS_PHONE
using System.Windows.Data;
using System.Globalization;
#endif

namespace Facebook.Client.Controls
{
    /// <summary>
    /// Scales a value by multiplying by a given factor.
    /// </summary>
    public class ScaleConverter : IValueConverter
    {
#if NETFX_CORE
        /// <summary>
        /// Scales the value by multiplying by a given factor.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The factor used to scale the value.</param>
        /// <param name="language">The culture to use in the converter (unused).</param>
        /// <returns>A converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
#endif
#if WINDOWS_PHONE
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
            return System.Convert.ToDouble(value) * System.Convert.ToDouble(parameter);
        }

#if NETFX_CORE
        /// <summary>
        /// Not implemented.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#endif
#if WINDOWS_PHONE
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
