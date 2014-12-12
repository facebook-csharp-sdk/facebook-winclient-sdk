namespace Facebook.Client.Controls
{
    using System;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Converts between PickerSelectionMode and ListViewSelectionMode values.
    /// </summary>
    public class PickerSelectionModeConverter : IValueConverter
    {
        /// <summary>
        /// Converts a PickerSelectionMode value to a ListViewSelectionMode value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">Unused.</param>
        /// <param name="language">The culture to use in the converter (unused).</param>
        /// <returns>A converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var pickerSelectionMode = (PickerSelectionMode)value;
            return (ListViewSelectionMode)pickerSelectionMode;
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
