using EPG.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace EPG.Converters
{
    internal class ShowHyphenatedEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return false;

            return value.Equals(PasswordMode.Pronounceable);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
           throw new NotImplementedException();
        }
    }
}
