using EPG.Code;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EPG.Models
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        public void FromSettings(EPGSettings settings)
        {
            PasswordMode = settings.PasswordMode;
            ShowHyphenated = settings.ShowHyphenated;
            NumberOfPasswords = settings.NumberOfPasswords;
            MinimumLength = settings.MinimumLength;
            MaximumLength = settings.MaximumLength;
            SmallSymbols = settings.SmallSymbols;
            CapitalSymbols = settings.CapitalSymbols;
            Numerals = settings.Numerals;
            SpecialSymbols = settings.SpecialSymbols;
            Exclude = settings.Exclude;
            Include = settings.Include;
            EnableBloom = settings.EnableBloom;
            ParanoidCheck = settings.ParanoidCheck;
            CalculateQuality = settings.CalculateQuality;
            Filter = settings.Filter;
        }

        private PasswordMode? passwordMode;
        public PasswordMode? PasswordMode
        {
            get => passwordMode;
            set
            {
                if (passwordMode != value)
                {
                    passwordMode = value;
                    NotifyPropertyChanged(nameof(PasswordMode));
                }
            }
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

        private uint numberOfPasswords;
        public uint NumberOfPasswords
        {
            get => numberOfPasswords;
            set
            {
                if (numberOfPasswords != value)
                {
                    numberOfPasswords = value;
                    NotifyPropertyChanged(nameof(NumberOfPasswords));
                }
            }
        }

        private uint minimumLength;
        public uint MinimumLength
        {
            get => minimumLength;
            set
            {
                if (minimumLength != value)
                {
                    minimumLength = value;
                    NotifyPropertyChanged(nameof(MinimumLength));
                }
            }
        }

        private uint maximumLength;
        public uint MaximumLength
        {
            get => maximumLength;
            set
            {
                if (maximumLength != value)
                {
                    maximumLength = value;
                    NotifyPropertyChanged(nameof(MaximumLength));
                }
            }
        }

        private bool? smallSymbols;
        public bool? SmallSymbols
        {
            get => smallSymbols;
            set
            {
                if (smallSymbols != value)
                {
                    smallSymbols = value;
                    NotifyPropertyChanged(nameof(SmallSymbols));
                }
            }
        }

        private bool? capitalSymbols;
        public bool? CapitalSymbols
        {
            get => capitalSymbols;
            set
            {
                if (capitalSymbols != value)
                {
                    capitalSymbols = value;
                    NotifyPropertyChanged(nameof(CapitalSymbols));
                }
            }
        }

        private bool? numerals;
        public bool? Numerals
        {
            get => numerals;
            set
            {
                if (numerals != value)
                {
                    numerals = value;
                    NotifyPropertyChanged(nameof(Numerals));
                }
            }
        }

        private bool? specialSymbols;
        public bool? SpecialSymbols
        {
            get => specialSymbols;
            set
            {
                if (specialSymbols != value)
                {
                    specialSymbols = value;
                    NotifyPropertyChanged(nameof(SpecialSymbols));
                }
            }
        }

        private string? exclude;
        public string? Exclude
        {
            get => exclude;
            set
            {
                if (exclude != value)
                {
                    exclude = value;
                    NotifyPropertyChanged(nameof(Exclude));
                }
            }
        }

        private string? include;
        public string? Include
        {
            get => include;
            set
            {
                if (include != value)
                {
                    include = value;
                    NotifyPropertyChanged(nameof(Include));
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

        private bool paranoidCheck;
        public bool ParanoidCheck
        {
            get => paranoidCheck;
            set
            {
                if (paranoidCheck != value)
                {
                    paranoidCheck = value;
                    NotifyPropertyChanged(nameof(ParanoidCheck));
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

        private string? filter;
        public string? Filter
        {
            get => filter;
            set
            {
                if (filter != value)
                {
                    filter = value;
                    NotifyPropertyChanged(nameof(Filter));
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
