using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Jewelry.Text
{
    public ref struct StringSplitter
    {
        public struct StringSpan
        {
            public int Start;
            public int Length;

#if NETSTANDARD2_1 || NETCOREAPP
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public string ToString(string src)
                => ToSpan(src).ToString();
#endif

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ReadOnlySpan<char> ToSpan(ReadOnlySpan<char> src)
                => src.Slice(Start, Length);
        }

        //private StringSpan[]? _arrayToReturnToPool;
        private StringSpan[] _arrayToReturnToPool;
        private Span<StringSpan> _buffer;

        public StringSplitter(Span<StringSpan> initialBuffer)
        {
            _arrayToReturnToPool = null;
            _buffer = initialBuffer;
        }

        public StringSplitter(int initialCapacity)
        {
            _arrayToReturnToPool = ArrayPool<StringSpan>.Shared.Rent(initialCapacity);
            _buffer = _arrayToReturnToPool;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            var toReturn = _arrayToReturnToPool;
            this = default;

            if (toReturn != null)
                ArrayPool<StringSpan>.Shared.Return(toReturn);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<StringSpan> Split(string text, char separator,
            StringSplitOptions options = StringSplitOptions.None)
        {
            if (text == null)
                return ReadOnlySpan<StringSpan>.Empty;

            return Split(text.AsSpan(), separator, options);
        }

        public ReadOnlySpan<StringSpan> Split(ReadOnlySpan<char> text, char separator,
            StringSplitOptions options = StringSplitOptions.None)
        {
            var offset = 0;
            var length = 0;
            var count = 0;

            var isRemoveEmptyEntries = options == StringSplitOptions.RemoveEmptyEntries;

            for (var i = 0; i != text.Length; ++i)
            {
                var c = text[i];

                if (c == separator)
                {
                    if (IsOmit(length, isRemoveEmptyEntries) == false)
                    {
                        if (_buffer.Length == count)
                            Glow(text.Length);

                        _buffer[count].Start = offset;
                        _buffer[count].Length = length;
                        ++count;
                    }

                    offset = i + 1;
                    length = 0;
                }
                else
                {
                    ++length;
                }
            }

            if (IsOmit(length, isRemoveEmptyEntries) == false)
            {
                if (_buffer.Length == count)
                    Glow(text.Length);

                _buffer[count].Start = offset;
                _buffer[count].Length = length;
                ++count;
            }

            return _buffer.Slice(0, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<StringSpan> Split(string text, ReadOnlySpan<char> separators,
            StringSplitOptions options = StringSplitOptions.None)
        {
            if (text == null)
                return ReadOnlySpan<StringSpan>.Empty;

            return Split(text.AsSpan(), separators, options);
        }

        public ReadOnlySpan<StringSpan> Split(ReadOnlySpan<char> text, ReadOnlySpan<char> separators,
            StringSplitOptions options = StringSplitOptions.None)
        {
            var offset = 0;
            var length = 0;
            var count = 0;

            var isRemoveEmptyEntries = options == StringSplitOptions.RemoveEmptyEntries;

            for (var i = 0; i != text.Length; ++i)
            {
                var c = text[i];

                var isHasSep = false;
                for (var j = 0; j != separators.Length; ++j)
                {
                    if (c == separators[j])
                    {
                        isHasSep = true;
                        break;
                    }
                }

                if (isHasSep)
                {
                    if (IsOmit(length, isRemoveEmptyEntries) == false)
                    {
                        if (_buffer.Length == count)
                            Glow(text.Length);

                        _buffer[count].Start = offset;
                        _buffer[count].Length = length;
                        ++count;
                    }

                    offset = i + 1;
                    length = 0;
                }
                else
                {
                    ++length;
                }
            }

            if (IsOmit(length, isRemoveEmptyEntries) == false)
            {
                if (_buffer.Length == count)
                    Glow(text.Length);

                _buffer[count].Start = offset;
                _buffer[count].Length = length;
                ++count;
            }

            return _buffer.Slice(0, count);
        }

        private void Glow(int minimumLength)
        {
            var length = Math.Max(_buffer.Length * 2, minimumLength);
            var nextBuffer = ArrayPool<StringSpan>.Shared.Rent(length);

            _buffer.CopyTo(nextBuffer);

            var toReturn = _arrayToReturnToPool;

            _arrayToReturnToPool = nextBuffer;
            _buffer = nextBuffer;

            if (toReturn != null)
                ArrayPool<StringSpan>.Shared.Return(toReturn);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsOmit(int length, bool isRemoveEmptyEntries)
            => isRemoveEmptyEntries && length == 0;
    }
}