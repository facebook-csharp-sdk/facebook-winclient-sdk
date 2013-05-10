namespace Facebook.Client.Controls
{
    using System;
    using System.Collections;
    using System.Linq;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;

    public class SizeToGridCellConverter : IValueConverter
    {
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

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
