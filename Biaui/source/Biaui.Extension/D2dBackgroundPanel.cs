using System;
using System.Runtime.CompilerServices;
using Biaui.Controls.Internals;
using Biaui.Controls.NodeEditor;
using Biaui.Environment;
using Biaui.Interfaces;
using Biaui.Internals;
using Jewelry.Collections;
using Jewelry.Memory;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using BezierSegment = SharpDX.Direct2D1.BezierSegment;
using Brush = SharpDX.Direct2D1.Brush;
using SolidColorBrush = SharpDX.Direct2D1.SolidColorBrush;
using PathGeometry = SharpDX.Direct2D1.PathGeometry;

namespace Biaui.Extension
{
    public class D2dBackgroundPanel : D2dControl.D2dControl, IBackgroundPanel
    {
        private readonly BiaNodeEditor _parent;

        private const float BaseLineWidth = 1.5f;
        private const float BaseArrowSize = 8.0f;

        public D2dBackgroundPanel(BiaNodeEditor parent)
        {
            IsAutoFrameUpdate = false;

            _parent = parent;
        }

        protected override void Render(DeviceContext target)
        {
            target.Clear(default(RawColor4));

            if (_parent.LinksSource == null)
                return;

            var scale = (float) _parent.Scale;
            var tx = (float) _parent.TranslateTransform.X;
            var ty = (float) _parent.TranslateTransform.Y;
            target.Transform = new RawMatrix3x2(scale, 0f, 0f, scale, tx, ty);

            var isDrawArrow = scale > 0.085f; 
            var lineWidth = BaseLineWidth / scale;
            DrawCurves(target, isDrawArrow, lineWidth);
        }

        private void DrawCurves(DeviceContext target, bool isDrawArrow, float lineWidth)
        {
            if (_parent.LinksSource == null)
                return;

            var arrowSize = BaseArrowSize / (float) _parent.ScaleTransform.ScaleX;

            var inflate = arrowSize;
            var viewport = _parent.TransformRect(ActualWidth, ActualHeight);
            var lineCullingRect = new ImmutableRect_float(
                (float) viewport.X - inflate,
                (float) viewport.Y - inflate,
                (float) viewport.Width + inflate * 2f,
                (float) viewport.Height + inflate * 2f
            );

            var hasHighlightCurves = false;

            var borderKey = HashCodeMaker.To32(ByteColor.Black.HashCode);
            if (ResourceCache.TryGetValue(borderKey, out var borderBrushObj) == false)
                borderBrushObj = ResourceCache.Add(borderKey, t => ColorToBrushConv(t, ByteColor.Black));
            var borderBrush = borderBrushObj as Brush;

            Span<ImmutableVec2_float> bezier = stackalloc ImmutableVec2_float[4];
            using var curves = new TempBuffer<(PathGeometry Geom, GeometrySink Sink, IBiaNodeLink Link)>(256);

            foreach (IBiaNodeLink? link in _parent.LinksSource)
            {
                if (link == null)
                    continue;

                if (link.IsVisible == false)
                    continue;

                if (link.IsLinked() == false)
                    continue;

                var isHighlight = link.IsHighlight();

                if (isHighlight)
                    hasHighlightCurves = true;

                // ハイライトがあれば、非ハイライトを表示しない
                if (hasHighlightCurves && isHighlight == false)
                    continue;

                link.MakeBezierCurve(bezier);
                var keyBezier = MakeHashCode(bezier);
                if (_boundingBoxCache.TryGetValue(keyBezier, out var bb) == false)
                {
                    bb = BiaNodeEditorHelper.MakeBoundingBox(bezier);
                    _boundingBoxCache.Add(keyBezier, bb);
                }

                if (bb.IntersectsWith(lineCullingRect) == false)
                    continue;

                var curveGeom = new PathGeometry(target.Factory);
                var curveSink = curveGeom.Open();
                curveSink.SetFillMode(FillMode.Winding);

                curveSink.BeginFigure(Unsafe.As<ImmutableVec2_float, RawVector2>(ref bezier[0]), FigureBegin.Hollow);
                curveSink.AddBezier(Unsafe.As<ImmutableVec2_float, BezierSegment>(ref bezier[1]));
                curveSink.EndFigure(FigureEnd.Open);

                if (isDrawArrow)
                    DrawArrow(curveSink, bezier, arrowSize);

                curveSink.Close();

                // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
                curves.Add((curveGeom, curveSink, link));
            }

            foreach (var (geom, sink, link) in curves.Buffer)
            {
                var isHighlight = link.IsHighlight();

                if (hasHighlightCurves == false || isHighlight)
                {
                    var key = HashCodeMaker.Make(link.Color, isHighlight);
                    var resKey = HashCodeMaker.To32(key);
                    if (ResourceCache.TryGetValue(resKey, out var brush) == false)
                        brush = ResourceCache.Add(resKey, t => ColorToBrushConv(t, link.Color));

                    target.DrawGeometry(geom, borderBrush, lineWidth * 2f);
                    target.DrawGeometry(geom, brush as Brush, lineWidth);
                    target.FillGeometry(geom, brush as Brush);
                }

                sink.Dispose();
                geom.Dispose();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long MakeHashCode(Span<ImmutableVec2_float> src)
        {
            unchecked
            {
                var p0 = Unsafe.As<ImmutableVec2_float, long>(ref src[0]);
                var p1 = Unsafe.As<ImmutableVec2_float, long>(ref src[1]);
                var p2 = Unsafe.As<ImmutableVec2_float, long>(ref src[2]);
                var p3 = Unsafe.As<ImmutableVec2_float, long>(ref src[3]);

                var hashCode = p0;

                hashCode = (hashCode * 397) ^ p1;
                hashCode = (hashCode * 397) ^ p2;
                hashCode = (hashCode * 397) ^ p3;

                return hashCode;
            }
        }

        private static void DrawArrow(
            GeometrySink sink,
            ReadOnlySpan<ImmutableVec2_float> bezier,
            float arrowSize)
        {
            var b1X = BiaNodeEditorHelper.Bezier(bezier[0].X, bezier[1].X, bezier[2].X, bezier[3].X, 0.5001f);
            var b1Y = BiaNodeEditorHelper.Bezier(bezier[0].Y, bezier[1].Y, bezier[2].Y, bezier[3].Y, 0.5001f);
            var b2X = BiaNodeEditorHelper.Bezier(bezier[0].X, bezier[1].X, bezier[2].X, bezier[3].X, 0.5f);
            var b2Y = BiaNodeEditorHelper.Bezier(bezier[0].Y, bezier[1].Y, bezier[2].Y, bezier[3].Y, 0.5f);

            var sx = b1X - b2X;
            var sy = b1Y - b2Y;
            var r = MathF.Atan2(sy, sx) + MathF.PI * 0.5f;
            var m = (MathF.Sin(r), MathF.Cos(r));

            var l1 = new ImmutableVec2_float(arrowSize / 1.732f, arrowSize / 1.732f * 2.0f);
            var l2 = new ImmutableVec2_float(-arrowSize / 1.732f, arrowSize / 1.732f * 2.0f);

            var t1X = (bezier[0].X + bezier[3].X) * 0.5f;
            var t1Y = (bezier[0].Y + bezier[3].Y) * 0.5f;

            var t2 = Rotate(m, l1);
            var t3 = Rotate(m, l2);

            sink.BeginFigure(new RawVector2(t1X, t1Y), FigureBegin.Filled);
            sink.AddLine(new RawVector2(t2.X + t1X, t2.Y + t1Y));
            sink.AddLine(new RawVector2(t3.X + t1X, t3.Y + t1Y));
            sink.EndFigure(FigureEnd.Closed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ImmutableVec2_float Rotate(in ValueTuple<float, float> m, in ImmutableVec2_float pos)
            => new ImmutableVec2_float(
                pos.X * m.Item2 - pos.Y * m.Item1,
                pos.X * m.Item1 + pos.Y * m.Item2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static SolidColorBrush ColorToBrushConv(RenderTarget t, ByteColor src)
            => new SolidColorBrush(
                t,
                new RawColor4(src.R * (1.0f / 255.0f), src.G * (1.0f / 255.0f), src.B * (1.0f / 255.0f), 1.0f));

        private static readonly LruCache<long, ImmutableRect_float> _boundingBoxCache =
            new LruCache<long, ImmutableRect_float>(10000);
    }
}