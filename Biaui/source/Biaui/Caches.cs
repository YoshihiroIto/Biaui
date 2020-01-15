using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui
{
    public static class Caches
    {
        public static Pen GetPen(ByteColor color, double thickness)
        {
            unchecked
            {
                var hashCode = HashCodeMaker.Make(color, thickness);

                if (_borderPens.TryGetValue(hashCode, out var p))
                    return p;

                p = new Pen(GetSolidColorBrush(color), thickness);
                p.Freeze();

                _borderPens.Add(hashCode, p);

                return p;
            }
        }

        public static Pen GetCapPen(ByteColor color, double thickness)
        {
            unchecked
            {
                var hashCode = HashCodeMaker.Make(color, thickness);

                if (_capPens.TryGetValue(hashCode, out var p))
                    return p;

                p = new Pen(GetSolidColorBrush(color), thickness)
                {
                    StartLineCap = PenLineCap.Round,
                    EndLineCap = PenLineCap.Round
                };
                p.Freeze();

                _capPens.Add(hashCode, p);

                return p;
            }
        }

        public static Pen GetDashedPen(ByteColor color, double thickness)
        {
            unchecked
            {
                var hashCode = HashCodeMaker.Make(color, thickness);

                if (_dashedPens.TryGetValue(hashCode, out var p))
                    return p;

                p = new Pen(GetSolidColorBrush(color), thickness)
                {
                    DashStyle = DashStyles.Dash
                };
                p.Freeze();

                _dashedPens.Add(hashCode, p);

                return p;
            }
        }

        public static SolidColorBrush GetSolidColorBrush(ByteColor color)
        {
            unchecked
            {
                var hashCode = HashCodeMaker.Make(color);

                if (_solidColorBrushes.TryGetValue(hashCode, out var b))
                    return b;

                b = new SolidColorBrush(color.ToColor());
                b.Freeze();

                _solidColorBrushes.Add(hashCode, b);

                return b;
            }
        }

        public static Geometry GetClipGeom(Visual visual, double w, double h, double cornerRadius, bool isWidthBorder)
        {
            unchecked
            {
                var hashCode = HashCodeMaker.Make(w, h, cornerRadius, isWidthBorder);

                if (_clipGeoms.TryGetValue(hashCode, out var c))
                    return c;

                if (isWidthBorder)
                {
                    c = new RectangleGeometry
                    {
                        RadiusX = cornerRadius,
                        RadiusY = cornerRadius,
                        Rect = visual.RoundLayoutRect(
                            FrameworkElementExtensions.BorderWidth * 0.5,
                            FrameworkElementExtensions.BorderWidth * 0.5,
                            w - FrameworkElementExtensions.BorderWidth,
                            h - FrameworkElementExtensions.BorderWidth)
                    };
                }
                else
                {
                    c = new RectangleGeometry
                    {
                        RadiusX = cornerRadius,
                        RadiusY = cornerRadius,
                        Rect = visual.RoundLayoutRect(0, 0, w, h)
                    };
                }

                c.Freeze();

                _clipGeoms.Add(hashCode, c);

                return c;
            }
        }

        private static readonly Dictionary<long, RectangleGeometry> _clipGeoms = new Dictionary<long, RectangleGeometry>();

        private static readonly Dictionary<long, Pen> _borderPens = new Dictionary<long, Pen>();
        private static readonly Dictionary<long, Pen> _capPens = new Dictionary<long, Pen>();
        private static readonly Dictionary<long, Pen> _dashedPens = new Dictionary<long, Pen>();
        private static readonly Dictionary<long, SolidColorBrush> _solidColorBrushes = new Dictionary<long, SolidColorBrush>();

        public static TraversalRequest PreviousTraversalRequest
            => _PreviousTraversalRequest ??= new TraversalRequest(FocusNavigationDirection.Previous);

        public static TraversalRequest NextTraversalRequest
            => _NextTraversalRequest ??= new TraversalRequest(FocusNavigationDirection.Next);

        private static TraversalRequest? _PreviousTraversalRequest;
        private static TraversalRequest? _NextTraversalRequest;
    }
}