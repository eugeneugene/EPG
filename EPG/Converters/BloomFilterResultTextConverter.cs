using System;
using EPG.Models;
using System.Globalization;
using System.Windows.Data;
using EPG.Code;

namespace EPG.Converters
{
    internal class BloomFilterResultTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BloomFilterResult result)
                return result.DisplayEnum() ?? string.Empty;
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
