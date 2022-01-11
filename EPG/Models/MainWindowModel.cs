using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EPG.Models
{
    public class MainWindowModel : INotifyPropertyChanged
    {
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

        public uint NumberOfPasswords { get; set; }
        public uint MinimumLength { get; set; }
        public uint MaximumLength { get; set; }
        public bool SmallLetters { get; set; }
        public bool CapitalSymbols { get; set; }
        public bool Numerals { get; set; }
        public bool SpecialSymbols { get; set; }
        public string? Exclude { get; set; }
        public string? Include { get; set; }
        public bool EnableBloom { get; set; }
        public bool ParanoidCheck { get; set; }
        public bool CalculateQuality { get; set; }
        public string? Filter { get; set; }
        public List<object> PassswordItems { get; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName))
                return;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
