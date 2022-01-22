// -- FILE ------------------------------------------------------------------
// name       : SettingValueCancelEventArgs.cs
// created    : Jani Giannoudis - 2008.04.25
// language   : c#
// environment: .NET 2.0
// --------------------------------------------------------------------------
using System.ComponentModel;

namespace EPG.Configuration
{
    // ------------------------------------------------------------------------
    public delegate void SettingValueCancelEventHandler(object sender, SettingValueCancelEventArgs e);
    // ------------------------------------------------------------------------
    public class SettingValueCancelEventArgs : CancelEventArgs
    {

        // ----------------------------------------------------------------------
        public SettingValueCancelEventArgs(ISetting setting, string name, object value) : this(setting, name, value, false)
        {
        } // SettingValueCancelEventArgs

        // ----------------------------------------------------------------------
        public SettingValueCancelEventArgs(ISetting setting, string name, object value, bool cancel) : base(cancel)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.setting = setting ?? throw new ArgumentNullException(nameof(setting));
            this.name = name;
            this.value = value ?? throw new ArgumentNullException(nameof(value));
            TargetValue = value;
        } // SettingValueCancelEventArgs

        // ----------------------------------------------------------------------
        public ISetting Setting
        {
            get { return setting; }
        } // Setting

        // ----------------------------------------------------------------------
        public string Name
        {
            get { return name; }
        } // Name

        // ----------------------------------------------------------------------
        public bool HasValue
        {
            get { return value is not null; }
        } // HasValue

        // ----------------------------------------------------------------------
        public object Value
        {
            get { return value; }
        } // Value

        // ----------------------------------------------------------------------
        public object TargetValue { get; set; }

        // ----------------------------------------------------------------------
        // members
        private readonly ISetting setting;
        private readonly string name;
        private readonly object value;

    } // class SettingValueCancelEventArgs
} // namespace EPG.Configuration
// -- EOF -------------------------------------------------------------------
