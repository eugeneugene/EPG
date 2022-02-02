using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EPG.Models
{
    public class PasswordResultModel : INotifyPropertyChanged
    {
        public PasswordResultModel()
        {
            DataCollection = new();
            DataCollection.CollectionChanged += DataCollectionChanged;
            mode = string.Empty;
            include = string.Empty;
            exclude = string.Empty;
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

        private string mode;
        public string Mode
        {
            get => mode;
            set
            {
                if (Mode != value)
                {
                    mode = value;
                    NotifyPropertyChanged(nameof(Mode));
                }
            }
        }

        private string include;
        public string Include
        {
            get => include;
            set
            {
                if (Include != value)
                {
                    include = value;
                    NotifyPropertyChanged(nameof(Include));
                }
            }
        }

        private string exclude;
        public string Exclude
        {
            get => exclude;
            set
            {
                if (Exclude != value)
                {
                    exclude = value;
                    NotifyPropertyChanged(nameof(Exclude));
                }
            }
        }

        public ObservableCollection<PasswordResultItem> DataCollection { get; }

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
