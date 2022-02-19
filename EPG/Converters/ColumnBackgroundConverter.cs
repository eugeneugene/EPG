using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace EPG.Converters
{
    internal class ColumnBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool manualMode)
            {
                if (manualMode)
                    return new SolidColorBrush(Color.FromRgb(127, 127, 127));
                else
                    return new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
