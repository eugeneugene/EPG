using EPG.Models;
using System.Configuration;

namespace EPG.Code
{
    [SettingsProvider(typeof(EPGSettingsProvider))]
    public sealed class EPGSettings : ApplicationSettingsBase
    {
        private static readonly EPGSettings _instance = (EPGSettings)Synchronized(new EPGSettings());

        public static EPGSettings Instance => _instance;

        public void FromModel(MainWindowModel model)
        {
            PasswordMode = model.PasswordMode;
            ShowHyphenated = model.ShowHyphenated;
            NumberOfPasswords = model.NumberOfPasswords;
            MinimumLength = model.MinimumLength;
            MaximumLength = model.MaximumLength;
            SmallSymbols = model.SmallSymbols;
            CapitalSymbols = model.CapitalSymbols;
            Numerals = model.Numerals;
            SpecialSymbols = model.SpecialSymbols;
            Exclude = model.Exclude;
            Include = model.Include;
            EnableBloom = model.EnableBloom;
            ParanoidCheck = model.ParanoidCheck;
            CalculateQuality = model.CalculateQuality;
            Filter = model.Filter;
        }

        [UserScopedSetting()]
        [DefaultSettingValue(null)]
        public PasswordMode? PasswordMode
        {
            get => (PasswordMode?)this[nameof(PasswordMode)];
            set => this[nameof(PasswordMode)] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue("false")]
        public bool ShowHyphenated
        {
            get => (bool)this[nameof(ShowHyphenated)];
            set => this[nameof(ShowHyphenated)] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue("0")]
        public uint NumberOfPasswords
        {
            get => (uint)this[nameof(NumberOfPasswords)];
            set => this[nameof(NumberOfPasswords)] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue("0")]
        public uint MinimumLength
        {
            get => (uint)this[nameof(MinimumLength)];
            set => this[nameof(MinimumLength)] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue("0")]
        public uint MaximumLength
        {
            get => (uint)this[nameof(MaximumLength)];
            set => this[nameof(MaximumLength)] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue(null)]
        public bool? SmallSymbols
        {
            get => (bool?)this[nameof(SmallSymbols)];
            set => this[nameof(SmallSymbols)] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue(null)]
        public bool? CapitalSymbols
        {
            get => (bool?)this[nameof(CapitalSymbols)];
            set => this[nameof(CapitalSymbols)] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue(null)]
        public bool? Numerals
        {
            get => (bool?)this[nameof(Numerals)];
            set => this[nameof(Numerals)] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue(null)]
        public bool? SpecialSymbols
        {
            get => (bool?)this[nameof(SpecialSymbols)];
            set => this[nameof(SpecialSymbols)] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue(null)]
        public string? Exclude
        {
            get => (string?)this[nameof(Exclude)];
            set => this[nameof(Exclude)] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue(null)]
        public string? Include
        {
            get => (string?)this[nameof(Include)];
            set => this[nameof(Include)] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue("false")]
        public bool EnableBloom
        {
            get => (bool)this[nameof(EnableBloom)];
            set => this[nameof(EnableBloom)] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue("false")]
        public bool ParanoidCheck
        {
            get => (bool)this[nameof(ParanoidCheck)];
            set => this[nameof(ParanoidCheck)] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue("false")]
        public bool CalculateQuality
        {
            get => (bool)this[nameof(CalculateQuality)];
            set => this[nameof(CalculateQuality)] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue(null)]
        public string? Filter
        {
            get => (string?)this[nameof(Filter)];
            set => this[nameof(Filter)] = value;
        }
    }
}
