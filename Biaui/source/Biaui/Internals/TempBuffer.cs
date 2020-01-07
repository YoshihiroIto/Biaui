using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

// ReSharper disable CheckNamespace

namespace Jewelry.Memory
{
    public ref struct TempBuffer<T>
    {
        public Span<T> Buffer => _buffer.Slice(0, _pos);
        public int Length => _pos;

        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                Debug.Assert(index < _pos);
                return ref _buffer[index];
            }
        }

        private T[] _arrayToReturnToPool;
        private Span<T> _buffer;
        private int _pos;

        public TempBuffer(Span<T> initialBuffer)
        {
            _arrayToReturnToPool = null;
            _buffer = initialBuffer;
            _pos = 0;
        }

        public TempBuffer(int initialCapacity)
        {
            _arrayToReturnToPool = ArrayPool<T>.Shared.Rent(initialCapacity);
            _buffer = _arrayToReturnToPool;
            _pos = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            var toReturn = _arrayToReturnToPool;
            this = default;

            if (toReturn != null)
                ArrayPool<T>.Shared.Return(toReturn);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T value)
        {
            if (_buffer.Length == _pos)
                Glow();

            _buffer[_pos] = value;
            ++_pos;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddFrom(IEnumerable source)
        {
            foreach (var o in source)
                Add((T)o);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddFrom(IEnumerable<T> source)
        {
            foreach (var o in source)
                Add(o);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T value)
        {
            for (var i = 0; i != _pos; ++i)
                if (EqualityComparer<T>.Default.Equals(_buffer[i], value))
                    return i;

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T FirstOrDefault(Func<T, bool> predicate)
        {
            for (var i = 0; i != _pos; ++i)
                if (predicate(_buffer[i]))
                    return _buffer[i];

            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T FirstOrDefault()
        {
            return Length != 0 ? _buffer[0] : default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T LastOrDefault()
        {
            return Length != 0 ? _buffer[_pos - 1] : default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Glow()
        {
            var l = Math.Max(_buffer.Length, 16);

            var length = l + (l >> 1);
            var nextBuffer = ArrayPool<T>.Shared.Rent(length);

            _buffer.CopyTo(nextBuffer);

            var toReturn = _arrayToReturnToPool;

            _arrayToReturnToPool = nextBuffer;
            _buffer = nextBuffer;

            if (toReturn != null)
                ArrayPool<T>.Shared.Return(toReturn);
        }
    }
}