using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace EPG.Converters
{
    internal class BloomFilterResultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                //using var memoryStream = new MemoryStream();
                //Resources.IconResources.NotChecked16.Save(memoryStream);
                var img = new BitmapImage();
                img.BeginInit();
                //img.StreamSource = memoryStream;
                img.UriSource = new Uri("pack://application:,,,/EPG;component/Resources/NotChecked16.png", UriKind.Absolute);
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
