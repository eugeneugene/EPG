using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EPG.Models
{
    public class PasswordResultModel : INotifyPropertyChanged
    {
        private string mode;
        private bool showHyphenated;
        private bool calculateComplexity;
        private string include;
        private string exclude;
        private bool manualMode;

        public PasswordResultModel()
        {
            DataCollection = new();
            DataCollection.CollectionChanged += DataCollectionChanged;
            mode = string.Empty;
            include = string.Empty;
            exclude = string.Empty;
            manualMode = false;
        }

        public PasswordResultModel(IEnumerable<PasswordResultItem> collection, string mode, bool showHyphenated, bool calculateComplexity, string include, string exclude)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));

            if (include is null)
                throw new ArgumentNullException(nameof(include));

            if (exclude is null)
                throw new ArgumentNullException(nameof(exclude));

            if (string.IsNullOrEmpty(mode))
                throw new ArgumentException($"'{nameof(mode)}' cannot be null or empty.", nameof(mode));

            DataCollection = new(collection);
            DataCollection.CollectionChanged += DataCollectionChanged;
            this.mode = mode;
            this.showHyphenated = showHyphenated;
            this.calculateComplexity = calculateComplexity;
            this.include = include;
            this.exclude = exclude;
        }

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

        public bool CalculateComplexity
        {
            get => calculateComplexity;
            set
            {
                if (calculateComplexity != value)
                {
                    calculateComplexity = value;
                    NotifyPropertyChanged(nameof(CalculateComplexity));
                }
            }
        }

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

        public bool ManualMode
        {
            get => manualMode;
            set
            {
                if (manualMode != value)
                {
                    manualMode = value;
                    NotifyPropertyChanged(nameof(ManualMode));
                }
            }
        }

        public ObservableCollection<PasswordResultItem> DataCollection { get; set; }

        private void DataCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
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
