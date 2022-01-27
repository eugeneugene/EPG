using System;

namespace EPG.Models
{
    public class PasswordResultFormHeader
    {
        public string Title { get; }
        public DateTime GenerationDate { get; }
        public int PasswordsGenerated { get; }
        public string Mode { get; }
        public int PageIndex { get; }
        public int PageCount { get; }

        public int PageIndexPlus1 => PageIndex + 1;

        public
            PasswordResultFormHeader(
                string title,
                DateTime generationDate,
                int passwordsGenerated,
                string mode,
                int pageIndex,
                int pageCount
            )
        {
            Title = title;
            GenerationDate = generationDate;
            PasswordsGenerated = passwordsGenerated;
            Mode = mode;
            PageIndex = pageIndex;
            PageCount = pageCount;
        }

        public PasswordResultFormHeader UpdatePageIndexCount(int pageIndex, int pageCount)
        {
            return
                new PasswordResultFormHeader(
                    Title,
                    GenerationDate,
                    PasswordsGenerated,
                    Mode,
                    pageIndex,
                    pageCount
                );
        }
    }
}
