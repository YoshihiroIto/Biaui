using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Biaui.Controls.NodeEditor;
using Biaui.Controls.NodeEditor.Internal;
using Biaui.Environment;
using Biaui.Interfaces;
using Biaui.Internals;
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
        private const float ArrowSize = 20.0f;

        public D2dBackgroundPanel(BiaNodeEditor parent)
        {
            IsAutoFrameUpdate = false;

            _parent = parent;

            ResCache.Add("LinkBrush", t => Conv(t, Colors.DimGray));
            ResCache.Add("HighlightLinkBrush", t => Conv(t, _parent.HighlightLinkColor));

            SolidColorBrush Conv(RenderTarget t, Color src)
                => new SolidColorBrush(
                    t,
                    new RawColor4(src.R / 255.0f, src.G / 255.0f, src.B / 255.0f, src.A / 255.0f));
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

            var isDrawArrow = _parent.Scale > 0.2;
            var lineWidth = BaseLineWidth / s;

            DrawCurves(ResCache["LinkBrush"] as Brush, false, target, isDrawArrow, lineWidth * 0.7f);
            DrawCurves(ResCache["HighlightLinkBrush"] as Brush, true, target, isDrawArrow, lineWidth);
        }

        private void DrawCurves(Brush brush, bool isHighlight, DeviceContext target, bool isDrawArrow, float lineWidth)
        {
            var bezierPos0 = new RawVector2();
            var bezierSegment = new BezierSegment();

            var curveSink = new PathGeometry(target.Factory);
            var arrowSink = new PathGeometry(target.Factory);

            using (var curveGeom = curveSink.Open())
            using (var arrowGeom = isDrawArrow ? arrowSink.Open() : null)
            {
                foreach (IBiaNodeLink link in _parent.LinksSource)
                {
                    if (link.IsVisible == false)
                        continue;

                    if (link.IsLinked() == false)
                        continue;

                    if (link.IsHighlight() != isHighlight)
                        continue;

                    var bezier = link.MakeBezierCurve();

                    // 接続線
                    {
                        bezierPos0.X = (float) bezier.Item1.X;
                        bezierPos0.Y = (float) bezier.Item1.Y;

                        curveGeom.BeginFigure(bezierPos0, FigureBegin.Hollow);
                        {
                            bezierSegment.Point1.X = (float) bezier.Item2.X;
                            bezierSegment.Point1.Y = (float) bezier.Item2.Y;
                            bezierSegment.Point2.X = (float) bezier.Item3.X;
                            bezierSegment.Point2.Y = (float) bezier.Item3.Y;
                            bezierSegment.Point3.X = (float) bezier.Item4.X;
                            bezierSegment.Point3.Y = (float) bezier.Item4.Y;

                            curveGeom.AddBezier(bezierSegment);
                        }
                        curveGeom.EndFigure(FigureEnd.Open);
                    }

                    // 矢印
                    if (isDrawArrow)
                        DrawArrow(arrowGeom, bezier);
                }

                curveGeom.Close();
                target.DrawGeometry(curveSink, brush, lineWidth);

                if (isDrawArrow)
                {
                    arrowGeom.Close();
                    target.FillGeometry(arrowSink, brush);
                }
            }
        }

        private static void DrawArrow(
            GeometrySink geom,
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

            geom.BeginFigure(new RawVector2(t1X, t1Y), FigureBegin.Filled);
            geom.AddLine(new RawVector2((float) t2.X + t1X, (float) t2.Y + t1Y));
            geom.AddLine(new RawVector2((float) t3.X + t1X, (float) t3.Y + t1Y));
            geom.EndFigure(FigureEnd.Closed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ImmutableVec2 Rotate(in ValueTuple<double, double> m, in ImmutableVec2 pos)
            => new ImmutableVec2(
                pos.X * m.Item2 - pos.Y * m.Item1,
                pos.X * m.Item1 + pos.Y * m.Item2);
    }
}