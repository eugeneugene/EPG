using System.Windows.Controls;
using System.Windows.Input;

namespace EPG.Printing.Controls
{
    public partial class PrintPreviewerControl : UserControl
    {
        IPrintPreviewer? Previewer => DataContext as IPrintPreviewer;

        public PrintPreviewerControl()
        {
            InitializeComponent();

            scrollViewer.PreviewMouseWheel += (sender, e) =>
            {
                if (Previewer is not null && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                {
                    Previewer.ScaleSelector.Zoom(e.Delta);
                    e.Handled = true;
                }
            };
        }
    }
}
