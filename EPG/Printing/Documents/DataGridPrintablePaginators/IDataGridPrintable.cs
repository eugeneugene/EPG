using System.Collections.Generic;

namespace EPG.Printing.Documents
{
    public interface IDataGridPrintable<TItem>
    {
        /// <summary>
        /// Gets the sequence of items.
        /// </summary>
        IEnumerable<TItem> Items { get; }

        /// <summary>
        /// Creates an object which represents a page with the specified items.
        /// </summary>
        IDataGridPrintable<TItem> CreatePage(IReadOnlyList<TItem> items, uint pageIndex, uint pageCount);
    }
}
