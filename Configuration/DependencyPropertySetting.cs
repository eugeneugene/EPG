// -- FILE ------------------------------------------------------------------
// name       : DependencyPropertySetting.cs
// created    : Jani Giannoudis - 2008.04.28
// language   : c#
// environment: .NET 2.0
// --------------------------------------------------------------------------
using System.Diagnostics;
using System.Windows;

namespace EPG.Configuration
{
    // ------------------------------------------------------------------------
    public class DependencyPropertySetting : ValueSettingBase
    {
        // ----------------------------------------------------------------------
        public static readonly DependencyProperty ApplicationSettingsProperty = DependencyProperty.RegisterAttached(
            "ApplicationSettings",
            typeof(ApplicationSettings),
            typeof(DependencyPropertySetting));

        // ----------------------------------------------------------------------
        public static readonly DependencyProperty PropertyProperty = DependencyProperty.RegisterAttached(
            "Property",
            typeof(DependencyProperty),
            typeof(DependencyPropertySetting),
            new FrameworkPropertyMetadata(OnDependencyPropertyChanged));

        // ----------------------------------------------------------------------
        public DependencyPropertySetting(DependencyObject obj, DependencyProperty property) : this(obj, property, null)
        {
        } // DependencyPropertySetting

        // ----------------------------------------------------------------------
        public DependencyPropertySetting(DependencyObject obj, DependencyProperty property, object defaultValue) : this(property.Name, obj, property, defaultValue)
        {
        } // DependencyPropertySetting

        // ----------------------------------------------------------------------
        public DependencyPropertySetting(string name, DependencyObject obj, DependencyProperty property) : this(name, obj, property, null)
        {
        } // DependencyPropertySetting

        // ----------------------------------------------------------------------
        public DependencyPropertySetting(string name, DependencyObject obj, DependencyProperty property, object defaultValue) : base(name, defaultValue)
        {
            dependencyObject = obj ?? throw new ArgumentNullException(nameof(obj));
            dependencyProperty = property ?? throw new ArgumentNullException(nameof(property));
        } // DependencyPropertySetting

        // ----------------------------------------------------------------------
        public DependencyProperty DependencyProperty
        {
            get { return dependencyProperty; }
        } // DependencyProperty

        // ----------------------------------------------------------------------
        public DependencyObject DependencyObject
        {
            get { return dependencyObject; }
        } // DependencyObject

        // ----------------------------------------------------------------------
        public override object OriginalValue
        {
            get { return LoadValue(Name, dependencyProperty.PropertyType, SerializeAs, DefaultValue); }
        } // OriginalValue

        // ----------------------------------------------------------------------
        public override object Value
        {
            get { return dependencyObject.GetValue(dependencyProperty); }
            set { dependencyObject.SetValue(dependencyProperty, value); }
        } // Value

        // ----------------------------------------------------------------------
        public static DependencyProperty GetProperty(DependencyObject obj)
        {
            return obj.GetValue(PropertyProperty) as DependencyProperty;
        } // GetProperty

        // ----------------------------------------------------------------------
        public static void SetProperty(DependencyObject obj, DependencyProperty property)
        {
            obj.SetValue(PropertyProperty, property);
        } // SetProperty

        // ----------------------------------------------------------------------
        public static ApplicationSettings GetApplicationSettings(DependencyObject obj)
        {
            return obj.GetValue(ApplicationSettingsProperty) as ApplicationSettings;
        } // GetApplicationSettings

        // ----------------------------------------------------------------------
        public static void SetApplicationSettings(DependencyObject obj, ApplicationSettings applicationSettings)
        {
            obj.SetValue(ApplicationSettingsProperty, applicationSettings);
        } // SetApplicationSettings

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
                SaveValue(Name, dependencyProperty.PropertyType, SerializeAs, value, DefaultValue);
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
        private static void OnDependencyPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is not IFrameworkInputElement frameworkInputElement)
            {
                Debug.WriteLine("DependencyPropertySetting: invalid framework element");
                return;
            }

            string elementName = frameworkInputElement.Name;
            if (string.IsNullOrEmpty(elementName))
            {
                Debug.WriteLine("DependencyPropertySetting: missing name for framework element " + frameworkInputElement);
                return; // name is required
            }

            if (e.NewValue is not DependencyProperty dependencyProperty)
            {
                Debug.WriteLine("DependencyPropertySetting: missing dependency property");
                return;
            }

            // search on the parent-tree for application settings
            ApplicationSettings applicationSettings = FindParentSettings(obj);
            if (applicationSettings is null)
            {
                Debug.WriteLine("DependencyPropertySetting: missing application settings in parent hierarchy");
                return;
            }

            string settingName = string.Concat(elementName, ".", dependencyProperty.Name);
            applicationSettings.Settings.Add(
                new DependencyPropertySetting(settingName, obj, dependencyProperty));
        } // OnDependencyPropertyChanged

        // ----------------------------------------------------------------------
        private static ApplicationSettings FindParentSettings(DependencyObject obj)
        {
            while (obj is not null)
            {
                if (obj.ReadLocalValue(ApplicationSettingsProperty) is ApplicationSettings applicationSettings)
                {
                    return applicationSettings;
                }
                obj = LogicalTreeHelper.GetParent(obj);
            }
            return null;
        } // FindParentSettings

        // ----------------------------------------------------------------------
        // members
        private readonly DependencyObject dependencyObject;
        private readonly DependencyProperty dependencyProperty;

    } // class DependencyPropertySetting
} // namespace EPG.Configuration
// -- EOF -------------------------------------------------------------------
