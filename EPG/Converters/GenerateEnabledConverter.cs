using EPG.Models;
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
            if (values[3] is not PasswordMode passwordMode)
                return false;

            bool SmallSymbols = (values[4] as bool?) ?? false;
            bool CapitalSymbols = (values[5] as bool?) ?? false;
            bool Numerals = (values[6] as bool?) ?? false;
            bool SpecialSymbols = (values[7] as bool?) ?? false;

            if (passwordMode == PasswordMode.Random)
            {
                if (!SmallSymbols && !CapitalSymbols && !Numerals && !SpecialSymbols)
                    return false;
            }
            if (passwordMode== PasswordMode.Pronounceable)
            {
                if (!SmallSymbols && !CapitalSymbols)
                    return false;
            }

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
