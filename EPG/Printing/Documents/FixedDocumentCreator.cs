using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace EPG.Printing.Documents
{
    public struct FixedDocumentCreator
    {
        public static FixedDocument FromDataContexts(IEnumerable contents, Size pageSize)
        {
            var isLandscape = pageSize.Width > pageSize.Height;
            var mediaSize = isLandscape ? new Size(pageSize.Height, pageSize.Width) : pageSize;

            var document = new FixedDocument();

            foreach (var content in contents)
            {
                var presenter = new ContentPresenter()
                {
                    Content = content,
                    Width = pageSize.Width,
                    Height = pageSize.Height,
                };

                if (isLandscape)
                {
                    presenter.LayoutTransform = new RotateTransform(90.0);
                }

                var page = new FixedPage()
                {
                    Width = mediaSize.Width,
                    Height = mediaSize.Height,
                };
                page.Children.Add(presenter);

                var pageContent = new PageContent() { Child = page };
                document.Pages.Add(pageContent);
            }

            return document;
        }
    }
}
