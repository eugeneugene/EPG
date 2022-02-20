using System;
using EPG.Models;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace EPG.Converters
{
    internal class BloomFilterResultImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                var img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri("pack://application:,,,/EPG;component/Resources/question.png", UriKind.Absolute);
                img.EndInit();
                return img;
            }
            if (value is BloomFilterResult result)
            {
                var img = new BitmapImage();
                img.BeginInit();
                img.UriSource = result switch
                {
                    BloomFilterResult.NOTFOUND => new Uri("pack://application:,,,/EPG;component/Resources/ok.png", UriKind.Absolute),
                    BloomFilterResult.NOTSAFE => new Uri("pack://application:,,,/EPG;component/Resources/warning.png", UriKind.Absolute),
                    BloomFilterResult.FOUND => new Uri("pack://application:,,,/EPG;component/Resources/alert.png", UriKind.Absolute),
                    _ => new Uri("pack://application:,,,/EPG;component/Resources/question.png", UriKind.Absolute),
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
