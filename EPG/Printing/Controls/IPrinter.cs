using System.Collections;
using System.Printing;
using System.Windows;

namespace EPG.Printing.Controls
{
    public interface IPrinter
    {
        string Name { get; }

        void Print(IEnumerable pages, Size pageSize);

        PrintQueue PrintQueue { get; }
        PrintTicket PrintTicket { get; }
    }
}
