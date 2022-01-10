using BFM.Models;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;

namespace BFM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly OpenFileDialog BloomFileDialog;
        private readonly MainWindowModel model = new();

        public MainWindow()
        {
            InitializeComponent();
            BloomFileDialog = new()
            {
                CheckFileExists = false,
                CheckPathExists = true,
                DefaultExt = ".bf",
                Filter = "Bloom Filter|*.bf|Any file|*.*",
                Title = "Open Bloom Filter"
            };
        }

        private void BloomFilterOpenCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var res = BloomFileDialog.ShowDialog(this) ?? false;
            if (res)
                model.OpenBloom(BloomFileDialog.FileName);
            e.Handled = true;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = model;
        }

        private void BloomFilterImportCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            using ImportDialog importDialog = new();
            importDialog.ShowDialog();
            if (importDialog.Model is not null && importDialog.Model.State == Code.LinesCounterState.FINISH && !string.IsNullOrEmpty(importDialog.Model.BloomFilter))
                model.OpenBloom(importDialog.Model.BloomFilter);
        }

        private void BloomFilterCloseCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            model.CloseBloom();
        }
    }
}
