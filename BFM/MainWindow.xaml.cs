using BFM.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Input;

namespace BFM
{
    public partial class MainWindow : Window
    {
        private readonly OpenFileDialog BloomFileDialog;
        private readonly MainWindowModel model = new();
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostApplicationLifetime _applicationLifetime;

        public MainWindow(IServiceProvider serviceProvider, IHostApplicationLifetime applicationLifetime)
        {
            _serviceProvider = serviceProvider;
            _applicationLifetime = applicationLifetime ?? throw new Exception(nameof(applicationLifetime));
            InitializeComponent();
            BloomFileDialog = new()
            {
                CheckFileExists = false,
                CheckPathExists = true,
                DefaultExt = ".bf",
                Filter = "Bloom Filter|*.bf|Any file|*.*",
                Title = "Open Bloom Filter"
            };
            _applicationLifetime.ApplicationStopping.Register(() => Close(), true);
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
            var importDialog = _serviceProvider.GetRequiredService<ImportDialog>();
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
