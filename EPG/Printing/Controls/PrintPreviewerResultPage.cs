using EPG.Models;
using System;
using System.Collections;
using System.Windows;

namespace EPG.Printing.Controls
{
    /// <summary>
    /// Для привязки типа к DataContext в PrintPreviewerControl.xaml
    /// </summary>
    public class PrintPreviewerResultPage : PrintPreviewer<PasswordResultPage>
    {
        public PrintPreviewerResultPage(PasswordResultPage model, Func<PasswordResultPage, Size, IEnumerable> paginate, IPrinter printer)
            : base(model, paginate, printer)
        { }
    }
}
