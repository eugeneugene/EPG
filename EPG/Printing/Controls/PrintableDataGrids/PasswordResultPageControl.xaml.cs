using EPG.Printing.Controls;
using System.Windows.Controls;

namespace EPG.Printing
{
    /// <summary>
    /// Interaction logic for PasswordResultPageControl.xaml
    /// </summary>
    public partial class PasswordResultPageControl : UserControl, IPrintableDataGridContainer
    {
        public PasswordResultPageControl()
        {
            InitializeComponent();
        }

        public IPrintableDataGrid DataGrid
        {
            get => dataGrid;
            set
            {
                if (value is PrintableDataGrid grid)
                    dataGrid = grid;
            }
        }
    }
}
