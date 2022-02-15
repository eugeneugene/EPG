using System;

namespace EPG.Models
{
    public class PasswordResultHeader
    {
        public string Title { get; }
        public string Version { get; }
        public DateTime GenerationDate { get; }
        public uint PasswordsGenerated { get; }
        public uint PageIndex { get; }
        public uint PageCount { get; }

        public uint PageIndexPlus1 => PageIndex + 1;

        public
            PasswordResultHeader(
                string title,
                string version,
                DateTime generationDate,
                uint passwordsGenerated,
                uint pageIndex,
                uint pageCount
            )
        {
            Title = title;
            Version = version;
            GenerationDate = generationDate;
            PasswordsGenerated = passwordsGenerated;
            PageIndex = pageIndex;
            PageCount = pageCount;
        }

        public PasswordResultHeader UpdatePageIndexCount(uint pageIndex, uint pageCount)
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
