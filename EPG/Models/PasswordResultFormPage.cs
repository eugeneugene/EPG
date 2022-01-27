using EPG.Printing.Documents;
using System.Collections.Generic;

namespace EPG.Models
{
    public class PasswordResultFormPage : IDataGridPrintable<PasswordResultItem>
    {
        public PasswordResultFormHeader Header { get; }
        public IReadOnlyList<PasswordResultItem> Items { get; }

        IEnumerable<PasswordResultItem> IDataGridPrintable<PasswordResultItem>.Items => Items;

        object IDataGridPrintable<PasswordResultItem>.CreatePage(IReadOnlyList<PasswordResultItem> items, int pageIndex, int pageCount)
        {
            var header = Header.UpdatePageIndexCount(pageIndex, pageCount);
            return new PasswordResultFormPage(header, items);
        }

        public PasswordResultFormPage(PasswordResultFormHeader header, IReadOnlyList<PasswordResultItem> items)
        {
            Header = header;
            Items = items;
        }
    }
}
