using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Jewelry.Collections;

namespace Biaui.Internals
{
    internal class TextRenderer
    {
        internal static readonly TextRenderer Default;

        static TextRenderer()
        {
            var fontFamily = Application.Current.FindResource("BiauiFontFamily") as FontFamily;
            var fontSize = (double) TextElement.FontSizeProperty.DefaultMetadata.DefaultValue;

            Default = new TextRenderer(
                fontFamily, fontSize,
                FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        }

        internal TextRenderer(
            FontFamily fontFamily,
            double fontSize,
            FontStyle style,
            FontWeight weight,
            FontStretch stretch)
        {
            if (fontFamily == null)
                return;

            _fontLineSpacing = fontFamily.LineSpacing;

            var typeface = new Typeface(fontFamily, style, weight, stretch);

            if (typeface.TryGetGlyphTypeface(out _glyphTypeface) == false)
            {
                // エラーの場合はデフォルトで作り直す
                typeface =
                    new Typeface(
                        (FontFamily) TextElement.FontFamilyProperty.DefaultMetadata.DefaultValue,
                        (FontStyle) TextElement.FontStyleProperty.DefaultMetadata.DefaultValue,
                        (FontWeight) TextElement.FontWeightProperty.DefaultMetadata.DefaultValue,
                        (FontStretch) TextElement.FontStretchProperty.DefaultMetadata.DefaultValue);

                // デフォルトでもだめなら以降処理しない
                if (typeface.TryGetGlyphTypeface(out _glyphTypeface) == false)
                    return;
            }

            _fontSize = fontSize;

            _toGlyphMap = _glyphTypeface.CharacterToGlyphMap;
            _advanceWidthsDict = _glyphTypeface.AdvanceWidths;

            _glyphTypeface.CharacterToGlyphMap.TryGetValue('.', out _dotGlyphIndex);
            _dotAdvanceWidth = _advanceWidthsDict[_dotGlyphIndex] * _fontSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal double Draw(
            Visual visual,
            string text,
            double x,
            double y,
            Brush brush,
            DrawingContext dc,
            double maxWidth,
            TextAlignment align)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            maxWidth = Math.Ceiling(maxWidth);

            return Draw(
                visual,
                text,
                0,
                text.Length,
                x,
                y,
                brush,
                dc,
                maxWidth,
                align);
        }

        internal double Draw(
            Visual visual,
            string text,
            int textStartIndex,
            int textLength,
            double x,
            double y,
            Brush brush,
            DrawingContext dc,
            double maxWidth,
            TextAlignment align)
        {
            if (NumberHelper.AreCloseZero(_fontSize))
                return 0;

            if (string.IsNullOrEmpty(text))
                return 0;

            if (maxWidth <= 0)
                return 0;

            var gr = MakeGlyphRun(visual, text, textStartIndex, textLength, x, y, maxWidth, align);
            if (gr == default)
                return 0;

            dc.DrawGlyphRun(brush, gr.Item1);

            return gr.Item2;
        }

        internal double CalcWidth(string text)
        {
            if (NumberHelper.AreCloseZero(_fontSize))
                return 0;

            if (string.IsNullOrEmpty(text))
                return 0;

            if (_textWidthCache.TryGetValue(text, out var textWidth))
                return textWidth;

            for (var i = 0; i != text.Length; ++i)
            {
                if (_glyphDataCache.TryGetValue(text[i], out var data) == false)
                {
                    if (_toGlyphMap.TryGetValue(text[i], out data.GlyphIndex) == false)
                        _toGlyphMap.TryGetValue(' ', out data.GlyphIndex);

                    data.AdvanceWidth = _advanceWidthsDict[data.GlyphIndex] * _fontSize;

                    _glyphDataCache.Add(text[i], data);
                }

                textWidth += data.AdvanceWidth;
            }

            _textWidthCache.Add(text, textWidth);

            return textWidth;
        }

        internal double FontHeight =>
            _fontLineSpacing * _fontSize;

        private (GlyphRun, double) MakeGlyphRun(
            Visual visual,
            string text,
            int textStartIndex,
            int textLength,
            double offsetX,
            double offsetY,
            double maxWidth,
            TextAlignment align)
        {
            var dpi = (float)visual.PixelsPerDip();

            int MakeHashCode()
            {
                var hashCode = text.GetHashCode();
                hashCode = (hashCode * 397) ^ textStartIndex.GetHashCode();
                hashCode = (hashCode * 397) ^ textLength.GetHashCode();
                hashCode = (hashCode * 397) ^ offsetX.GetHashCode();
                hashCode = (hashCode * 397) ^ offsetY.GetHashCode();
                hashCode = (hashCode * 397) ^ maxWidth.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)align;
                hashCode = (hashCode * 397) ^ dpi.GetHashCode();
                return hashCode;
            }

            var textKey = MakeHashCode();

            if (_textCache.TryGetValue(textKey, out var gr))
                return gr;

            var glyphIndexes = new ushort[textLength];
            var advanceWidths = new double[textLength];
            var textWidth = 0.0;
            var isRequiredTrimming = false;
            {
                for (var i = 0; i != textLength; ++i)
                {
                    if (_glyphDataCache.TryGetValue(text[textStartIndex + i], out var data) == false)
                    {
                        if (_toGlyphMap.TryGetValue(text[textStartIndex + i], out data.GlyphIndex) == false)
                            _toGlyphMap.TryGetValue(' ', out data.GlyphIndex);

                        data.AdvanceWidth = _advanceWidthsDict[data.GlyphIndex] * _fontSize;

                        _glyphDataCache.Add(text[textStartIndex + i], data);
                    }

                    glyphIndexes[i] = data.GlyphIndex;
                    advanceWidths[i] = data.AdvanceWidth;

                    textWidth += data.AdvanceWidth;

                    if (textWidth > maxWidth)
                    {
                        Array.Resize(ref glyphIndexes, i + 1);
                        Array.Resize(ref advanceWidths, i + 1);
                        isRequiredTrimming = true;
                        break;
                    }
                }

                if (isRequiredTrimming)
                    textWidth = TrimGlyphRun(ref glyphIndexes, ref advanceWidths, textWidth, maxWidth);
            }

            if (NumberHelper.AreCloseZero(textWidth))
                return default;

            var x = offsetX;
            var y = offsetY + _glyphTypeface.Baseline * _fontSize;

            {
                switch (align)
                {
                    case TextAlignment.Left:
                        break;

                    case TextAlignment.Right:
                        x += maxWidth - textWidth;
                        break;

                    case TextAlignment.Center:
                        x += (maxWidth - textWidth) / 2;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(align), align, null);
                }
            }

            gr =
                (new GlyphRun(
                    _glyphTypeface,
                    0,
                    false,
                    _fontSize,
                    dpi,
                    glyphIndexes,
                    new Point(x, y),
                    advanceWidths,
                    null, null, null, null, null, null), textWidth);

            _textCache.Add(textKey, gr);

            return gr;
        }

        private double TrimGlyphRun(
            ref ushort[] glyphIndexes,
            ref double[] advanceWidths,
            double textWidth,
            double maxWidth)
        {
            Debug.Assert(glyphIndexes.Length == advanceWidths.Length);
            Debug.Assert(textWidth > maxWidth);

            // 文字列に ... を加える文を考慮して削る文字数を求める
            var dot3Width = _dotAdvanceWidth * 3.0;

            var removeCount = 1;
            var newTextWidth = textWidth;
            {
                for (var i = glyphIndexes.Length - 1; i >= 0; --i)
                {
                    newTextWidth -= advanceWidths[i];

                    if (maxWidth - newTextWidth >= dot3Width)
                        break;

                    ++removeCount;
                }
            }

            var newCount = glyphIndexes.Length - removeCount + 3;
            if (newCount < 3)
                return 0.0;

            // 文字列に ... を追加する
            Array.Resize(ref glyphIndexes, newCount);
            Array.Resize(ref advanceWidths, newCount);
            glyphIndexes[glyphIndexes.Length - 1 - 2] = _dotGlyphIndex;
            glyphIndexes[glyphIndexes.Length - 1 - 1] = _dotGlyphIndex;
            glyphIndexes[glyphIndexes.Length - 1 - 0] = _dotGlyphIndex;
            advanceWidths[glyphIndexes.Length - 1 - 2] = _dotAdvanceWidth;
            advanceWidths[glyphIndexes.Length - 1 - 1] = _dotAdvanceWidth;
            advanceWidths[glyphIndexes.Length - 1 - 0] = _dotAdvanceWidth;

            return newTextWidth + dot3Width;
        }


        private readonly LruCache<int, (GlyphRun, double)> _textCache = new LruCache<int, (GlyphRun, double)>(10_0000, false);

        // 最大65536エントリ
        private readonly Dictionary<int, (ushort GlyphIndex, double AdvanceWidth)> _glyphDataCache = new Dictionary<int, (ushort GlyphIndex, double AdvanceWidth)>();

        private readonly LruCache<string, double> _textWidthCache = new LruCache<string, double>(10_1000, false);

        private readonly GlyphTypeface _glyphTypeface;
        private readonly ushort _dotGlyphIndex;
        private readonly double _dotAdvanceWidth;
        private readonly double _fontSize;
        private readonly double _fontLineSpacing;

        private readonly IDictionary<int, ushort> _toGlyphMap;
        private readonly IDictionary<ushort, double> _advanceWidthsDict;
    }
}