using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;

namespace EPG;

public partial class App : Application
{
    private readonly IHost host;

    public App()
    {
        var hostBuilder = Host.CreateDefaultBuilder(Environment.GetCommandLineArgs());
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

internal static class ConsoleHelper
{
    private const int ATTACH_PARENT_PROCESS = -1;
    private static bool ConsoleAllocated;

    [DllImport("kernel32.dll")]
    static extern bool AttachConsole(int dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AllocConsole();

    public static void AllocateConsole()
    {
        if (!ConsoleAllocated)
            AttachConsole(ATTACH_PARENT_PROCESS);
        ConsoleAllocated = true;
    }
}
