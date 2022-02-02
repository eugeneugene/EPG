using EPG.Models;
using EPG.Printing.Controls;
using EPG.Printing.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Windows;

namespace EPG
{
    /// <summary>
    /// Interaction logic for PreviewerWindow.xaml
    /// </summary>
    public partial class PreviewerWindow : Window
    {
        public IEnumerable<PasswordResultItem> Items { get; }

        public PreviewerWindow(PrintQueue printQueue, PrintTicket printTicket, IEnumerable<PasswordResultItem> items)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));

            InitializeComponent();

            PasswordResultPage page = new(new PasswordResultHeader("Title",
                DateTime.Now,
                Items.Count(), "mode", 0, 1),
                Items.ToList());
            var previewer = new PrintPreviewer<PasswordResultPage>(page,
                DataGridPrintablePaginator<PasswordResultItem>.Paginate,
                new Printer(printQueue, printTicket));

            DataContext = previewer;

            Loaded += (sender, e) =>
            {
                previewer.UpdatePreview();
            };
        }
    }
}
