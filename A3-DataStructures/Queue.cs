namespace COIS2020.priashabarua0778496.Assignment3
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class Queue<T> : IEnumerable<T>
    {
        public const int DefaultCapacity = 8;

        private T[] buffer;
        private int start;
        private int end;

        public Queue() : this(DefaultCapacity) { }

        public Queue(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than zero.");
            }

            buffer = new T[capacity];
            start = 0;
            end = 0;
        }

        public bool IsEmpty => start == end;

        public int Count
        {
            get
            {
                if (end >= start)
                {
                    return end - start;
                }
                return buffer.Length - start + end;
            }
        }

        public int Capacity => buffer.Length;

        public void Enqueue(T item)
        {
            if (Count == Capacity - 1)
            {
                Grow();
            }

            buffer[end] = item;
            end = (end + 1) % buffer.Length;
        }

        public T Dequeue()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Queue is empty.");
            }

            T item = buffer[start];
            buffer[start] = default(T);
            start = (start + 1) % buffer.Length;
            return item;
        }

        public T Peek()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Queue is empty.");
            }

            return buffer[start];
        }

        private void Grow()
        {
            int newCapacity = Capacity * 2;
            T[] newBuffer = new T[newCapacity];

            if (end > start)
            {
                Array.Copy(buffer, start, newBuffer, 0, end - start);
            }
            else
            {
                Array.Copy(buffer, start, newBuffer, 0, buffer.Length - start);
                Array.Copy(buffer, 0, newBuffer, buffer.Length - start, end);
            }

            buffer = newBuffer;
            start = 0;
            end = Count;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return buffer[(start + i) % buffer.Length];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
