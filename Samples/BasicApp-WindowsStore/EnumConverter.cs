namespace BasicApp
{
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;

    public class CropModeToBooleanConverter : EnumToBooleanConverter<Facebook.Client.Controls.CropMode>
    {
    }

    public class EnumToBooleanConverter<T> : IValueConverter
        where T : struct
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Type enumType = typeof(T);
            string s = Enum.GetName(enumType, value);
            object b = Enum.Parse(enumType, parameter.ToString());
            return b.ToString() == s;

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {         
            Type enumType = typeof(T);
            return Enum.Parse(enumType, parameter.ToString());
        }
    }
}

