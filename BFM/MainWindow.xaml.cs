using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BFM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OpenFileDialog ofd;
        public MainWindow()
        {
            InitializeComponent();
            ofd = new();
            ofd.CheckFileExists = true;
            ofd.DefaultExt = ".bf";
            ofd.Filter = "Bloom Filter|*.bf|Any file|*.*";
            ofd.Title = "Open Bloom Filter";
        }

        private void BloomFilterBrowseCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var res = ofd.ShowDialog(this) ?? false;
            if (res)
            {

            }
            e.Handled = true;
        }
    }
}
