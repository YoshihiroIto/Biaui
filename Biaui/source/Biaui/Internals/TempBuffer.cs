using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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

    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public static class TempBufferExtensions
    {
        public static TempBuffer<T> ToTempBuffer<T>(this IEnumerable<T> source, Span<T> initialBuffer)
        {
            var initialLength = InitialLength(source);

            if (initialLength == -1 || initialLength <= initialBuffer.Length)
            {
                var tb = new TempBuffer<T>(initialBuffer);

                tb.AddFrom(source);

                return tb;
            }
            else
            {
                var tb = new TempBuffer<T>(initialLength);

                tb.AddFrom(source);

                return tb;
            }
        }

        public static TempBuffer<T> ToTempBuffer<T>(this IEnumerable<T> source, int initialCapacity = 64)
        {
            var maxLength = Math.Max(InitialLength(source), initialCapacity);

            var tb = new TempBuffer<T>(maxLength);

            tb.AddFrom(source);

            return tb;
        }

        public static TempBuffer<T> ToTempBuffer<T>(this IEnumerable source, Span<T> initialBuffer)
        {
            var initialLength = InitialLength<T>(source);

            if (initialLength == -1 || initialLength <= initialBuffer.Length)
            {
                var tb = new TempBuffer<T>(initialBuffer);

                tb.AddFrom(source);

                return tb;
            }
            else
            {
                var tb = new TempBuffer<T>(initialLength);

                tb.AddFrom(source);

                return tb;
            }
        }

        public static TempBuffer<T> ToTempBuffer<T>(this IEnumerable source, int initialCapacity = 64)
        {
            var maxLength = Math.Max(InitialLength<T>(source), initialCapacity);

            var tb = new TempBuffer<T>(maxLength);

            tb.AddFrom(source);

            return tb;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int InitialLength<T>(IEnumerable<T> source)
        {
            if (source is ICollection c1)
                return c1.Count;

            if (source is ICollection<T> c2)
                return c2.Count;

            if (source is IReadOnlyCollection<T> c3)
                return c3.Count;

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int InitialLength<T>(IEnumerable source)
        {
            if (source is ICollection c1)
                return c1.Count;

            if (source is ICollection<T> c2)
                return c2.Count;

            if (source is IReadOnlyCollection<T> c3)
                return c3.Count;

            return -1;
        }
    }
}