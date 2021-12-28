using BFM.Code;
using BFM.Models;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;

namespace BFM
{
    /// <summary>
    /// Interaction logic for ImportDialog.xaml
    /// </summary>
    public partial class ImportDialog : Window
    {
        private readonly OpenFileDialog BloomFileDialog;
        private readonly OpenFileDialog TextFileDialog;
        private readonly ImportModel model;
        private readonly LinesCounter linesCounter;

        public ImportDialog()
        {
            InitializeComponent();
            BloomFileDialog = new()
            {
                CheckFileExists = false,
                DefaultExt = ".bf",
                Filter = "Bloom Filter|*.bf|Any file|*.*",
                Title = "Open Bloom Filter"
            };
            TextFileDialog = new()
            {
                CheckFileExists = true,
                DefaultExt = ".txt",
                Filter = "Text file|*.txt|Any file|*.*",
                Title = "Open Text file"
            };
            model = new();
            linesCounter = new(model);
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = model;
        }

        private void BloomFilterCreateCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var res = BloomFileDialog.ShowDialog(this) ?? false;
            if (res)
                model.BloomFilter = BloomFileDialog.FileName;
        }

        private void TextFileOpenCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var res = TextFileDialog.ShowDialog(this) ?? false;
            if (res)
                model.TextFile = TextFileDialog.FileName;
        }

        private void BloomFilterImportCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
        }
    }
}
