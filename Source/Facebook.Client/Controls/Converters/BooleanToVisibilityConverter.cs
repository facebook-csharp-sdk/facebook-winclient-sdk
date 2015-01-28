namespace Facebook.Client.Controls
{
#if NETFX_CORE
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;
#endif
#if WP8
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
#endif

    /// <summary>
    /// Translates boolean values to Visibility constants.
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Set to true to convert true to Collapsed and false to Visible.
        /// </summary>
        public bool IsReversed { get; set; }

#if NETFX_CORE
        /// <summary>
        /// Converts a boolean value to a Visibility constant.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">Unused.</param>
        /// <param name="language">The culture to use in the converter (unused).</param>
        /// <returns>A Visibility constant.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
#endif
#if WP8
        /// <summary>
        /// Converts a boolean value to a Visibility constant.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">Unused.</param>
        /// <param name="culture">The culture to use in the converter (unused).</param>
        /// <returns>A Visibility constant.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#endif
        {
            var state = System.Convert.ToBoolean(value);
            if (this.IsReversed)
            {
                state = !state;
            }

            return state ? Visibility.Visible : Visibility.Collapsed;
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
            throw new NotImplementedException();
        }
    }
}
