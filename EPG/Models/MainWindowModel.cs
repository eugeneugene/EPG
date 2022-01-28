using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EPG.Models
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        public MainWindowModel()
        {
            ResultModel = new();
            ResultModel.PropertyChanged += ResultModelPropertyChanged;
        }

        private void ResultModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(ResultModel) + "." + e.PropertyName);
        }

        public void FromSettings(EPGSettings settings)
        {
            PasswordMode = settings.PasswordMode;
            ShowHyphenated = settings.ShowHyphenated;
            NumberOfPasswords = settings.NumberOfPasswords;
            MinimumLength = settings.MinimumLength;
            MaximumLength = settings.MaximumLength;
            SmallSymbols = ToBoolNull(settings.SmallSymbols);
            CapitalSymbols = ToBoolNull(settings.CapitalSymbols);
            Numerals = ToBoolNull(settings.Numerals);
            SpecialSymbols = ToBoolNull(settings.SpecialSymbols);
            Exclude = string.IsNullOrWhiteSpace(settings.Exclude) ? string.Empty : settings.Exclude;
            Include = string.IsNullOrWhiteSpace(settings.Include) ? string.Empty : settings.Include;
            EnableBloom = settings.EnableBloom;
            ParanoidCheck = settings.ParanoidCheck;
            CalculateQuality = settings.CalculateQuality;
            Filter = string.IsNullOrWhiteSpace(settings.Filter) ? string.Empty : settings.Filter;
            AutoClear = settings.AutoClear;
        }

        public PasswordResultModel ResultModel { get; }

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

                    if (passwordMode is not null && passwordMode == Models.PasswordMode.Pronounceable)
                    {
                        if (smallSymbols is null && capitalSymbols is null)
                        {
                            SmallSymbols = true;
                            CapitalSymbols = true;
                        }
                    }

                    if (SmallSymbols is null)
                        SmallSymbols = false;
                    if (CapitalSymbols is null)
                        CapitalSymbols = false;
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

                    if (PasswordMode is not null && PasswordMode == Models.PasswordMode.Pronounceable)
                    {
                        if (!(smallSymbols ?? false) && !(capitalSymbols ?? false))
                            CapitalSymbols = true;
                    }
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

                    if (PasswordMode is not null && PasswordMode == Models.PasswordMode.Pronounceable)
                    {
                        if (!(capitalSymbols ?? false) && !(smallSymbols ?? false))
                            SmallSymbols = true;
                    }
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

        private bool autoClear;

        public bool AutoClear
        {
            get => autoClear;
            set
            {
                if (autoClear != value)
                {
                    autoClear = value;
                    NotifyPropertyChanged(nameof(AutoClear));
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

        private static bool? ToBoolNull(ThreeStateValue value)
        {
            return value switch
            {
                ThreeStateValue.True => true,
                ThreeStateValue.False => false,
                _ => null,
            };
        }
    }
}
