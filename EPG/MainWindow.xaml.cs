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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WPFLocalizeExtension.Engine;
using WPFLocalizeExtension.Providers;
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

            LocalizeDictionary.Instance.Culture = new System.Globalization.CultureInfo("ru");
            (LocalizeDictionary.Instance.DefaultProvider as ResxLocalizationProvider).SearchCultures =
                new List<System.Globalization.CultureInfo>()
                {
                    System.Globalization.CultureInfo.GetCultureInfo("ru-ru"),
                    System.Globalization.CultureInfo.GetCultureInfo("en"),
                };
            LocalizeDictionary.Instance.OutputMissingKeys = true;

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

            StringBuilder builderMode = new();
            Password.Modes modes = Password.Modes.NoMode;

            if (model.SmallSymbols is null)
            {
                modes |= Password.Modes.LowersForced;
                builderMode.Append('l');
            }
            else if (model.SmallSymbols == true)
            {
                modes |= Password.Modes.Lowers;
                builderMode.Append('L');
            }

            if (model.CapitalSymbols is null)
            {
                modes |= Password.Modes.Capitals;
                builderMode.Append('c');
            }
            else if (model.CapitalSymbols == true)
            {
                modes |= Password.Modes.CapitalsForced;
                builderMode.Append('C');
            }

            if (model.Numerals is null)
            {
                modes |= Password.Modes.Numerals;
                builderMode.Append('n');
            }
            else if (model.Numerals == true)
            {
                modes |= Password.Modes.NumeralsForced;
                builderMode.Append('N');
            }

            if (model.SpecialSymbols is null)
            {
                modes |= Password.Modes.Symbols;
                builderMode.Append('s');
            }
            else if (model.SpecialSymbols == true)
            {
                modes |= Password.Modes.SymbolsForced;
                builderMode.Append('S');
            }

            if (modes == Password.Modes.NoMode)
                return;

            model.ResultModel.Mode = builderMode.ToString();
            model.ResultModel.Include = model.Include ?? string.Empty;
            model.ResultModel.Exclude = model.Exclude ?? string.Empty;

            if (model.AutoClear)
            {
                model.ResultModel.DataCollection.Clear();
                model.ResultModel.ShowHyphenated = false;
                model.ResultModel.CalculateComplexity = false;
                model.AmountGenerated = 0;
            }

            if (model.PasswordMode == PasswordMode.Pronounceable && model.ShowHyphenated)
                model.ResultModel.ShowHyphenated = true;

            if (model.CalculateComplexity)
                model.ResultModel.CalculateComplexity = true;

            model.ResultModel.ManualMode = false;

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
                    List<PasswordResultItem> data = new();
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
                                    int? complexity = null;
                                    if (model.CalculateComplexity)
                                        complexity = Complexity.CalculateComplexity(pass);
                                    data.Add(new(++model.AmountGenerated, pass, hpass, bloomFilterResult, complexity, false));
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
                                    int? complexity = null;
                                    if (model.CalculateComplexity)
                                        complexity = Complexity.CalculateComplexity(pass);
                                    data.Add(new(++model.AmountGenerated, pass, null, bloomFilterResult, complexity, false));
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
            model.ResultModel.ShowHyphenated = false;
            model.ResultModel.CalculateComplexity = false;
            model.AmountGenerated = 0;
            model.ResultModel.ManualMode = false;
        }

        private BloomFilterResult? CheckBloom(Bloom bloom, string password)
        {
            if (bloom is null)
                throw new ArgumentNullException(nameof(bloom));
            if (string.IsNullOrEmpty(password))
                return null;

            bool bloomres = bloom.CheckString(password);
            if (bloomres)
                return BloomFilterResult.FOUND;
            if (model.ParanoidCheck)
            {
                var res1 = bloom.CheckString(password.ToLowerInvariant());
                if (res1)
                    return BloomFilterResult.NOTSAFE;
                var res2 = bloom.CheckString(password.ToUpperInvariant());
                if (res2)
                    return BloomFilterResult.NOTSAFE;
                var res3 = bloom.CheckString(string.Concat(password[0].ToString().ToUpper(), password.AsSpan(1)));
                if (res3)
                    return BloomFilterResult.NOTSAFE;
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
            model.ResultModel.ManualMode = false;
            PrintDialog printDlg = new();
            if (printDlg.ShowDialog().GetValueOrDefault())
            {
                PreviewerWindow previewer = new(printDlg.PrintQueue, printDlg.PrintTicket, model);
                previewer.ShowDialog();
            }
        }

        private void CommandCopyCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = ResultDataGrid.SelectedItems.Count > 0;
        }

        private void ResultDataGridBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            var item = e.Row.Item as PasswordResultItem;
            if (item is not null)
            {
                if (!model.ResultModel.ManualMode || !item.ManuallyEnterred)
                    e.Cancel = true;
            }
        }

        private void ResultDataGridAddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new PasswordResultItem(++model.AmountGenerated, string.Empty, string.Empty, null, null, true);
        }

        private void ResultDataGridCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditingElement is TextBox textBox)
            {
                var pass = textBox.Text;
                Bloom? bloom = null;
                if (model.EnableBloom && !string.IsNullOrEmpty(model.Filter))
                {
                    bloom = new();
                    bloom.Open(model.Filter);
                    bloom.Load();
                }
                BloomFilterResult? bloomFilterResult = null;
                if (bloom is not null)
                    bloomFilterResult = CheckBloom(bloom, pass);
                int? complexity = null;
                if (model.CalculateComplexity)
                {
                    complexity = Complexity.CalculateComplexity(pass);
                    model.ResultModel.CalculateComplexity = true;
                }

                var item = e.Row.Item as PasswordResultItem;
                if (item is not null)
                {
                    item.Password = pass;
                    item.BloomFilterResult = bloomFilterResult;
                    item.Complexity = complexity;
                }
                bloom?.Close();
                bloom?.Dispose();
            }
        }

        private void LangEnglishExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            LocalizeDictionary.Instance.Culture=new System.Globalization.CultureInfo("en-US");
        }

        private void LangRussianExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            LocalizeDictionary.Instance.Culture = new System.Globalization.CultureInfo("ru-RU");
        }
    }
}
