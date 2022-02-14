// -- FILE ------------------------------------------------------------------
// name       : ApplicationSettings.cs
// created    : Jani Giannoudis - 2008.04.25
// language   : c#
// environment: .NET 2.0
// --------------------------------------------------------------------------
using System.Collections.Specialized;
using System.Configuration;

namespace EPG.Configuration
{
    // ------------------------------------------------------------------------
    public class ApplicationSettings : ApplicationSettingsBase
    {
        // ----------------------------------------------------------------------
        public event SettingValueCancelEventHandler SettingSaving;
        public event SettingValueCancelEventHandler SettingLoading;
        public event SettingCollectorCancelEventHandler CollectingSetting;

        // ----------------------------------------------------------------------
        public const string UpgradeSettingsKey = "UpgradeSettings";

        // ----------------------------------------------------------------------
        public ApplicationSettings(object obj) : this(obj.GetType().Name, obj, null)
        {
        } // ApplicationSettings

        // ----------------------------------------------------------------------
        public ApplicationSettings(object obj, LocalFileSettingsProvider provider) : this(obj.GetType().Name, obj, provider)
        {
        } // ApplicationSettings

        // ----------------------------------------------------------------------
        public ApplicationSettings(string settingsKey) : this(settingsKey, null)
        {
        } // ApplicationSettings

        // ----------------------------------------------------------------------
        public ApplicationSettings(string settingsKey, LocalFileSettingsProvider provider) : this(settingsKey, null, provider)
        {
        } // ApplicationSettings

        // ----------------------------------------------------------------------
        public ApplicationSettings(string settingsKey, object obj, LocalFileSettingsProvider provider) : base(settingsKey)
        {
            settings = new SettingCollection(this);

            // provider
            if (provider is null)
            {
                this.provider = new LocalFileSettingsProvider();
                this.provider.Initialize("LocalFileSettingsProvider", new NameValueCollection());
            }
            else
                this.provider = provider;

            base.Providers.Add(this.provider);

            // upgrade
            upgradeSettings = new ValueSetting(UpgradeSettingsKey, typeof(bool), true);
            UseAutoUpgrade = true;

            if (obj is not null)
            {
                Settings.AddAll(obj);
            }
        } // ApplicationSettings

        // ----------------------------------------------------------------------
        public SettingsProvider Provider
        {
            get { return provider; }
        } // DefaultProvider

        // ----------------------------------------------------------------------
        public SettingCollection Settings
        {
            get { return settings; }
        } // Settings

        // ----------------------------------------------------------------------
        public SettingCollectorCollection SettingCollectors
        {
            get
            {
                if (settingCollectors is null)
                {
                    settingCollectors = new SettingCollectorCollection(this);
                    settingCollectors.CollectingSetting += SettingCollectorsCollectingSetting;
                }
                return settingCollectors;
            }
        } // SettingCollectors

        // ----------------------------------------------------------------------
        public bool UseAutoUpgrade
        {
            get { return settings.Contains(upgradeSettings); }
            // ReSharper disable ValueParameterNotUsed
            set
            // ReSharper restore ValueParameterNotUsed
            {
                if (UseAutoUpgrade)
                {
                    return;
                }
                settings.Add(upgradeSettings);
            }
        } // UseAutoUpgrade

        // ----------------------------------------------------------------------
        //public static string UserConfigurationFilePath
        //{
        //    get
        //    {
        //        System.Configuration.Configuration config =
        //            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
        //        return config.FilePath;
        //    }
        //} // UserConfigurationFilePath

        // ----------------------------------------------------------------------
        public void Load()
        {
            Load(true);
        } // Load

        // ----------------------------------------------------------------------
        public virtual void Load(bool upgrade)
        {
            CollectSettings();
            LoadSettings();
            if (!upgrade)
            {
                return;
            }

            if ((bool)upgradeSettings.Value)
            {
                Upgrade();
                upgradeSettings.Value = false;
            }
            LoadSettings();
        } // Load

        // ----------------------------------------------------------------------
        public new virtual void Reload()
        {
            base.Reload();
            LoadSettings();
        } // Reload

        // ----------------------------------------------------------------------
        public new virtual void Reset()
        {
            base.Reset();
            LoadSettings();
        } // Reload

        // ----------------------------------------------------------------------
        public override void Save()
        {
            SaveSettings();
            base.Save();
        } // Save

        // ----------------------------------------------------------------------
        private void CollectSettings()
        {
            if (settingCollectors is null)
            {
                return; // no colectors present
            }
            settingCollectors.Collect();
        } // CollectSettings

        // ----------------------------------------------------------------------
        private void LoadSettings()
        {
            foreach (ISetting userSetting in settings)
            {
                if (SettingLoading is not null)
                {
                    userSetting.ValueLoading += UserSettingLoading;
                }
                userSetting.Load();
                if (SettingLoading is not null)
                {
                    userSetting.ValueLoading -= UserSettingLoading;
                }
            }
        } // LoadSettings

        // ----------------------------------------------------------------------
        private void SaveSettings()
        {
            foreach (ISetting userSetting in settings)
            {
                if (SettingSaving is not null)
                {
                    userSetting.ValueSaving += UserSettingSaving;
                }
                userSetting.Save();
                if (SettingSaving is not null)
                {
                    userSetting.ValueSaving -= UserSettingSaving;
                }
            }
        } // SaveSettings

        // ----------------------------------------------------------------------
        protected virtual void OnCollectingSetting(SettingCollectorCancelEventArgs e)
        {
            CollectingSetting?.Invoke(this, e);
        } // OnCollectingSetting

        // ----------------------------------------------------------------------
        private void UserSettingSaving(object sender, SettingValueCancelEventArgs e)
        {
            SettingSaving?.Invoke(sender, e);
        } // UserSettingSaving

        // ----------------------------------------------------------------------
        private void UserSettingLoading(object sender, SettingValueCancelEventArgs e)
        {
            SettingLoading?.Invoke(sender, e);
        } // UserSettingLoading

        // ----------------------------------------------------------------------
        private void SettingCollectorsCollectingSetting(object sender, SettingCollectorCancelEventArgs e)
        {
            OnCollectingSetting(e);
        } // SettingCollectorsCollectingSetting

        // ----------------------------------------------------------------------
        // members
        private readonly LocalFileSettingsProvider provider;
        private readonly SettingCollection settings;
        private SettingCollectorCollection settingCollectors;
        private readonly ValueSetting upgradeSettings;

    } // class ApplicationSettings
} // namespace EPG.Configuration
// -- EOF -------------------------------------------------------------------
