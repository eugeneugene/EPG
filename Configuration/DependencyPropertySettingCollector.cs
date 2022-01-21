// -- FILE ------------------------------------------------------------------
// name       : DependencyPropertySettingCollector.cs
// created    : Jani Giannoudis - 2008.05.09
// language   : c#
// environment: .NET 2.0
// --------------------------------------------------------------------------
using System.Diagnostics;
using System.Windows;

namespace EPG.Configuration
{
    // ------------------------------------------------------------------------
    public class DependencyPropertySettingCollector : SettingCollector
    {
        // ----------------------------------------------------------------------
        public DependencyPropertySettingCollector(FrameworkElement owner, DependencyProperty dependencyProperty)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            this.dependencyProperty = dependencyProperty ?? throw new ArgumentNullException(nameof(dependencyProperty));
        } // DependencyPropertySettingCollector

        // ----------------------------------------------------------------------
        public FrameworkElement Owner
        {
            get { return owner; }
        } // Owner

        // ----------------------------------------------------------------------
        public DependencyProperty DependencyProperty
        {
            get { return dependencyProperty; }
        } // DependencyProperty

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
                    bool add = dependencyProperty.OwnerType.IsAssignableFrom(frameworkElement.GetType());
                    // ReSharper restore UseMethodIsInstanceOfType

                    if (add && string.IsNullOrEmpty(frameworkElement.Name))
                    {
                        add = false;
                        Debug.WriteLine("DependencyPropertySettingCollector: missing name for framework element " + frameworkElement);
                    }

                    if (add && !OnCollectingSetting(frameworkElement))
                    {
                        add = false;
                    }

                    if (add)
                    {
                        string settingName = string.Concat(frameworkElement.Name, ".", dependencyProperty.Name);
                        DependencyPropertySetting dependencyPropertySetting = new(settingName, frameworkElement, dependencyProperty);
                        dependencyPropertySetting.DefaultValue = dependencyPropertySetting.Value;
                        ApplicationSettings.Settings.Add(dependencyPropertySetting);
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
        private readonly DependencyProperty dependencyProperty;

    } // class DependencyPropertySettingCollector
} // namespace EPG.Configuration
// -- EOF -------------------------------------------------------------------
