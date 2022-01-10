using BFM.Code;
using BFM.Models;
using BloomCS;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
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
        private readonly LinesCounter linesCounter;
        private readonly CancellationTokenSource cancellationTokenSource;

        private Task? task;
        private Regex? regex;
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
            Model.PropertyChanged += ModelPropertyChanged;
            linesCounter = new(Model);
            cancellationTokenSource = new();
            task = null;
            regex = null;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = Model;
        }

        private void ModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!ReferenceEquals(Model, sender))
                return;

            if (e.PropertyName == nameof(ImportModel.Comments) && !string.IsNullOrEmpty(Model.Comments))
                regex = new(Model.Comments);
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
            task = Task.Run(async () => await ImportAsync());
            Model.ImportTask = true;
        }

        private async Task ImportAsync()
        {
            if (Model.Lines is null || Model.State != LinesCounterState.FINISH || string.IsNullOrEmpty(Model.BloomFilter) || string.IsNullOrEmpty(Model.TextFile))
                return;

            using Bloom bloom = new();

            try
            {
                bloom.Create(Model.BloomFilter);
                bloom.Allocate(Model.Lines.Value);

                using StreamReader reader = new(Model.TextFile, new FileStreamOptions() { Mode = FileMode.Open, Access = FileAccess.Read, Options = FileOptions.SequentialScan });
                uint lines = 0;
                double p;
                double p10 = 0;

                Model.ErrorMsg = "Importing...";
                while (true)
                {
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var line = await reader.ReadLineAsync();
                    if (line is null)
                        break;
                    if (regex is null || !regex.IsMatch(line))
                    {
                        bloom.PutString(line);
                        lines++;
                        p = Math.Round(100.0 * lines / Model.Lines.Value, 1, MidpointRounding.ToZero);
                        if (p > p10)
                        {
                            p10 = p;
                            Model.ErrorMsg = $"Importing... {p10:F1}% imported";
                        }
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
                    Model.PropertyChanged -= ModelPropertyChanged;
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
