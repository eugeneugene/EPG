using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Point = System.Windows.Point;

namespace WpfNotification
{
    public partial class PopupWindow : Window
    {
        public static IConcurrentLinkedList<PopupWindow> PopupWindows { get; } = ConcurrentLinkedList.Create<PopupWindow>();

        private readonly object lockObject = new();

        public byte MaxPopup { get; set; }

        public Brush FontColor
        {
            get => TextBoxTitle.Foreground;

            set
            {
                TextBoxTitle.Foreground = value;
                TextBoxShortDescription.Foreground = value;
            }
        }

        public new Brush BorderBrush
        {
            get => borderBackground.BorderBrush;
            set => borderBackground.BorderBrush = value;
        }

        public new Brush Background
        {
            get => borderBackground.Background;
            set => borderBackground.Background = value;
        }

        public object HyperlinkObjectForRaisedEvent { get; set; }

        public PopupTrait Traits { get; set; }

        public event EventHandler<EventArgs> ClosedByUser;

        public event EventHandler<HyperLinkEventArgs> HyperlinkClicked;

        public PopupWindow(string title, string text, NotificationType notificationType)
            : this(title, notificationType)
        {
            TextBoxShortDescription.Text = text;
        }

        public PopupWindow(string title, string text, string hyperlinkText, NotificationType notificationType, object hyperlinkObjectForRaisedEvent = null)
            : this(title, text, notificationType)
        {
            HyperlinkObjectForRaisedEvent = hyperlinkObjectForRaisedEvent;
            SetHyperLinkButton(hyperlinkText);
        }

        public PopupWindow(string title, IEnumerable<Inline> textInlines, string hyperlinkText, ImageSource imageSource, object hyperlinkObjectForRaisedEvent = null)
            : this(title)
        {
            imageLeft.Source = imageSource;
            TextBoxShortDescription.Inlines.AddRange(textInlines);
            SetHyperLinkButton(hyperlinkText);
            HyperlinkObjectForRaisedEvent = hyperlinkObjectForRaisedEvent;
        }

        public PopupWindow(string title, IEnumerable<Inline> textInlines, string hyperlinkText, Bitmap imageSource, object hyperlinkObjectForRaisedEvent = null)
            : this(title, textInlines, hyperlinkText, NullChecker(imageSource).ToBitmapImage(), hyperlinkObjectForRaisedEvent)
        {
        }

        public PopupWindow(string title, IEnumerable<Inline> textInlines, string hyperlinkText, NotificationType notificationType, object hyperlinkObjectForRaisedEvent = null)
            : this(title, notificationType)
        {
            HyperlinkObjectForRaisedEvent = hyperlinkObjectForRaisedEvent;
            TextBoxShortDescription.Inlines.AddRange(textInlines);
            SetHyperLinkButton(hyperlinkText);
        }

        public PopupWindow(string title, string text, string hyperlinkText, Bitmap imageSource, object hyperlinkObjectForRaisedEvent = null)
            : this(title)
        {
            TextBoxShortDescription.Text = text;
            SetHyperLinkButton(hyperlinkText);
            imageLeft.Source = NullChecker(imageSource).ToBitmapImage();
            HyperlinkObjectForRaisedEvent = hyperlinkObjectForRaisedEvent;
        }

        public PopupWindow(string title, string text, string hyperlinkText, ImageSource imageSource, Action hyperlinkClick)
            : this(title)
        {
            TextBoxShortDescription.Text = text;
            SetHyperLinkButton(hyperlinkText);
            buttonView.Click += delegate { hyperlinkClick(); };
            imageLeft.Source = imageSource;
        }

        public PopupWindow(string title, string text, string hyperlinkText, Bitmap imageSource, Action hyperlinkClick)
            : this(title, text, hyperlinkText, NullChecker(imageSource).ToBitmapImage(), hyperlinkClick)
        {
        }

        public PopupWindow(string title, string text, string hyperlinkText, Action hyperlinkClick)
            : this(title)
        {
            TextBoxShortDescription.Text = text;
            SetHyperLinkButton(hyperlinkText);
            buttonView.Click += delegate { hyperlinkClick(); };
        }

        public PopupWindow(string title, string text, string hyperlinkText, ImageSource imageSource, object hyperlinkObjectForRaisedEvent = null)
            : this(title)
        {
            TextBoxShortDescription.Text = text;
            HyperlinkObjectForRaisedEvent = hyperlinkObjectForRaisedEvent;
            SetHyperLinkButton(hyperlinkText);
            imageLeft.Source = imageSource;
        }

        private PopupWindow(string title, NotificationType notificationType)
            : this(title)
        {
            switch (notificationType)
            {
                case NotificationType.Help:
                    SetStyle(notificationType.ToString());
                    Traits = PopupTrait.CenterParent;
                    break;

                case NotificationType.Error:
                    SetStyle(notificationType.ToString());
                    Traits = PopupTrait.CenterScreen;
                    break;

                case NotificationType.Notification:
                    SetStyle(notificationType.ToString());
                    Traits = PopupTrait.CenterScreen;
                    break;

                case NotificationType.Toast:
                    //SetStyle(notificationType.ToString());    // Default style
                    Traits = PopupTrait.BottomRight;
                    break;

                case NotificationType.Warning:
                    Traits = PopupTrait.CenterScreen;
                    SetStyle(notificationType.ToString());
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(notificationType));
            }
        }

        private PopupWindow(string title)
        {
            InitializeComponent();
            if (Application.Current.MainWindow is not null)
                Application.Current.MainWindow.Closing += MainWindowClosing;

            TextBoxTitle.Text = title;
            Title = title;
        }

        public new void Show()
        {
            int PopupCount = PopupWindows.Count();

            if (MaxPopup > 0 && PopupCount > MaxPopup)
            {
                Close();
                return;
            }

            IInputElement focusedElement = Keyboard.FocusedElement;

            Topmost = true;
            base.Show();
            PopupWindows.Enqueue(this);

            if (Owner is null)
                Owner = Application.Current.MainWindow;  // Parent should set Owner

            Closed += NotificationWindowClosed;
            ArrangeWindows();

            if (focusedElement is not null)
            {
                // Restore keyboard focus to the original element that had focus. That way if someone
                // was typing into a control we don't steal keyboard focus away from that control.
                focusedElement.Focusable = true;
                Keyboard.Focus(focusedElement);
            }
        }

        public static void CloseAll()
        {
            while (PopupWindows.Any())
            {
                var window = PopupWindows.Dequeue();
                window.Value.Close();
            }
        }

        public new void ShowDialog()
        {
            throw new NotImplementedException("ShowDialog() is not supported.  Use Show() instead.");
        }

        private void SetStyle(string Style)
        {
            if (string.IsNullOrWhiteSpace(Style))
                return;

            var uri = new Uri("/WpfNotification;component/Styles/" + Style + "Style.xaml", UriKind.Relative);
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;

            Resources.Clear();
            Resources.MergedDictionaries.Add(resourceDict);
        }

        protected virtual void OnClosedByUser(EventArgs e)
        {
            ClosedByUser?.Invoke(this, e);
        }

        protected virtual void OnHyperlinkClicked(HyperLinkEventArgs e)
        {
            HyperlinkClicked?.Invoke(this, e);
        }

        private void ButtonViewClick(object sender, RoutedEventArgs e)
        {
            OnHyperlinkClicked(new HyperLinkEventArgs { HyperlinkObjectForRaisedEvent = HyperlinkObjectForRaisedEvent });
        }

        private void DoubleAnimationCompleted(object sender, EventArgs e)
        {
            if (!IsMouseOver)
                Close();
        }

        private void ImageMouseUp(object sender, MouseButtonEventArgs e)
        {
            OnClosedByUser(new EventArgs());
            Close();
        }

        private void MainWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseAll();
        }

        private void NotificationWindowClosed(object sender, EventArgs e)
        {
            if (Traits == PopupTrait.BottomRight)
            {
                foreach (var window in PopupWindows.Where(item => item.Traits == PopupTrait.BottomRight))
                {
                    if (window != this)
                    {
                        // Adjust any windows that were above this one to drop down
                        if (window.Top < Top && window.Left == Left)
                        {
                            window.Top += ActualHeight;

                            if (!WindowsExistToTheRight(Left))
                                window.Left += ActualWidth;
                        }
                    }
                }
            }

            var head = PopupWindows.Head.Next;
            while (head != PopupWindows.Tail && head is not null)
            {
                var t = head;
                head = head.Next;
                if (t.IsDummyNode || t.Removed)
                    continue;
                if (t.Value == this)
                {
                    t.Remove();
                    break;
                }
            }

            ArrangeWindows();
        }

        private bool WindowsExistToTheRight(double left)
        {
            foreach (var window in PopupWindows)
            {
                if (window != this && left == SystemParameters.WorkArea.Width - Width)
                    return true;
            }

            return false;
        }

        private void ArrangeWindows()
        {
            lock (lockObject)
            {
                if (Traits == PopupTrait.BottomRight)
                {
                    Left = SystemParameters.WorkArea.Width - ActualWidth;
                    var top = SystemParameters.WorkArea.Height - ActualHeight;

                    foreach (var window in PopupWindows.Where(item => item.Traits == PopupTrait.BottomRight))
                    {
                        if (window != this)
                        {
                            window.Topmost = true;

                            if (Left == window.Left)
                                top -= window.ActualHeight;

                            if (top < 0)
                            {
                                Left -= ActualWidth;
                                top = SystemParameters.WorkArea.Bottom - ActualHeight;
                            }
                        }
                    }

                    Top = top;
                }
                else
                {
                    if (PopupWindows.Any(item => item != this && item.Traits == Traits))
                    {
                        foreach (var window in PopupWindows.Where(item => item != this && item.Traits == Traits))
                        {
                            if (window.Left >= Left)
                            {
                                Left = window.Left + 20;
                                Top = window.Top + 20;
                            }
                        }
                    }
                    else
                    {
                        Point point;
                        if (Traits == PopupTrait.CenterParent)
                            point = new Point(Owner.Left + (Owner.Width - Width) / 2, Owner.Top + (Owner.Height - Height) / 2);
                        else
                            point = new Point((SystemParameters.WorkArea.Width - Width) / 2, (SystemParameters.WorkArea.Height - Height) / 2);

                        Left = point.X;
                        Top = point.Y;
                    }
                }
            }
        }

        private void SetHyperLinkButton(string hyperlinkText)
        {
            if (!string.IsNullOrWhiteSpace(hyperlinkText))
            {
                buttonView.Content = hyperlinkText;
                buttonView.Visibility = Visibility.Visible;
            }
        }

        private static T NullChecker<T>(T argument)
        {
            if (argument is null)
                throw new ArgumentNullException(nameof(argument));
            return argument;
        }
    }
}