// -- FILE ------------------------------------------------------------------
// name       : PropertySettingCollector.cs
// created    : Jani Giannoudis - 2008.05.09
// language   : c#
// environment: .NET 2.0
// --------------------------------------------------------------------------
using System.Diagnostics;
using System.Windows;

namespace EPG.Configuration
{
    // ------------------------------------------------------------------------
    public class PropertySettingCollector : SettingCollector
    {
        // ----------------------------------------------------------------------
        public PropertySettingCollector(FrameworkElement owner, Type elementType, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            this.elementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
            this.propertyName = propertyName;
        } // PropertySettingCollector

        // ----------------------------------------------------------------------
        public FrameworkElement Owner
        {
            get { return owner; }
        } // Owner

        // ----------------------------------------------------------------------
        public Type ElementType
        {
            get { return elementType; }
        } // ElementType

        // ----------------------------------------------------------------------
        public string PropertyName
        {
            get { return propertyName; }
        } // PropertyName

        // ----------------------------------------------------------------------
        public override void Collect()
        {
            Collect(owner, true);
        } // Collect

        // ----------------------------------------------------------------------
        private void Collect(DependencyObject currentObject, bool recursive)
        {
            foreach (object child in LogicalTreeHelper.GetChildren(currentObject))
            {
                if (child is not DependencyObject dependencyObject)
                {
                    continue;
                }

                if (child is FrameworkElement frameworkElement)
                {
                    // ReSharper disable UseMethodIsInstanceOfType
                    bool add = elementType.IsAssignableFrom(frameworkElement.GetType());
                    // ReSharper restore UseMethodIsInstanceOfType

                    if (add && string.IsNullOrEmpty(frameworkElement.Name))
                    {
                        add = false;
                        Debug.WriteLine("PropertySettingCollector: missing name for framework element " + frameworkElement);
                    }

                    if (add && !OnCollectingSetting(frameworkElement))
                    {
                        add = false;
                    }

                    if (add)
                    {
                        string settingName = string.Concat(frameworkElement.Name, ".", propertyName);
                        PropertySetting propertySetting = new(settingName, frameworkElement, propertyName);
                        propertySetting.DefaultValue = propertySetting.Value;
                        ApplicationSettings.Settings.Add(propertySetting);
                    }
                }

                if (recursive)
                {
                    Collect(dependencyObject, true);
                }
            }
        } // Collect

        // ----------------------------------------------------------------------
        // members
        private readonly FrameworkElement owner;
        private readonly Type elementType;
        private readonly string propertyName;

    } // class PropertySettingCollector
} // namespace EPG.Configuration
// -- EOF -------------------------------------------------------------------
