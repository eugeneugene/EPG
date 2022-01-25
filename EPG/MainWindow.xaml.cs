using CSAdapter;
using EPG.Code;
using EPG.Configuration;
using EPG.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfNotification;

namespace EPG
{
    public partial class MainWindow : Window
    {
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly Random random;

        private readonly MainWindowModel model;

        private readonly EPGSettings settings;
        private readonly ApplicationSettings applicationSettings;
        private readonly OpenFileDialog BloomFileDialog;

        public MainWindow(IHostApplicationLifetime applicationLifetime)
        {
            _applicationLifetime = applicationLifetime ?? throw new Exception(nameof(applicationLifetime));

            model = new();
            settings = new();
            applicationSettings = new(settings);
            applicationSettings.Load();

            InitializeComponent();

            model.FromSettings(settings);
            random = new(DateTime.Now.Millisecond);

            _applicationLifetime.ApplicationStopping.Register(() => Close(), true);

            BloomFileDialog = new()
            {
                CheckFileExists = false,
                CheckPathExists = true,
                DefaultExt = ".bf",
                Filter = "Bloom Filter|*.bf|Any file|*.*",
                Title = "Open Bloom Filter"
            };
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            DataContext = model;
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            settings.FromModel(model);
            applicationSettings.Save();
        }

        private async void CommandGenerateExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (model.PasswordMode is null)
                return;
            if (model.NumberOfPasswords == 0U)
                return;
            if (model.MinimumLength == 0U)
                return;
            if (model.MaximumLength == 0U)
                return;
            if (model.MinimumLength > model.MaximumLength)
                return;

            Password.Modes modes = Password.Modes.NoMode;
            if (model.SmallSymbols is null)
                modes |= Password.Modes.Lowers;
            if (model.SmallSymbols == true)
                modes |= Password.Modes.LowersForced;
            if (model.CapitalSymbols is null)
                modes |= Password.Modes.Capitals;
            if (model.CapitalSymbols == true)
                modes |= Password.Modes.CapitalsForced;
            if (model.Numerals is null)
                modes |= Password.Modes.Numerals;
            if (model.Numerals == true)
                modes |= Password.Modes.NumeralsForced;
            if (model.SpecialSymbols is null)
                modes |= Password.Modes.Symbols;
            if (model.SpecialSymbols == true)
                modes |= Password.Modes.SymbolsForced;

            if (modes == Password.Modes.NoMode)
                return;

            if (model.AutoClear)
                model.ResultModel.DataCollection.Clear();

            model.ResultModel.ShowHyphenated = model.ShowHyphenated;
            model.ResultModel.CalculateQuality = model.CalculateQuality;

            Bloom? bloom = null;
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                await Task.Run(() =>
                {
                    Password password = new(modes, model.Include ?? string.Empty, model.Exclude ?? string.Empty);
                    if (model.EnableBloom && !string.IsNullOrEmpty(model.Filter))
                    {
                        bloom = new();
                        bloom.Open(model.Filter);
                        bloom.Load();
                    }
                    List<DataItem> data = new();
                    for (uint i = 0; i < model.NumberOfPasswords; i++)
                    {
                        uint length = (uint)random.Next((int)model.MinimumLength, (int)model.MaximumLength);
                        switch (model.PasswordMode)
                        {
                            case PasswordMode.Pronounceable:
                                {
                                    if (!password.GenerateWord(length))
                                        return;
                                    string pass = password.GetWord();
                                    string hpass = string.Empty;
                                    if (model.ShowHyphenated)
                                        hpass = password.GetHyphenatedWord();
                                    BloomFilterResult? bloomFilterResult = null;
                                    if (bloom is not null)
                                        bloomFilterResult = CheckBloom(bloom, pass);
                                    int? Quality = null;
                                    if (model.CalculateQuality)
                                        Quality = PasswordQuality.CalculateQuality(pass);
                                    data.Add(new(pass, hpass, bloomFilterResult, Quality));
                                }
                                break;
                            case PasswordMode.Random:
                                {
                                    if (!password.GenerateWord(length))
                                        return;
                                    string pass = password.GetWord();
                                    BloomFilterResult? bloomFilterResult = null;
                                    if (bloom is not null)
                                        bloomFilterResult = CheckBloom(bloom, pass);
                                    int? Quality = null;
                                    if (model.CalculateQuality)
                                        Quality = PasswordQuality.CalculateQuality(pass);
                                    data.Add(new(pass, null, bloomFilterResult, Quality));
                                }
                                break;
                        }
                    }
                    Dispatcher.Invoke(() =>
                    {
                        foreach (var d in data)
                            model.ResultModel.DataCollection.Add(d);
                    });
                });
                bloom?.Close();
                bloom?.Dispose();
            }
            catch (Exception ex)
            {
                bloom?.Abort();
                var sb = new StringBuilder();
                sb.AppendLine("Exception: " + ex.Message);
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                    sb.AppendLine("InnerException: " + ex.Message);
                }

                var popup = new PopupWindow("Exception", sb.ToString(), NotificationType.Error);
                popup.Show();
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void CommandCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Close();
        }

        private void ClearExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            model.ResultModel.DataCollection.Clear();
        }

        private BloomFilterResult CheckBloom(Bloom bloom, string password)
        {
            if (bloom is null)
                throw new ArgumentNullException(nameof(bloom));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            bool bloomres = bloom.CheckString(password);
            if (bloomres)
                return BloomFilterResult.FOUND;
            if (model.ParanoidCheck)
            {
                var res1 = bloom.CheckString(password.ToLowerInvariant());
                if (res1)
                    return BloomFilterResult.UNSAFE;
                var res2 = bloom.CheckString(password.ToUpperInvariant());
                if (res2)
                    return BloomFilterResult.UNSAFE;
                var res3 = bloom.CheckString(string.Concat(password[0].ToString().ToUpper(), password.AsSpan(1)));
                if (res3)
                    return BloomFilterResult.UNSAFE;
            }
            return BloomFilterResult.NOTFOUND;
        }

        private void FilterBrowse(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var res = BloomFileDialog.ShowDialog(this) ?? false;
            if (res)
                model.Filter = BloomFileDialog.FileName;
        }

        private void CommandPrintExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DataGridPrint.Print(ResultDataGrid, "Password results");
        }
    }
}
