using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace EPG.Code
{
    internal class EPGSettingsProvider : LocalFileSettingsProvider
    {
        private static string GetUserSettingsPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EPG", "EPG.config");

        public override string Name => nameof(EPGSettingsProvider);

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection properties)
        {
            try
            {
                string userSettingsPath = GetUserSettingsPath;
                string? sectionName = GetSectionName(context);
                if (sectionName is null)
                    throw new InvalidOperationException("Section name cannot be null");

                SettingElementCollection userSettings = ReadSettingsFromFile(userSettingsPath, sectionName);
                return MergeSettingsIntoPropertyValues(userSettings, properties);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to GetPropertyValues.", ex);
                throw;
            }
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection values)
        {
            try
            {
                string path = GetUserSettingsPath;
                string? sectionName = GetSectionName(context);
                if (sectionName is null)
                    throw new InvalidOperationException("Section name cannot be null");

                WriteSettingsToFile(path, sectionName, values);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to SetPropertyValues.", ex);
                throw;
            }
        }

        private static string? GetSectionName(SettingsContext context)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            return context["GroupName"]?.ToString();
        }

        private static System.Configuration.Configuration GetConfiguration(string path)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));

            return ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
            {
                ExeConfigFilename = path
            }, ConfigurationUserLevel.None);
        }

        private static SettingElementCollection GetSettingElementCollection(System.Configuration.Configuration config, string sectionName)
        {
            ConfigurationSectionGroup userSettings = config.GetSectionGroup("userSettings");
            if (userSettings is null)
            {
                userSettings = new UserSettingsGroup();
                config.SectionGroups.Add("userSettings", userSettings);
            }

            if (userSettings.Sections.Get(sectionName) is not ClientSettingsSection section)
            {
                section = new ClientSettingsSection();
                userSettings.Sections.Add(sectionName, section);
            }

            return section.Settings;
        }

        private static SettingElementCollection ReadSettingsFromFile(string path, string sectionName)
        {
            try
            {
                if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(sectionName))
                    return GetSettingElementCollection(GetConfiguration(path), sectionName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to read settings.", ex);
            }

            return new SettingElementCollection();
        }

        private static SettingsPropertyValueCollection MergeSettingsIntoPropertyValues(SettingElementCollection userSettings, SettingsPropertyCollection properties)
        {
            SettingsPropertyValueCollection values = new();
            foreach (SettingsProperty property in properties)
            {
                SettingsPropertyValue value = new(property);
                ApplySettingToValue(value, userSettings);
                value.IsDirty = false;
                values.Add(value);
            }

            return values;
        }

        private static void ApplySettingToValue(SettingsPropertyValue value, SettingElementCollection settings)
        {
            SettingElement setting = settings.Get(value.Name);
            if (setting != null)
            {
                value.SerializedValue = setting.Value.ValueXml.InnerText;
                value.Deserialized = false;
            }
        }

        private static void WriteSettingsToFile(string path, string sectionName, SettingsPropertyValueCollection values)
        {
            try
            {
                if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(sectionName))
                {
                    var configuration = GetConfiguration(path);
                    UpdateSettingsFromPropertyValues(GetSettingElementCollection(configuration, sectionName), values);
                    configuration.Save();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to write settings.", ex);
            }
        }

        private static void UpdateSettingsFromPropertyValues(SettingElementCollection settings, SettingsPropertyValueCollection values)
        {
            foreach (SettingsPropertyValue value in values)
            {
                if (value.IsDirty)
                {
                    SettingElement element = settings.Get(value.Name);
                    if (element == null)
                    {
                        element = new SettingElement(value.Name, SettingsSerializeAs.String);
                        settings.Add(element);
                    }

                    element.SerializeAs = SettingsSerializeAs.String;
                    element.Value.ValueXml = CreateXmlValue(value.SerializedValue);
                }
            }
        }

        private static XmlNode CreateXmlValue(object serializedValue)
        {
            XmlElement xmlElement = new XmlDocument().CreateElement("value");
            if (serializedValue is null)
                xmlElement.InnerText = string.Empty;
            else
                xmlElement.InnerText = serializedValue.ToString() ?? string.Empty;
            return xmlElement;
        }
    }
}
