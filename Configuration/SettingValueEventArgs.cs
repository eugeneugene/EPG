// -- FILE ------------------------------------------------------------------
// name       : SettingValueEventArgs.cs
// created    : Jani Giannoudis - 2008.04.25
// language   : c#
// environment: .NET 2.0
// --------------------------------------------------------------------------

namespace EPG.Configuration
{
    // ------------------------------------------------------------------------
    public delegate void SettingValueEventHandler(object sender, SettingValueEventArgs e);

    // ------------------------------------------------------------------------
    public class SettingValueEventArgs : EventArgs
    {
        // ----------------------------------------------------------------------
        public SettingValueEventArgs(ISetting setting, string name, object value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.setting = setting ?? throw new ArgumentNullException(nameof(setting));
            this.name = name;
            this.value = value ?? throw new ArgumentNullException(nameof(value));
            TargetValue = value;
        } // SettingValueEventArgs

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
            get { return value != null; }
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

    } // class SettingValueEventArgs
} // namespace EPG.Configuration
// -- EOF -------------------------------------------------------------------
