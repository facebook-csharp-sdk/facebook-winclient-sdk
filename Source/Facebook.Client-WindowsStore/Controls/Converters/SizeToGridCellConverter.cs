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
            var count = data.Cast<object>().Count();
            var cells = Math.Ceiling(Math.Sqrt(count));
            if (bool.Parse((string) parameter))
            {
                return ((control.Width - 0) / cells) - 10;
            }
            else
            {
                return ((control.Height - 0) / cells) - 10;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
