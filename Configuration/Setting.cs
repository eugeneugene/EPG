// -- FILE ------------------------------------------------------------------
// name       : Setting.cs
// created    : Jani Giannoudis - 2008.04.25
// language   : c#
// environment: .NET 2.0
// --------------------------------------------------------------------------
using System.Configuration;

namespace EPG.Configuration
{
    // ------------------------------------------------------------------------
    public abstract class Setting : ISetting
    {
        // ----------------------------------------------------------------------
        public event SettingValueCancelEventHandler ValueSaving;
        public event SettingValueCancelEventHandler ValueLoading;

        // ----------------------------------------------------------------------
        protected Setting()
        {
            Scope = SettingScope.User;
        } // Setting

        // ----------------------------------------------------------------------
        public ApplicationSettings ApplicationSettings { get; set; }

        // ----------------------------------------------------------------------
        public SettingScope Scope { get; set; }

        // ----------------------------------------------------------------------
        public string Description { get; set; }

        // ----------------------------------------------------------------------
        public bool ThrowOnErrorDeserializing { get; set; }

        // ----------------------------------------------------------------------
        public bool ThrowOnErrorSerializing { get; set; }

        // ----------------------------------------------------------------------
        public bool ThrowOnErrorSaving { get; set; }

        // ----------------------------------------------------------------------
        public bool ThrowOnErrorLoading { get; set; }

        // ----------------------------------------------------------------------
        public abstract bool HasChanged { get; }

        // ----------------------------------------------------------------------
        public abstract void Load();

        // ----------------------------------------------------------------------
        public abstract void Save();

        // ----------------------------------------------------------------------
        protected virtual object OnValueSaving(string name, object value)
        {
            if (ValueSaving is null)
            {
                return value;
            }

            SettingValueCancelEventArgs e = new(this, name, value);
            ValueSaving(this, e);
            return e.Cancel ? null : e.TargetValue;
        } // OnValueSaving

        // ----------------------------------------------------------------------
        protected virtual object OnValueLoading(string name, object value)
        {
            if (ValueLoading is null)
            {
                return value;
            }

            SettingValueCancelEventArgs e = new(this, name, value);
            ValueLoading(this, e);
            return e.Cancel ? null : e.TargetValue;
        } // OnValueLoading

        // ----------------------------------------------------------------------
        protected void CreateSettingProperty(string name, Type type, SettingsSerializeAs serializeAs, object defaultValue)
        {
            ApplicationSettings applicationSettings = ApplicationSettings;
            if (applicationSettings is null || applicationSettings.DefaultProvider is null)
            {
                return;
            }

            SettingsProperty settingsProperty = applicationSettings.Properties[name];
            if (settingsProperty is not null)
            {
                return; // already present
            }

            SettingsAttributeDictionary attributes = new();
            SettingAttribute attribute;
            switch (Scope)
            {
                case SettingScope.Application:
                    // attribute = new ApplicationScopedSettingAttribute();
                    throw new NotImplementedException(); // currently not supported
                case SettingScope.User:
                    attribute = new UserScopedSettingAttribute();
                    break;
                default:
                    return;
            }
            attributes.Add(attribute.TypeId, attribute);

            settingsProperty = new SettingsProperty(
                name, // name
                type, // type
                applicationSettings.DefaultProvider, // settings provider
                false, // is readonly
                defaultValue, // default
                serializeAs, // serialize as
                attributes, // attribute
                ThrowOnErrorDeserializing, // throw on deserialization
                ThrowOnErrorSerializing); // throw on serialization

            applicationSettings.Properties.Add(settingsProperty);
        } // CreateSettingProperty

        // ----------------------------------------------------------------------
        protected void CreateSettingPropertyValue(string name, Type type, SettingsSerializeAs serializeAs, object defaultValue)
        {
            CreateSettingProperty(name, type, serializeAs, defaultValue);

            ApplicationSettings applicationSettings = ApplicationSettings;
            SettingsProperty settingsProperty = applicationSettings.Properties[name];
            if (settingsProperty is not null)
            {
                return; // already present
            }

            settingsProperty = new SettingsProperty(name);
            SettingsPropertyValue settingsPropertyValue = new(settingsProperty);
            applicationSettings.PropertyValues.Add(settingsPropertyValue);
        } // CreateSettingPropertyValue

        // ----------------------------------------------------------------------
        protected object LoadValue(string name, Type type, SettingsSerializeAs serializeAs)
        {
            return LoadValue(name, type, serializeAs, null);
        } // LoadValue

        // ----------------------------------------------------------------------
        protected object LoadValue(string name, Type type, SettingsSerializeAs serializeAs, object defaultValue)
        {
            CreateSettingProperty(name, type, serializeAs, defaultValue);
            object value = ApplicationSettings[name];
            return OnValueLoading(name, value);
        } // LoadValue

        // ----------------------------------------------------------------------
        protected void SaveValue(string name, Type type, SettingsSerializeAs serializeAs, object value)
        {
            SaveValue(name, type, serializeAs, value, null);
        } // SaveValue

        // ----------------------------------------------------------------------
        protected void SaveValue(string name, Type type, SettingsSerializeAs serializeAs, object value, object defaultValue)
        {
            if (OnValueSaving(name, value) is null)
            {
                return;
            }

            CreateSettingPropertyValue(name, type, serializeAs, defaultValue);
            ApplicationSettings[name] = value;
        } // SaveValue
    } // class Setting
} // namespace EPG.Configuration
// -- EOF -------------------------------------------------------------------
