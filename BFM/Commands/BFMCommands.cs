using System.Windows.Input;

namespace BFM.Commands
{
    internal static class BFMCommands
    {
        public static RoutedCommand BloomFilterBrowseCommand { get; } = new RoutedCommand(nameof(BloomFilterBrowseCommand), typeof(App));
    }
}
