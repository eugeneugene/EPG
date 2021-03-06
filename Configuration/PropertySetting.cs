// -- FILE ------------------------------------------------------------------
// name       : PropertySetting.cs
// created    : Jani Giannoudis - 2008.04.25
// language   : c#
// environment: .NET 2.0
// --------------------------------------------------------------------------
using System.Reflection;

namespace EPG.Configuration
{
    // ------------------------------------------------------------------------
    public class PropertySetting : ValueSettingBase
    {
        // ----------------------------------------------------------------------
        public PropertySetting(object component, PropertyInfo propertyInfo) : this(propertyInfo.Name, component, propertyInfo)
        {
        } // PropertySetting

        // ----------------------------------------------------------------------
        public PropertySetting(string name, object component, PropertyInfo propertyInfo) : this(name, component, propertyInfo, null)
        {
        } // PropertySetting

        // ----------------------------------------------------------------------
        public PropertySetting(string name, object component, PropertyInfo propertyInfo, object defaultValue) : base(name, defaultValue)
        {
            this.component = component ?? throw new ArgumentNullException(nameof(component));
            this.propertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            CheckProperty();
        } // PropertySetting

        // ----------------------------------------------------------------------
        public PropertySetting(object component, string propertyName) : this(propertyName, component, propertyName)
        {
        } // PropertySetting

        // ----------------------------------------------------------------------
        public PropertySetting(string name, object component, string propertyName) : this(name, component, propertyName, null)
        {
        } // PropertySetting

        // ----------------------------------------------------------------------
        public PropertySetting(string name, object component, string propertyName, object defaultValue) : base(name, defaultValue)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            this.component = component ?? throw new ArgumentNullException(nameof(component));
            propertyInfo = component.GetType().GetProperty(propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (propertyInfo is null)
            {
                throw new ArgumentException("missing setting property: " + propertyName);
            }
            CheckProperty();
        } // PropertySetting

        // ----------------------------------------------------------------------
        public PropertyInfo PropertyInfo
        {
            get { return propertyInfo; }
        } // PropertyInfo

        // ----------------------------------------------------------------------
        public string PropertyName
        {
            get { return propertyInfo.Name; }
        } // PropertyName

        // ----------------------------------------------------------------------
        public object Component
        {
            get { return component; }
        } // Component

        // ----------------------------------------------------------------------
        public override object OriginalValue
        {
            get { return LoadValue(Name, propertyInfo.PropertyType, SerializeAs, DefaultValue); }
        } // OriginalValue

        // ----------------------------------------------------------------------
        public override object Value
        {
            get { return propertyInfo.GetValue(component, null); }
            set { propertyInfo.SetValue(component, value, null); }
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
                SaveValue(Name, propertyInfo.PropertyType, SerializeAs, value, DefaultValue);
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
        private void CheckProperty()
        {
            if (!propertyInfo.CanRead) // no get; accessor
            {
                throw new ArgumentException("setting property '" + PropertyName + "' must be readable");
            }
            if (!propertyInfo.CanWrite) // no set; accessor
            {
                throw new ArgumentException("setting property '" + PropertyName + "' must be writeable");
            }
        } // CheckProperty

        // ----------------------------------------------------------------------
        // members
        private readonly object component;
        private readonly PropertyInfo propertyInfo;

    } // class PropertySetting
} // namespace EPG.Configuration
// -- EOF -------------------------------------------------------------------
