using System.Windows.Input;

namespace BFM.Commands
{
    internal static class BFMCommands
    {
        public static RoutedCommand BloomFilterOpenCommand { get; } = new RoutedCommand(nameof(BloomFilterOpenCommand), typeof(App));
        public static RoutedCommand BloomFilterImportCommand { get; } = new RoutedCommand(nameof(BloomFilterImportCommand), typeof(App));
        public static RoutedCommand BloomFilterCloseCommand { get; } = new RoutedCommand(nameof(BloomFilterCloseCommand), typeof(App));

        public static RoutedCommand BloomFilterCreateCommand { get; } = new RoutedCommand(nameof(BloomFilterCreateCommand), typeof(App));
        public static RoutedCommand TextFileOpenCommand { get; } = new RoutedCommand(nameof(TextFileOpenCommand), typeof(App));
    }
}
