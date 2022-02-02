using EPG.Printing.Documents;
using System.Collections;
using System.Printing;
using System.Windows;

namespace EPG.Printing.Controls
{
    public sealed class Printer : IPrinter
    {
        public PrintQueue PrintQueue { get; }
        public PrintTicket PrintTicket { get; }

        public string Name => PrintQueue.Name;

        public void Print(IEnumerable pages, Size pageSize)
        {
            //var isLandscape = pageSize.Width > pageSize.Height;
            //var mediaSize = isLandscape ? new Size(pageSize.Height, pageSize.Width) : pageSize;

            // Set up print ticket.
            //var ticket = PrintQueue.DefaultPrintTicket;
            //ticket.PageMediaSize = new PageMediaSize(mediaSize.Width, mediaSize.Height);
            //ticket.PageOrientation = PageOrientation.Portrait;

            // Generate FixedDocument to be printed from data contexts.
            var document = FixedDocumentCreator.FromDataContexts(PrintTicket, pages);

            // Print.
            var writer = PrintQueue.CreateXpsDocumentWriter(PrintQueue);
            writer.Write(document);
        }

        public Printer(PrintQueue printQueue, PrintTicket printTicket)
        {
            PrintQueue = printQueue;
            PrintTicket = printTicket;
        }
    }
}
