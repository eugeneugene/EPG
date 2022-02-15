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
        public PreviewerWindow(PrintQueue printQueue, PrintTicket printTicket, MainWindowModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

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
                    passwordsGenerated: model.AmountGenerated,
                    pageIndex: 0,
                    pageCount: 1), resultModel: model.ResultModel);

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
