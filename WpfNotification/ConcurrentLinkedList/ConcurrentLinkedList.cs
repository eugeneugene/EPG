using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace WpfNotification
{
    /// <summary>
    /// Provides an implementation of a LockFree DoublyLinkedList.
    /// </summary>
    public static class ConcurrentLinkedList
    {
        /// <summary>
        /// Creates a new <see cref="IConcurrentLinkedList{T}"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements in the created
        /// <see cref="IConcurrentLinkedList{T}"/>.
        /// </typeparam>
        /// <returns>The newly created instance.</returns>
        public static IConcurrentLinkedList<T> Create<T>() where T : class
        {
            return new ConcurrentLinkedListInternal<T>();
        }

        /// <summary>
        /// Creates a new <see cref="IConcurrentLinkedList{T}"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements in the created
        /// <see cref="IConcurrentLinkedList{T}"/>.
        /// </typeparam>
        /// <param name="initial">
        /// The initial elements in the created list.
        /// </param>
        /// <returns>The newly created instance.</returns>
        public static IConcurrentLinkedList<T> Create<T>(IEnumerable<T> initial) where T : class
        {
            return new ConcurrentLinkedListInternal<T>(initial);
        }

        private static bool CompareExchangeNodeLink<T>(ref NodeLink<T> location, NodeLink<T> value, NodeLink<T> comparandByValue) where T : class
        {
            return ThreadingExtensions.ConditionalCompareExchange(ref location, value, original => original.Equals(comparandByValue));
        }

        private static bool CompareExchangeNodeLinkInPair<T>(ref ValueNodeLinkPair<T> location, NodeLink<T> newLink, NodeLink<T> comparadByValue) where T : class
        {
            Thread.MemoryBarrier();
            T currentValue = location.Value;
            return ThreadingExtensions.ConditionalCompareExchange(ref location, new ValueNodeLinkPair<T>(currentValue, newLink), original => original.Link.Equals(comparadByValue) && ReferenceEquals(original.Value, currentValue));
        }

        private class ConcurrentLinkedListInternal<T> : IConcurrentLinkedList<T> where T : class
        {
            public readonly Node<T> HeadNode;
            public readonly Node<T> TailNode;

            public IEnumerator<T> GetEnumerator()
            {
                IConcurrentLinkedListNode<T> current = Head;
                while (true)
                {
                    current = current.Next;
                    if (current == Tail)
                        yield break;
                    yield return current.Value;
                }
            }

            public IConcurrentLinkedListNode<T> Head => HeadNode;

            public IConcurrentLinkedListNode<T> Tail => TailNode;

            public IConcurrentLinkedListNode<T> UnDequeue(T value)
            {
                SpinWait spin = new();
                Node<T> node = new(this);
                Node<T> prev = HeadNode;

                Thread.MemoryBarrier();
                Node<T> next = prev.Next_.Link.P;

                while (true)
                {
                    /* node has not been made public yet,
                     * so no synchronization constructs are necessary. */
                    node.Prev_ = new NodeLink<T>(prev, false);
                    node.Next_ = new ValueNodeLinkPair<T>(value, new NodeLink<T>(next, false));

                    if (CompareExchangeNodeLinkInPair(ref prev.Next_, new NodeLink<T>(node, false), new NodeLink<T>(next, false)))
                        break;

                    next = prev.Next_.Link.P;
                    spin.SpinOnce();
                }

                PushEnd(node, next, spin);
                Thread.MemoryBarrier();
                return node;
            }

            public IConcurrentLinkedListNode<T> Enqueue(T value)
            {
                SpinWait spin = new();
                Node<T> node = new(this);
                Node<T> next = TailNode;

                Thread.MemoryBarrier();
                Node<T> prev = next.Prev_.P;

                while (true)
                {
                    /* node has not been made public yet,
                     * so no threading constructs are necessary. */
                    node.Prev_ = new NodeLink<T>(prev, false);
                    node.Next_ = new ValueNodeLinkPair<T>(value, new NodeLink<T>(next, false));

                    if (CompareExchangeNodeLinkInPair(ref prev.Next_, new NodeLink<T>(node, false), new NodeLink<T>(next, false)))
                        break;

                    prev = CorrectPrev(prev, next);
                    spin.SpinOnce();
                }

                PushEnd(node, next, spin);
                Thread.MemoryBarrier();
                return node;
            }

            public IConcurrentLinkedListNode<T> Dequeue()
            {
                SpinWait spin = new();
                Node<T> next = TailNode;

                Thread.MemoryBarrier();
                Node<T> node = next.Prev_.P;

                while (true)
                {
                    Thread.MemoryBarrier();
                    if (!node.Next_.Link.Equals(new NodeLink<T>(next, false)))
                    {
                        node = CorrectPrev(node, next);
                        continue;
                    }

                    if (node == HeadNode)
                    {
                        Thread.MemoryBarrier();
                        return null;
                    }

                    if (CompareExchangeNodeLinkInPair(ref node.Next_, new NodeLink<T>(next, true), new NodeLink<T>(next, false)))
                    {
                        Node<T> prev = node.Prev_.P;
                        CorrectPrev(prev, next);
                        Thread.MemoryBarrier();
                        return node;
                    }

                    spin.SpinOnce();
                }
            }

            public static void SetMark(ref NodeLink<T> link)
            {
                Thread.MemoryBarrier();
                NodeLink<T> node = link;

                while (true)
                {
                    if (node.D)
                        break;

                    NodeLink<T> prevalent = Interlocked.CompareExchange(ref link, new NodeLink<T>(node.P, true), node);

                    if (prevalent == node)
                        break;
                    node = prevalent;
                }
            }

            public static Node<T> CorrectPrev(Node<T> prev, Node<T> node)
            {
                return CorrectPrev(prev, node, new SpinWait());
            }

            public static Node<T> CorrectPrev(Node<T> prev, Node<T> node, SpinWait spin)
            {
                Node<T> lastLink = null;
                while (true)
                {
                    Thread.MemoryBarrier();
                    NodeLink<T> link1 = node.Prev_;

                    if (link1.D)
                        break;

                    Thread.MemoryBarrier();
                    NodeLink<T> prev2 = prev.Next_.Link;

                    if (prev2.D)
                    {
                        if (lastLink is not null)
                        {
                            SetMark(ref prev.Prev_);

                            CompareExchangeNodeLinkInPair(ref lastLink.Next_,
                                (NodeLink<T>)prev2.P, (NodeLink<T>)prev);

                            prev = lastLink;
                            lastLink = null;
                            continue;
                        }

                        Thread.MemoryBarrier();
                        prev2 = prev.Prev_;
                        prev = prev2.P;
                        continue;
                    }
                    /* The paper simply states „Prev_ != node“,
                     * but the types are different.
                     * It is probably assumed
                     * that the comaparison is performed as follows:
                     * !(prev2.P == node && !prev2.D).
                     * Since prev2.D is always false here,
                     * simplification is possible. */
                    if (prev2.P != node)
                    {
                        lastLink = prev;
                        prev = prev2.P;
                        continue;
                    }

                    if (CompareExchangeNodeLink(ref node.Prev_, new NodeLink<T>(prev, false), link1))
                    {
                        if (prev.Prev_.D)
                            continue;
                        break;
                    }

                    spin.SpinOnce();
                }

                return prev;
            }

            /// <summary>
            /// Creates a new empty concurrentLinkedList.
            /// </summary>
            public ConcurrentLinkedListInternal()
            {
                HeadNode = new Node<T>(this);
                TailNode = new Node<T>(this);

                HeadNode.Prev_ = new NodeLink<T>(null, false);
                HeadNode.Next_ = new ValueNodeLinkPair<T>(null, new NodeLink<T>(TailNode, false));
                TailNode.Prev_ = new NodeLink<T>(HeadNode, false);
                TailNode.Next_ = new ValueNodeLinkPair<T>(null, new NodeLink<T>(null, false));
                Thread.MemoryBarrier();
            }

            /// <summary>
            /// Creates a new concurrentLinkedList
            /// which contains the contents of the enumeration initial.
            /// </summary>
            /// <param name="initial">The enumeration to copy.</param>
            public ConcurrentLinkedListInternal(IEnumerable<T> initial) : this()
            {
                if (initial is null)
                    throw new ArgumentNullException(nameof(initial));
                foreach (T value in initial)
                    Enqueue(value);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private static void PushEnd(Node<T> node, Node<T> next, SpinWait spin)
            {
                while (true)
                {
                    Thread.MemoryBarrier();
                    NodeLink<T> link1 = next.Prev_;

                    if (link1.D)
                        break;

                    Thread.MemoryBarrier();
                    if (!node.Next_.Link.Equals(new NodeLink<T>(next, false)))
                        break;

                    if (CompareExchangeNodeLink(ref next.Prev_, new NodeLink<T>(node, false), link1))
                    {
                        if (node.Prev_.D)
                            CorrectPrev(node, next);
                        break;
                    }

                    spin.SpinOnce();
                }
            }
        }

        private class Node<T> : IConcurrentLinkedListNode<T> where T : class
        {
            public ValueNodeLinkPair<T> Next_;
            public NodeLink<T> Prev_;
            public readonly ConcurrentLinkedListInternal<T> List_;

#if DEBUG
            public long Id { get; }
#endif

            public IConcurrentLinkedList<T> List => List_;

            public bool IsDummyNode =>
                this == List_.HeadNode || this == List_.TailNode;

            public T Value
            {
                get
                {
                    if (IsDummyNode)
                        ThrowIsDummyNodeException();
                    /* At the commented out code it is assumed
                     * that Value_ is not allowed to be readout
                     * once the node was deleted.
                     * However, this behaviour does not seem useful. */
                    Thread.MemoryBarrier();
                    T value = Next_.Value;
                    Thread.MemoryBarrier();
                    return value;
                }
                set
                {
                    if (IsDummyNode)
                        ThrowIsDummyNodeException();
                    Thread.MemoryBarrier();
                    while (true)
                    {
                        ValueNodeLinkPair<T> currentPair = Next_;
                        if (Interlocked.CompareExchange(ref Next_, new ValueNodeLinkPair<T>(value, currentPair.Link), currentPair) == currentPair)
                            break;
                    }
                }
            }

            public IConcurrentLinkedListNode<T> Next
            {
                get
                {
                    Node<T> cursor = this;
                    bool b = ToNext(ref cursor);
                    Thread.MemoryBarrier();
                    if (b)
                        return cursor;
                    return null;
                }
            }

            public IConcurrentLinkedListNode<T> Prev
            {
                get
                {
                    Node<T> cursor = this;
                    bool b = ToPrev(ref cursor);
                    Thread.MemoryBarrier();
                    if (b)
                        return cursor;
                    return null;
                }
            }

            public bool Removed
            {
                get
                {
                    Thread.MemoryBarrier();
                    bool result = Next_.Link.D;
                    Thread.MemoryBarrier();
                    return result;
                }
            }

            public IConcurrentLinkedListNode<T> InsertBefore(T newValue)
            {
                IConcurrentLinkedListNode<T> result = InsertBefore(newValue, this);
                Thread.MemoryBarrier();
                return result;
            }

            public IConcurrentLinkedListNode<T> InsertAfter(T newValue)
            {
                IConcurrentLinkedListNode<T> result = InsertAfter(newValue, this);
                Thread.MemoryBarrier();
                return result;
            }

            public IConcurrentLinkedListNode<T> InsertAfterIf(T newValue, Func<T, bool> condition)
            {
                if (IsDummyNode)
                    return null;

                SpinWait spin = new();
                Node<T> cursor = this;
                Node<T> node = new(List_);
                Node<T> prev = cursor;
                Node<T> next;
                while (true)
                {
                    Thread.MemoryBarrier();
                    ValueNodeLinkPair<T> nextLink = prev.Next_;

                    next = nextLink.Link.P;
                    node.Prev_ = new NodeLink<T>(prev, false);
                    node.Next_ = new ValueNodeLinkPair<T>(newValue, new NodeLink<T>(next, false));

                    bool cexSuccess;
                    ValueNodeLinkPair<T> currentPair = nextLink;
                    while (true)
                    {
                        if (!condition(currentPair.Value))
                        {
                            Thread.MemoryBarrier();
                            return null;
                        }

                        if (!currentPair.Link.Equals(
                            new NodeLink<T>(next, false)))
                        {
                            cexSuccess = false;
                            break;
                        }

                        ValueNodeLinkPair<T> prevalent = Interlocked.CompareExchange(ref cursor.Next_, new ValueNodeLinkPair<T>(currentPair.Value, new NodeLink<T>(node, false)), currentPair);
                        if (ReferenceEquals(prevalent, currentPair))
                        {
                            cexSuccess = true;
                            break;
                        }

                        currentPair = prevalent;
                    }

                    if (cexSuccess)
                        break;

                    if (currentPair.Link.D)
                    {
                        Thread.MemoryBarrier();
                        return null;
                    }

                    spin.SpinOnce();
                }

                ConcurrentLinkedListInternal<T>.CorrectPrev(prev, next);
                Thread.MemoryBarrier();
                return node;
            }

            public bool Remove()
            {
                if (IsDummyNode)
                    return false;

                while (true)
                {
                    Thread.MemoryBarrier();
                    NodeLink<T> next = Next_.Link;

                    if (next.D)
                    {
                        Thread.MemoryBarrier();
                        return false;
                    }

                    if (CompareExchangeNodeLinkInPair(ref Next_, new NodeLink<T>(next.P, true), next))
                    {
                        NodeLink<T> prev;
                        while (true)
                        {
                            prev = Prev_;

                            if (prev.D)
                                break;

                            if (CompareExchangeNodeLink(ref Prev_, new NodeLink<T>(prev.P, true), prev))
                                break;
                        }

                        ConcurrentLinkedListInternal<T>.CorrectPrev(prev.P, next.P);
                        Thread.MemoryBarrier();
                        return true;
                    }
                }
            }

            public T CompareExchangeValue(T newValue, T comparand)
            {
                ValueNodeLinkPair<T> currentPair;
                Thread.MemoryBarrier();
                while (true)
                {
                    currentPair = Next_;
                    if (!ReferenceEquals(currentPair.Value, comparand))
                        return currentPair.Value;
                    if (ReferenceEquals(Interlocked.CompareExchange(ref Next_, new ValueNodeLinkPair<T>(newValue, currentPair.Link), currentPair), currentPair))
                        break;
                }

                return currentPair.Value;
            }

            public Node(ConcurrentLinkedListInternal<T> list)
            {
                List_ = list;
#if DEBUG
                Id = Interlocked.Increment(ref nextId) - 1;
#endif
                /* Value_ is flushed
                 * at the moment the current node instance is published
                 * (by CompareExchange). */
            }

#if DEBUG
            private static long nextId;
#endif

            private static void ThrowIsDummyNodeException()
            {
                throw new InvalidOperationException(
                    "The current node is the dummy head or dummy tail node " +
                    "of the current List, so it may not store any value.");
            }

            private bool ToNext(ref Node<T> cursor)
            {
                while (true)
                {
                    if (cursor == List_.TailNode)
                        return false;

                    Thread.MemoryBarrier();
                    Node<T> next = cursor.Next_.Link.P;

                    Thread.MemoryBarrier();
                    bool d = next.Next_.Link.D;

                    if (d)
                    {
                        Thread.MemoryBarrier();
                        if (!cursor.Next_.Link.Equals(new NodeLink<T>(next, true)))
                        {
                            ConcurrentLinkedListInternal<T>.SetMark(ref next.Prev_);

                            Thread.MemoryBarrier();
                            Node<T> p = next.Next_.Link.P;
                            _ = CompareExchangeNodeLinkInPair(ref cursor.Next_, (NodeLink<T>)p, (NodeLink<T>)next);
                            continue;
                        }
                    }

                    cursor = next;
                    if (!d)
                        return true;
                }
            }

            private bool ToPrev(ref Node<T> cursor)
            {
                while (true)
                {
                    if (cursor == List_.HeadNode)
                        return false;

                    Thread.MemoryBarrier();
                    Node<T> prev = cursor.Prev_.P;

                    Thread.MemoryBarrier();

                    if (prev.Next_.Link.Equals(new NodeLink<T>(cursor, false)))
                    {
                        Thread.MemoryBarrier();
                        if (!cursor.Next_.Link.D)
                        {
                            cursor = prev;
                            return true;
                        }
                        else
                        {
                            Thread.MemoryBarrier();

                            if (cursor.Next_.Link.D)
                                ToNext(ref cursor);
                            else
                                ConcurrentLinkedListInternal<T>.CorrectPrev(prev, cursor);
                        }
                    }
                }
            }

            private IConcurrentLinkedListNode<T> InsertBefore(T value, Node<T> cursor, SpinWait spin = new SpinWait())
            {
                if (cursor == List_.HeadNode)
                    return InsertAfter(value);
                Node<T> node = new(List_);
                Thread.MemoryBarrier();
                Node<T> prev = cursor.Prev_.P;
                Node<T> next;

                while (true)
                {
                    while (true)
                    {
                        Thread.MemoryBarrier();
                        if (!cursor.Next_.Link.D)
                            break;

                        /* Since cursor was deleted
                         * the method correctPrev has not returned a node 
                         * which is logically before cursor;
                         * the return value shall not have semantic meaning.
                         * As correctPrev apparently exprects
                         * a logical predecessor of node / cursor,
                         * prev cannot be passed to the method.
                         * This is dire for program execution
                         * especially when prev == List_.tailNode. */

                        ToNext(ref cursor);

                        #region Bugfix 1
                        /* Ascertain a new predecessor of cursor. */
                        Thread.MemoryBarrier();
                        prev = cursor.Prev_.P;
                        #endregion

                        prev = ConcurrentLinkedListInternal<T>.CorrectPrev(prev, cursor);
                    }

                    next = cursor;
                    node.Prev_ = new NodeLink<T>(prev, false);
                    node.Next_ = new ValueNodeLinkPair<T>(value, new NodeLink<T>(next, false));

                    if (CompareExchangeNodeLinkInPair(ref prev.Next_, new NodeLink<T>(node, false), new NodeLink<T>(cursor, false)))
                        break;

                    prev = ConcurrentLinkedListInternal<T>.CorrectPrev(prev, cursor);
                    spin.SpinOnce();
                }

                ConcurrentLinkedListInternal<T>.CorrectPrev(prev, next);
                return node;
            }

            private IConcurrentLinkedListNode<T> InsertAfter(T value, Node<T> cursor)
            {
                SpinWait spin = new();

                if (cursor == List_.TailNode)
                    return InsertBefore(value, cursor, spin);
                Node<T> node = new(List_);
                Node<T> prev = cursor;
                Node<T> next;
                while (true)
                {
                    Thread.MemoryBarrier();
                    next = prev.Next_.Link.P;

                    node.Prev_ = new NodeLink<T>(prev, false);
                    node.Next_ = new ValueNodeLinkPair<T>(value, new NodeLink<T>(next, false));

                    if (CompareExchangeNodeLinkInPair(ref cursor.Next_, new NodeLink<T>(node, false), new NodeLink<T>(next, false)))
                        break;

                    if (prev.Next_.Link.D)
                        return InsertBefore(value, cursor, spin);
                    spin.SpinOnce();
                }

                ConcurrentLinkedListInternal<T>.CorrectPrev(prev, next);
                return node;
            }
        }

        private class NodeLink<T> where T : class
        {
            public Node<T> P { get; }
            public bool D { get; }

            public override bool Equals(object obj)
            {
                if (!(obj is NodeLink<T>))
                    return false;

                var other = (NodeLink<T>)obj;
                return D == other.D && P == other.P;
            }

            public bool Equals(NodeLink<T> other)
            {
                if (other is null)
                    return false;

                return D == other.D && P == other.P;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(P, D);
            }

            public static explicit operator NodeLink<T>(Node<T> node)
            {
                return new NodeLink<T>(node, false);
            }

            public static explicit operator Node<T>(NodeLink<T> link)
            {
#if DEBUG
                /* I am not sure,
                 * whether it is simply assumed in the document
                 * that the conversion is always possible. */
                if (link.D)
                    throw new ArgumentException("link.D is null");
#endif
                return link.P;
            }

            public NodeLink(Node<T> p, bool d)
            {
                P = p;
                D = d;
            }
        }

        private class ValueNodeLinkPair<T> where T : class
        {
            public T Value { get; }
            public NodeLink<T> Link { get; }

            public ValueNodeLinkPair(T value, NodeLink<T> link)
            {
                Value = value;
                Link = link;
            }
        }
    }
}
