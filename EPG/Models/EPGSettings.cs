using Itenso.Configuration;
using System.Configuration;

namespace EPG.Models
{
    public sealed class EPGSettings : ApplicationSettingsBase
    {
        public void FromModel(MainWindowModel model)
        {
            PasswordMode = model.PasswordMode;
            ShowHyphenated = model.ShowHyphenated;
            NumberOfPasswords = model.NumberOfPasswords;
            MinimumLength = model.MinimumLength;
            MaximumLength = model.MaximumLength;
            SmallSymbols = ToThreeStateValue(model.SmallSymbols);
            CapitalSymbols = ToThreeStateValue(model.CapitalSymbols);
            Numerals = ToThreeStateValue(model.Numerals);
            SpecialSymbols = ToThreeStateValue(model.SpecialSymbols);
            Exclude = model.Exclude;
            Include = model.Include;
            EnableBloom = model.EnableBloom;
            ParanoidCheck = model.ParanoidCheck;
            CalculateQuality = model.CalculateQuality;
            Filter = model.Filter;
            AutoClear = model.AutoClear;
        }

        [PropertySetting(DefaultValue = null)]
        public PasswordMode? PasswordMode { get; set; }

        [PropertySetting(DefaultValue = false)]
        public bool ShowHyphenated { get; set; }

        [PropertySetting(DefaultValue = 0)]
        public uint NumberOfPasswords { get; set; }

        [PropertySetting(DefaultValue = 0)]
        public uint MinimumLength { get; set; }

        [PropertySetting(DefaultValue = 0)]
        public uint MaximumLength { get; set; }

        [PropertySetting(DefaultValue = ThreeStateValue.Null)]
        public ThreeStateValue SmallSymbols { get; set; }

        [PropertySetting(DefaultValue = ThreeStateValue.Null)]
        public ThreeStateValue CapitalSymbols { get; set; }

        [PropertySetting(DefaultValue = ThreeStateValue.Null)]
        public ThreeStateValue Numerals { get; set; }

        [PropertySetting(DefaultValue = ThreeStateValue.Null)]
        public ThreeStateValue SpecialSymbols { get; set; }

        [PropertySetting(DefaultValue = "")]
        public string? Exclude { get; set; }

        [PropertySetting(DefaultValue = "")]
        public string? Include { get; set; }

        [PropertySetting(DefaultValue = false)]
        public bool EnableBloom { get; set; }

        [PropertySetting(DefaultValue = false)]
        public bool ParanoidCheck { get; set; }

        [PropertySetting(DefaultValue = false)]
        public bool CalculateQuality { get; set; }

        [PropertySetting(DefaultValue = "")]
        public string? Filter { get; set; }

        [PropertySetting(DefaultValue = false)]
        public bool AutoClear { get; set; }
        private static ThreeStateValue ToThreeStateValue(bool? value)
        {
            return value switch
            {
                true => ThreeStateValue.True,
                false => ThreeStateValue.False,
                _ => ThreeStateValue.Null,
            };
        }
    }
}
