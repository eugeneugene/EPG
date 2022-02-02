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

        public PreviewerWindow(PrintQueue printQueue, PrintTicket printTicket, PasswordResultModel resultModel)
        {
            if (resultModel is null)
                throw new ArgumentNullException(nameof(resultModel));

            Items = resultModel.DataCollection;

            InitializeComponent();

            PasswordResultPage resultPage = new(
                header: new PasswordResultHeader(
                    title: "Title",
                    version: "1.0",
                    include: resultModel.Include,
                    exclude: resultModel.Exclude,
                    generationDate: DateTime.Now, 
                    passwordsGenerated: Items.Count(),
                    mode: resultModel.Mode,
                    pageIndex: 0,
                    pageCount: 1),
                items: Items.ToList());
            var previewer = new PrintPreviewer<PasswordResultPage>(
                model: resultPage,
                paginate: DataGridPrintablePaginator<PasswordResultItem>.Paginate,
                printer: new Printer(printQueue, printTicket));

            DataContext = previewer;

            Loaded += (sender, e) =>
            {
                previewer.UpdatePreview();
            };
        }
    }
}
