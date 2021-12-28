using BFM.Code;
using System;
using System.Globalization;
using System.Windows.Data;

namespace BFM.Converters
{
    internal class ImportEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LinesCounterState state)
                return state == LinesCounterState.FINISH;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
