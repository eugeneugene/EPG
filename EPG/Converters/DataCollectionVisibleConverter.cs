using EPG.Models;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EPG.Converters
{
    internal class DataCollectionVisibleConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is ObservableCollection<PasswordResultItem> collection && values[1] is bool manualMode)
                return (manualMode || collection.Count > 0) ? Visibility.Visible : Visibility.Hidden;
            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
