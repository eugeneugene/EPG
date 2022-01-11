using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;

namespace EPG
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost host;

        public App()
        {
            var args = Environment.GetCommandLineArgs();
            var hostBuilder = Host.CreateDefaultBuilder(args);
            hostBuilder.ConfigureAppConfiguration((context, builder) => { });
            hostBuilder.ConfigureServices((context, services) =>
            {
                ConfigureServices(services);
            });
            host = hostBuilder.Build();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CurrentUICulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CurrentUICulture;

            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            await host.StartAsync();

            var mainWindow = host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            host.Dispose();
            base.OnExit(e);
        }

        private void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Debug.WriteLine("Unhandled exception: {0}", e.Exception);
#if DEBUG
            MessageBox.Show(e.Exception.ToString(), "Unhandled exception");
#else
            MessageBox.Show(e.Exception.Message, "Unhandled exception");
#endif
            if (e.Exception is COMException comException && comException.ErrorCode == -2147221040)
                e.Handled = true;
        }
    }
}
