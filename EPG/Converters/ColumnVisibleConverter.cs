using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EPG.Converters
{
    internal class ColumnVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool val)
                return val ? Visibility.Visible : Visibility.Hidden;
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
