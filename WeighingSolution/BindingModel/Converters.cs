using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WeighingSolution.Converters
{
    /// <summary>Visible when the bound object is not null (used to hide the editor until an integration is selected).</summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value != null ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// <summary>
    /// Two-way binds a string "enum" property to a RadioButton's IsChecked, comparing
    /// against ConverterParameter. Used for the Body-mode radio group.
    /// </summary>
    public class EnumToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value?.ToString() == parameter?.ToString();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => (value is bool b && b) ? parameter?.ToString() : Binding.DoNothing;
    }

    /// <summary>Visible only when BodyMode is one of the raw text modes.</summary>
    public class RawBodyVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string mode = value?.ToString();
            // inside RawBodyVisibilityConverter.Convert
            bool isRawLike = mode == "RawJson" || mode == "RawXml" || mode == "RawText";
            return isRawLike ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    public class EnumToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return Visibility.Collapsed;
            return value.ToString() == parameter.ToString() ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// <summary>Visible only when BodyMode is FormData or UrlEncoded (key/value grid modes).</summary>
    public class FieldsBodyVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string mode = value?.ToString();
            bool isFields = mode == "FormData" || mode == "UrlEncoded";
            return isFields ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
