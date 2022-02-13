using EPG.Models;
using EPG.Printing.Controls;
using EPG.Printing.Documents;
using System;
using System.Printing;
using System.Reflection;
using System.Windows;

namespace EPG
{
    /// <summary>
    /// Interaction logic for PreviewerWindow.xaml
    /// </summary>
    public partial class PreviewerWindow : Window
    {
        public PreviewerWindow(PrintQueue printQueue, PrintTicket printTicket, PasswordResultModel resultModel)
        {
            if (resultModel is null)
                throw new ArgumentNullException(nameof(resultModel));

            InitializeComponent();

            string product = "Title";
            string version = "Version";

            var assembly = Assembly.GetEntryAssembly();
            if (assembly is not null)
            {
                var versionAttr = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
                if (versionAttr is not null)
                    version = versionAttr.InformationalVersion;
                var productAttr = assembly.GetCustomAttribute<AssemblyProductAttribute>();
                if (productAttr is not null)
                    product = productAttr.Product;
            }

            PasswordResultPage resultPage = new(
                header: new PasswordResultHeader(
                    title: product,
                    version: version,
                    generationDate: DateTime.Now,
                    passwordsGenerated: resultModel.DataCollection.Count,
                    pageIndex: 0,
                    pageCount: 1), resultModel: resultModel);

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
