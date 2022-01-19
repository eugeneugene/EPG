﻿using CSAdapter;
using EPG.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Input;
using WpfNotification;

namespace EPG
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowModel model = new();
        private readonly EPGSettings settings = new();
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly Random random = new(DateTime.Now.Millisecond);

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

        private void CommandGenerateExecuted(object sender, ExecutedRoutedEventArgs e)
        {
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
            model.ResultModel.EnableBloom = model.EnableBloom;
            model.ResultModel.CalculateQuality = model.CalculateQuality;

            try
            {
                Password password = new(modes, model.Include ?? string.Empty, model.Exclude ?? string.Empty);
                Bloom? bloom = null;
                if (model.EnableBloom && !string.IsNullOrEmpty(model.Filter))
                {
                    bloom = new();
                    bloom.Open(model.Filter);
                    bloom.Load();
                }
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
                                model.ResultModel.DataCollection.Add(new(pass, hpass, bloomFilterResult, null));
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
                                model.ResultModel.DataCollection.Add(new(pass, null, bloomFilterResult, null));
                            }
                            break;
                    }
                }
                bloom?.Dispose();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Exception: " + ex.Message);
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    sb.AppendLine("InnerException: " + ex.Message);
                }

                var popup = new PopupWindow("Exception", sb.ToString(), NotificationType.Error);
                popup.Show();
                Debug.WriteLine(ex.ToString());
            }
        }

        private void CommandCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void ClearExecuted(object sender, ExecutedRoutedEventArgs e)
        {
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
                    return BloomFilterResult.FOUNDPARANOID;
                var res2 = bloom.CheckString(password.ToUpperInvariant());
                if (res2)
                    return BloomFilterResult.FOUNDPARANOID;
                var res3 = bloom.CheckString(string.Concat(password[0].ToString().ToUpper(), password.AsSpan(1)));
                if (res3)
                    return BloomFilterResult.FOUNDPARANOID;
            }
            return BloomFilterResult.NOTFOUND;
        }
    }
}
