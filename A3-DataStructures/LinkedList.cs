namespace COIS2020.priashabarua0778496.Assignment3
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public sealed class Node<T>
    {
        public T Item { get; set; }
        public Node<T>? Next { get; internal set; }
        public Node<T>? Prev { get; internal set; }

        public Node(T item)
        {
            Item = item;
        }
    }

    public class LinkedList<T> : IEnumerable<T>
    {
        public Node<T>? Head { get; protected set; }
        public Node<T>? Tail { get; protected set; }

        public LinkedList()
        {
            Head = null;
            Tail = null;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<T> GetEnumerator()
        {
            Node<T>? curr = Head;
            while (curr != null)
            {
                yield return curr.Item;
                curr = curr.Next;
            }
        }

        [MemberNotNullWhen(false, nameof(Head))]
        [MemberNotNullWhen(false, nameof(Tail))]
        public bool IsEmpty
        {
            get
            {
                bool h = Head == null;
                bool t = Tail == null;
                if (h ^ t)
                    throw new Exception("Head and Tail should either both be null or both non-null.");
                return h;
            }
        }

        public void AddFront(T item)
        {
            AddFront(new Node<T>(item));
        }

        public void AddFront(Node<T> node)
        {
            if (IsEmpty)
            {
                Head = Tail = node;
            }
            else
            {
                node.Next = Head;
                Head.Prev = node;
                Head = node;
            }
        }

        public void AddBack(T item)
        {
            AddBack(new Node<T>(item));
        }

        public void AddBack(Node<T> node)
        {
            if (IsEmpty)
            {
                Head = Tail = node;
            }
            else
            {
                node.Prev = Tail;
                Tail.Next = node;
                Tail = node;
            }
        }

        public void InsertAfter(Node<T> node, T item)
        {
            InsertAfter(node, new Node<T>(item));
        }

        public void InsertAfter(Node<T> node, Node<T> newNode)
        {
            if (node == Tail)
            {
                AddBack(newNode);
            }
            else
            {
                newNode.Next = node.Next;
                newNode.Prev = node;
                node.Next!.Prev = newNode;
                node.Next = newNode;
            }
        }

        public void InsertBefore(Node<T> node, T item)
        {
            InsertBefore(node, new Node<T>(item));
        }

        public void InsertBefore(Node<T> node, Node<T> newNode)
        {
            if (node == Head)
            {
                AddFront(newNode);
            }
            else
            {
                newNode.Next = node;
                newNode.Prev = node.Prev;
                node.Prev!.Next = newNode;
                node.Prev = newNode;
            }
        }

        public void Remove(Node<T> node)
        {
            if (node == Head)
            {
                Head = Head.Next;
                if (Head != null)
                {
                    Head.Prev = null;
                }
            }
            else if (node == Tail)
            {
                Tail = Tail.Prev;
                if (Tail != null)
                {
                    Tail.Next = null;
                }
            }
            else
            {
                node.Prev!.Next = node.Next;
                node.Next!.Prev = node.Prev;
            }

            if (Head == null)
            {
                Tail = null;
            }
        }

        public void Remove(T item)
        {
            Node<T>? node = Find(item);
            if (node != null)
            {
                Remove(node);
            }
        }

        public LinkedList<T> SplitAfter(Node<T> node)
        {
            if (node == null || node.Next == null)
            {
                return new LinkedList<T>();
            }

            LinkedList<T> newList = new LinkedList<T>
            {
                Head = node.Next,
                Tail = Tail
            };

            newList.Head!.Prev = null;
            node.Next = null;
            Tail = node;

            return newList;
        }

        public void AppendAll(LinkedList<T> otherList)
        {
            if (otherList.IsEmpty)
            {
                return;
            }

            if (IsEmpty)
            {
                Head = otherList.Head;
                Tail = otherList.Tail;
            }
            else
            {
                Tail!.Next = otherList.Head;
                otherList.Head!.Prev = Tail;
                Tail = otherList.Tail;
            }

            otherList.Head = otherList.Tail = null;
        }

        public int Count()
        {
            int count = 0;
            Node<T>? curr = Head;
            while (curr != null)
            {
                count++;
                curr = curr.Next;
            }
            return count;
        }

        public Node<T>? Find(T item)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            Node<T>? curr = Head;
            while (curr != null)
            {
                if (comparer.Equals(curr.Item, item))
                {
                    return curr;
                }
                curr = curr.Next;
            }
            return null;
        }
    }
}
