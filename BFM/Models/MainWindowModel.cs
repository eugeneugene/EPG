using CSAdapter;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace BFM.Models
{
    internal class MainWindowModel : INotifyPropertyChanged
    {
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

        private string? checkWord;
        public string? CheckWord
        {
            get => checkWord;
            set
            {
                if (checkWord != value)
                {
                    checkWord = value;
                    NotifyPropertyChanged(nameof(CheckWord));

                    if (validBloomFilter)
                    {
                        if (string.IsNullOrEmpty(checkWord))
                            CheckWordResult = null;
                        else
                        {
                            try
                            {
                                CheckWordResult = Bloom.CheckString(checkWord);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                                CheckWordResult = false;
                            }
                        }
                    }
                    else
                        CheckWordResult = false;
                }
            }
        }

        private bool? checkWordResult;
        public bool? CheckWordResult
        {
            get => checkWordResult;
            set
            {
                if (checkWordResult != value)
                {
                    checkWordResult = value;
                    NotifyPropertyChanged(nameof(CheckWordResult));
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

        public Bloom Bloom { get; } = new();
        public void OpenBloom(string FileName)
        {
            CheckWord = null;
            Bloom.Abort();
            try
            {
                Bloom.Open(FileName);
                HeaderVersion = Bloom.HeaderVersion();
                HeaderSize = Bloom.HeaderSize();
                HeaderHashFunc = Bloom.HeaderHashFunc();
                BloomFilterStatus = "OK";
                ValidBloomFilter = true;
                Bloom.Load();
            }
            catch (BloomException ex)
            {
                Bloom.Abort();
                BloomFilterStatus = ex.Message;
                ValidBloomFilter = false;
                HeaderVersion = null;
                HeaderSize = null;
                HeaderHashFunc = null;
            }
            BloomFilterPath = FileName;
            BloomFilterFile = System.IO.Path.GetFileName(FileName);
        }

        public void CloseBloom()
        {
            CheckWord = null;
            Bloom.Abort();
            BloomFilterStatus = string.Empty;
            ValidBloomFilter = false;
            HeaderVersion = null;
            HeaderSize = null;
            HeaderHashFunc = null;
            BloomFilterPath = string.Empty;
            BloomFilterFile = string.Empty;
        }
    }
}
