
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
    /// <summary>可以作为双端队列访问的对象的列表集合</summary>
    /// <typeparam name="T">指定队列中元素的类型.</typeparam>
    [Serializable]
    public class Deque<T> : IEnumerable<T>, ICollection, IEnumerable
    {
        // 在内部，deque存储为一个数组，其中head指向第一个元素

        private T[] array;
        private int head;
        private int size;
        private int version;

        /// <summary>获取一个值，该值指示是否访问 <see cref="T:System.Collections.ICollection" /> 
        ///是同步的（线程安全）。</summary>
        /// <returns>如果访问 <see cref="T:System.Collections.ICollection" /> 同步（线程安全）；否则为false。在deque的默认实现中，此属性始终返回false。</returns>
        bool ICollection.IsSynchronized { get { return false; } }

        /// <summary>获取一个对象，该对象可用于同步
        /// <see cref="T:System.Collections.ICollection" />.</summary>
        /// <returns>可用于同步
        /// <see cref="T:System.Collections.ICollection" />.  在deque的默认实现中
        /// 此属性始终返回当前实例.</returns>
        object ICollection.SyncRoot { get { return this; } }

        /// <summary>获取deque中包含的元素数</summary>
        /// <returns>deque中包含的元素数.</returns>
        public int Count { get { return size; } }

        internal int Capacity { get { return array.Length; } }

        /// <summary>初始化deque类的新实例，该实例为空，并且具有默认的初始容量.</summary>
        public Deque () {
            array = new T[0];
        }

        /// <summary>用指定的容量初始化deque类的新实例.</summary>
        /// <param name="capacity">The initial capacity of the deque.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="capacity" /> is less than zero.</exception>
        public Deque (int capacity) {
            if (capacity < 0) {
                throw new ArgumentOutOfRangeException("count");
            }
            array = new T[capacity];
        }

        /// <summary>初始化包含从指定集合复制的元素的deque类的新实例.</summary>
        /// <param name="collection">其元素被复制到新deque的集合.</param>
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

        /// <summary>复制<see cref="T:System.Collections.ICollection" /> 
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

        /// <summary>从deque中移除所有对象.</summary>
        /// <filterpriority>1</filterpriority>
        public void Clear () {
            Array.Clear(array, 0, array.Length);
            head = size = 0;
            version++;
        }

        /// <summary>确定元素是否在deque中.</summary>
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

        /// <summary>将deque元素复制到现有的一维 <see cref="T:System.Array" />, 从指定的数组索引开始.</summary>
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

        /// <summary>移除并返回deque开头的对象.</summary>
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

        /// <summary>移除并返回deque末尾的对象.</summary>
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

        /// <summary>返回deque开头的对象，而不删除它.</summary>
        /// <returns>The object at the beginning of the deque.</returns>
        /// <exception cref="T:System.InvalidOperationException">The deque is empty.</exception>
        public T PeekFirst () {
            if (size == 0) { throw new InvalidOperationException(); }
            return array[head];
        }

        /// <summary>返回deque末尾的对象，而不移除它.</summary>
        /// <returns>The object at the beginning of the deque.</returns>
        /// <exception cref="T:System.InvalidOperationException">The deque is empty.</exception>
        public T PeekLast () {
            if (size == 0) { throw new InvalidOperationException(); }
            int last = (head + size - 1) % array.Length;
            return array[last];
        }

        /// <summary>在deque的末尾添加一个对象.</summary>
        /// <param name="item">The object to add to the deque. The value can be null for reference types.</param>
        public void AddLast (T item) {
            if (size == array.Length) { SetCapacity(System.Math.Max(size * 2, 4)); }

            int next = (head + size) % array.Length;
            array[next] = item;

            size++;
            version++;
        }

        /// <summary>在deque的开头前面添加一个对象.</summary>
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

        /// <summary>将deque元素复制到新数组中</summary>
        /// <returns>A new array containing elements copied from the deque.</returns>
        public T[] ToArray () {
            T[] newarray = new T[size];
            CopyTo(newarray, 0);
            return newarray;
        }

        /// <summary>如果容量小于当前容量的90%，则将容量设置为deque中的实际元素数.</summary>
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

        /// <summary>返回遍历deque的枚举器</summary>
        /// <returns>An enumerator for the deque.</returns>
        public Deque<T>.Enumerator GetEnumerator () {
            return new Deque<T>.Enumerator(this);
        }


        /// <summary>枚举元素.</summary>
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

            /// <summary>获取枚举器当前位置的元素.</summary>
            /// <returns>The element in the collection at the current position of the enumerator.</returns>
            /// <exception cref="T:System.InvalidOperationException">The enumerator is positioned 
            /// before the first element of the collection or after the last element. </exception>
            object IEnumerator.Current { get { return this.Current; } }

            /// <summary>获取枚举器当前位置的元素.</summary>
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

            /// <summary>将枚举数设置为其初始位置，该位置位于集合中第一个元素之前.</summary>
            /// <exception cref="T:System.InvalidOperationException">集合已修改 
            /// 创建枚举器之后</exception>
            void IEnumerator.Reset () {
                if (version != d.version) { throw new InvalidOperationException(); }
                index = NOT_STARTED;
            }

            /// <summary>释放枚举器使用的所有资源.</summary>
            public void Dispose () {
                index = NOT_STARTED; //? FINISHED?
            }

            /// <summary>将枚举数推进到deque的下一个元素.</summary>
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

