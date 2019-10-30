using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Biaui.Controls.Internals;
using Biaui.Controls.NodeEditor;
using Biaui.Controls.NodeEditor.Internal;
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
            target.Clear(new RawColor4());

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

        private readonly Dictionary<(Color color, bool isHighlight), (PathGeometry curveGeom, GeometrySink curveSink, PathGeometry arrowGeom, GeometrySink arrowSink)>
            _sinks = new Dictionary<(Color color, bool isHighlight), (PathGeometry curveGeom, GeometrySink curveSink, PathGeometry arrowGeom, GeometrySink arrowSink)>();

        private void DrawCurves(DeviceContext target, bool isDrawArrow, float lineWidth)
        {
            var inflate = ArrowSize * _parent.ScaleTransform.ScaleX;
            var viewport = _parent.TransformRect(ActualWidth, ActualHeight);
            var lineCullingRect = new ImmutableRect(
                viewport.X - inflate,
                viewport.Y - inflate,
                viewport.Width + inflate * 2,
                viewport.Height + inflate * 2
            );

            var bezierPos0 = new RawVector2();
            var bezierSegment = new BezierSegment();

            var hasHighlightCurves = false;

            foreach (IBiaNodeLink link in _parent.LinksSource)
            {
                if (link.IsVisible == false)
                    continue;

                if (link.IsLinked() == false)
                    continue;

                var isHighlight = link.IsHighlight();

                if (isHighlight)
                    hasHighlightCurves = true;

                GeometrySink curveSink;
                GeometrySink arrowSink;
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

                var bezier = link.MakeBezierCurve();

                var bb = _boundingBoxCache.GetOrAdd(
                    MakeHashCode(bezier),
                    x => BiaNodeEditorHelper.MakeBoundingBox(
                        Unsafe.As<Point, ImmutableVec2>(ref bezier.Item1),
                        Unsafe.As<Point, ImmutableVec2>(ref bezier.Item2),
                        Unsafe.As<Point, ImmutableVec2>(ref bezier.Item3),
                        Unsafe.As<Point, ImmutableVec2>(ref bezier.Item4)));

                if (bb.IntersectsWith(lineCullingRect) == false)
                    continue;

                // 接続線
                {
                    bezierPos0.X = (float) bezier.Item1.X;
                    bezierPos0.Y = (float) bezier.Item1.Y;

                    curveSink.BeginFigure(bezierPos0, FigureBegin.Hollow);
                    {
                        bezierSegment.Point1.X = (float) bezier.Item2.X;
                        bezierSegment.Point1.Y = (float) bezier.Item2.Y;
                        bezierSegment.Point2.X = (float) bezier.Item3.X;
                        bezierSegment.Point2.Y = (float) bezier.Item3.Y;
                        bezierSegment.Point3.X = (float) bezier.Item4.X;
                        bezierSegment.Point3.Y = (float) bezier.Item4.Y;

                        curveSink.AddBezier(bezierSegment);
                    }
                    curveSink.EndFigure(FigureEnd.Open);
                }

                // 矢印
                if (isDrawArrow)
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
                    target.DrawGeometry(sink.Value.curveGeom, (Brush) brush, sink.Key.isHighlight ? lineWidth * 2.0f : lineWidth);
                    sink.Value.curveSink.Dispose();
                    sink.Value.curveGeom.Dispose();
                }

                // 矢印
                if (sink.Value.arrowSink != null)
                {
                    sink.Value.arrowSink.Close();
                    target.FillGeometry(sink.Value.arrowGeom, (Brush) brush);
                    sink.Value.arrowSink.Dispose();
                    sink.Value.arrowGeom.Dispose();
                }
            }

            _sinks.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe int MakeHashCode(in ValueTuple<Point, Point, Point, Point> src)
        {
            unchecked
            {
                // ReSharper disable InconsistentNaming
                var p1x = src.Item1.X;
                var p1y = src.Item1.Y;
                var p2x = src.Item2.X;
                var p2y = src.Item2.Y;
                var p3x = src.Item3.X;
                var p3y = src.Item3.Y;
                var p4x = src.Item4.X;
                var p4y = src.Item4.Y;
                // ReSharper restore InconsistentNaming

                var hashCode = *(long*) &p1x;

                hashCode = (hashCode * 397) ^ *(long*)&p1y;
                hashCode = (hashCode * 397) ^ *(long*)&p2x;
                hashCode = (hashCode * 397) ^ *(long*)&p2y;
                hashCode = (hashCode * 397) ^ *(long*)&p3x;
                hashCode = (hashCode * 397) ^ *(long*)&p3y;
                hashCode = (hashCode * 397) ^ *(long*)&p4x;
                hashCode = (hashCode * 397) ^ *(long*)&p4y;

                return (int)(hashCode * 397) ^ (int) (hashCode >> 32);
            }
        }

        private static readonly LruCache<int, ImmutableRect> _boundingBoxCache =
            new LruCache<int, ImmutableRect>(10000, false);

        private static void DrawArrow(
            GeometrySink sink,
            in (Point p1, Point c1, Point c2, Point p2) bezier)
        {
            var b1X = BiaNodeEditorHelper.Bezier(bezier.p1.X, bezier.c1.X, bezier.c2.X, bezier.p2.X, 0.5001);
            var b1Y = BiaNodeEditorHelper.Bezier(bezier.p1.Y, bezier.c1.Y, bezier.c2.Y, bezier.p2.Y, 0.5001);
            var b2X = BiaNodeEditorHelper.Bezier(bezier.p1.X, bezier.c1.X, bezier.c2.X, bezier.p2.X, 0.5);
            var b2Y = BiaNodeEditorHelper.Bezier(bezier.p1.Y, bezier.c1.Y, bezier.c2.Y, bezier.p2.Y, 0.5);

            var sx = b1X - b2X;
            var sy = b1Y - b2Y;
            var r = Math.Atan2(sy, sx) + Math.PI * 0.5;
            var m = (Math.Sin(r), Math.Cos(r));

            var l1 = new ImmutableVec2(ArrowSize / 1.732, ArrowSize / 1.732 * 2);
            var l2 = new ImmutableVec2(-ArrowSize / 1.732, ArrowSize / 1.732 * 2);

            var t1X = (float) (bezier.p1.X + bezier.p2.X) * 0.5f;
            var t1Y = (float) (bezier.p1.Y + bezier.p2.Y) * 0.5f;

            var t2 = Rotate(m, l1);
            var t3 = Rotate(m, l2);

            sink.BeginFigure(new RawVector2(t1X, t1Y), FigureBegin.Filled);
            sink.AddLine(new RawVector2((float) t2.X + t1X, (float) t2.Y + t1Y));
            sink.AddLine(new RawVector2((float) t3.X + t1X, (float) t3.Y + t1Y));
            sink.EndFigure(FigureEnd.Closed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ImmutableVec2 Rotate(in ValueTuple<double, double> m, in ImmutableVec2 pos)
            => new ImmutableVec2(
                pos.X * m.Item2 - pos.Y * m.Item1,
                pos.X * m.Item1 + pos.Y * m.Item2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SolidColorBrush ColorToBrushConv(RenderTarget t, Color src)
            => new SolidColorBrush(
                t,
                new RawColor4(src.R / 255.0f, src.G / 255.0f, src.B / 255.0f, 1.0f));
    }
}