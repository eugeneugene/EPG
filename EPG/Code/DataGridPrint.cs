using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace EPG.Code
{
    public static class DataGridPrint
    {
        public static void Print(DataGrid dataGrid, string title)
        {
            PrintDialog printDialog = new();

            if (printDialog.ShowDialog() == true)
            {
                FlowDocument fd = new();

                Paragraph p = new(new Run(title));
                p.FontStyle = dataGrid.FontStyle;
                p.FontFamily = dataGrid.FontFamily;
                p.FontSize = 18;
                fd.Blocks.Add(p);

                Table table = new();
                TableRowGroup tableRowGroup = new();
                TableRow r = new();
                fd.PageWidth = printDialog.PrintableAreaWidth;
                fd.PageHeight = printDialog.PrintableAreaHeight;
                fd.BringIntoView();

                fd.TextAlignment = TextAlignment.Center;
                fd.ColumnWidth = 500;
                table.CellSpacing = 0;

                var headerList = dataGrid.Columns.Select(e => e.Header?.ToString() ?? string.Empty).ToList();
                List<dynamic> bindList = new();

                for (int j = 0; j < headerList.Count; j++)
                {
                    r.Cells.Add(new TableCell(new Paragraph(new Run(headerList[j]))));
                    r.Cells[j].ColumnSpan = 4;
                    r.Cells[j].Padding = new Thickness(4);
                    r.Cells[j].BorderBrush = Brushes.Black;
                    r.Cells[j].FontWeight = FontWeights.Bold;
                    r.Cells[j].Background = Brushes.DarkGray;
                    r.Cells[j].Foreground = Brushes.White;
                    r.Cells[j].BorderThickness = new Thickness(1, 1, 1, 1);

                    var column = dataGrid.Columns[j] as DataGridBoundColumn;
                    if (column is not null)
                    {
                        var binding = column.Binding as Binding;
                        if (binding is not null)
                            bindList.Add(binding.Path.Path);
                    }
                }

                tableRowGroup.Rows.Add(r);
                table.RowGroups.Add(tableRowGroup);

                for (int i = 0; i < dataGrid.Items.Count; i++)
                {
                    dynamic row;

                    if (dataGrid.ItemsSource.ToString().ToLower() == "system.data.linqdataview")
                    {
                        row = (DataRowView)dataGrid.Items.GetItemAt(i);
                    }
                    else
                    {
                        //row = (BalanceClient)dataGrid.Items.GetItemAt(i);
                        row = (DataRowView)dataGrid.Items.GetItemAt(i);
                    }

                    table.BorderBrush = Brushes.Gray;
                    table.BorderThickness = new Thickness(1, 1, 0, 0);
                    table.FontStyle = dataGrid.FontStyle;
                    table.FontFamily = dataGrid.FontFamily;
                    table.FontSize = 13;
                    tableRowGroup = new TableRowGroup();
                    r = new TableRow();

                    for (int j = 0; j < row.Row.ItemArray.Count(); j++)
                    {
                        if (dataGrid.ItemsSource.ToString().ToLower() == "system.data.linqdataview")
                        {
                            r.Cells.Add(new TableCell(new Paragraph(new Run(row.Row.ItemArray[j].ToString()))));
                        }
                        else
                        {
                            r.Cells.Add(new TableCell(new Paragraph(new Run(row.GetType().GetProperty(bindList[j]).GetValue(row, null)))));
                        }

                        r.Cells[j].ColumnSpan = 4;
                        r.Cells[j].Padding = new Thickness(4);
                        r.Cells[j].BorderBrush = Brushes.DarkGray;
                        r.Cells[j].BorderThickness = new Thickness(0, 0, 1, 1);
                    }

                    tableRowGroup.Rows.Add(r);
                    table.RowGroups.Add(tableRowGroup);
                }

                fd.Blocks.Add(table);
                printDialog.PrintDocument(((IDocumentPaginatorSource)fd).DocumentPaginator, "");
            }
        }
    }
}
