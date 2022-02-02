using System;

namespace EPG.Models
{
    public class PasswordResultHeader
    {
        public string Title { get; }
        public string Version { get; }
        public string Include { get; }
        public string Exclude { get; }
        public DateTime GenerationDate { get; }
        public int PasswordsGenerated { get; }
        public string Mode { get; }
        public int PageIndex { get; }
        public int PageCount { get; }

        public int PageIndexPlus1 => PageIndex + 1;

        public
            PasswordResultHeader(
                string title,
                string version,
                string include,
                string exclude,
                DateTime generationDate,
                int passwordsGenerated,
                string mode,
                int pageIndex,
                int pageCount
            )
        {
            Title = title;
            Version = version;
            Include = include;
            Exclude = exclude;
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
                    Version,
                    Include,
                    Exclude,
                    GenerationDate,
                    PasswordsGenerated,
                    Mode,
                    pageIndex,
                    pageCount
                );
        }
    }
}
