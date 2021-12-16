using System.Windows.Input;

namespace BFM.Commands
{
    internal static class BFMCommands
    {
        public static RoutedCommand BloomFilterOpenCommand { get; } = new RoutedCommand(nameof(BloomFilterOpenCommand), typeof(App));
        public static RoutedCommand BloomFilterImportCommand { get; } = new RoutedCommand(nameof(BloomFilterImportCommand), typeof(App));
    }
}
