﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BFF.DataVirtualizingCollection
{
    internal abstract class VirtualizationBase<T> :
        IVirtualizationBase<T>
    {
        public void CopyTo(Array array, int index)
        {
            throw new NotSupportedException();
        }

        int ICollection.Count => GetCountInner();

        public bool IsSynchronized => false;
        
        public object SyncRoot { get; } = new object();
        
        public abstract int Count { get; }
        
        protected readonly CompositeDisposable CompositeDisposable = new CompositeDisposable();

        int ICollection<T>.Count => GetCountInner();

        int IReadOnlyCollection<T>.Count => GetCountInner();

        bool ICollection<T>.IsReadOnly => true;
        object? IList.this[int index]
        {
            get => IndexerInnerGet(index);
            set => throw new NotSupportedException();
        }

        protected abstract T IndexerInnerGet(int index);

        public bool IsReadOnly => true;

        public T this[int index]
        {
            get => IndexerInnerGet(index);
            set => throw new NotSupportedException();
        }

        private int GetCountInner() => Count;
        
        public IEnumerator<T> GetEnumerator()
        {
            return Iterate().GetEnumerator();

            IEnumerable<T> Iterate()
            {
                for (var i = 0; i < Count; i++)
                {
                    yield return GetItemForEnumerator(i);
                }
            }
        }

        protected abstract T GetItemForEnumerator(int i);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private int IndexOfInner() => -1;

        public int IndexOf(T item)
        {
            return IndexOfInner();
        }

        private bool ContainsInner() => IndexOfInner() != -1;

        public bool Contains(T item)
        {
            return ContainsInner();
        }

        public void Add(T item)
        {
            throw new NotSupportedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        public void Remove(object value)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public bool IsFixedSize => true;

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public int Add(object value)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(object value)
        {
            return ContainsInner();
        }

        public int IndexOf(object value)
        {
            return IndexOf((T) value);
        }

        public void Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public abstract void Reset();

        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public void Dispose()
        {
            InitializationCompleted
                .ContinueWith(_ => CompositeDisposable.Dispose());
        }

        protected void OnCollectionChangedReplace(T newItem, T oldItem, int index)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, index));
        }

        protected void OnCollectionChangedRemove(T oldItem, int index)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index));
        }

        protected void OnCollectionChangedAdd(T newItem, int index)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem, index));
        }

        protected virtual void OnCollectionChangedReset()
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnIndexerChanged()
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            OnPropertyChanged("Item[]");
        }

        public abstract Task InitializationCompleted { get; }
        public abstract int SelectedIndex { get; set; }
    }
}