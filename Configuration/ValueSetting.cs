// -- FILE ------------------------------------------------------------------
// name       : ValueSetting.cs
// created    : Jani Giannoudis - 2008.04.25
// language   : c#
// environment: .NET 2.0
// --------------------------------------------------------------------------

namespace EPG.Configuration
{
    // ------------------------------------------------------------------------
    public class ValueSetting : ValueSettingBase
    {
        // ----------------------------------------------------------------------
        public ValueSetting(string name, Type valueType) : this(name, valueType, null, null)
        {
        } // ValueSetting

        // ----------------------------------------------------------------------
        public ValueSetting(string name, Type valueType, object value) : this(name, valueType, value, null)
        {
        } // ValueSetting

        // ----------------------------------------------------------------------
        public ValueSetting(string name, Type valueType, object value, object defaultValue) : base(name, defaultValue)
        {
            if (valueType is null)
            {
                throw new ArgumentNullException(nameof(valueType));
            }
            if (defaultValue is not null && defaultValue.GetType() != valueType)
            {
                throw new ArgumentException(null, nameof(defaultValue));
            }

            this.valueType = valueType;
            ChangeValue(value);
        } // ValueSetting

        // ----------------------------------------------------------------------
        public override object OriginalValue
        {
            get { return LoadValue(Name, valueType, SerializeAs, DefaultValue); }
        } // OriginalValue

        // ----------------------------------------------------------------------
        public override object Value
        {
            get { return value; }
            set { ChangeValue(value); }
        } // Value

        // ----------------------------------------------------------------------
        public override void Load()
        {
            try
            {
                object originalValue = OriginalValue;
                if (originalValue is null && LoadUndefinedValue == false)
                {
                    return;
                }
                Value = originalValue;
            }
            catch
            {
                if (ThrowOnErrorLoading)
                {
                    throw;
                }
            }
        } // Load

        // ----------------------------------------------------------------------
        public override void Save()
        {
            try
            {
                object toSaveValue = Value;
                if (toSaveValue is null && SaveUndefinedValue == false)
                {
                    return;
                }
                SaveValue(Name, valueType, SerializeAs, toSaveValue, DefaultValue);
            }
            catch
            {
                if (ThrowOnErrorSaving)
                {
                    throw;
                }
            }
        } // Save

        // ----------------------------------------------------------------------
        private void ChangeValue(object newValue)
        {
            if (newValue is not null && newValue.GetType() != valueType)
            {
                throw new ArgumentException(null, nameof(newValue));
            }
            value = newValue;
        } // ChangeValue

        // ----------------------------------------------------------------------
        // members
        private readonly Type valueType;
        private object value;

    } // class ValueSetting
} // namespace EPG.Configuration
// -- EOF -------------------------------------------------------------------
