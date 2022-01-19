using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace WpfNotification
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Notification : UserControl
    {
        public static readonly DependencyProperty TextBlockHeaderProperty = DependencyProperty.Register("Text", typeof(string), typeof(Notification));

        public Notification()
        {
            InitializeComponent();
        }

        [Description("The text displayed in the Toast Pop up."), Category("Common Properties")]
        public string Text
        {
            get
            {
                return (string)GetValue(TextBlockHeaderProperty);
            }

            set
            {
                SetValue(TextBlockHeaderProperty, value);
            }
        }
    }
}
