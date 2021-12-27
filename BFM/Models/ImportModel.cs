using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BFM.Models
{
    internal class ImportModel : INotifyPropertyChanged
    {
        private string? bloomFilter;
        public string? BloomFilter
        {
            get => bloomFilter;
            set
            {
                if (bloomFilter != value)
                {
                    bloomFilter = value;
                    NotifyPropertyChanged(nameof(BloomFilter));
                }
            }
        }

        private string? textFile;
        public string? TextFile
        {
            get => textFile;
            set
            {
                if (textFile != value)
                {
                    textFile = value;
                    NotifyPropertyChanged(nameof(TextFile));
                }
            }
        }

        private string? comments;
        public string? Comments
        {
            get => comments;
            set
            {
                if (comments != value)
                {
                    comments = value;
                    NotifyPropertyChanged(nameof(Comments));
                }
            }
        }

        private string? lines;
        public string? Lines
        {
            get => lines;
            set
            {
                if (lines != value)
                {
                    lines = value;
                    NotifyPropertyChanged(nameof(Lines));
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
