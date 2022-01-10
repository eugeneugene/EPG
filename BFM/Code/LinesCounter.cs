using BFM.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BFM.Code
{
    internal class LinesCounter : IDisposable
    {
        private readonly ImportModel model;

        private bool disposedValue;

        private Task? task;
        private CancellationTokenSource? cancellationTokenSource;
        private Regex? regex;

        public LinesCounter(ImportModel Model)
        {
            model = Model ?? throw new ArgumentNullException(nameof(Model));
            model.PropertyChanged += ModelPropertyChanged;
            task = null;
            cancellationTokenSource = null;
            regex = null;
        }

        private void ModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!ReferenceEquals(model, sender))
                return;

            if (e.PropertyName == nameof(ImportModel.TextFile) || e.PropertyName == nameof(ImportModel.Comments))
                Restart();
        }

        public void Restart()
        {
            if (task is not null && !task.IsCompleted)
            {
                cancellationTokenSource?.Cancel();
                task.Wait();
            }
            task?.Dispose();
            task = Task.Run(async () =>
            {
                cancellationTokenSource = new();

                try
                {
                    model.State = LinesCounterState.RUN;
                    await InternalCounterAsync();
                    model.State = LinesCounterState.FINISH;
                }
                catch (Exception ex)
                {
                    model.State = LinesCounterState.ERROR;
                    model.Lines = null;
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    cancellationTokenSource.Dispose();
                    cancellationTokenSource = null;
                }
            });
        }

        private async Task InternalCounterAsync()
        {
            if (model.TextFile is null)
                return;

            if (!string.IsNullOrEmpty(model.Comments))
                regex = new(model.Comments);

            using StreamReader reader = new(model.TextFile, new FileStreamOptions() { Mode = FileMode.Open, Access = FileAccess.Read, Options = FileOptions.SequentialScan });
            model.Lines = 0;
            while (true)
            {
                cancellationTokenSource!.Token.ThrowIfCancellationRequested();
                var line = await reader.ReadLineAsync();
                if (line is null)
                    break;
                if (regex is null || !regex.IsMatch(line))
                    model.Lines++;
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
                    model.PropertyChanged -= ModelPropertyChanged;
                    task?.Dispose();
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
