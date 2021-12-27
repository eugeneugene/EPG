using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BFM.Models
{
    internal class MainWindowModel : INotifyPropertyChanged
    {
        private MainWindowBloomFilterModel? mainWindowBloomFilterModel;
        public MainWindowBloomFilterModel? MainWindowBloomFilterModel
        {
            get => mainWindowBloomFilterModel;
            set
            {
                if (mainWindowBloomFilterModel != value)
                {
                    mainWindowBloomFilterModel = value;
                    NotifyPropertyChanged(nameof(MainWindowBloomFilterModel));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName))
                return;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
