using EPG.Printing.Documents;
using System.Collections.Generic;

namespace EPG.Models
{
    public class PasswordResultPage : IDataGridPrintable<PasswordResultItem>
    {
        public PasswordResultHeader Header { get; }
        public PasswordResultModel ResultModel { get; }

        public IEnumerable<PasswordResultItem> Items => ResultModel.DataCollection;

        IDataGridPrintable<PasswordResultItem> IDataGridPrintable<PasswordResultItem>.CreatePage(IReadOnlyList<PasswordResultItem> items, uint pageIndex, uint pageCount)
        {
            var header = Header.UpdatePageIndexCount(pageIndex, pageCount);
            return new PasswordResultPage(header, new PasswordResultModel(items, ResultModel.Mode, ResultModel.ShowHyphenated, ResultModel.CalculateComplexity, ResultModel.Include, ResultModel.Exclude));
        }

        public PasswordResultPage(PasswordResultHeader header, PasswordResultModel resultModel)
        {
            Header = header;
            ResultModel = resultModel;
        }
    }
}
