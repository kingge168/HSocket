using System;
using System.Collections;
using System.Collections.Generic;
namespace Spider.Data
{
    //非线程安全的线性表
    public class LineList<T> : IList<T>
    {
        private class Node<D>
        {
            public Node(D data, Node<D> next)
            {
                Data = data;
                Next = next;
            }
            public Node(D data)
                : this(data, null)
            {
            }
            public Node()
                : this(default(D))
            {
            }
            public D Data { set; get; }
            public Node<D> Next { set; get; }
        }
        private Node<T> _head;
        private int _count;
        private Node<T> Head
        {
            get { return _head; }
            set { _head = value; }
        }
        public LineList()
        {
            Head = new Node<T>();
            Count = 0;
        }
        public LineList(IEnumerable<T> nodes)
            : this()
        {
            if (nodes != null)
            {
                foreach (T item in nodes)
                {
                    Add(item);
                }
            }
        }
        public int IndexOf(T item)
        {
            int index = -1;
            bool flag = false;
            Node<T> node = Head.Next;
            while (node != null)
            {
                index++;
                if (node.Data.Equals(item))
                {
                    flag = true;
                    break;
                }
                node = node.Next;
            }
            if (flag)
            {
                return index;
            }
            else
            {
                return -1;
            }
        }

        public void Insert(int index, T item)
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("The object is readonly.");
            }
            else if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            else
            {
                int count = 0;
                bool success = false;
                Node<T> node = Head;
                while (node.Next != null)
                {
                    if (index == count)
                    {
                        Node<T> ItemNode = new Node<T>(item);
                        ItemNode.Next = node.Next;
                        node.Next = ItemNode;
                        Count++;
                        success = true;
                        break;
                    }
                    node = node.Next;
                    count++;
                }
                if (!success)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
            }
        }

        public void RemoveAt(int index)
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("The object is readonly.");
            }
            else
            {
                int count = 0;
                bool success = false;
                Node<T> node = Head;
                while (node.Next != null)
                {
                    if (index == count)
                    {
                        node.Next = node.Next.Next;
                        success = true;
                        break;
                    }
                    node = node.Next;
                    count++;
                }
                if (!success)
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }

        public T this[int index]
        {
            get
            {
                if (index < 0)
                {
                    throw new IndexOutOfRangeException();
                }
                Node<T> node = Head.Next;
                int count = 0;
                while (node != null)
                {
                    if (index == count)
                    {
                        return node.Data;
                    }
                    node = node.Next;
                    count++;
                }
                throw new IndexOutOfRangeException();
            }
            set
            {
                if (index < 0)
                {
                    throw new IndexOutOfRangeException();
                }
                Node<T> node = Head.Next;
                bool flag = false;
                int count = 0;
                while (node != null)
                {
                    if (index == count)
                    {
                        node.Data = value;
                        flag = true;
                        break;
                    }
                    node = node.Next;
                    count++;
                }
                if (!flag)
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }

        public void Add(T item)
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("The object is readonly.");
            }
            else
            {
                Node<T> node = Head;
                while (node.Next != null)
                {
                    node = node.Next;
                }
                Node<T> last = new Node<T>(item);
                node.Next = last;
                Count++;
            }
        }

        public void Clear()
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("The object is readonly.");
            }
            else
            {
                Head.Next = null;
                Count = 0;
            }
        }

        public bool Contains(T item)
        {
            Node<T> node = Head.Next;
            while (node != null)
            {
                if (node.Data.Equals(item))
                {
                    return true;
                }
                node = node.Next;
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }
            else
            {
                Array.Copy(this.ToArray(), 0, array, arrayIndex, Count);
            }
        }
        protected T[] ToArray()
        {
            T[] array = new T[Count];
            Node<T> node = Head;
            int i = 0;
            while (node.Next != null)
            {
                node = node.Next;
                array[i] = node.Data;
                i++;
            }
            return array;
        }
        public int Count
        {
            get
            {
                return _count;
            }
            private set
            {
                _count = value;
            }
        }

        public bool IsReadOnly
        {
            set;
            get;
        }

        public bool Remove(T item)
        {
            Node<T> node = Head;
            while (node.Next != null)
            {
                if (node.Next.Data.Equals(item))
                {
                    node.Next = node.Next.Next;
                    Count--;
                    return true;
                }
                node = node.Next;
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            Node<T> node = Head;
            while (node.Next != null)
            {
                node = node.Next;
                yield return node.Data;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            Node<T> node = Head;
            while (node.Next != null)
            {
                node = node.Next;
                yield return node.Data;
            }
        }
    }
}
