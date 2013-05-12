namespace Facebook.Client.Controls
{
    using System;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Formats the Facebook user's name.
    /// </summary>
    public class UserDisplayNameConverter : IValueConverter
    {
        /// <summary>
        /// Returns the formatted name of the Facebook user as specified by the DisplayFormat parameter.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">Unused.</param>
        /// <param name="language">The culture to use in the converter (unused).</param>
        /// <returns>The formatted name of the Facebook user.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var user = (GraphUser)value;
            var displayOrder = (FriendPickerDisplayOrder)parameter;
            return FriendPicker.FormatDisplayName(user, displayOrder);
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
