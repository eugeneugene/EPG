using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BFM.Models
{
    internal class MainWindowBloomFilterModel : INotifyPropertyChanged
    {
        public MainWindowBloomFilterModel(bool validBloomFilter, string? bloomFilterStatus, string? bloomFilterPath, string? bloomFilterFile, ushort? headerVersion, ulong? headerSize, byte? headerHashFunc)
        {
            this.validBloomFilter = validBloomFilter;
            this.bloomFilterStatus = bloomFilterStatus;
            this.bloomFilterPath = bloomFilterPath;
            this.bloomFilterFile = bloomFilterFile;
            this.headerVersion = headerVersion;
            this.headerSize = headerSize;
            this.headerHashFunc = headerHashFunc;
        }

        public MainWindowBloomFilterModel()
        { }

        private string? bloomFilterStatus;
        public string? BloomFilterStatus
        {
            get => bloomFilterStatus;
            set
            {
                if (bloomFilterStatus != value)
                {
                    bloomFilterStatus = value;
                    NotifyPropertyChanged(nameof(BloomFilterStatus));
                }
            }
        }

        private string? bloomFilterPath;
        public string? BloomFilterPath
        {
            get => bloomFilterPath;
            set
            {
                if (bloomFilterPath != value)
                {
                    bloomFilterPath = value;
                    NotifyPropertyChanged(nameof(BloomFilterPath));
                }
            }
        }

        private string? bloomFilterFile;
        public string? BloomFilterFile
        {
            get => bloomFilterFile;
            set
            {
                if (bloomFilterFile != value)
                {
                    bloomFilterFile = value;
                    NotifyPropertyChanged(nameof(BloomFilterFile));
                }
            }
        }

        private bool validBloomFilter;
        public bool ValidBloomFilter
        {
            get => validBloomFilter;
            set
            {
                if (validBloomFilter != value)
                {
                    validBloomFilter = value;
                    NotifyPropertyChanged(nameof(ValidBloomFilter));
                }
            }
        }

        private ushort? headerVersion;
        public ushort? HeaderVersion
        {
            get => headerVersion;
            set
            {
                if (headerVersion != value)
                {
                    headerVersion = value;
                    NotifyPropertyChanged(nameof(HeaderVersion));
                }
            }
        }

        private ulong? headerSize;
        public ulong? HeaderSize
        {
            get => headerSize;
            set
            {
                if (headerSize != value)
                {
                    headerSize = value;
                    NotifyPropertyChanged(nameof(HeaderSize));
                }
            }
        }

        private byte? headerHashFunc;

        public byte? HeaderHashFunc
        {
            get => headerHashFunc;
            set
            {
                if (headerHashFunc != value)
                {
                    headerHashFunc = value;
                    NotifyPropertyChanged(nameof(HeaderHashFunc));
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
