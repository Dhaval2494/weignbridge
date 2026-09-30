using System;
using System.Globalization;
using System.Windows.Data;

namespace WeighingSolution.Converters
{
    public class BooleanAndConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var v in values)
            {
                if (v is bool b && !b)
                    return false;
                if (!(v is bool))
                    return false; // treat unset/null as false-safe
            }
            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}