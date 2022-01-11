using EPG.Code;
using EPG.Models;
using System.Windows;

namespace EPG
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowModel model = new();
        private readonly EPGSettings settings = new();

        public MainWindow()
        {
            model.PasswordMode = settings.PasswordMode;
            model.ShowHyphenated = settings.ShowHyphenated;
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = model;
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            settings.PasswordMode = model.PasswordMode;
            settings.ShowHyphenated = model.ShowHyphenated;
            settings.Save();
        }
    }
}
