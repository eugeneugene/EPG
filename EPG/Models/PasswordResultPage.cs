using EPG.Printing.Documents;
using System.Collections.Generic;

namespace EPG.Models
{
    public class PasswordResultPage : IDataGridPrintable<PasswordResultItem>
    {
        public PasswordResultHeader Header { get; }
        public IReadOnlyList<PasswordResultItem>? Items { get; }

        IEnumerable<PasswordResultItem> IDataGridPrintable<PasswordResultItem>.Items => Items;

        object IDataGridPrintable<PasswordResultItem>.CreatePage(IReadOnlyList<PasswordResultItem> items, int pageIndex, int pageCount)
        {
            var header = Header.UpdatePageIndexCount(pageIndex, pageCount);
            return new PasswordResultPage(header, items);
        }

        public PasswordResultPage(PasswordResultHeader header, IReadOnlyList<PasswordResultItem>? items)
        {
            Header = header;
            Items = items;
        }
    }
}
