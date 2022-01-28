using EPG.Models;
using EPG.Printing.Controls;
using EPG.Printing.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace EPG
{
    /// <summary>
    /// Interaction logic for PreviewerWindow.xaml
    /// </summary>
    public partial class PreviewerWindow : Window
    {
        public IEnumerable<PasswordResultItem> Items { get; }

        public PreviewerWindow(IEnumerable<PasswordResultItem> items)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));

            InitializeComponent();

            PasswordResultPage page = new(new PasswordResultHeader("Title",
                 DateTime.Now,
                 Items.Count(), "mode", 0, 1),
                 Items.ToList());
            var previewer = new PrintPreviewer<PasswordResultPage>(page,
                DataGridPrintablePaginator<PasswordResultItem>.Paginate,
                PrinterSelector<IPrinter>.FromLocalServer<IPrinter>(q => new Printer(q)));

            DataContext = previewer;

            Loaded += (sender, e) =>
            {
                previewer.UpdatePreview();
            };
        }
    }
}
