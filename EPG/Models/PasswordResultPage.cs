using EPG.Printing.Documents;
using System.Collections.Generic;

namespace EPG.Models
{
    public class PasswordResultPage : IDataGridPrintable<PasswordResultItem>
    {
        public PasswordResultHeader Header { get; }
        public PasswordResultModel ResultModel { get; }

        public IEnumerable<PasswordResultItem> Items => ResultModel.DataCollection;

        object IDataGridPrintable<PasswordResultItem>.CreatePage(IReadOnlyList<PasswordResultItem> items, int pageIndex, int pageCount)
        {
            var header = Header.UpdatePageIndexCount(pageIndex, pageCount);
            return new PasswordResultPage(header, ResultModel);
        }

        public PasswordResultPage(PasswordResultHeader header, PasswordResultModel resultModel)
        {
            Header = header;
            ResultModel = resultModel;
        }
    }
}
