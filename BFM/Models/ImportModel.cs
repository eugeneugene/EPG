using BFM.Code;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace BFM.Models
{
    internal class ImportModel : INotifyPropertyChanged, IDisposable
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
        private bool disposedValue;

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

        private readonly SimpleReaderWriterLock<uint?> lines = new(null);
        public uint? Lines
        {
            get => lines.Value;
            set
            {
                if (lines.Value != value)
                {
                    lines.Value = value;
                    NotifyPropertyChanged(nameof(Lines));
                }
            }
        }

        private readonly SimpleReaderWriterLock<LinesCounterState> state = new(LinesCounterState.NOTRUN);
        public LinesCounterState State
        {
            get => state.Value;
            set
            {
                if (state.Value != value)
                {
                    state.Value = value;
                    NotifyPropertyChanged(nameof(State));
                    Debug.WriteLine("LinesCounterState: {0}", value);
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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    lines.Dispose();
                    state.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
