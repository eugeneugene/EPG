using System;
using System.Globalization;
using System.Windows.Data;

namespace EPG.Converters
{
    internal class PasswordComplexityPercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal dec)
            {
                if (dec > 128m)
                    dec = 128m;
                return string.Format(CultureInfo.CurrentCulture, "{0:P1}", dec / 128m);
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
