using System;

namespace WpfNotification
{
    /// <summary>
    /// A node of a LockFreeDoublyLinkedList instance.
    /// </summary>
    public interface IConcurrentLinkedListNode<T> where T : class
    {
        /// <summary>
        /// The value stored by the current node instance.
        /// </summary>
        T Value { get; set; }

        /// <summary>
        /// Returns the corresponding LockFreeDoublyLinkedList
        /// of the current node instance.
        /// </summary>
        IConcurrentLinkedList<T> List { get; }

        /// <summary>
        /// The right neighbor node or null,
        /// if the current node is the dummy tail node.
        /// </summary>
        IConcurrentLinkedListNode<T> Next { get; }
        /// <summary>
        /// The left neighbor node or null,
        /// if the current node is the dummy head node.
        /// </summary>
        IConcurrentLinkedListNode<T> Prev { get; }

        /// <summary>
        /// Returns, if the current node has been removed.
        /// </summary>
        bool Removed { get; }

        /// <summary>
        /// Returns, if the current node is
        /// the dummy head or dummy tail node of List.
        /// </summary>
        bool IsDummyNode { get; }

        /// <summary>
        /// Inserts a new node left beside the current node instance.
        /// </summary>
        /// <param name="newValue">The initial value of the new node.</param>
        /// <returns>The new inserted node.</returns>
        IConcurrentLinkedListNode<T> InsertBefore(T newValue);
        /// <summary>
        /// Inserts a new node right beside the current node instance.
        /// </summary>
        /// <param name="newValue">The initial value of the new node.</param>
        /// <returns>The new inserted node.</returns>
        IConcurrentLinkedListNode<T> InsertAfter(T newValue);

        /// <summary>
        /// Inserts a new node after the current node instance
        /// if and only if:
        /// <list type="bullet">
        ///     <item><description>
        ///         the current value is no dummy node,
        ///     </description></item>
        ///     <item><description>
        ///         the current node has not yet been deleted
        ///     </description></item>
        ///     <item><description>
        ///         and the <paramref name="condition"/>
        ///         for the current node’s value is satisfied.
        ///     </description></item>
        /// </list>
        /// </summary>
        /// <param name="newValue">
        /// The new value for the node to insert.
        /// </param>
        /// <param name="condition">
        /// The condition which the current node’s value
        /// needs to satisfy for the insertion to take place.
        /// </param>
        /// <returns>
        /// The inserted node,
        /// if the insertion could be performed.
        /// <c>null</c> else.
        /// </returns>
        IConcurrentLinkedListNode<T> InsertAfterIf(T newValue, Func<T, bool> condition);

        /// <summary>
        /// Removes the current node
        /// from the corresponding LockFreeDoublyLinkedList instance.
        /// </summary>
        /// <returns>
        /// Whether the current node has been deleted by this thread.
        /// Also returns false if the current node is a dummy node.
        /// </returns>
        bool Remove();

        /// <summary>
        /// Compares the Value property to comparand
        /// and replaces the prevalent value with newValue
        /// if and only if comparand reference equals the prevalent value.
        /// This happens in a single atomic operation.
        /// </summary>
        /// <param name="newValue">The value to write.</param>
        /// <param name="comparand">The original value </param>
        /// <returns>
        /// The prevalent value,
        /// regardless of whether the replacement took place.
        /// </returns>
        T CompareExchangeValue(T newValue, T comparand);
    }
}
