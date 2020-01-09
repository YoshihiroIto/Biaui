using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;

namespace Biaui.Internals
{
    internal static class Caches
    {
        internal static Pen GetPen(Color color, double thickness)
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

        internal static Pen GetCapPen(Color color, double thickness)
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

        internal static Pen GetDashedPen(Color color, double thickness)
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

        internal static SolidColorBrush GetSolidColorBrush(Color color)
        {
            unchecked
            {
                var hashCode = (int)color.R;
                hashCode = (hashCode * 397) ^ color.G;
                hashCode = (hashCode * 397) ^ color.B;
                hashCode = (hashCode * 397) ^ color.A;

                if (_solidColorBrushes.TryGetValue(hashCode, out var b))
                    return b;

                b = new SolidColorBrush(color);
                b.Freeze();

                _solidColorBrushes.Add(hashCode, b);

                return b;
            }
        }

        internal static Geometry GetClipGeom(Visual visual, double w, double h, double cornerRadius, bool isWidthBorder)
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

        private static readonly
            Dictionary<int, RectangleGeometry> _clipGeoms = new Dictionary<int, RectangleGeometry>();

        private static readonly Dictionary<int, Pen> _borderPens = new Dictionary<int, Pen>();
        private static readonly Dictionary<int, Pen> _capPens = new Dictionary<int, Pen>();
        private static readonly Dictionary<int, Pen> _dashedPens = new Dictionary<int, Pen>();
        private static readonly Dictionary<int, SolidColorBrush> _solidColorBrushes = new Dictionary<int, SolidColorBrush>();

        internal static TraversalRequest PreviousTraversalRequest
            => _PreviousTraversalRequest ??= new TraversalRequest(FocusNavigationDirection.Previous);

        internal static TraversalRequest NextTraversalRequest
            => _NextTraversalRequest ??= new TraversalRequest(FocusNavigationDirection.Next);

        private static TraversalRequest? _PreviousTraversalRequest;
        private static TraversalRequest? _NextTraversalRequest;
    }
}