// -- FILE ------------------------------------------------------------------
// name       : ListViewSetting.cs
// created    : Jani Giannoudis - 2008.05.09
// language   : c#
// environment: .NET 2.0
// --------------------------------------------------------------------------
using System.Configuration;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace EPG.Configuration
{
    // ------------------------------------------------------------------------
    public class ListViewSetting : Setting
    {
        // ----------------------------------------------------------------------
        public static readonly DependencyProperty SettingProperty = DependencyProperty.RegisterAttached(
            "Setting",
            typeof(string),
            typeof(ListViewSetting),
            new FrameworkPropertyMetadata(OnListViewSettingChanged));

        // ----------------------------------------------------------------------
        public ListViewSetting(ListView listView) : this(listView.Name, listView)
        {
        } // ListViewSetting

        // ----------------------------------------------------------------------
        public ListViewSetting(string name, ListView listView)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.name = name;
            this.listView = listView ?? throw new ArgumentNullException(nameof(listView));
            listView.Initialized += ListViewInitialized;
            SetupGridViewColumns();
        } // ListViewSetting

        // ----------------------------------------------------------------------
        public string Name
        {
            get { return name; }
        } // Name

        // ----------------------------------------------------------------------
        public ListView ListView
        {
            get { return listView; }
        } // ListView

        // ----------------------------------------------------------------------
        public bool UseWidth
        {
            get { return useWidth; }
            set { useWidth = value; }
        } // UseWidth

        // ----------------------------------------------------------------------
        public bool UseDisplayIndex
        {
            get { return useDisplayIndex; }
            set { useDisplayIndex = value; }
        } // UseDisplayIndex

        // ----------------------------------------------------------------------
        public override bool HasChanged
        {
            get
            {
                GridViewColumnSetting[] originalColumnSettings = OriginalColumnSettings;
                GridViewColumnSetting[] columnSettings = ColumnSettings;
                if (originalColumnSettings is null || columnSettings is null ||
                    originalColumnSettings == columnSettings)
                {
                    return false;
                }

                if (originalColumnSettings.Length != columnSettings.Length)
                {
                    return true;
                }

                for (int i = 0; i < originalColumnSettings.Length; i++)
                {
                    if (!originalColumnSettings[i].Equals(columnSettings[i]))
                    {
                        return true;
                    }
                }

                return false;
            }
        } // HasChanged

        // ----------------------------------------------------------------------
        private GridViewColumnSetting[] OriginalColumnSettings
        {
            get
            {
                return LoadValue(
                    Name,
                    typeof(GridViewColumnSetting[]),
                    SettingsSerializeAs.Xml,
                    null) as GridViewColumnSetting[];
            }
        } // OriginalColumnSettings

        // ----------------------------------------------------------------------
        private GridViewColumnSetting[] ColumnSettings
        {
            get
            {
                if (listView.View is not GridView gridView || gridView.Columns.Count == 0)
                {
                    return null;
                }

                List<GridViewColumnSetting> columnSettings =
                    new(gridView.Columns.Count);
                for (int displayIndex = 0; displayIndex < gridView.Columns.Count; displayIndex++)
                {
                    GridViewColumn gridViewColumn = gridView.Columns[displayIndex];
                    int index = gridViewColumns.IndexOf(gridViewColumn);
                    columnSettings.Add(new GridViewColumnSetting(gridViewColumn, index, displayIndex));
                }

                return columnSettings.ToArray();
            }
        } // ColumnSettings

        // ----------------------------------------------------------------------
        public static string GetSetting(DependencyObject dependencyObject)
        {
            return dependencyObject.GetValue(SettingProperty) as string;
        } // SetSetting

        // ----------------------------------------------------------------------
        public static void SetSetting(DependencyObject dependencyObject, string settingKey)
        {
            dependencyObject.SetValue(SettingProperty, settingKey);
        } // SetSetting

        // ----------------------------------------------------------------------
        public override void Load()
        {
            try
            {
                if (listView.View is not GridView gridView || gridView.Columns.Count == 0)
                {
                    return;
                }

                GridViewColumnSetting[] columnSettings = OriginalColumnSettings;
                if (columnSettings is null || columnSettings.Length == 0)
                {
                    return;
                }

                for (int displayIndex = 0; displayIndex < columnSettings.Length; displayIndex++)
                {
                    GridViewColumnSetting columnSetting = columnSettings[displayIndex];
                    if (columnSetting.Index < 0 || columnSetting.Index >= gridViewColumns.Count)
                    {
                        continue;
                    }

                    GridViewColumn gridViewColumn = gridViewColumns[columnSetting.Index];

                    if (useWidth)
                    {
                        gridViewColumn.Width = columnSetting.Width;
                    }

                    if (!useDisplayIndex)
                    {
                        continue;
                    }

                    if (columnSetting.Index == columnSetting.DisplayIndex)
                    {
                        continue;
                    }

                    int oldIndex = gridView.Columns.IndexOf(gridViewColumn);
                    gridView.Columns.Move(oldIndex, columnSetting.DisplayIndex);
                }
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
                GridViewColumnSetting[] columnSettings = ColumnSettings;
                if (columnSettings is null)
                {
                    return;
                }

                SaveValue(
                    Name,
                    typeof(GridViewColumnSetting[]),
                    SettingsSerializeAs.Xml,
                    columnSettings,
                    null);
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
        public override string ToString()
        {
            return string.Concat(name, " (ListView)");
        } // ToString

        // ----------------------------------------------------------------------
        private void SetupGridViewColumns()
        {
            gridViewColumns.Clear();

            if (listView.View is not GridView gridView)
            {
                return;
            }

            for (int i = 0; i < gridView.Columns.Count; i++)
            {
                gridViewColumns.Add(gridView.Columns[i]);
            }
        } // SetupGridViewColumns

        // ----------------------------------------------------------------------
        private void ListViewInitialized(object sender, EventArgs e)
        {
            SetupGridViewColumns();
        } // ListViewInitialized

        // ----------------------------------------------------------------------
        private static void OnListViewSettingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is not ListView listView)
            {
                Debug.WriteLine("ListViewSetting: invalid property attachment");
                return;
            }

            // search on the parent-tree for application settings
            ApplicationSettings applicationSettings = FindParentSettings(dependencyObject);
            if (applicationSettings is null)
            {
                Debug.WriteLine("ListViewSetting: missing application settings in parent hierarchy");
                return;
            }

            applicationSettings.Settings.Add(new ListViewSetting(listView));
        } // OnListViewSettingChanged

        // ----------------------------------------------------------------------
        private static ApplicationSettings FindParentSettings(DependencyObject element)
        {
            while (element is not null)
            {
                if (element.ReadLocalValue(DependencyPropertySetting.ApplicationSettingsProperty) is ApplicationSettings applicationSettings)
                {
                    return applicationSettings;
                }
                element = LogicalTreeHelper.GetParent(element);
            }
            return null;
        } // FindParentSettings

        // ----------------------------------------------------------------------
        // members
        private readonly ListView listView;
        private readonly List<GridViewColumn> gridViewColumns = new();

        private readonly string name;
        private bool useWidth = true;
        private bool useDisplayIndex = true;

    } // class ListViewSetting
} // namespace EPG.Configuration
// -- EOF -------------------------------------------------------------------
