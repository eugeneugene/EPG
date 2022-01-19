using System;
using System.Threading;

namespace WpfNotification
{
    internal static class ThreadingExtensions
    {
        /// <summary>
        /// Replaces the value under location
        /// if a distinct condition is fulfilled for the prevalent value.
        /// </summary>
        /// <typeparam name="T">The type of the value to replace.</typeparam>
        /// <param name="location">
        /// The place, where the value shall be replaced.
        /// </param>
        /// <param name="value">
        /// The value, which is assigned, when the condition is fulfilled.
        /// </param>
        /// <param name="condition">
        /// The condition,
        /// which shall be fulfilled when the condition is being performed.
        /// </param>
        /// <param name="current">
        /// The value, which has been prevalent before the replacement.
        /// </param>
        /// <returns>
        /// Whether the replacement has occurred.
        /// </returns>
        public static bool ConditionalCompareExchange<T>(ref T location, T value, Func<T, bool> condition, out T current) where T : class
        {
            Thread.MemoryBarrier();
            current = location;
            while (true)
            {
                if (!condition(current))
                    return false;
                T prevalent = Interlocked.CompareExchange(
                    ref location, value, current);
                if (ReferenceEquals(prevalent, current))
                    return true;
                current = prevalent;
            }
        }

        /// <summary>
        /// Replaces the value under location
        /// if a distinct condition is fulfilled for the prevalent value.
        /// </summary>
        /// <typeparam name="T">The type of the value to replace.</typeparam>
        /// <param name="location">
        /// The place, where the value shall be replaced.
        /// </param>
        /// <param name="value">
        /// The value, which is assigned, when the condition is fulfilled.
        /// </param>
        /// <param name="condition">
        /// The condition,
        /// which shall be fulfilled when the condition is being performed.
        /// </param>
        /// <returns>
        /// Whether the replacement has occurred.
        /// </returns>
        public static bool ConditionalCompareExchange<T>(ref T location, T value, Func<T, bool> condition) where T : class
        {
            return ConditionalCompareExchange(
                ref location, value, condition, out _);
        }
    }
}
