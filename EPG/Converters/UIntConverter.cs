using System;
using System.Globalization;
using System.Windows.Data;

namespace EPG.Converters
{
    internal class UIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
           return value?.ToString() ?? "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && uint.TryParse(str, out uint result))
                return result;
            return 0U;
        }
    }
}
