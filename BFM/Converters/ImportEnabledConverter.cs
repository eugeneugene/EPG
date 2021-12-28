using BFM.Code;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BFM.Converters
{
    internal class ImportEnabledConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var state = values[0] as LinesCounterState?;
            var bloomFilter = values[1] as string;
            var importTask = values[2] as Task;
            return state is not null && state == LinesCounterState.FINISH && !string.IsNullOrEmpty(bloomFilter) && importTask is null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
