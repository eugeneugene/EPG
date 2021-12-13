using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPG.Model
{
    public class DataModel : INotifyPropertyChanged
    {
        public Mode PasswordMode { get; set; }
        public bool ShowHyphenated { get; set; }
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
    }
}
