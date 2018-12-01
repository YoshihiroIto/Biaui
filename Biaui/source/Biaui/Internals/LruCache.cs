// Copyright (c) 2013-2016 Yoshihiro Ito (yo.i.jewelry.bab@gmail.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is furnished
// to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

// ReSharper disable CheckNamespace

using System;
using System.Collections.Generic;

namespace Jewelry.Collections
{
    /// <summary>
    /// Simple Implementation C# LRU Cache
    /// </summary>
    public class LruCache<TKey, TValue>
    {
        public LruCache(int maxCapacity, bool isThreadSafe)
        {
            _maxCapacity = maxCapacity;
            _lockObj = isThreadSafe ? new object() : null;
        }

        protected virtual int GetValueSize(TValue value)
        {
            return 1;
        }

        protected virtual void OnDiscardedValue(TKey key, TValue value)
        {
        }

        public void Clear()
        {
            if (_lockObj == null)
                ClearInternal();

            else
            {
                lock (_lockObj)
                {
                    ClearInternal();
                }
            }
        }

        public bool Contains(TKey key)
        {
            if (_lockObj == null)
                return ContainsInternal(key);

            else
            {
                lock (_lockObj)
                {
                    return ContainsInternal(key);
                }
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_lockObj == null)
                return TryGetValueInternal(key, out value);

            else
            {
                lock (_lockObj)
                {
                    return TryGetValueInternal(key, out value);
                }
            }
        }

        public TValue Get(TKey key)
        {
            if (_lockObj == null)
                return GetInternal(key);

            else
            {
                lock (_lockObj)
                {
                    return GetInternal(key);
                }
            }
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            if (_lockObj == null)
                return GetOrAddInternal(key, valueFactory);

            else
            {
                lock (_lockObj)
                {
                    return GetOrAddInternal(key, valueFactory);
                }
            }
        }

        public void Add(TKey key, TValue value)
        {
            if (_lockObj == null)
                AddInternal(key, value);

            else
            {
                lock (_lockObj)
                {
                    AddInternal(key, value);
                }
            }
        }

        public void Remove(TKey key)
        {
            if (_lockObj == null)
                RemoveInternal(key);

            else
            {
                lock (_lockObj)
                {
                    RemoveInternal(key);
                }
            }
        }

        private void ClearInternal()
        {
            foreach (var valueNode in _list)
                OnDiscardedValue(valueNode.Key, valueNode.Value);

            _list.Clear();
            _lookup.Clear();
        }

        private bool ContainsInternal(TKey key)
        {
            return _lookup.ContainsKey(key);
        }

        private bool TryGetValueInternal(TKey key, out TValue value)
        {
            if (_lookup.TryGetValue(key, out var listNode))
            {
                _list.Remove(listNode);
                _list.AddFirst(listNode);

                value = listNode.Value.Value;
                return true;
            }

            value = default(TValue);
            return false;
        }

        private TValue GetInternal(TKey key)
        {
            if (_lookup.TryGetValue(key, out var listNode))
            {
                _list.Remove(listNode);
                _list.AddFirst(listNode);

                return listNode.Value.Value;
            }

            return default(TValue);
        }

        private TValue GetOrAddInternal(TKey key, Func<TKey, TValue> valueFactory)
        {
            if (_lookup.TryGetValue(key, out var listNode))
            {
                _list.Remove(listNode);
                _list.AddFirst(listNode);

                return listNode.Value.Value;
            }

            var value = valueFactory(key);
            AddInternal(key, value);
            return value;
        }

        private void AddInternal(TKey key, TValue value)
        {
            if (_lookup.TryGetValue(key, out var listNode))
            {
                _currentSize -= GetValueSize(listNode.Value.Value);

                _list.Remove(listNode);
                _list.AddFirst(listNode);

                listNode.Value.Value = value;

                _currentSize += GetValueSize(listNode.Value.Value);
            }
            else
            {
                var keyValue = new KeyValue {Key = key, Value = value};

                listNode = _list.AddFirst(keyValue);

                _lookup.Add(key, listNode);

                _currentSize += GetValueSize(listNode.Value.Value);
            }

            while (_currentSize > _maxCapacity)
            {
                var valueNode = _list.Last;

                _list.RemoveLast();
                _lookup.Remove(valueNode.Value.Key);

                _currentSize -= GetValueSize(valueNode.Value.Value);

                OnDiscardedValue(valueNode.Value.Key, valueNode.Value.Value);
            }
        }

        private void RemoveInternal(TKey key)
        {
            if (_lookup.TryGetValue(key, out var listNode) == false)
                return;

            OnDiscardedValue(listNode.Value.Key, listNode.Value.Value);

            _currentSize -= GetValueSize(listNode.Value.Value);

            _list.Remove(listNode);
            _lookup.Remove(key);
        }

        private class KeyValue
        {
            public TKey Key { get; set; }
            public TValue Value { get; set; }
        }

        private readonly LinkedList<KeyValue> _list = new LinkedList<KeyValue>();

        private readonly Dictionary<TKey, LinkedListNode<KeyValue>> _lookup =
            new Dictionary<TKey, LinkedListNode<KeyValue>>();

        private readonly object _lockObj;

        private readonly int _maxCapacity;
        private int _currentSize;
    }
}