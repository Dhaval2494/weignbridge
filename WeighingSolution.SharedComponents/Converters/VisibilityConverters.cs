using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WeighingSolution.Converters
{
    public class BooleanConverter<T> : IValueConverter
    {
        public BooleanConverter(T trueValue, T falseValue)
        {
            True = trueValue;
            False = falseValue;
        }

        public T True { get; set; }
        public T False { get; set; }

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool && ((bool)value) ? True : False;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is T && EqualityComparer<T>.Default.Equals((T)value, True);
        }
    }

    public class BoolVisibilityConverter : BooleanConverter<Visibility>
    {
        public BoolVisibilityConverter() :
            base(Visibility.Visible, Visibility.Collapsed)
        { }
    }

    public class IntegerIndexToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                int CurrentIndex = System.Convert.ToInt32(parameter);
                int MaxValue = System.Convert.ToInt32(value);

                return CurrentIndex <= MaxValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class ListToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            try
            {
                IEnumerable enumerable = (IEnumerable)value;
                bool EnumerableEmpty = true;
                foreach (var item in enumerable)
                {
                    EnumerableEmpty = false;
                    break;
                }
                return EnumerableEmpty ? Visibility.Collapsed : Visibility.Visible;
            }
            catch (Exception)
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class WeightVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter.ToString() == "Visible")
            {
                if (value == null || String.IsNullOrEmpty(value.ToString()))
                    return Visibility.Visible;
                Decimal dummy;
                if (Decimal.TryParse(value.ToString(), out dummy) && dummy > 0)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
            else
            {
                if (value == null || String.IsNullOrEmpty(value.ToString()))
                    return Visibility.Collapsed;
                Decimal dummy;
                if (Decimal.TryParse(value.ToString(), out dummy) && dummy > 0)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class EqualityVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (parameter.GetType() == typeof(string))
                {
                    if (parameter.ToString().Contains("|"))
                    {
                        string[] arr = parameter.ToString().Split('|');
                        bool hasMatch = false;
                        for (int i = 0; i < arr.Length; i++)
                        {
                            if (value.Equals(arr[i]))
                            {
                                hasMatch = true;
                                break;
                            }
                        }
                        return hasMatch ? Visibility.Visible : Visibility.Collapsed;
                    }
                    else
                        return value.Equals(parameter) ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    return value.Equals(parameter) ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class InEqualityVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (parameter.GetType() == typeof(string))
                {
                    if (parameter.ToString().Contains("|"))
                    {
                        string[] arr = parameter.ToString().Split('|');
                        bool hasMatch = false;
                        for (int i = 0; i < arr.Length; i++)
                        {
                            if (value.Equals(arr[i]))
                            {
                                hasMatch = true;
                                break;
                            }
                        }
                        return !hasMatch ? Visibility.Visible : Visibility.Collapsed;
                    }
                    else
                        return !value.Equals(parameter) ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    return !value.Equals(parameter) ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
