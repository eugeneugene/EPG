using BFM.Models;
using System;
using System.Diagnostics;
using System.IO;
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

        public LinesCounter(ImportModel Model)
        {
            model = Model;
            model.PropertyChanged += ModelPropertyChanged;
            task = null;
            cancellationTokenSource = null;
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

            StreamReader reader = new(model.TextFile, new FileStreamOptions() { Mode = FileMode.Open, Access = FileAccess.Read, Options = FileOptions.SequentialScan });
            model.Lines = 0;
            while (!cancellationTokenSource!.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync();
                if (line is null)
                    break;
                model.Lines++;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    model.PropertyChanged -= ModelPropertyChanged;
                    task?.Dispose();
                    cancellationTokenSource?.Dispose();
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
