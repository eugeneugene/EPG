using BFM.Code;
using BFM.Models;
using BloomCS;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BFM
{
    /// <summary>
    /// Interaction logic for ImportDialog.xaml
    /// </summary>
    public partial class ImportDialog : Window, IDisposable
    {
        private readonly OpenFileDialog BloomFileDialog;
        private readonly OpenFileDialog TextFileDialog;
        private readonly ImportModel model;
        private readonly LinesCounter linesCounter;
        private readonly CancellationTokenSource cancellationTokenSource;

        private bool disposedValue;

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
            cancellationTokenSource = new();
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
            var task = Task.Run(async () => await ImportAsync());
            model.ImportTask = true;
        }

        private async Task ImportAsync()
        {
            if (model.Lines is null || model.State != LinesCounterState.FINISH || string.IsNullOrEmpty(model.BloomFilter) || string.IsNullOrEmpty(model.TextFile))
                return;

            using Bloom bloom = new();

            try
            {
                bloom.Create(model.BloomFilter);
                bloom.Allocate(model.Lines.Value);

                StreamReader reader = new(model.TextFile, new FileStreamOptions() { Mode = FileMode.Open, Access = FileAccess.Read, Options = FileOptions.SequentialScan });
                uint lines = 0;
                double p;
                double p10 = 0;

                model.ErrorMsg = "Importing...";
                while (true)
                {
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var line = await reader.ReadLineAsync();
                    if (line is null)
                        break;
                    bloom.PutString(line);
                    lines++;
                    p = Math.Round(100.0 * lines / model.Lines.Value, 1, MidpointRounding.ToZero);
                    if (p > p10)
                    {
                        p10 = p;
                        model.ErrorMsg = $"Importing... {p10:F1}% imported";
                    }
                }
                bloom.Store();
                bloom.Close();
                model.ErrorMsg = "Import finished";
            }
            catch (Exception ex)
            {
                model.ErrorMsg = ex.Message;
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                bloom.Abort();
                model.ImportTask = false;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    linesCounter.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cancellationTokenSource.Cancel();
        }
    }
}
