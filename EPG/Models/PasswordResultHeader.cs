using System;

namespace EPG.Models
{
    public class PasswordResultHeader
    {
        public string Title { get; }
        public DateTime GenerationDate { get; }
        public int PasswordsGenerated { get; }
        public string Mode { get; }
        public int PageIndex { get; }
        public int PageCount { get; }

        public int PageIndexPlus1 => PageIndex + 1;

        public
            PasswordResultHeader(
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

        public PasswordResultHeader UpdatePageIndexCount(int pageIndex, int pageCount)
        {
            return
                new PasswordResultHeader(
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
