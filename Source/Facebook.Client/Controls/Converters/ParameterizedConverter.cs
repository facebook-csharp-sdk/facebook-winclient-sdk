namespace Facebook.Client.Controls
{
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Wraps a value converter allowing the converter parameters to be bound.
    /// </summary>
    public class ParameterizedConverter : DependencyObject, IValueConverter
    {
        /// <summary>
        /// Delegates to a wrapped value converter providing it with the bound converter parameter.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">Unused.</param>
        /// <param name="language">The culture to use in the converter (unused).</param>
        /// <returns>The value returned by the wrapped value converter.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return this.Converter.Convert(value, targetType, this.ConverterParameter, language);
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return this.Converter.ConvertBack(value, targetType, this.ConverterParameter, language);
        }

        #region Converter

        /// <summary>
        /// Gets or sets the converter to delegate to.
        /// </summary>
        public IValueConverter Converter
        {
            get { return (IValueConverter)GetValue(ConverterProperty); }
            set { this.SetValue(ConverterProperty, value); }
        }

        /// <summary>
        /// Identifies the Converter dependency property.
        /// </summary>
        public static readonly DependencyProperty ConverterProperty =
            DependencyProperty.Register("Converter", typeof(IValueConverter), typeof(ParameterizedConverter), new PropertyMetadata(null));

        #endregion Converter

        #region ConverterParameter

        /// <summary>
        /// Gets or sets the parameter to pass through to the inner value converter.
        /// </summary>
        public object ConverterParameter
        {
            get { return (object)GetValue(ConverterParameterProperty); }
            set { this.SetValue(ConverterParameterProperty, value); }
        }

        /// <summary>
        /// Identifies the ConverterParameter dependency property.
        /// </summary>
        public static readonly DependencyProperty ConverterParameterProperty =
            DependencyProperty.Register("ConverterParameter", typeof(object), typeof(ParameterizedConverter), new PropertyMetadata(null));

        #endregion ConverterParameter
    }
}
