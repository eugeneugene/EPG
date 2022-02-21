using System.Windows.Input;

namespace EPG.Commands
{
    internal class EpgCommands
    {
        public static RoutedCommand CommandGenerate { get; } = new RoutedCommand(nameof(CommandGenerate), typeof(MainWindow));
        public static RoutedCommand EditClear { get; } = new RoutedCommand(nameof(EditClear), typeof(MainWindow));
        public static RoutedCommand FilterBrowse { get; } = new RoutedCommand(nameof(FilterBrowse), typeof(MainWindow));
        public static RoutedCommand LangEnglish { get; } = new RoutedCommand(nameof(LangEnglish), typeof(MainWindow));
        public static RoutedCommand LangRussian { get; } = new RoutedCommand(nameof(LangRussian), typeof(MainWindow));
    }
}
