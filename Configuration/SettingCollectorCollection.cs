// -- FILE ------------------------------------------------------------------
// name       : SettingCollectorCollection.cs
// created    : Jani Giannoudis - 2008.04.25
// language   : c#
// environment: .NET 2.0
// --------------------------------------------------------------------------
using System.Collections;

namespace EPG.Configuration
{
    // ------------------------------------------------------------------------
    public sealed class SettingCollectorCollection : IEnumerable
    {
        // ----------------------------------------------------------------------
        public event SettingCollectorCancelEventHandler CollectingSetting;

        // ----------------------------------------------------------------------
        public SettingCollectorCollection(ApplicationSettings applicationSettings)
        {
            this.applicationSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));
        } // SettingCollectorCollection

        // ----------------------------------------------------------------------
        public ApplicationSettings ApplicationSettings
        {
            get { return applicationSettings; }
        } // ApplicationSettings

        // ----------------------------------------------------------------------
        public int Count
        {
            get { return settingCollectors.Count; }
        } // Count

        // ----------------------------------------------------------------------
        public IEnumerator GetEnumerator()
        {
            return settingCollectors.GetEnumerator();
        } // GetEnumerator

        // ----------------------------------------------------------------------
        public void Add(ISettingCollector setting)
        {
            if (setting == null)
            {
                throw new ArgumentNullException(nameof(setting));
            }
            setting.ApplicationSettings = applicationSettings;
            setting.CollectingSetting += SettingCollectingSetting;
            settingCollectors.Add(setting);
        } // Add

        // ----------------------------------------------------------------------
        public void Remove(ISettingCollector setting)
        {
            if (setting == null)
            {
                throw new ArgumentNullException(nameof(setting));
            }
            setting.CollectingSetting -= SettingCollectingSetting;
            settingCollectors.Remove(setting);
        } // Remove

        // ----------------------------------------------------------------------
        public void Clear()
        {
            foreach (ISettingCollector settingCollector in settingCollectors)
            {
                Remove(settingCollector);
            }
        } // Clear

        // ----------------------------------------------------------------------
        public void Collect()
        {
            foreach (ISettingCollector settingCollector in settingCollectors)
            {
                settingCollector.Collect();
            }
        } // Collect

        // ----------------------------------------------------------------------
        private void SettingCollectingSetting(object sender, SettingCollectorCancelEventArgs e)
        {
            CollectingSetting?.Invoke(this, e);
        } // SettingCollectingSetting

        // ----------------------------------------------------------------------
        // members
        private readonly ArrayList settingCollectors = new();
        private readonly ApplicationSettings applicationSettings;

    } // class SettingCollectorCollection
} // namespace EPG.Configuration
// -- EOF -------------------------------------------------------------------
