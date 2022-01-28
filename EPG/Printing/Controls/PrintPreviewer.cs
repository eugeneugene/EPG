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
        readonly TModel model;
        readonly Func<TModel, Size, IEnumerable> paginate;

        static readonly IReadOnlyList<PrintPreviewPage> emptyPages = Array.Empty<PrintPreviewPage>();

        IReadOnlyList<PrintPreviewPage> pages = emptyPages;
        public IReadOnlyList<PrintPreviewPage> Pages
        {
            get { return pages; }
            set { SetProperty(ref pages, value); }
        }

        public MediaSizeSelector MediaSizeSelector { get; } = new MediaSizeSelector();

        bool isLandscape;
        public bool IsLandscape
        {
            get { return isLandscape; }
            set { SetProperty(ref isLandscape, value); }
        }
        public ScaleSelector ScaleSelector { get; } = new ScaleSelector();

        public PrinterSelector<IPrinter> PrinterSelector { get; }

        public DelegateCommand PreviewCommand { get; }

        public DelegateCommand PrintCommand { get; }

        Size PageSize
        {
            get
            {
                var mediaSize = MediaSizeSelector.SelectedSize;
                return IsLandscape ? new Size(mediaSize.Height, mediaSize.Width) : mediaSize;
            }
        }

        public void UpdatePreview()
        {
            var pageSize = PageSize;
            Pages = paginate(model, PageSize)
                .Cast<object>()
                .Select(content => new PrintPreviewPage(content, pageSize))
                .ToArray();
        }

        public void Print()
        {
            var printer = PrinterSelector.SelectedPrinterOrNull;
            if (printer is null)
                return;

            var pageSize = PageSize;
            printer.Print(paginate(model, pageSize), pageSize);
        }

        public void Dispose()
        {
            PrinterSelector.Dispose();
        }

        public
            PrintPreviewer(
                TModel printable,
                Func<TModel, Size, IEnumerable> paginate,
                PrinterSelector<IPrinter> printerSelector
            )
        {
            this.model = printable;
            this.paginate = paginate;
            PrinterSelector = printerSelector;

            PreviewCommand = new DelegateCommand(UpdatePreview);
            PrintCommand = new DelegateCommand(Print);
        }
    }
}
