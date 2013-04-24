using System;
using Windows.UI.Xaml.Data;

namespace Facebook.Client.Controls
{
    /// <summary>
    /// Scales a value by multiplying by a given factor.
    /// </summary>
    internal class ScaleConverter : IValueConverter
    {
        /// <summary>
        /// Scales the value by multiplying by a given factor.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The factor used to scale the value.</param>
        /// <param name="language">The culture to use in the converter (unused).</param>
        /// <returns>A converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToDouble(value) * System.Convert.ToDouble(parameter);
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new System.NotImplementedException();
        }
    }
}
