using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EPG.Models
{
    public class PasswordResultItem : INotifyPropertyChanged
    {
        public PasswordResultItem()
        {
            password = string.Empty;
        }

        public PasswordResultItem(uint counter, string password, string? hyphenatedPassword, BloomFilterResult? bloomFilterResult, decimal? complexity, bool manuallyEnterred)
        {
            this.counter = counter;
            this.password = password;
            this.hyphenatedPassword = hyphenatedPassword;
            this.bloomFilterResult = bloomFilterResult;
            this.complexity = complexity;
            this.manuallyEnterred = manuallyEnterred;
        }

        private uint counter;
        public uint Counter
        {
            get => counter;
            set
            {
                if (counter != value)
                {
                    counter = value;
                    NotifyPropertyChanged(nameof(Counter));
                }
            }
        }

        private string password;
        public string Password
        {
            get => password;
            set
            {
                if (password != value)
                {
                    password = value;
                    NotifyPropertyChanged(nameof(Password));
                }
            }
        }

        private string? hyphenatedPassword;
        public string? HyphenatedPassword
        {
            get => hyphenatedPassword;
            set
            {
                if (hyphenatedPassword != value)
                {
                    hyphenatedPassword = value;
                    NotifyPropertyChanged(nameof(HyphenatedPassword));
                }
            }
        }

        private BloomFilterResult? bloomFilterResult;
        public BloomFilterResult? BloomFilterResult
        {
            get => bloomFilterResult;
            set
            {
                if (bloomFilterResult != value)
                {
                    bloomFilterResult = value;
                    NotifyPropertyChanged(nameof(BloomFilterResult));
                }
            }
        }

        private decimal? complexity;
        public decimal? Complexity
        {
            get => complexity;
            set
            {
                if (complexity != value)
                {
                    complexity = value;
                    NotifyPropertyChanged(nameof(Complexity));
                }
            }
        }

        private bool manuallyEnterred;
        public bool ManuallyEnterred
        {
            get => manuallyEnterred;
            set
            {
                if (manuallyEnterred != value)
                {
                    manuallyEnterred = value;
                    NotifyPropertyChanged(nameof(ManuallyEnterred));
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

        public override string ToString()
        {
            return $"{Counter}. {Password} {BloomFilterResult}";
        }
    }
}
