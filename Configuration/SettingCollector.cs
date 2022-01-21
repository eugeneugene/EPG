// -- FILE ------------------------------------------------------------------
// name       : SettingCollector.cs
// created    : Jani Giannoudis - 2008.05.15
// language   : c#
// environment: .NET 2.0
// --------------------------------------------------------------------------
namespace EPG.Configuration
{
    // ------------------------------------------------------------------------
    public abstract class SettingCollector : ISettingCollector
    {
        // ----------------------------------------------------------------------
        public event SettingCollectorCancelEventHandler CollectingSetting;

        // ----------------------------------------------------------------------
        public ApplicationSettings ApplicationSettings { get; set; }

        // ----------------------------------------------------------------------
        public abstract void Collect();

        // ----------------------------------------------------------------------
        protected virtual bool OnCollectingSetting(object element)
        {
            if (CollectingSetting != null)
            {
                SettingCollectorCancelEventArgs e = new(element);
                CollectingSetting(this, e);
                return e.Cancel == false;
            }

            return true;
        } // OnCollectingSetting
    } // class SettingCollector
} // namespace EPG.Configuration
// -- EOF -------------------------------------------------------------------
