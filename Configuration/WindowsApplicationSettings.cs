// -- FILE ------------------------------------------------------------------
// name       : WindowsApplicationSettings.cs
// created    : Jani Giannoudis - 2008.04.29
// language   : c#
// environment: .NET 3.0
// --------------------------------------------------------------------------
using System.Configuration;
using System.Windows;

namespace EPG.Configuration
{
    // ------------------------------------------------------------------------
    public class WindowsApplicationSettings : ApplicationSettings
    {
        // ----------------------------------------------------------------------
        public WindowsApplicationSettings(Application application) : this(application, application.GetType())
        {
        } // WindowsApplicationSettings

        public WindowsApplicationSettings(Application application, LocalFileSettingsProvider provider) : this(application, application.GetType(), provider)
        {
        } // WindowsApplicationSettings

        // ----------------------------------------------------------------------
        public WindowsApplicationSettings(Application application, Type type) : this(application, type.Name, null)
        {
        } // WindowsApplicationSettings

        public WindowsApplicationSettings(Application application, Type type, LocalFileSettingsProvider provider) : this(application, type.Name, provider)
        {
        } // WindowsApplicationSettings

        // ----------------------------------------------------------------------
        public WindowsApplicationSettings(Application application, string settingsKey, LocalFileSettingsProvider provider) : base(settingsKey, provider)
        {
            this.application = application ?? throw new ArgumentNullException(nameof(application));
            application.Startup += ApplicationStartup;
            application.Exit += ApplicationExit;
        } // WindowsApplicationSettings

        // ----------------------------------------------------------------------
        public Application Application
        {
            get { return application; }
        } // Application

        // ----------------------------------------------------------------------
        public bool SaveOnClose
        {
            get { return saveOnClose; }
            set { saveOnClose = value; }
        } // SaveOnClose

        // ----------------------------------------------------------------------
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            Load();
        } // ApplicationStartup

        // ----------------------------------------------------------------------
        private void ApplicationExit(object sender, ExitEventArgs e)
        {
            if (saveOnClose == false)
            {
                return;
            }
            Save();
        } // ApplicationExit

        // ----------------------------------------------------------------------
        // members
        private readonly Application application;
        private bool saveOnClose = true;

    } // class WindowsApplicationSettings
} // namespace EPG.Configuration
// -- EOF -------------------------------------------------------------------
