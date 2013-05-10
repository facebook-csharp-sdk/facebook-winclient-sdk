namespace Facebook.Client.Controls
{
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Translates boolean values to Visibility constants.
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Set to true to convert true to Collapsed and false to Visible.
        /// </summary>
        public bool IsReversed { get; set; }

        /// <summary>
        /// Converts a boolean value to a Visibility constant.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">Unused.</param>
        /// <param name="language">The culture to use in the converter (unused).</param>
        /// <returns>A Vibility constant.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var state = System.Convert.ToBoolean(value);
            if (this.IsReversed)
            {
                state = !state;
            }

            return state ? Visibility.Visible : Visibility.Collapsed;
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
