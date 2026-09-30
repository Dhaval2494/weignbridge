using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WeighingSolution.DataLayer
{
    [ValueConversion(typeof(string), typeof(ImageSource))]
    public class CameraImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                value = "/Images/no-img.png";

            if (value.GetType() != typeof(string) || targetType != typeof(ImageSource)) return false;

            string filePath = "";
            //Default Image
            if (value.ToString().ToLower() == "/images/default-image.png")
            {
                filePath = "pack://application:,,,/" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ";component/Images/no-img.png";
            }
            //DB Relative path image <Guid>.jpb
            else if (!Path.IsPathRooted(value.ToString()))
            {
                filePath = CameraConstant.CameraImageLocation + "\\" + value.ToString();
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


    public class Base64ToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null) return null;

                byte[] imageBytes = System.Convert.FromBase64String(value.ToString());

                using (var ms = new MemoryStream(imageBytes))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    image.Freeze();
                    return image;
                }
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
