using BFM.Models;
using BloomCS;
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
        //private readonly OpenFileDialog TextFileDialog;
        private readonly MainWindowModel model = new();
        private readonly Bloom bloom = new();

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
            //TextFileDialog = new()
            //{
            //    CheckFileExists = true,
            //    DefaultExt = ".txt",
            //    Filter = "Text File|*.txt|Any file|*.*",
            //    Title = "Import Text File"
            //};
            model.MainWindowBloomFilterModel = new();
        }

        private void BloomFilterOpenCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var res = BloomFileDialog.ShowDialog(this) ?? false;
            if (res)
            {
                bloom.Close();
                model.MainWindowBloomFilterModel = new();
                try
                {
                    bloom.Open(BloomFileDialog.FileName);
                    model.MainWindowBloomFilterModel.BloomFilterStatus = "OK";
                    model.MainWindowBloomFilterModel.ValidBloomFilter = true;
                    model.MainWindowBloomFilterModel.BloomFilterPath = BloomFileDialog.FileName;
                    model.MainWindowBloomFilterModel.BloomFilterFile = System.IO.Path.GetFileName(BloomFileDialog.FileName);
                    model.MainWindowBloomFilterModel.HeaderVersion = bloom.HeaderVersion();
                    model.MainWindowBloomFilterModel.HeaderSize = bloom.HeaderSize();
                    model.MainWindowBloomFilterModel.HeaderHashFunc = bloom.HeaderHashFunc();
                }
                catch (BloomException ex)
                {
                    bloom.Abort();
                    model.MainWindowBloomFilterModel.ValidBloomFilter = false;
                    model.MainWindowBloomFilterModel.BloomFilterStatus = ex.Message;
                    model.MainWindowBloomFilterModel.BloomFilterPath = BloomFileDialog.FileName;
                    model.MainWindowBloomFilterModel.BloomFilterFile = System.IO.Path.GetFileName(BloomFileDialog.FileName);
                }
            }
            e.Handled = true;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = model;
        }

        private void BloomFilterImportCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ImportDialog importDialog = new ();
            importDialog.ShowDialog();
        }
    }
}
