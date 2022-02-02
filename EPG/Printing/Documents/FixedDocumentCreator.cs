using System.Collections;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace EPG.Printing.Documents
{
    public static class FixedDocumentCreator
    {
        public static FixedDocument FromDataContexts(PrintTicket printTicket, IEnumerable contents)
        {
            var document = new FixedDocument();

            var ticketSize = new Size(printTicket.PageMediaSize.Width ?? 0.0, printTicket.PageMediaSize.Height ?? 0.0);
            var orientationSize = printTicket.PageOrientation switch
            {
                PageOrientation.Landscape => new Size(printTicket.PageMediaSize.Height ?? 0.0, printTicket.PageMediaSize.Width ?? 0.0),
                PageOrientation.ReverseLandscape => new Size(printTicket.PageMediaSize.Height ?? 0.0, printTicket.PageMediaSize.Width ?? 0.0),
                _ => new Size(printTicket.PageMediaSize.Width ?? 0.0, printTicket.PageMediaSize.Height ?? 0.0),
            };

            foreach (var content in contents)
            {
                var presenter = new ContentPresenter()
                {
                    Content = content,
                    Width = orientationSize.Width,
                    Height = orientationSize.Height,
                };

                switch (printTicket.PageOrientation)
                {
                    case PageOrientation.Landscape:
                        presenter.LayoutTransform = new RotateTransform(90.0);
                        break;
                    case PageOrientation.ReversePortrait:
                        presenter.LayoutTransform = new RotateTransform(180.0);
                        break;
                    case PageOrientation.ReverseLandscape:
                        presenter.LayoutTransform = new RotateTransform(-90.0);
                        break;
                }

                var page = new FixedPage()
                {
                    Width = ticketSize.Width,
                    Height = ticketSize.Height,
                    PrintTicket = printTicket,
                };
                page.Children.Add(presenter);

                var pageContent = new PageContent()
                {
                    Child = page,
                    Width = orientationSize.Width,
                    Height = orientationSize.Height,
                };
                document.Pages.Add(pageContent);
            }

            return document;
        }
    }
}
