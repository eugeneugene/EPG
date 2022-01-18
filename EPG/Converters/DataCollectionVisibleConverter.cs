using EPG.Models;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace EPG.Converters
{
    internal class DataCollectionVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<DataItem> collection)
                return collection.Count > 0;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
