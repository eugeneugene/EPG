using BFM.Code;
using BFM.Models;
using CSAdapter;
using Microsoft.Win32;
using System;
using System.Diagnostics;
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
        public ImportModel Model { get; }

        private readonly OpenFileDialog BloomFileDialog;
        private readonly OpenFileDialog TextFileDialog;
        private readonly BloomLoader linesCounter;
        private readonly CancellationTokenSource cancellationTokenSource;

        private Task? task;
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
            Model = new();
            linesCounter = new(Model);
            cancellationTokenSource = new();
            task = null;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = Model;
        }

        private void BloomFilterCreateCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var res = BloomFileDialog.ShowDialog(this) ?? false;
            if (res)
                Model.BloomFilter = BloomFileDialog.FileName;
        }

        private void TextFileOpenCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var res = TextFileDialog.ShowDialog(this) ?? false;
            if (res)
                Model.TextFile = TextFileDialog.FileName;
        }

        private void BloomFilterImportCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            task = Task.Run(() => Import());
            Model.ImportTask = true;
        }

        private void Import()
        {
            if (Model.Lines is null || Model.State != LinesCounterState.FINISH || string.IsNullOrEmpty(Model.BloomFilter) || string.IsNullOrEmpty(Model.TextFile))
                return;

            using Bloom bloom = new();

            try
            {
                bloom.Create(Model.BloomFilter);
                bloom.Allocate(Model.Lines.Value);

                uint lines = 0;
                double p;
                double p10 = 0;

                Model.ErrorMsg = "Importing...";
                foreach (var line in Model.Strings)
                {
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    bloom.PutString(line);
                    lines++;
                    p = Math.Round(100.0 * lines / Model.Lines.Value, 1, MidpointRounding.ToZero);
                    if (p > p10)
                    {
                        p10 = p;
                        Model.ErrorMsg = $"Importing... {p10:F1}% imported";
                    }
                }
                bloom.Store();
                bloom.Close();
                Model.ErrorMsg = "Import finished";
            }
            catch (Exception ex)
            {
                Model.ErrorMsg = ex.Message;
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                bloom.Abort();
                Model.ImportTask = false;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    cancellationTokenSource?.Cancel();
                    cancellationTokenSource?.Dispose();
                    linesCounter.Dispose();
                    task?.Dispose();
                    Model.Dispose();
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
    }
}
