using System;
using System.Globalization;
using System.Windows.Data;

namespace EPG.Converters
{
    internal class DoublePercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double)value) * 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return double.Parse((string)value) / 100;
        }

        public static DoublePercentConverter Instance { get; } = new DoublePercentConverter();
    }
}
