namespace Facebook.Client.Controls
{
#if NETFX_CORE
    using System;
    using Windows.UI.Xaml.Data;
#endif
#if WINDOWS_PHONE
    using System;
    using System.Globalization;
    using System.Windows.Data;
#endif

    /// <summary>
    /// Formats the Facebook user's name.
    /// </summary>
    public class UserDisplayNameConverter : IValueConverter
    {
#if NETFX_CORE 
        /// <summary>
        /// Returns the formatted name of the Facebook user as specified by the DisplayFormat parameter.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The display order.</param>
        /// <param name="language">The culture to use in the converter (unused).</param>
        /// <returns>The formatted name of the Facebook user.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
#endif
#if WINDOWS_PHONE
        /// <summary>
        /// Returns the formatted name of the Facebook user as specified by the DisplayFormat parameter.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The display order.</param>
        /// <param name="culture">The culture to use in the converter (unused).</param>
        /// <returns>The formatted name of the Facebook user.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#endif
        {
            var user = (GraphUser)value;
            var displayOrder = (FriendPickerDisplayOrder)parameter;
            return FriendPicker.FormatDisplayName(user, displayOrder);
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
