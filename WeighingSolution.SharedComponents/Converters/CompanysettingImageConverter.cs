using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WeighingSolution.Converters
{
    [ValueConversion(typeof(string), typeof(ImageSource))]
    public class CompanysettingImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value.GetType() != typeof(string) || targetType != typeof(ImageSource)) return false;

            string filePath = "";
            //Default Image
            if (value.ToString().ToLower() == "logo.png")
            {
                filePath = "pack://application:,,,/" + Assembly.GetExecutingAssembly().GetName().Name + ";component/Images/logo.png";
            }
            //DB Relative path image <Guid>.jpb
            else if (!Path.IsPathRooted(value.ToString()))
            {
                filePath = WeighingSolution.DataLayer.Constant.CustomImagesPath + @"\Logos\" + value.ToString();
                if (!File.Exists(filePath)) return DependencyProperty.UnsetValue;
            }
            //Newly selected image, eg. "C:\Pictures\MyImage.jpg"
            else
            {
                filePath = value as string;
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return DependencyProperty.UnsetValue;
            }

            if (string.IsNullOrEmpty(filePath)) return DependencyProperty.UnsetValue;

            //This is to ensure, file is not locked by the application.
            BitmapImage image = new BitmapImage();
            try
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                image.UriSource = new Uri(filePath, UriKind.RelativeOrAbsolute);
                image.EndInit();
            }
            catch { return DependencyProperty.UnsetValue; }
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
