using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public class MySortedList<T> : IList<T> where T : IComparable
    {
        private List<T> internalList = new List<T>();

        public MySortedList() { }

        public MySortedList(List<T> t, bool sort)
        {
            internalList = t;
            if (sort)
                internalList.Sort();
        }

        public MySortedList(MySortedList<T> t)
        {
            internalList = t.List();
        }

        public void Add(T item)
        {
            int i = findPositionByBinarySearch(this, item);
            if (i > internalList.Count - 1)
                internalList.Add(item);
            else
                internalList.Insert(i, item);
        }

        public bool Remove(T item)
        {
            int i = IndexOf(item);
            int l = Count;
            RemoveAt(i);
            return l != Count;
        }

        public void RemoveAt(int i)
        {
            if (i < 0 || i >= internalList.Count)
                return;
            internalList.RemoveAt(i);
        }

        public int IndexOf(T item)
        {
            int i = findPositionByBinarySearch(this, item);
            if (i > internalList.Count - 1)
                return -1;
            if (item.Equals(internalList[i]))
            {
                return i;
            } 
            else
            {
                return -1;
            }
        }

        private int findPositionByBinarySearch(MySortedList<T> list, T item)
        {
            
            int length = list.Count;
            if (length == 0)
                return 0;
            T x = list[length / 2];
            if (x.Equals(item))
            {
                return length / 2;
            }
            else if (x.CompareTo(item) > 0)
            {
                return findPositionByBinarySearch(new MySortedList<T>(list.Take(length / 2 - 1)), item);
            }
            else
            {
                return length / 2 + 1 + findPositionByBinarySearch(new MySortedList<T>(list.Skip(length / 2 + 1)), item);
            }
        }

        public List<T> List()
        {
            return new List<T>(internalList);
        }

        public MySortedList<T> Take(int take)
        {
             return new MySortedList<T>(internalList.Take(take).ToList(), false);
        }

        public MySortedList<T> Skip(int skip)
        {
            return new MySortedList<T>(internalList.Skip(skip).ToList(), false);
        }

        public void Sort()
        {
            internalList.Sort();
        }

        public void Insert(int index, T item)
        {
            internalList.Insert(index, item);
        }

        public T this[int index]
        {
            get
            {
                return internalList[index];
            }
            set
            {
                internalList[index] = value;
            }
        }


        public void Clear()
        {
            internalList.Clear();
        }

        public bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            internalList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return internalList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return internalList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return internalList.GetEnumerator();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (internalList.Count == 0)
                return "()";
            sb.Append("(");
            foreach (T x in internalList)
            {
                sb.Append(x+", ");
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append(")");
            return sb.ToString();
        }

        public MySortedList<T> Concat(MySortedList<T> msl, bool sort)
        {
            return new MySortedList<T>(internalList.Concat(msl.internalList).ToList(), sort);
        }
    }
}
