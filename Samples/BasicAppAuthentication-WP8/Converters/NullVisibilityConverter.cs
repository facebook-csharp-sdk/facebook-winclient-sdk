// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullVisibilityConverter.cs" company="Microsoft">
//   2013
// </copyright>
// <summary>
//   Defines the NullVisibilityConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BasicAppAuthentication.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Visibility converter for when an object is null
    /// </summary>
    public class NullVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="NullVisibilityConverter"/> is inverted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if inverted; otherwise, <c>false</c>.
        /// </value>
        public bool Inverted { get; set; }

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (this.Inverted)
            {
                return value == null ? Visibility.Visible : Visibility.Collapsed;
            }

            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay" /> bindings.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <exception cref="System.NotImplementedException">Not implemented</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
