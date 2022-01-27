using System;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Xps;

namespace EPG.Code
{
    public class WpfPrinting
    {
        public const double cm = 37;
        public double Margin = 0.5 * cm;
        //public double PageWidth = 21 * cm;
        //public double PageHeight = 29 * cm;
        public double RowHeight = 0.7 * cm;
        public bool PageNumberVisibility = true;
        public bool DateVisibility = true;
        public double FontSize = 14;
        public double HeaderFontSize = 14;
        public bool IsBold = false;

        public void PrintDataGrid(FrameworkElement? header, DataGrid grid, FrameworkElement? footer, PrintDialog printDialog)
        {
            Size pageSize = new(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);
            double PageWidthWithMargin = pageSize.Width - Margin * 2;
            double PageHeightWithMargin = pageSize.Height - Margin * 2;

            double headerWidth = PageWidthWithMargin;
            double headerHeight = 0.0;

            double footerWidth = PageWidthWithMargin;
            double footerHeight = 0.0;

            if (header is null)
            {
                header = new FrameworkElement
                {
                    Width = 1,
                    Height = 1
                };
            }
            else
            {
                var headerScale = header.Width / PageWidthWithMargin;
                headerHeight = header.Height * headerScale;
                header.Height = headerHeight;
                header.Width = headerWidth;
            }

            if (footer is null)
            {
                footer = new FrameworkElement
                {
                    Width = 1,
                    Height = 1
                };
            }
            else
            {
                double footerScale = footer.Width / PageWidthWithMargin;
                footerHeight = footer.Height * footerScale;
                footer.Height = footerHeight;
                footer.Width = footerWidth;
            }

            FixedDocument fixedDoc = new();
            fixedDoc.DocumentPaginator.PageSize = pageSize;

            double GridActualWidth = grid.ActualWidth == 0 ? grid.Width : grid.ActualWidth;

            int pageNumber = 1;
            string Now = DateTime.Now.ToShortDateString();

            //add the header 
            FixedPage fixedPage = new();
            fixedPage.Background = Brushes.White;
            fixedPage.Width = pageSize.Width;
            fixedPage.Height = pageSize.Height;

            FixedPage.SetTop(header, Margin);
            FixedPage.SetLeft(header, Margin);

            fixedPage.Children.Add(header);
            // its like cursor for current page Height to start add grid rows
            double CurrentPageHeight = headerHeight + 1 * cm;
            int lastRowIndex = 0;
            bool IsFooterAdded = false;

            while (true)
            {
                int AvaliableRowNumber;

                var SpaceNeededForRestRows = (CurrentPageHeight + (grid.Items.Count - lastRowIndex) * RowHeight);

                //To avoid printing the footer in a separate page
                if (SpaceNeededForRestRows > (pageSize.Height - footerHeight - Margin) && (SpaceNeededForRestRows < (pageSize.Height - Margin)))
                    AvaliableRowNumber = (int)((pageSize.Height - CurrentPageHeight - Margin - footerHeight) / RowHeight);
                // calc the Avaliable Row acording to CurrentPageHeight
                else
                    AvaliableRowNumber = (int)((pageSize.Height - CurrentPageHeight - Margin) / RowHeight);

                // create new page except first page cause we created it prev
                if (pageNumber > 1)
                {
                    fixedPage = new FixedPage
                    {
                        Background = Brushes.White,
                        Width = pageSize.Width,
                        Height = pageSize.Height
                    };
                }

                // create new data grid with  columns width and binding
                DataGrid gridToAdd;
                gridToAdd = GetDataGrid(grid, GridActualWidth, PageWidthWithMargin);

                FixedPage.SetTop(gridToAdd, CurrentPageHeight); // top margin
                FixedPage.SetLeft(gridToAdd, Margin); // left margin

                // add the avaliable rows to the cuurent grid 
                for (int i = lastRowIndex; i < grid.Items.Count && i < AvaliableRowNumber + lastRowIndex; i++)
                {
                    gridToAdd.Items.Add(grid.Items[i]);
                }
                lastRowIndex += gridToAdd.Items.Count + 1;

                // add date
                TextBlock dateText = new();
                if (DateVisibility)
                    dateText.Visibility = Visibility.Visible;
                else
                    dateText.Visibility = Visibility.Hidden;
                dateText.Text = Now;

                // add page number
                TextBlock PageNumberText = new();
                if (PageNumberVisibility)
                    PageNumberText.Visibility = Visibility.Visible;
                else
                    PageNumberText.Visibility = Visibility.Hidden;
                PageNumberText.Text = "Page : " + pageNumber;

                FixedPage.SetTop(dateText, PageHeightWithMargin);
                FixedPage.SetLeft(dateText, Margin);

                FixedPage.SetTop(PageNumberText, PageHeightWithMargin);
                FixedPage.SetLeft(PageNumberText, PageWidthWithMargin - PageNumberText.Text.Length * 10);

                fixedPage.Children.Add(gridToAdd);
                fixedPage.Children.Add(dateText);
                fixedPage.Children.Add(PageNumberText);

                // calc Current Page Height to know the rest Height of this page
                CurrentPageHeight += gridToAdd.Items.Count * RowHeight;

                // all grid rows added
                if (lastRowIndex >= grid.Items.Count)
                {
                    // if footer have space it will be added to the same page
                    if (footerHeight < (PageHeightWithMargin - CurrentPageHeight))
                    {
                        FixedPage.SetTop(footer, CurrentPageHeight + Margin);
                        FixedPage.SetLeft(footer, Margin);

                        fixedPage.Children.Add(footer);
                        IsFooterAdded = true;
                    }
                }

                fixedPage.Measure(pageSize);
                fixedPage.Arrange(new Rect(new Point(), pageSize));
                fixedPage.UpdateLayout();

                PageContent pageContent = new();
                ((IAddChild)pageContent).AddChild(fixedPage);
                fixedDoc.Pages.Add(pageContent);

                pageNumber++;
                // go to start position : New page Top
                CurrentPageHeight = Margin;

                // this mean that lastRowIndex >= grid.Items.Count  and the footer dont have enough space
                if (lastRowIndex >= grid.Items.Count && !IsFooterAdded)
                {
                    FixedPage ffixedPage = new();
                    ffixedPage.Background = Brushes.White;
                    ffixedPage.Width = pageSize.Width;
                    ffixedPage.Height = pageSize.Height;

                    FixedPage.SetTop(footer, Margin);
                    FixedPage.SetLeft(footer, Margin);

                    TextBlock fdateText = new();
                    if (DateVisibility)
                        fdateText.Visibility = Visibility.Visible;
                    else
                        fdateText.Visibility = Visibility.Hidden;
                    dateText.Text = Now;

                    TextBlock fPageNumberText = new();
                    if (PageNumberVisibility)
                        fPageNumberText.Visibility = Visibility.Visible;
                    else
                        fPageNumberText.Visibility = Visibility.Hidden;
                    fPageNumberText.Text = "Page : " + pageNumber;

                    FixedPage.SetTop(fdateText, PageHeightWithMargin);
                    FixedPage.SetLeft(fdateText, Margin);

                    FixedPage.SetTop(fPageNumberText, PageHeightWithMargin);
                    FixedPage.SetLeft(fPageNumberText, PageWidthWithMargin - PageNumberText.ActualWidth);

                    ffixedPage.Children.Add(footer);
                    ffixedPage.Children.Add(fdateText);
                    ffixedPage.Children.Add(fPageNumberText);

                    ffixedPage.Measure(pageSize);
                    ffixedPage.Arrange(new Rect(new Point(), pageSize));
                    ffixedPage.UpdateLayout();

                    PageContent fpageContent = new();
                    ((IAddChild)fpageContent).AddChild(ffixedPage);
                    fixedDoc.Pages.Add(fpageContent);
                    IsFooterAdded = true;
                }

                if (IsFooterAdded)
                {
                    break;
                }
            }
            PrintFixedDocument(fixedDoc, printDialog);
        }

        private DataGrid GetDataGrid(DataGrid grid, double GridActualWidth, double PageWidthWithMargin)
        {
            DataGrid printed = new();

            // styling the grid
            Style rowStyle = new(typeof(DataGridRow));
            rowStyle.Setters.Add(new Setter(Control.BackgroundProperty, Brushes.White));
            rowStyle.Setters.Add(new Setter(Control.FontSizeProperty, FontSize));
            if (IsBold)
                rowStyle.Setters.Add(new Setter(Control.FontWeightProperty, FontWeights.Bold));
            rowStyle.Setters.Add(new Setter(Control.HeightProperty, RowHeight));

            Style columnStyle = new(typeof(DataGridColumnHeader));
            columnStyle.Setters.Add(new Setter(Control.FontSizeProperty, HeaderFontSize));
            columnStyle.Setters.Add(new Setter(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Center));
            columnStyle.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(0, 0.5, 0, 1.5)));
            columnStyle.Setters.Add(new Setter(Control.BackgroundProperty, Brushes.White));
            columnStyle.Setters.Add(new Setter(Control.BorderBrushProperty, Brushes.Black));
            columnStyle.Setters.Add(new Setter(Control.FontWeightProperty, FontWeights.SemiBold));

            printed.RowStyle = rowStyle;

            printed.VerticalGridLinesBrush = Brushes.Black;
            printed.HorizontalGridLinesBrush = Brushes.Black;
            printed.FontFamily = new FontFamily("Arial");

            printed.RowBackground = Brushes.White;
            printed.Background = Brushes.White;
            printed.Foreground = Brushes.Black;

            // get the columns of grid 
            foreach (var column in grid.Columns)
            {
                if (column.Visibility != Visibility.Visible)
                    continue;
                if (column is DataGridTextColumn dataGridTextColumn)
                {
                    DataGridTextColumn textColumn = new();
                    textColumn.HeaderStyle = columnStyle;
                    textColumn.Header = dataGridTextColumn.Header;
                    textColumn.Width = dataGridTextColumn.ActualWidth / GridActualWidth * PageWidthWithMargin;
                    textColumn.Binding = dataGridTextColumn.Binding;
                    printed.Columns.Add(textColumn);
                }
                else if (column is DataGridCheckBoxColumn dataGridCheckBoxColumn)
                {
                    DataGridCheckBoxColumn checkboxColumn = new();
                    checkboxColumn.HeaderStyle = columnStyle;
                    checkboxColumn.Header = dataGridCheckBoxColumn.Header;
                    checkboxColumn.Width = dataGridCheckBoxColumn.ActualWidth / GridActualWidth * PageWidthWithMargin;
                    checkboxColumn.Binding = dataGridCheckBoxColumn.Binding;
                    printed.Columns.Add(checkboxColumn);
                }
                else if (column is DataGridHyperlinkColumn dataGridHyperlinkColumn)
                {
                    DataGridHyperlinkColumn hyperlinkColumn = new();
                    hyperlinkColumn.HeaderStyle = columnStyle;
                    hyperlinkColumn.Header = dataGridHyperlinkColumn.Header;
                    hyperlinkColumn.Width = dataGridHyperlinkColumn.ActualWidth / GridActualWidth * PageWidthWithMargin;
                    hyperlinkColumn.Binding = dataGridHyperlinkColumn.Binding;
                    printed.Columns.Add(hyperlinkColumn);
                }
                else if (column is DataGridComboBoxColumn dataGridComboBoxColumn)
                {
                    DataGridComboBoxColumn comboBoxColumn = new();
                    comboBoxColumn.HeaderStyle = columnStyle;
                    comboBoxColumn.Header = dataGridComboBoxColumn.Header;
                    comboBoxColumn.Width = dataGridComboBoxColumn.ActualWidth / GridActualWidth * PageWidthWithMargin;
                    comboBoxColumn.ItemsSource = dataGridComboBoxColumn.ItemsSource;
                    printed.Columns.Add(comboBoxColumn);
                }
                else if (column is DataGridTemplateColumn dataGridTemplateColumn)
                {
                    DataGridTemplateColumn templateColumn = new();
                    templateColumn.HeaderStyle = columnStyle;
                    templateColumn.Header = dataGridTemplateColumn.Header;
                    templateColumn.Width = dataGridTemplateColumn.ActualWidth / GridActualWidth * PageWidthWithMargin;
                    templateColumn.CellTemplate = dataGridTemplateColumn.CellTemplate;
                    printed.Columns.Add(templateColumn);
                }
            }
            printed.BorderBrush = Brushes.Black;

            return printed;
        }

        private static void PrintFixedDocument(FixedDocument fixedDoc, PrintDialog printDialog)
        {
            XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(printDialog.PrintQueue);
            writer.Write(fixedDoc, printDialog.PrintTicket);
        }

        public static void PrintDataGrid(DataGrid grid, PrintDialog printDialog)
        {
            var fixedDoc = GetFixedDocument(grid, printDialog);
            PrintFixedDocument(fixedDoc, printDialog);
        }

        public static FixedDocument GetFixedDocument(FrameworkElement element, PrintDialog printDialog)
        {
            PrintCapabilities capabilities = printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);
            Size pageSize = new(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);
            Size visibleSize = new(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);
            FixedDocument fixedDoc = new();

            //If the toPrint visual is not displayed on screen we neeed to measure and arrange it
            element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            element.Arrange(new Rect(new Point(0, 0), element.DesiredSize));

            Size size = element.DesiredSize;
            //Will assume for simplicity the control fits horizontally on the page
            double yOffset = 0;
            while (yOffset < size.Height)
            {
                VisualBrush vb = new(element)
                {
                    Stretch = Stretch.None,
                    AlignmentX = AlignmentX.Left,
                    AlignmentY = AlignmentY.Top,
                    ViewboxUnits = BrushMappingMode.Absolute,
                    TileMode = TileMode.None,
                    Viewbox = new Rect(0, yOffset, visibleSize.Width, visibleSize.Height)
                };
                PageContent pageContent = new();
                FixedPage page = new();
                ((IAddChild)pageContent).AddChild(page);
                fixedDoc.Pages.Add(pageContent);
                page.Width = pageSize.Width;
                page.Height = pageSize.Height;
                Canvas canvas = new();
                FixedPage.SetLeft(canvas, capabilities.PageImageableArea.OriginWidth);
                FixedPage.SetTop(canvas, capabilities.PageImageableArea.OriginHeight);
                canvas.Width = visibleSize.Width;
                canvas.Height = visibleSize.Height;
                canvas.Background = vb;
                page.Children.Add(canvas);
                yOffset += visibleSize.Height;
            }
            return fixedDoc;
        }
    }
}
