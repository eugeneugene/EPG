// -- FILE ------------------------------------------------------------------
// name       : UserConfig.cs
// created    : Jani Giannoudis - 2008.05.16
// language   : c#
// environment: .NET 2.0
// --------------------------------------------------------------------------
using System.Configuration;
using System.IO;

namespace EPG.Configuration
{

    // ------------------------------------------------------------------------
    public class UserConfig
    {

        // ----------------------------------------------------------------------
        public UserConfig(System.Configuration.Configuration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        } // UserConfig

        // ----------------------------------------------------------------------
        public System.Configuration.Configuration Configuration
        {
            get { return configuration; }
        } // Configuration

        // ----------------------------------------------------------------------
        public string FilePath
        {
            get { return configuration.FilePath; }
        } // FilePath

        // ----------------------------------------------------------------------
        public string FileName
        {
            get { return new FileInfo(configuration.FilePath).Name; }
        } // FileName

        // ----------------------------------------------------------------------
        public List<ClientSettingsSection> Sections
        {
            get
            {
                List<ClientSettingsSection> sections = new();

                foreach (ConfigurationSectionGroup sectionGroup in configuration.SectionGroups)
                {
                    if (sectionGroup is not UserSettingsGroup userSettingsGroup)
                    {
                        continue;
                    }

                    foreach (ConfigurationSection section in userSettingsGroup.Sections)
                    {
                        if (section is not ClientSettingsSection clientSettingsSection)
                        {
                            continue;
                        }

                        sections.Add(clientSettingsSection);
                    }
                }

                return sections;
            }
        } // Sections

        // ----------------------------------------------------------------------
        private UserSettingsGroup UserSettingsGroup
        {
            get
            {
                foreach (ConfigurationSectionGroup sectionGroup in configuration.SectionGroups)
                {
                    if (sectionGroup is UserSettingsGroup userSettingsGroup)
                    {
                        return userSettingsGroup;
                    }
                }
                return null;
            }
        } // UserSettingsGroup

        // ----------------------------------------------------------------------
        public void RemoveAllSections()
        {
            UserSettingsGroup userSettingsGroup = UserSettingsGroup;
            if (userSettingsGroup is null)
            {
                return;
            }
            configuration.SectionGroups.Remove(userSettingsGroup.SectionGroupName);
        } // RemoveAllSections

        // ----------------------------------------------------------------------
        public void RemoveSection(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            UserSettingsGroup userSettingsGroup = UserSettingsGroup;
            if (userSettingsGroup is null)
            {
                return;
            }

            ConfigurationSection section = userSettingsGroup.Sections[name];
            if (section is null)
            {
                throw new InvalidOperationException("invalid section " + name);
            }
            userSettingsGroup.Sections.Remove(name);
        } // RemoveSection

        // ----------------------------------------------------------------------
        public bool HasSameSettings(UserConfig compareUserConfig)
        {
            if (compareUserConfig is null)
            {
                throw new ArgumentNullException(nameof(compareUserConfig));
            }

            if (configuration.SectionGroups.Count != compareUserConfig.configuration.SectionGroups.Count)
            {
                return false;
            }

            foreach (ConfigurationSectionGroup compareSectionGroup in compareUserConfig.configuration.SectionGroups)
            {
                if (compareSectionGroup is not UserSettingsGroup compareUserSettingsGroup)
                {
                    continue;
                }

                if (configuration.SectionGroups[compareSectionGroup.Name] is not UserSettingsGroup userSettingsGroup || userSettingsGroup.Sections.Count != compareSectionGroup.Sections.Count)
                {
                    return false;
                }

                foreach (ConfigurationSection compareSection in compareSectionGroup.Sections)
                {
                    if (compareSection is not ClientSettingsSection compareClientSettingsSection)
                    {
                        continue;
                    }

                    if (userSettingsGroup.Sections[compareSection.SectionInformation.Name] is not ClientSettingsSection clientSettingsSection || clientSettingsSection.Settings.Count != compareClientSettingsSection.Settings.Count)
                    {
                        return false;
                    }

                    foreach (SettingElement compateSettingElement in compareClientSettingsSection.Settings)
                    {
                        SettingElement settingElement = clientSettingsSection.Settings.Get(compateSettingElement.Name);
                        if (settingElement is null || !settingElement.Value.ValueXml.InnerXml.Equals(compateSettingElement.Value.ValueXml.InnerXml))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        } // HasSameSettings

        // ----------------------------------------------------------------------
        public void Save()
        {
            configuration.Save();
        } // Save

        // ----------------------------------------------------------------------
        public void SaveAs(string fileName)
        {
            configuration.SaveAs(fileName);
        } // SaveAs

        // ----------------------------------------------------------------------
        public void Import(UserConfig importUserConfig, bool overwriteSettings)
        {
            if (importUserConfig is null)
            {
                throw new ArgumentNullException(nameof(importUserConfig));
            }

            foreach (ConfigurationSectionGroup importSectionGroup in importUserConfig.configuration.SectionGroups)
            {
                if (importSectionGroup is not UserSettingsGroup importUserSettingsGroup)
                {
                    continue;
                }

                if (configuration.SectionGroups[importSectionGroup.Name] is not UserSettingsGroup userSettingsGroup)
                {
                    userSettingsGroup = new UserSettingsGroup();
                    configuration.SectionGroups.Add(importSectionGroup.Name, userSettingsGroup);
                }

                foreach (ConfigurationSection importSection in importSectionGroup.Sections)
                {
                    if (importSection is not ClientSettingsSection importClientSettingsSection)
                    {
                        continue;
                    }

                    if (userSettingsGroup.Sections[importSection.SectionInformation.Name] is not ClientSettingsSection clientSettingsSection)
                    {
                        clientSettingsSection = new ClientSettingsSection();
                        userSettingsGroup.Sections.Add(importSection.SectionInformation.Name, clientSettingsSection);
                    }

                    foreach (SettingElement importSettingElement in importClientSettingsSection.Settings)
                    {
                        bool newSetting = false;

                        SettingElement settingElement = clientSettingsSection.Settings.Get(importSettingElement.Name);
                        if (settingElement is null)
                        {
                            newSetting = true;
                            settingElement = new SettingElement();
                            settingElement.Name = importSettingElement.Name;
                            settingElement.SerializeAs = importSettingElement.SerializeAs;
                            clientSettingsSection.Settings.Add(settingElement);
                        }

                        if (!newSetting && !overwriteSettings)
                        {
                            continue;
                        }

                        SettingValueElement settingValueElement = new();
                        settingValueElement.ValueXml = importSettingElement.Value.ValueXml;
                        settingElement.SerializeAs = importSettingElement.SerializeAs;
                        settingElement.Value = settingValueElement;
                        clientSettingsSection.Settings.Add(settingElement);
                    }
                }
            }
        } // Import

        // ----------------------------------------------------------------------
        public static UserConfig FromOpenExe()
        {
            System.Configuration.Configuration configuration =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            return new UserConfig(configuration);
        } // UserSettings

        // ----------------------------------------------------------------------
        public static UserConfig FromFile(string path)
        {
            ExeConfigurationFileMap exeConfigurationFileMap = new();
            exeConfigurationFileMap.ExeConfigFilename = path;
            System.Configuration.Configuration configuration =
                ConfigurationManager.OpenMappedExeConfiguration(exeConfigurationFileMap, ConfigurationUserLevel.None);
            return new UserConfig(configuration);
        } // FromFile

        // ----------------------------------------------------------------------
        // members
        private readonly System.Configuration.Configuration configuration;

    } // class UserConfig
} // namespace EPG.Configuration
// -- EOF -------------------------------------------------------------------
