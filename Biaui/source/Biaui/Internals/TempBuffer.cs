using System;
using System.Buffers;
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