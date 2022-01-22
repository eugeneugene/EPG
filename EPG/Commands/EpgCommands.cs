﻿using System.Windows.Input;

namespace EPG.Commands
{
    internal class EpgCommands
    {
        public static RoutedCommand CommandGenerate { get; } = new RoutedCommand(nameof(CommandGenerate), typeof(MainWindow));
        public static RoutedCommand EditClear { get; } = new RoutedCommand(nameof(EditClear), typeof(MainWindow));
    }
}