
/****************************************************
 * FileName:		Deque.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-02-13:06:57
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SomaSim.Collections
{
    /// <summary>������Ϊ˫�˶��з��ʵĶ�����б���</summary>
    /// <typeparam name="T">ָ��������Ԫ�ص�����.</typeparam>
    [Serializable]
    public class Deque<T> : IEnumerable<T>, ICollection, IEnumerable
    {
        // ���ڲ���deque�洢Ϊһ�����飬����headָ���һ��Ԫ��

        private T[] array;
        private int head;
        private int size;
        private int version;

        /// <summary>��ȡһ��ֵ����ֵָʾ�Ƿ���� <see cref="T:System.Collections.ICollection" /> 
        ///��ͬ���ģ��̰߳�ȫ����</summary>
        /// <returns>������� <see cref="T:System.Collections.ICollection" /> ͬ�����̰߳�ȫ��������Ϊfalse����deque��Ĭ��ʵ���У�������ʼ�շ���false��</returns>
        bool ICollection.IsSynchronized { get { return false; } }

        /// <summary>��ȡһ�����󣬸ö��������ͬ��
        /// <see cref="T:System.Collections.ICollection" />.</summary>
        /// <returns>������ͬ��
        /// <see cref="T:System.Collections.ICollection" />.  ��deque��Ĭ��ʵ����
        /// ������ʼ�շ��ص�ǰʵ��.</returns>
        object ICollection.SyncRoot { get { return this; } }

        /// <summary>��ȡdeque�а�����Ԫ����</summary>
        /// <returns>deque�а�����Ԫ����.</returns>
        public int Count { get { return size; } }

        internal int Capacity { get { return array.Length; } }

        /// <summary>��ʼ��deque�����ʵ������ʵ��Ϊ�գ����Ҿ���Ĭ�ϵĳ�ʼ����.</summary>
        public Deque () {
            array = new T[0];
        }

        /// <summary>��ָ����������ʼ��deque�����ʵ��.</summary>
        /// <param name="capacity">The initial capacity of the deque.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="capacity" /> is less than zero.</exception>
        public Deque (int capacity) {
            if (capacity < 0) {
                throw new ArgumentOutOfRangeException("count");
            }
            array = new T[capacity];
        }

        /// <summary>��ʼ��������ָ�����ϸ��Ƶ�Ԫ�ص�deque�����ʵ��.</summary>
        /// <param name="collection">��Ԫ�ر����Ƶ���deque�ļ���.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="collection" /> is null.</exception>
        public Deque (IEnumerable<T> collection) {
            if (collection == null) {
                throw new ArgumentNullException("collection");
            }

            ICollection<T> icoll = collection as ICollection<T>;
            int num = (icoll == null) ? 0 : icoll.Count;
            array = new T[num];
            foreach (T current in collection) {
                AddLast(current);
            }
        }

        /// <summary>����<see cref="T:System.Collections.ICollection" /> 
        /// to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.</summary>
        /// <param name="target">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="target" /> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="target" /> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="index" /> is less than zero.</exception>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="target" /> is multidimensional.-or-<paramref name="target" /> does not have zero-based 
        ///   indexing.-or-The number of elements in the source <see cref="T:System.Collections.ICollection" /> is 
        ///   greater than the available space from <paramref name="index" /> to the end of the destination 
        ///   <paramref name="target" />.-or-The type of the source <see cref="T:System.Collections.ICollection" /> 
        ///   cannot be cast automatically to the type of the destination <paramref name="target" />.</exception>
        void ICollection.CopyTo (Array target, int index) {
            if (target == null) { throw new ArgumentNullException("array"); }
            if (index > target.Length) { throw new ArgumentOutOfRangeException("index"); }
            if (target.Length - index < size) { throw new ArgumentException(); }

            if (size == 0) {
                return;
            }

            try {
                int lengthFromHead = array.Length - head;
                Array.Copy(array, head, target, index, System.Math.Min(size, lengthFromHead));
                if (this.size > lengthFromHead) {
                    Array.Copy(this.array, 0, target, index + lengthFromHead, size - lengthFromHead);
                }
            } catch (ArrayTypeMismatchException) {
                throw new ArgumentException();
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator () { return GetEnumerator(); }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator () { return GetEnumerator(); }

        /// <summary>��deque���Ƴ����ж���.</summary>
        /// <filterpriority>1</filterpriority>
        public void Clear () {
            Array.Clear(array, 0, array.Length);
            head = size = 0;
            version++;
        }

        /// <summary>ȷ��Ԫ���Ƿ���deque��.</summary>
        /// <returns>true if <paramref name="item" /> is found in the deque; otherwise, false.</returns>
        /// <param name="item">The object to locate in the deque. The value can be null for reference types.</param>
        public bool Contains (T item) {
            if (item == null) {
                foreach (T current in this) {
                    if (current == null) { return true; }
                }
            } else {
                foreach (T current in this) {
                    if (item.Equals(current)) { return true; }
                }
            }
            return false;
        }

        /// <summary>��dequeԪ�ظ��Ƶ����е�һά <see cref="T:System.Array" />, ��ָ��������������ʼ.</summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from deque. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="array" /> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="arrayIndex" /> is less than zero.</exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in the source deque is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.</exception>
        public void CopyTo (T[] array, int idx) {
            if (array == null) { throw new ArgumentNullException(); }
            ((ICollection)this).CopyTo(array, idx);
        }

        /// <summary>�Ƴ�������deque��ͷ�Ķ���.</summary>
        /// <returns>The object that is removed from the beginning of the deque.</returns>
        /// <exception cref="T:System.InvalidOperationException">The deque is empty.</exception>
        public T RemoveFirst() { 
            T result = PeekFirst();

            array[head] = default(T); // gc helper

            head = (head + 1) % array.Length;
            size--;
            version++;
            return result;
        }

        /// <summary>�Ƴ�������dequeĩβ�Ķ���.</summary>
        /// <returns>The object that is removed from the end of the deque.</returns>
        /// <exception cref="T:System.InvalidOperationException">The deque is empty.</exception>
        public T RemoveLast () {
            T result = PeekLast();

            int last = (head + size - 1) % array.Length;
            array[last] = default(T); // gc helper

            size--;
            version++;
            return result;
        }

        /// <summary>����deque��ͷ�Ķ��󣬶���ɾ����.</summary>
        /// <returns>The object at the beginning of the deque.</returns>
        /// <exception cref="T:System.InvalidOperationException">The deque is empty.</exception>
        public T PeekFirst () {
            if (size == 0) { throw new InvalidOperationException(); }
            return array[head];
        }

        /// <summary>����dequeĩβ�Ķ��󣬶����Ƴ���.</summary>
        /// <returns>The object at the beginning of the deque.</returns>
        /// <exception cref="T:System.InvalidOperationException">The deque is empty.</exception>
        public T PeekLast () {
            if (size == 0) { throw new InvalidOperationException(); }
            int last = (head + size - 1) % array.Length;
            return array[last];
        }

        /// <summary>��deque��ĩβ���һ������.</summary>
        /// <param name="item">The object to add to the deque. The value can be null for reference types.</param>
        public void AddLast (T item) {
            if (size == array.Length) { SetCapacity(System.Math.Max(size * 2, 4)); }

            int next = (head + size) % array.Length;
            array[next] = item;

            size++;
            version++;
        }

        /// <summary>��deque�Ŀ�ͷǰ�����һ������.</summary>
        /// <param name="item">The object to add to the deque. The value can be null for reference types.</param>
        public void AddFirst (T item) {
            if (size == array.Length) { SetCapacity(System.Math.Max(size * 2, 4)); }

            if (--head < 0) {
                head = array.Length - 1;
            }

            array[head] = item;
            size++;
            version++;
        }

        /// <summary>��dequeԪ�ظ��Ƶ���������</summary>
        /// <returns>A new array containing elements copied from the deque.</returns>
        public T[] ToArray () {
            T[] newarray = new T[size];
            CopyTo(newarray, 0);
            return newarray;
        }

        /// <summary>�������С�ڵ�ǰ������90%������������Ϊdeque�е�ʵ��Ԫ����.</summary>
        public void TrimExcess () {
            if (size < array.Length * 0.9) {
                SetCapacity(size);
            }
        }

        private void SetCapacity (int newSize) {
            if (newSize == array.Length) {
                return;
            }

            if (newSize < size) {
                throw new InvalidOperationException("Cannot set capacity below current number of elements");
            }

            T[] newarray = new T[newSize];
            if (size > 0) {
                CopyTo(newarray, 0);
            }

            array = newarray;
            head = 0;
            version++;
        }

        /// <summary>���ر���deque��ö����</summary>
        /// <returns>An enumerator for the deque.</returns>
        public Deque<T>.Enumerator GetEnumerator () {
            return new Deque<T>.Enumerator(this);
        }


        /// <summary>ö��Ԫ��.</summary>
        [Serializable]
        public struct Enumerator : IEnumerator, IDisposable, IEnumerator<T>
        {
            private const int NOT_STARTED = -2;
            private const int FINISHED = -1;

            private Deque<T> d;
            private int index;
            private int version;

            internal Enumerator (Deque<T> deq) {
                index = NOT_STARTED;
                d = deq;
                version = deq.version;
            }

            /// <summary>��ȡö������ǰλ�õ�Ԫ��.</summary>
            /// <returns>The element in the collection at the current position of the enumerator.</returns>
            /// <exception cref="T:System.InvalidOperationException">The enumerator is positioned 
            /// before the first element of the collection or after the last element. </exception>
            object IEnumerator.Current { get { return this.Current; } }

            /// <summary>��ȡö������ǰλ�õ�Ԫ��.</summary>
            /// <returns>The element in the deque at the current position of the enumerator.</returns>
            /// <exception cref="T:System.InvalidOperationException">The enumerator is positioned 
            /// before the first element of the collection or after the last element. </exception>
            public T Current {
                get {
                    if (index < 0) { throw new InvalidOperationException(); }

                    int last = d.head + d.size - 1;
                    int deqindex = (last - index) % d.array.Length;
                    return d.array[deqindex];
                }
            }

            /// <summary>��ö��������Ϊ���ʼλ�ã���λ��λ�ڼ����е�һ��Ԫ��֮ǰ.</summary>
            /// <exception cref="T:System.InvalidOperationException">�������޸� 
            /// ����ö����֮��</exception>
            void IEnumerator.Reset () {
                if (version != d.version) { throw new InvalidOperationException(); }
                index = NOT_STARTED;
            }

            /// <summary>�ͷ�ö����ʹ�õ�������Դ.</summary>
            public void Dispose () {
                index = NOT_STARTED; //? FINISHED?
            }

            /// <summary>��ö�����ƽ���deque����һ��Ԫ��.</summary>
            /// <returns>true if the enumerator was successfully advanced to the next element; 
            /// false if the enumerator has passed the end of the collection.</returns>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified 
            /// after the enumerator was created. </exception>
            public bool MoveNext () {
                if (version != d.version) { throw new InvalidOperationException(); }

                if (index == NOT_STARTED) {
                    index = d.size;
                }

                return index != FINISHED && --index != FINISHED;
            }
        }

    }
}

