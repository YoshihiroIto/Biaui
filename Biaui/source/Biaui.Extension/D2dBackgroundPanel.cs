using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Biaui.Controls.Internals;
using Biaui.Controls.NodeEditor;
using Biaui.Environment;
using Biaui.Interfaces;
using Biaui.Internals;
using Jewelry.Collections;
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

        private const float BaseLineWidth = 1.0f;
        private const float ArrowSize = 40.0f;

        public D2dBackgroundPanel(BiaNodeEditor parent)
        {
            IsAutoFrameUpdate = false;

            _parent = parent;
        }

        public override void Render(DeviceContext target)
        {
            target.Clear(default(RawColor4));

            if (_parent.LinksSource == null)
                return;

            var s = (float) _parent.Scale;
            var tx = (float) _parent.TranslateTransform.X;
            var ty = (float) _parent.TranslateTransform.Y;
            target.Transform = new RawMatrix3x2(s, 0, 0, s, tx, ty);

            var isDrawArrow = _parent.Scale > 0.125;
            var lineWidth = BaseLineWidth / s;

            DrawCurves(target, isDrawArrow, lineWidth);
        }

        private readonly Dictionary<(ImmutableByteColor color, bool isHighlight), (PathGeometry curveGeom, GeometrySink curveSink, PathGeometry? arrowGeom, GeometrySink? arrowSink)>
            _sinks = new Dictionary<(ImmutableByteColor color, bool isHighlight), (PathGeometry curveGeom, GeometrySink curveSink, PathGeometry? arrowGeom, GeometrySink? arrowSink)>();

        private void DrawCurves(DeviceContext target, bool isDrawArrow, float lineWidth)
        {
            if (_parent.LinksSource == null)
                return;

            var inflate = ArrowSize * (float) _parent.ScaleTransform.ScaleX;
            var viewport = _parent.TransformRect(ActualWidth, ActualHeight);
            var lineCullingRect = new ImmutableRect_float(
                (float) viewport.X - inflate,
                (float) viewport.Y - inflate,
                (float) viewport.Width + inflate * 2.0f,
                (float) viewport.Height + inflate * 2.0f
            );

            Span<ImmutableVec2_float> bezier = stackalloc ImmutableVec2_float[4];

            var hasHighlightCurves = false;

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

                GeometrySink curveSink;
                GeometrySink? arrowSink;
                {
                    var key = (link.Color, isHighlight);

                    if (_sinks.TryGetValue(key, out var p))
                    {
                        curveSink = p.curveSink;
                        arrowSink = p.arrowSink;
                    }
                    else
                    {
                        var curveGeom = new PathGeometry(target.Factory);
                        curveSink = curveGeom.Open();
                        curveSink.SetFillMode(FillMode.Winding);

                        var arrowGeom = isDrawArrow ? new PathGeometry(target.Factory) : null;
                        arrowSink = arrowGeom?.Open();
                        arrowSink?.SetFillMode(FillMode.Winding);

                        _sinks[key] = (curveGeom, curveSink, arrowGeom, arrowSink);
                    }
                }

                link.MakeBezierCurve(bezier);
                var keyBezier = MakeHashCode(bezier);
                if (_boundingBoxCache.TryGetValue(keyBezier, out var bb) == false)
                {
                    bb = BiaNodeEditorHelper.MakeBoundingBox(
                        bezier[0],
                        bezier[1],
                        bezier[2],
                        bezier[3]);

                    _boundingBoxCache.Add(keyBezier, bb);
                }

                if (bb.IntersectsWith(lineCullingRect) == false)
                    continue;

                // 接続線
                {
                    curveSink.BeginFigure(Unsafe.As<ImmutableVec2_float, RawVector2>(ref bezier[0]), FigureBegin.Hollow);
                    curveSink.AddBezier(Unsafe.As<ImmutableVec2_float, BezierSegment>(ref bezier[1]));
                    curveSink.EndFigure(FigureEnd.Open);
                }

                // 矢印
                if (arrowSink != null)
                    DrawArrow(arrowSink, bezier);
            }

            foreach (var sink in _sinks)
            {
                // ブラシ取得
                var resKey = sink.Key.GetHashCode();
                if (ResourceCache.TryGetValue(resKey, out var brush) == false)
                {
                    ResourceCache.Add(resKey, t => ColorToBrushConv(t, sink.Key.color));
                    brush = ResourceCache[resKey];
                }

                // ハイライトがあれば、非ハイライトを表示しない
                if (hasHighlightCurves && sink.Key.isHighlight == false)
                    continue;

                // 接続線カーブ
                {
                    sink.Value.curveSink.Close();
                    target.DrawGeometry(sink.Value.curveGeom, brush as Brush, sink.Key.isHighlight ? lineWidth * 2.0f : lineWidth);
                    sink.Value.curveSink.Dispose();
                    sink.Value.curveGeom.Dispose();
                }

                // 矢印
                if (sink.Value.arrowSink != null)
                {
                    sink.Value.arrowSink.Close();
                    target.FillGeometry(sink.Value.arrowGeom, brush as Brush);
                    sink.Value.arrowSink.Dispose();
                    sink.Value.arrowGeom?.Dispose();
                }
            }

            _sinks.Clear();
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
            Span<ImmutableVec2_float> bezier)
        {
            var b1X = BiaNodeEditorHelper.Bezier(bezier[0].X, bezier[1].X, bezier[2].X, bezier[3].X, 0.5001f);
            var b1Y = BiaNodeEditorHelper.Bezier(bezier[0].Y, bezier[1].Y, bezier[2].Y, bezier[3].Y, 0.5001f);
            var b2X = BiaNodeEditorHelper.Bezier(bezier[0].X, bezier[1].X, bezier[2].X, bezier[3].X, 0.5f);
            var b2Y = BiaNodeEditorHelper.Bezier(bezier[0].Y, bezier[1].Y, bezier[2].Y, bezier[3].Y, 0.5f);

            var sx = b1X - b2X;
            var sy = b1Y - b2Y;
            var r = Math.Atan2(sy, sx) + Math.PI * 0.5;
            var m = ((float) Math.Sin(r), (float) Math.Cos(r));

            var l1 = new ImmutableVec2_float(ArrowSize / 1.732f, ArrowSize / 1.732f * 2.0f);
            var l2 = new ImmutableVec2_float(-ArrowSize / 1.732f, ArrowSize / 1.732f * 2.0f);

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
        private static SolidColorBrush ColorToBrushConv(RenderTarget t, ImmutableByteColor src)
            => new SolidColorBrush(
                t,
                new RawColor4(src.R * (1.0f / 255.0f), src.G * (1.0f / 255.0f), src.B * (1.0f / 255.0f), 1.0f));

        private static readonly LruCache<long, ImmutableRect_float> _boundingBoxCache =
            new LruCache<long, ImmutableRect_float>(10000, false);
    }
}