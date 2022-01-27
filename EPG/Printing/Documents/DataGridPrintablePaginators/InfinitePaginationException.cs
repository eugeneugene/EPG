using System;

namespace EPG.Printing.Documents
{
    public class InfinitePaginationException : InvalidOperationException
    {
        /// <summary>
        /// Gets the error message.
        /// </summary>
        public override string Message =>
            "Page size is too small to display any item in the data grid.";
    }
}
