using System;
using System.Globalization;
using System.Windows.Data;

namespace WeighingSolution.Converters
{
    public class NullReplaceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ?? parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return value.Equals(parameter) ? null : value;
            else if (value == null && parameter.ToString() == "")
                return null;
            else if (value == null && System.Convert.ToInt32(parameter) == 0)
                return 0;
            return value;
        }
    }
}
