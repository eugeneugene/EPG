using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EPG.Converters
{
    internal class GenerateEnabledConverter : IMultiValueConverter
    {
        public object Convert(object?[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue ||
                values[1] == DependencyProperty.UnsetValue ||
                values[2] == DependencyProperty.UnsetValue)
                return false;

            uint NumberOfPasswords = System.Convert.ToUInt32(values[0] ?? 0U);
            uint MinimumLength = System.Convert.ToUInt32(values[1] ?? 0U);
            uint MaximumLength = System.Convert.ToUInt32(values[2] ?? 0U);

            if (NumberOfPasswords == 0)
                return false;
            if (MinimumLength == 0 || MaximumLength == 0)
                return false;

            return MaximumLength >= MinimumLength;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
