using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace EPG.Printing.Controls
{
    public class PrintPreviewer<TModel> : BindableBase, IPrintPreviewer
    {
        private readonly TModel model;
        private readonly Func<TModel, Size, IEnumerable> paginate;

        private IReadOnlyList<PrintPreviewPage> pages = Array.Empty<PrintPreviewPage>();
        public IReadOnlyList<PrintPreviewPage> Pages
        {
            get { return pages; }
            set { SetProperty(ref pages, value); }
        } 

        public ScaleSelector ScaleSelector { get; } = new ScaleSelector();

        public IPrinter Printer { get; }

        public DelegateCommand PreviewCommand { get; }

        public DelegateCommand PrintCommand { get; }

        private Size PageSize
        {
            get
            {
                var mediaSize = Printer.PrintTicket.PageMediaSize;
                if (Printer.PrintTicket.PageOrientation == System.Printing.PageOrientation.Landscape || Printer.PrintTicket.PageOrientation == System.Printing.PageOrientation.ReverseLandscape)
                    return new Size(mediaSize.Height ?? 0.0, mediaSize.Width ?? 0.0);
                return new Size(mediaSize.Width ?? 0.0, mediaSize.Height ?? 0.0);
            }
        }

        public void UpdatePreview()
        {
            Pages = paginate(model, PageSize)
                .Cast<object>()
                .Select(content => new PrintPreviewPage(content, PageSize))
                .ToArray();
        }

        public void Print()
        {
            Printer.Print(paginate(model, PageSize), PageSize);
        }

        public PrintPreviewer(TModel model, Func<TModel, Size, IEnumerable> paginate, IPrinter printer)
        {
            this.model = model;
            this.paginate = paginate;
            Printer = printer;

            PreviewCommand = new DelegateCommand(UpdatePreview);
            PrintCommand = new DelegateCommand(Print);
        }
    }
}
