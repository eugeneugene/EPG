using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace BFM.Converters
{
    internal class CheckWordResultImageConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return null;
            if (value is bool result)
            {
                var img = new BitmapImage();
                img.BeginInit();
                img.UriSource = result switch
                {
                    false => new Uri("pack://application:,,,/BFM;component/Resources/ok.png", UriKind.Absolute),
                    true => new Uri("pack://application:,,,/BFM;component/Resources/alert.png", UriKind.Absolute),
                };
                img.EndInit();
                return img;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
