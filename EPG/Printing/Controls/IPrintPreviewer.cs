using System;

namespace EPG.Printing.Controls
{
    public interface IPrintPreviewer : IDisposable
    {
        ScaleSelector ScaleSelector { get; }
    }
}
