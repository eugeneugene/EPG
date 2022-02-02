using EPG.Printing.Documents;
using System.Collections;
using System.Printing;
using System.Windows;

namespace EPG.Printing.Controls
{
    public sealed class Printer : IPrinter
    {
        private readonly PrintQueue printQueue;

        public string Name => printQueue.Name;

        public void Print(IEnumerable pages, Size pageSize)
        {
            var isLandscape = pageSize.Width > pageSize.Height;
            var mediaSize = isLandscape ? new Size(pageSize.Height, pageSize.Width) : pageSize;

            // Set up print ticket.
            var ticket = printQueue.DefaultPrintTicket;
            ticket.PageMediaSize = new PageMediaSize(mediaSize.Width, mediaSize.Height);
            ticket.PageOrientation = PageOrientation.Portrait;

            // Generate FixedDocument to be printed from data contexts.
            var document = FixedDocumentCreator.FromDataContexts(pages, pageSize);

            // Print.
            var writer = PrintQueue.CreateXpsDocumentWriter(printQueue);
            writer.Write(document);
        }

        public Printer(PrintQueue printQueue)
        {
            this.printQueue = printQueue;
        }
    }
}
