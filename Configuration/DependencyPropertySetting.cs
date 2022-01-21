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
        public DependencyPropertySetting(DependencyObject dependencyObject, DependencyProperty dependencyProperty) : this(dependencyObject, dependencyProperty, null)
        {
        } // DependencyPropertySetting

        // ----------------------------------------------------------------------
        public DependencyPropertySetting(DependencyObject dependencyObject, DependencyProperty dependencyProperty, object defaultValue) : this(dependencyProperty.Name, dependencyObject, dependencyProperty, defaultValue)
        {
        } // DependencyPropertySetting

        // ----------------------------------------------------------------------
        public DependencyPropertySetting(string name, DependencyObject dependencyObject, DependencyProperty dependencyProperty) : this(name, dependencyObject, dependencyProperty, null)
        {
        } // DependencyPropertySetting

        // ----------------------------------------------------------------------
        public DependencyPropertySetting(string name, DependencyObject dependencyObject, DependencyProperty dependencyProperty, object defaultValue) : base(name, defaultValue)
        {
            this.dependencyObject = dependencyObject ?? throw new ArgumentNullException(nameof(dependencyObject));
            this.dependencyProperty = dependencyProperty ?? throw new ArgumentNullException(nameof(dependencyProperty));
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
        public static void SetProperty(DependencyObject obj, DependencyProperty dependencyProperty)
        {
            obj.SetValue(PropertyProperty, dependencyProperty);
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
                if (originalValue == null && LoadUndefinedValue == false)
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
                if (value == null && SaveUndefinedValue == false)
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
        private static void OnDependencyPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is not IFrameworkInputElement frameworkInputElement)
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
            ApplicationSettings applicationSettings = FindParentSettings(dependencyObject);
            if (applicationSettings == null)
            {
                Debug.WriteLine("DependencyPropertySetting: missing application settings in parent hierarchy");
                return;
            }

            string settingName = string.Concat(elementName, ".", dependencyProperty.Name);
            applicationSettings.Settings.Add(
                new DependencyPropertySetting(settingName, dependencyObject, dependencyProperty));
        } // OnDependencyPropertyChanged

        // ----------------------------------------------------------------------
        private static ApplicationSettings FindParentSettings(DependencyObject element)
        {
            while (element != null)
            {
                if (element.ReadLocalValue(ApplicationSettingsProperty) is ApplicationSettings applicationSettings)
                {
                    return applicationSettings;
                }
                element = LogicalTreeHelper.GetParent(element);
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
