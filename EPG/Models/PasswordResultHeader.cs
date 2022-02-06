using System;

namespace EPG.Models
{
    public class PasswordResultHeader
    {
        public string Title { get; }
        public string Version { get; }
        public DateTime GenerationDate { get; }
        public int PasswordsGenerated { get; }
        public int PageIndex { get; }
        public int PageCount { get; }

        public int PageIndexPlus1 => PageIndex + 1;

        public
            PasswordResultHeader(
                string title,
                string version,
                DateTime generationDate,
                int passwordsGenerated,
                int pageIndex,
                int pageCount
            )
        {
            Title = title;
            Version = version;
            GenerationDate = generationDate;
            PasswordsGenerated = passwordsGenerated;
            PageIndex = pageIndex;
            PageCount = pageCount;
        }

        public PasswordResultHeader UpdatePageIndexCount(int pageIndex, int pageCount)
        {
            return
                new PasswordResultHeader(
                    Title,
                    Version,
                    GenerationDate,
                    PasswordsGenerated,
                    pageIndex,
                    pageCount
                );
        }
    }
}
