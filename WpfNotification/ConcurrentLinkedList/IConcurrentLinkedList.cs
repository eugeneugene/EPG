using System.Collections.Generic;

namespace WpfNotification
{
    public interface IConcurrentLinkedList<T> : IEnumerable<T> where T : class
    {
        /// <summary>
        /// The dummy head node (leftmost).
        /// </summary>
        IConcurrentLinkedListNode<T> Head { get; }
        /// <summary>
        /// The dummy tail node (rightmost).
        /// </summary>
        IConcurrentLinkedListNode<T> Tail { get; }

        /// <summary>
        /// Removes the rightmost non-dummy node, if it exists.
        /// </summary>
        /// <returns>
        /// null, if the list is empty; else the removed node.
        /// </returns>
        IConcurrentLinkedListNode<T> Dequeue();

        /// <summary>
        /// Inserts a new node at the head position.
        /// </summary>
        /// <param name="value">The initial value of the new node.</param>
        /// <returns>The new inserted node.</returns>
        IConcurrentLinkedListNode<T> UnDequeue(T value);

        /// <summary>
        /// Inserts a new node at the tail position.
        /// </summary>
        /// <param name="value">The initial value of the new node.</param>
        /// <returns>The new inserted node.</returns>
        IConcurrentLinkedListNode<T> Enqueue(T value);
    }
}