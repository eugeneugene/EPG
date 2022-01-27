using System.Windows;

namespace EPG.Printing.Controls
{
    public sealed class PrintPreviewPage
    {
        public object Content { get; }
        public Size PageSize { get; }

        public PrintPreviewPage(object content, Size pageSize)
        {
            Content = content;
            PageSize = pageSize;
        }
    }
}
