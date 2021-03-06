// -- FILE ------------------------------------------------------------------
// name       : ISetting.cs
// created    : Jani Giannoudis - 2008.05.15
// language   : c#
// environment: .NET 2.0
// --------------------------------------------------------------------------

namespace EPG.Configuration
{
    // ------------------------------------------------------------------------
    public interface ISettingCollector
    {

        // ----------------------------------------------------------------------
        event SettingCollectorCancelEventHandler CollectingSetting;

        // ----------------------------------------------------------------------
        ApplicationSettings ApplicationSettings { get; set; }

        // ----------------------------------------------------------------------
        void Collect();

    } // interface ISettingCollector
} // namespace EPG.Configuration
// -- EOF -------------------------------------------------------------------
