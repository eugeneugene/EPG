using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EPG.Models
{
    public class ResultModel : INotifyPropertyChanged
    {
        public ResultModel()
        {
            DataCollection = new();
            DataCollection.CollectionChanged += DataCollectionChanged;
        }

        private bool showHyphenated;
        public bool ShowHyphenated
        {
            get => showHyphenated;
            set
            {
                if (showHyphenated != value)
                {
                    showHyphenated = value;
                    NotifyPropertyChanged(nameof(ShowHyphenated));
                }
            }
        }

        private bool enableBloom;
        public bool EnableBloom
        {
            get => enableBloom;
            set
            {
                if (enableBloom != value)
                {
                    enableBloom = value;
                    NotifyPropertyChanged(nameof(EnableBloom));
                }
            }
        }

        private bool calculateQuality;
        public bool CalculateQuality
        {
            get => calculateQuality;
            set
            {
                if (calculateQuality != value)
                {
                    calculateQuality = value;
                    NotifyPropertyChanged(nameof(CalculateQuality));
                }
            }
        }

        public ObservableCollection<DataItem> DataCollection { get; }

        private void DataCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(DataCollection));
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
