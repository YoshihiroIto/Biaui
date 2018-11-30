using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

// ReSharper disable CompareOfFloatsByEqualityOperator

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

            _glyphTypeface.CharacterToGlyphMap.TryGetValue('.', out _dotGlyphIndex);
            _dotAdvanceWidth = _glyphTypeface.AdvanceWidths[_dotGlyphIndex] * _fontSize;
        }

        internal void Draw(
            string text,
            double x,
            double y,
            Brush brush,
            DrawingContext dc,
            double maxWidth,
            TextAlignment align)
        {
            if (_fontSize == default(double))
                return;

            if (string.IsNullOrEmpty(text))
                return;

            var gr = MakeGlyphRun(text, maxWidth, align);
            if (gr == null)
                return;

            var key = (x, y);
            if (_transCache.TryGetValue(key, out var t) == false)
            {
                t = new TranslateTransform(x, y);
                _transCache.Add(key, t);
            }

            dc.PushTransform(t);
            dc.DrawGlyphRun(brush, gr);
            dc.Pop();
        }

        private readonly Dictionary<(double X, double Y), TranslateTransform> _transCache =
            new Dictionary<(double X, double Y), TranslateTransform>();

        internal double CalcWidth(string text)
        {
            if (_fontSize == default(double))
                return 0;

            if (string.IsNullOrEmpty(text))
                return 0;

            var gr = MakeGlyphRun(text, double.PositiveInfinity, TextAlignment.Left);
            if (gr == null)
                return 0;

            return gr.AdvanceWidths.Sum();
        }

        private GlyphRun MakeGlyphRun(
            string text,
            double maxWidth,
            TextAlignment align)
        {
            maxWidth = Math.Truncate(maxWidth);

            var useFullText = false;
            if (align == TextAlignment.Left)
            {
                if (_fullTextWidthCache.TryGetValue(text, out var w))
                    useFullText = w < maxWidth;
            }

            var trimmedTextKey = (text, useFullText ? FullTextWidthHashCode : maxWidth, align).GetHashCode();
            if (_trimmedTextCache.TryGetValue(trimmedTextKey, out var gr))
                return gr;

            var glyphIndexes = new ushort[text.Length];
            var advanceWidths = new double[text.Length];
            var textWidth = 0.0;
            var isRequiredTrimming = false;
            {
                for (var i = 0; i != text.Length; ++i)
                {
                    if (_glyphDataCache.TryGetValue(text[i], out var data) == false)
                    {
                        if (_glyphTypeface.CharacterToGlyphMap.TryGetValue(text[i], out data.GlyphIndex) == false)
                            throw new Exception();

                        data.AdvanceWidth = _glyphTypeface.AdvanceWidths[data.GlyphIndex] * _fontSize;

                        _glyphDataCache.Add(text[i], data);
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

            if (textWidth == 0.0)
                return null;

            double x;
            {
                switch (align)
                {
                    case TextAlignment.Left:
                        x = 0.0f;
                        break;

                    case TextAlignment.Right:
                        x = maxWidth - textWidth;
                        break;

                    case TextAlignment.Center:
                        x = (maxWidth - textWidth) / 2;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(align), align, null);
                }
            }

            var y = _glyphTypeface.Baseline * _fontSize;

            gr =
                new GlyphRun(
                    _glyphTypeface,
                    0,
                    false,
                    _fontSize,
                    (float) WpfHelper.PixelsPerDip,
                    glyphIndexes,
                    new Point(x, y),
                    advanceWidths,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null);

            if (align == TextAlignment.Left)
            {
                if (isRequiredTrimming == false)
                {
                    _fullTextWidthCache.Add(text, textWidth);
                    trimmedTextKey = (text, FullTextWidthHashCode, align).GetHashCode();
                }
            }

            _trimmedTextCache.Add(trimmedTextKey, gr);

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

#if false
            var dot3width = _dotAdvanceWidth * 3.0;
#else
            var dot3width = 0.0;
#endif

            var removeCount = 1;
            var newTextWidth = textWidth;
            {
                for (var i = glyphIndexes.Length - 1; i >= 0; --i)
                {
                    newTextWidth -= advanceWidths[i];

                    if (maxWidth - newTextWidth >= dot3width)
                        break;

                    ++removeCount;
                }
            }

            var newCount = glyphIndexes.Length - removeCount + 3;
            if (newCount < 3)
                return 0.0;

#if false
// 文字列に ... を追加する
            Array.Resize(ref glyphIndexes, newCount);
            Array.Resize(ref advanceWidths, newCount);
            glyphIndexes[glyphIndexes.Length - 1 - 2] = _dotGlyphIndex;
            glyphIndexes[glyphIndexes.Length - 1 - 1] = _dotGlyphIndex;
            glyphIndexes[glyphIndexes.Length - 1 - 0] = _dotGlyphIndex;
            advanceWidths[glyphIndexes.Length - 1 - 2] = _dotAdvanceWidth;
            advanceWidths[glyphIndexes.Length - 1 - 1] = _dotAdvanceWidth;
            advanceWidths[glyphIndexes.Length - 1 - 0] = _dotAdvanceWidth;
#endif

            return newTextWidth + dot3width;
        }

        private readonly Dictionary<string, double> _fullTextWidthCache = new Dictionary<string, double>();
        private readonly Dictionary<int, GlyphRun> _trimmedTextCache = new Dictionary<int, GlyphRun>();

        private readonly Dictionary<int, (ushort GlyphIndex, double AdvanceWidth)> _glyphDataCache =
            new Dictionary<int, (ushort GlyphIndex, double AdvanceWidth)>();

        private readonly GlyphTypeface _glyphTypeface;
        private readonly ushort _dotGlyphIndex;
        private readonly double _dotAdvanceWidth;
        private readonly double _fontSize;

        private const double FullTextWidthHashCode = double.MaxValue;
    }
}