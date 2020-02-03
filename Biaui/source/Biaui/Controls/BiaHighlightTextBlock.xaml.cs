using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Biaui.Internals;
using Jewelry.Text;

namespace Biaui.Controls
{
    public class BiaHighlightTextBlock : BiaTextBlock
    {
        #region Highlight

        public Brush? Highlight
        {
            get => _Highlight;
            set
            {
                if (value != _Highlight)
                    SetValue(HighlightProperty, value);
            }
        }

        private Brush? _Highlight;

        public static readonly DependencyProperty HighlightProperty =
            DependencyProperty.Register(
                nameof(Highlight),
                typeof(Brush),
                typeof(BiaHighlightTextBlock),
                new FrameworkPropertyMetadata(
                    default,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaHighlightTextBlock) s;
                        self._Highlight = (Brush) e.NewValue;
                    }));

        #endregion

        #region Words

        public string? Words
        {
            get => _Words;
            set
            {
                if (value != _Words)
                    SetValue(WordsProperty, value);
            }
        }

        private string? _Words;

        public static readonly DependencyProperty WordsProperty =
            DependencyProperty.Register(
                nameof(Words),
                typeof(string),
                typeof(BiaHighlightTextBlock),
                new FrameworkPropertyMetadata(
                    default,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaHighlightTextBlock) s;
                        self._Words = (string) e.NewValue;
                    }));

        #endregion

        static BiaHighlightTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaHighlightTextBlock),
                new FrameworkPropertyMetadata(typeof(BiaHighlightTextBlock)));
        }

        [SuppressMessage("ReSharper", "PossiblyImpureMethodCallOnReadonlyVariable")]
        protected override void OnRender(DrawingContext dc)
        {
            using var ss = new StringSplitter(stackalloc StringSplitter.StringSpan[32]);

            var wordsArray = ss.Split(Words, ' ', StringSplitOptions.RemoveEmptyEntries);

            if (wordsArray.Length == 0)
                base.OnRender(dc);
            else
            {
                if (Words != null)
                    RenderHighlight(dc, wordsArray, Words);
            }
        }

        private void RenderHighlight(DrawingContext dc, ReadOnlySpan<StringSplitter.StringSpan> wordsSpans, string words)
        {
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;

            if (string.IsNullOrEmpty(Text))
                return;

            var textStates = ArrayPool<byte>.Shared.Rent(Text.Length);

            try
            {
                Array.Clear(textStates, 0, Text.Length);

                ReadOnlySpan<char> textSpan = Text;

                foreach (var wordsSpan in wordsSpans)
                {
                    var stateOffset = 0;

                    var wordSpan = wordsSpan.ToSpan(words);

                    while (true)
                    {
                        var wordIndex = textSpan.Slice(stateOffset).IndexOf(wordSpan, StringComparison.OrdinalIgnoreCase);
                        if (wordIndex == -1)
                            break;

                        for (var i = 0; i != wordSpan.Length; ++i)
                            textStates[stateOffset + wordIndex + i] = 1;

                        stateOffset += wordIndex + wordSpan.Length;
                    }
                }

                var state = textStates[0];
                var index = 0;
                var startIndex = 0;

                var x = 0.0;

                while (true)
                {
                    if (textStates[index] != state)
                    {
                        var brush =
                            textStates[startIndex] == 0
                                ? Foreground
                                : Highlight;

                        if (brush != null)
                            x = RenderText(dc, Text, startIndex, index - 1, x, brush);

                        state = textStates[index];
                        startIndex = index;
                    }

                    ++index;

                    if (index == Text.Length)
                    {
                        var brush =
                            textStates[startIndex] == 0
                                ? Foreground
                                : Highlight;

                        if (brush != null)
                            RenderText(dc, Text, startIndex, index - 1, x, brush);

                        break;
                    }
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(textStates);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double RenderText(DrawingContext dc, string text, int startIndex, int endIndex, double x, Brush brush)
        {
            var length = endIndex - startIndex + 1;

            var w = DefaultTextRenderer.Instance.Draw(
                this,
                text.AsSpan(startIndex, length),
                x, 0,
                brush,
                dc,
                ActualWidth - x,
                TextAlignment.Left,
                TextTrimming);

            return x + w;
        }
    }
}