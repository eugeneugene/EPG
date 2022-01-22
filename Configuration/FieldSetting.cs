// -- FILE ------------------------------------------------------------------
// name       : FieldSetting.cs
// created    : Jani Giannoudis - 2008.04.25
// language   : c#
// environment: .NET 2.0
// --------------------------------------------------------------------------
using System.Reflection;

namespace EPG.Configuration
{
    // ------------------------------------------------------------------------
    public class FieldSetting : ValueSettingBase
    {
        // ----------------------------------------------------------------------
        public FieldSetting(object component, FieldInfo fieldInfo) : this(fieldInfo.Name, component, fieldInfo)
        {
        } // FieldSetting

        // ----------------------------------------------------------------------
        public FieldSetting(string name, object component, FieldInfo fieldInfo) : this(name, component, fieldInfo, null)
        {
        } // FieldSetting

        // ----------------------------------------------------------------------
        public FieldSetting(string name, object component, FieldInfo fieldInfo, object defaultValue) : base(name, defaultValue)
        {
            this.component = component ?? throw new ArgumentNullException(nameof(component));
            this.fieldInfo = fieldInfo ?? throw new ArgumentNullException(nameof(fieldInfo));
            CheckField();
        } // FieldSetting

        // ----------------------------------------------------------------------
        public FieldSetting(object component, string fieldName) : this(fieldName, component, fieldName)
        {
        } // FieldSetting

        // ----------------------------------------------------------------------
        public FieldSetting(string name, object component, string fieldName) : this(name, component, fieldName, null)
        {
        } // FieldSetting

        // ----------------------------------------------------------------------
        public FieldSetting(string name, object component, string fieldName, object defaultValue) : base(name, defaultValue)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException(nameof(fieldName));
            }

            this.component = component ?? throw new ArgumentNullException(nameof(component));
            fieldInfo = component.GetType().GetField(fieldName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fieldInfo is null)
            {
                throw new ArgumentException("missing setting field: " + fieldName);
            }
            CheckField();
        } // FieldSetting

        // ----------------------------------------------------------------------
        public FieldInfo FieldInfo
        {
            get { return fieldInfo; }
        } // FieldInfo

        // ----------------------------------------------------------------------
        public string FieldName
        {
            get { return fieldInfo.Name; }
        } // FieldName

        // ----------------------------------------------------------------------
        public object Component
        {
            get { return component; }
        } // Component

        // ----------------------------------------------------------------------
        public override object OriginalValue
        {
            get { return LoadValue(Name, fieldInfo.FieldType, SerializeAs, DefaultValue); }
        } // OriginalValue

        // ----------------------------------------------------------------------
        public override object Value
        {
            get { return fieldInfo.GetValue(component); }
            set { fieldInfo.SetValue(component, value); }
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
                object value = Value;
                if (value is null && SaveUndefinedValue == false)
                {
                    return;
                }
                SaveValue(Name, fieldInfo.FieldType, SerializeAs, value, DefaultValue);
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
        private void CheckField()
        {
            if (fieldInfo.IsInitOnly) // readonly field
            {
                throw new ArgumentException("setting field '" + FieldName + "' is readonly");
            }
        } // CheckField

        // ----------------------------------------------------------------------
        // members
        private readonly object component;
        private readonly FieldInfo fieldInfo;

    } // class FieldSetting
} // namespace EPG.Configuration
// -- EOF -------------------------------------------------------------------
