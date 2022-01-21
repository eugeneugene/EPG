// -- FILE ------------------------------------------------------------------
// name       : SettingCollectorCancelEventArgs.cs
// created    : Jani Giannoudis - 2008.05.11
// language   : c#
// environment: .NET 2.0
// --------------------------------------------------------------------------
using System.ComponentModel;

namespace EPG.Configuration
{
    // ------------------------------------------------------------------------
    public delegate void SettingCollectorCancelEventHandler(object sender, SettingCollectorCancelEventArgs e);

    // ------------------------------------------------------------------------
    public class SettingCollectorCancelEventArgs : CancelEventArgs
    {
        // ----------------------------------------------------------------------
        public SettingCollectorCancelEventArgs(object element) :
            this(element, false)
        {
        } // SettingCollectorCancelEventArgs

        // ----------------------------------------------------------------------
        public SettingCollectorCancelEventArgs(object element, bool cancel) :
            base(cancel)
        {
            this.element = element ?? throw new ArgumentNullException(nameof(element));
        } // SettingCollectorCancelEventArgs

        // ----------------------------------------------------------------------
        public object Element
        {
            get { return element; }
        } // Element

        // ----------------------------------------------------------------------
        // members
        private readonly object element;

    } // class SettingCollectorCancelEventArgs
} // namespace EPG.Configuration
// -- EOF -------------------------------------------------------------------
