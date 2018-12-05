using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            if (NumberHelper.AreCloseZero(_fontSize))
                return;

            if (string.IsNullOrEmpty(text))
                return;

            var gr = MakeGlyphRun(text, x, y, maxWidth, align);
            if (gr == null)
                return;

            dc.DrawGlyphRun(brush, gr);
        }

        internal double CalcWidth(string text)
        {
            if (NumberHelper.AreCloseZero(_fontSize))
                return 0;

            if (string.IsNullOrEmpty(text))
                return 0;

            var textWidth = 0.0;

            for (var i = 0; i != text.Length; ++i)
            {
                if (_glyphDataCache.TryGetValue(text[i], out var data) == false)
                {
                    if (_glyphTypeface.CharacterToGlyphMap.TryGetValue(text[i], out data.GlyphIndex) == false)
                        throw new Exception();

                    data.AdvanceWidth = _glyphTypeface.AdvanceWidths[data.GlyphIndex] * _fontSize;

                    _glyphDataCache.Add(text[i], data);
                }

                textWidth += data.AdvanceWidth;
            }

            return textWidth;
        }

        internal double FontHeight =>
            _fontLineSpacing * _fontSize;

        private GlyphRun MakeGlyphRun(
            string text,
            double offsetX,
            double offsetY,
            double maxWidth,
            TextAlignment align)
        {
            var textKey = (text, offsetX, offsetY, maxWidth, align);

            if (_textCache.TryGetValue(textKey, out var gr))
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

            if (NumberHelper.AreCloseZero(textWidth))
                return null;

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
                new GlyphRun(
                    _glyphTypeface,
                    0,
                    false,
                    _fontSize,
                    (float) WpfHelper.PixelsPerDip,
                    glyphIndexes,
                    new Point(x, y),
                    advanceWidths,
                    null, null, null, null, null, null);

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
            var dot3width = _dotAdvanceWidth * 3.0;

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

            // 文字列に ... を追加する
            Array.Resize(ref glyphIndexes, newCount);
            Array.Resize(ref advanceWidths, newCount);
            glyphIndexes[glyphIndexes.Length - 1 - 2] = _dotGlyphIndex;
            glyphIndexes[glyphIndexes.Length - 1 - 1] = _dotGlyphIndex;
            glyphIndexes[glyphIndexes.Length - 1 - 0] = _dotGlyphIndex;
            advanceWidths[glyphIndexes.Length - 1 - 2] = _dotAdvanceWidth;
            advanceWidths[glyphIndexes.Length - 1 - 1] = _dotAdvanceWidth;
            advanceWidths[glyphIndexes.Length - 1 - 0] = _dotAdvanceWidth;

            return newTextWidth + dot3width;
        }

        private readonly LruCache<(string, double, double, double, TextAlignment), GlyphRun> _textCache =
            new LruCache<(string, double, double, double, TextAlignment), GlyphRun>(10_0000, false);

        // 最大65536エントリ
        private readonly Dictionary<int, (ushort GlyphIndex, double AdvanceWidth)> _glyphDataCache =
            new Dictionary<int, (ushort GlyphIndex, double AdvanceWidth)>();

        private readonly GlyphTypeface _glyphTypeface;
        private readonly ushort _dotGlyphIndex;
        private readonly double _dotAdvanceWidth;
        private readonly double _fontSize;
        private readonly double _fontLineSpacing;
    }
}