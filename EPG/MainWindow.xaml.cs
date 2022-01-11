using EPG.Code;
using EPG.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;

namespace EPG
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowModel model = new();
        private readonly EPGSettings settings = new();
        private readonly IHostApplicationLifetime _applicationLifetime;

        public MainWindow(IHostApplicationLifetime applicationLifetime)
        {
            _applicationLifetime = applicationLifetime ?? throw new Exception(nameof(applicationLifetime));
            model.FromSettings(settings);
            InitializeComponent();
            _applicationLifetime.ApplicationStopping.Register(() => Close(), true);
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = model;
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            settings.FromModel(model);
            settings.Save();
        }
    }
}
