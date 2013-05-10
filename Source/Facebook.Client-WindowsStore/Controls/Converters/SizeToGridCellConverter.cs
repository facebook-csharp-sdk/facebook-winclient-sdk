namespace Facebook.Client.Controls
{
    using System;
    using System.Collections;
    using System.Linq;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Calculates the size of group templates used by the zoomed out view to arrange them evenly.
    /// </summary>
    public class SizeToGridCellConverter : IValueConverter
    {
        /// <summary>
        /// Calculates the width and height of the group templates based on the size of the control.
        /// </summary>
        /// <param name="value">A reference to the control hosting the group templates.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">Unused.</param>
        /// <param name="language">The culture to use in the converter (unused).</param>
        /// <returns>.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var control = (Control)value;
            var data = (IEnumerable) ((CollectionViewSource) control.DataContext).Source;
            var totalCells = data.Cast<object>().Count();
            var numRows = Math.Round(Math.Sqrt(totalCells * control.Height / control.Width));
            var numColumns = Math.Round(totalCells / numRows);
            if (numRows * numColumns < totalCells)
            {
                numRows++;
            }

            if (bool.Parse((string) parameter))
            {
                return ((int) (control.Width / numColumns)) - 12;
            }
            else
            {
                return ((int)(control.Height / numRows)) - 12;
            }
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
