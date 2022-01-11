using EPG.Models;
using System.Configuration;

namespace EPG.Code
{
    [SettingsProvider(typeof(EPGSettingsProvider))]
    internal sealed class EPGSettings : ApplicationSettingsBase
    {
        private static readonly EPGSettings _instance = (EPGSettings)Synchronized(new EPGSettings());

        public static EPGSettings Instance => _instance;

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
            get=>(bool)this[nameof(ShowHyphenated)];
            set => this[nameof(ShowHyphenated)] = value;
        }
    }
}
