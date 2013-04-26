using System;

#if NETFX_CORE
using Windows.UI.Xaml.Data;
using System.Globalization;
#endif
#if WINDOWS_PHONE
using System.Windows.Data;
using System.Globalization;
#endif

namespace Facebook.Client.Controls
{
    /// <summary>
    /// Checks if a value is null or empty.
    /// </summary>
    public class IsNullOrEmptyConverter : IValueConverter
    {
#if NETFX_CORE
        /// <summary>
        /// Checks if a value is null or empty.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">Set to True to indicate that the return value should be negated.</param>
        /// <param name="language">The culture to use in the converter (unused).</param>
        /// <returns>True if the value is null or empty.</returns>
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
            bool isNegated;
            bool.TryParse((string) parameter, out isNegated);

            bool result;
            if (value is string)
            {
                result = string.IsNullOrEmpty((string)value);
            }
            else
            {
                result = value != null;
            }

            return isNegated ? !result : result;
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
