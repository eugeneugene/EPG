using EPG.Printing.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EPG.Printing.Documents
{
    public struct DataGridPrintablePaginator<TItem>
    {
        private sealed class PaginateFunction
        {
            private readonly IDataGridPrintable<TItem> printable;
            private readonly TItem[] allItems;
            private readonly Size pageSize;

            private int index;
            private int pageIndex;

            ContentPresenter PagePresenterFromRestItems()
            {
                var restItems = new ArraySegment<TItem>(allItems, index, allItems.Length - index);
                var presenter = new ContentPresenter()
                {
                    Content = printable.CreatePage(restItems, pageIndex, pageCount: 1),
                    Width = pageSize.Width,
                    Height = pageSize.Height,
                };
                presenter.Measure(pageSize);
                presenter.Arrange(new Rect(new Point(0, 0), pageSize));
                presenter.UpdateLayout();
                return presenter;
            }

            IPrintableDataGrid DataGridFromPagePresenter(ContentPresenter presenter)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(presenter); i++)
                {
                    var child = VisualTreeHelper.GetChild(presenter, i);
                    if (child is IPrintableDataGridContainer printable)
                        return printable.DataGrid;
                }
                throw new InvalidOperationException($"{nameof(DataTemplate)} of printable page must directly generate a control implementing {nameof(IPrintableDataGridContainer)}.");
            }

            int CountVisibleRows(IPrintableDataGrid dataGrid)
            {
                var actualMeasure = dataGrid.ActualMeasure;

                var totalMeasure = 0.0;
                var count = 0;

                while (index + count < allItems.Length)
                {
                    totalMeasure += dataGrid.ItemMeasure(count);
                    if (totalMeasure > actualMeasure)
                        break;

                    count++;
                }

                Debug.WriteLine("Count: {0}", count);
                return count;
            }

            IEnumerable PagesFromChunks(IEnumerable<ArraySegment<TItem>> chunks)
            {
                var pages = new List<IDataGridPrintable<TItem>>();
                int page = 0;
                foreach (var chunk in chunks)
                    pages.Add(printable.CreatePage(chunk, page++, chunks.Count()));

                return pages;
            }

            public IEnumerable Paginate()
            {
                var chunks = new List<ArraySegment<TItem>>();

                while (index < allItems.Length)
                {
                    var presenter = PagePresenterFromRestItems();
                    var dataGrid = DataGridFromPagePresenter(presenter);
                    var count = CountVisibleRows(dataGrid);
                    if (count == 0)
                        throw new InfinitePaginationException();

                    chunks.Add(new ArraySegment<TItem>(allItems, index, count));
                    index += count;
                    pageIndex++;
                }

                return PagesFromChunks(chunks);
            }

            public PaginateFunction(IDataGridPrintable<TItem> printable, TItem[] allItems, Size pageSize)
            {
                this.printable = printable;
                this.pageSize = pageSize;
                this.allItems = allItems;
            }
        }

        /// <summary>
        /// Paginates a printable into papers of the specified size.
        /// </summary>
        /// <param name="printable"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IEnumerable Paginate(IDataGridPrintable<TItem> printable, Size pageSize)
        {
            var allItems = printable.Items.ToArray();
            var function = new PaginateFunction(printable, allItems, pageSize);
            return function.Paginate();
        }
    }
}
