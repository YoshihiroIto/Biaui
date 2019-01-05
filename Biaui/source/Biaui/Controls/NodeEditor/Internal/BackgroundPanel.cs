using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Biaui.Controls.Internals;
using Biaui.Interfaces;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal class BackgroundPanel : Canvas
    {
        private readonly BiaNodeEditor _parent;

        static BackgroundPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BackgroundPanel),
                new FrameworkPropertyMetadata(typeof(BackgroundPanel)));
        }

        internal BackgroundPanel(BiaNodeEditor parent, MouseOperator mouseOperator)
        {
            _parent = parent;

            _parent.Translate.Changed += (_, __) => InvalidateVisual();
            _parent.Scale.Changed += (_, __) => InvalidateVisual();
            _parent.NodeItemMoved += (_, __) => InvalidateVisual();
            _parent.LinksSourceChanging += (_, __) => InvalidateVisual();
            _parent.LinkChanged += (_, __) => InvalidateVisual();

            mouseOperator.PreMouseLeftButtonUp += (_, __) => InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;

            DrawGrid(dc);

            DrawNodeLink(dc);
        }

        private readonly StreamGeometry _gridGeom = new StreamGeometry();

        private void DrawGrid(DrawingContext dc)
        {
            const double unit = 1024;

            var p = this.GetBorderPen(Color.FromRgb(0x37, 0x37, 0x40));

            var s = _parent.Scale.ScaleX;
            var tx = _parent.Translate.X;
            var ty = _parent.Translate.Y;

            var bx = FrameworkElementHelper.RoundLayoutValue(ActualWidth);
            var by = FrameworkElementHelper.RoundLayoutValue(ActualHeight);

            _gridGeom.Clear();

            var geomCtx = _gridGeom.Open();
            {
                for (var h = 0;; ++h)
                {
                    var x = (h * unit) * s + tx;

                    x = FrameworkElementHelper.RoundLayoutValue(x);

                    if (x < 0) continue;

                    if (x > ActualWidth) break;

                    geomCtx.BeginFigure(new Point(x, 0), false, false);
                    geomCtx.LineTo(new Point(x, by), true, false);
                }

                for (var h = 0;; --h)
                {
                    var x = (h * unit) * s + tx;

                    x = FrameworkElementHelper.RoundLayoutValue(x);

                    if (x > ActualWidth) continue;

                    if (x < 0) break;

                    geomCtx.BeginFigure(new Point(x, 0), false, false);
                    geomCtx.LineTo(new Point(x, by), true, false);
                }

                for (var v = 0;; ++v)
                {
                    var y = (v * unit) * s + ty;

                    y = FrameworkElementHelper.RoundLayoutValue(y);

                    if (y < 0) continue;

                    if (y > ActualHeight) break;

                    geomCtx.BeginFigure(new Point(0, y), false, false);
                    geomCtx.LineTo(new Point(bx, y), true, false);
                }

                for (var v = 0;; --v)
                {
                    var y = (v * unit) * s + ty;

                    y = FrameworkElementHelper.RoundLayoutValue(y);

                    if (y > ActualHeight) continue;

                    if (y < 0) break;

                    geomCtx.BeginFigure(new Point(0, y), false, false);
                    geomCtx.LineTo(new Point(bx, y), true, false);
                }
            }
            ((IDisposable) geomCtx).Dispose();
            dc.DrawGeometry(null, p, _gridGeom);
        }

        private static readonly
            Dictionary<(Color Color, BiaNodeLinkStyle Style), (StreamGeometry Geom, StreamGeometryContext Ctx)> _curves
                = new Dictionary<(Color, BiaNodeLinkStyle), (StreamGeometry, StreamGeometryContext)>();

        private void DrawNodeLink(DrawingContext dc)
        {
            if (_parent.LinksSource == null)
                return;

            Span<Point> bezierPoints = stackalloc Point[4];
            var hitTestWork = MemoryMarshal.Cast<Point, ImmutableVec2>(bezierPoints);

            var viewport = _parent.TransformRect(ActualWidth, ActualHeight);

            var backgroundColor = ((SolidColorBrush) _parent.Background).Color;

            var alpha = _parent.IsNodePortDragging
                ? 0.2
                : 1.0;

            foreach (var link in _parent.LinksSource)
            {
                InternalBiaNodeLinkData internalData;

                if (link.InternalData == null)
                {
                    internalData = new InternalBiaNodeLinkData
                    {
                        Port1 = link.ItemPort1.FindPort(),
                        Port2 = link.ItemPort2.FindPort()
                    };

                    link.InternalData = internalData;
                }
                else
                {
                    internalData = (InternalBiaNodeLinkData) link.InternalData;
                }

                if (internalData.Port1 == null || internalData.Port2 == null)
                    continue;

                var pos1 = link.ItemPort1.Item.MakePortPos(internalData.Port1);
                var pos2 = link.ItemPort2.Item.MakePortPos(internalData.Port2);

                var pos12 = BiaNodeEditorHelper.MakeBezierControlPoint(pos1, internalData.Port1.Dir);
                var pos21 = BiaNodeEditorHelper.MakeBezierControlPoint(pos2, internalData.Port2.Dir);

                // ※.HitTestBezier を呼ぶと_bezierPointsは書き変わる
                bezierPoints[0] = pos1;
                bezierPoints[1] = pos12;
                bezierPoints[2] = pos21;
                bezierPoints[3] = pos2;

                if (BiaNodeEditorHelper.HitTestBezier(hitTestWork, viewport) == false)
                    continue;

                var color = ColorHelper.Lerp(alpha, backgroundColor, link.Color);
                var key = (color, link.Style);

                if (_curves.TryGetValue(key, out var curve) == false)
                {
                    var geom = new StreamGeometry
                    {
                        FillRule = FillRule.Nonzero
                    };
                    var ctx = geom.Open();

                    curve = (geom, ctx);
                    _curves.Add(key, curve);
                }

                // 線
                var item1 = link.ItemPort1.Item;
                var item2 = link.ItemPort2.Item;

                DrawLine(curve.Ctx, ref pos1, ref pos2, item1, item2, internalData);

#if false
                // 矢印
                if ((link.Style & BiaNodeLinkStyle.Arrow) != 0)
                    DrawArrow(curve.Ctx, in pos1, in pos12, in pos21, in pos2);
#endif
            }

            dc.PushTransform(_parent.Translate);
            dc.PushTransform(_parent.Scale);
            {
                foreach (var c in _curves)
                {
                    var pen =
                        (c.Key.Style & BiaNodeLinkStyle.DashedLine) != 0
                            ? Caches.GetDashedPen(c.Key.Color, 3)
                            : Caches.GetPen(c.Key.Color, 3);

                    ((IDisposable) c.Value.Ctx).Dispose();
                    dc.DrawGeometry(Caches.GetSolidColorBrush(c.Key.Color), pen, c.Value.Geom);
                }
            }
            dc.Pop();
            dc.Pop();

            _curves.Clear();
        }

        private static void DrawLine(
            StreamGeometryContext ctx,
            ref Point pos1, ref Point pos2,
            IBiaNodeItem item1,
            IBiaNodeItem item2,
            InternalBiaNodeLinkData internalData)
        {
            var u1 = new PortUnit(Unsafe.As<Point, ImmutableVec2>(ref pos1), item1, internalData.Port1);
            var u2 = new PortUnit(Unsafe.As<Point, ImmutableVec2>(ref pos2), item2, internalData.Port2);

            if (u1.Port.Dir != u2.Port.Dir &&
                u1.IsHorizontal &&
                u2.IsHorizontal)
            {
                DrawDifferenceLine(ctx, u1, u2, true);
            }
            else if (u1.Port.Dir != u2.Port.Dir &&
                     u1.IsVertical &&
                     u2.IsVertical)
            {
                DrawDifferenceLine(ctx, u1, u2, false);
            }
            else if (u1.Port.Dir == u2.Port.Dir &&
                     u1.Port.Dir.IsHorizontal())
            {
                DrawSameLine(ctx, u1, u2, true);
            }
            else if (u1.Port.Dir == u2.Port.Dir &&
                     u1.IsVertical)
            {
                DrawSameLine(ctx, u1, u2, false);
            }
            else
            {
                DrawHVLine(ctx, u1, u2);
            }
        }

        const double minPortOffset = 24.0;

        private static void DrawDifferenceLine(
            StreamGeometryContext ctx,
            in PortUnit unit1,
            in PortUnit unit2,
            bool isHorizontal)
        {
            var b = isHorizontal;

            var (start, end) =
                unit1.Port.Dir == Transposer.NodePortDir(BiaNodePortDir.Right, b)
                    ? (unit1, unit2)
                    : (unit2, unit1);

            var startItemCenter = start.Item.AlignedPos().Y(b) + start.Item.Size.Height(b) * 0.5;
            var startItemPortOffset =
                start.Item.Size.Height(b) * 0.5 - NumberHelper.Abs(startItemCenter - start.Pos.Y(b));
            var fold = minPortOffset + startItemPortOffset;
            var cornerRadius = fold * 0.5;

            var startFoldPos = start.Pos.X(b) + fold;
            var foldStartPos = Transposer.CreateImmutableVec2(startFoldPos, start.Pos.Y(b), b);

            if (startFoldPos < end.Pos.X(b) - fold)
            {
                var foldEndPos = Transposer.CreateImmutableVec2(startFoldPos, end.Pos.Y(b), b);

                DrawLines(ctx, cornerRadius,
                    new[]
                    {
                        start.Pos,
                        foldStartPos,
                        foldEndPos,
                        end.Pos
                    });
            }
            else
            {
                var foldEndPos = Transposer.CreateImmutableVec2(end.Pos.X(b) - fold, end.Pos.Y(b), b);

                var foldV = start.Pos.Y(b) < end.Pos.Y(b)
                    ? start.Item.AlignedPos().Y(b) + start.Item.Size.Height(b) + fold
                    : end.Item.AlignedPos().Y(b) + end.Item.Size.Height(b) + fold;

                DrawLines(ctx, cornerRadius,
                    new[]
                    {
                        start.Pos,
                        foldStartPos,
                        Transposer.CreateImmutableVec2(foldStartPos.X(b), foldV, b),
                        Transposer.CreateImmutableVec2(foldEndPos.X(b), foldV, b),
                        foldEndPos,
                        end.Pos
                    });
            }
        }

        private static void DrawSameLine(
            StreamGeometryContext ctx,
            in PortUnit unit1,
            in PortUnit unit2,
            bool isHorizontal)
        {
            var b = isHorizontal;

            var startItemCenter = unit1.Item.AlignedPos().Y(b) + unit1.Item.Size.Height(b) * 0.5;
            var startItemPortOffset =
                unit1.Item.Size.Height(b) * 0.5 - NumberHelper.Abs(startItemCenter - unit1.Pos.Y(b));

            var endItemCenter = unit2.Item.AlignedPos().Y(b) + unit2.Item.Size.Height(b) * 0.5;
            var endItemPortOffset = unit2.Item.Size.Height(b) * 0.5 - NumberHelper.Abs(endItemCenter - unit2.Pos.Y(b));
            var fold = minPortOffset + (startItemPortOffset, endItemPortOffset).Max();

            var foldPos = unit1.Port.Dir == Transposer.NodePortDir(BiaNodePortDir.Left, b)
                ? (unit1.Pos.X(b), unit2.Pos.X(b)).Min() - fold
                : (unit1.Pos.X(b), unit2.Pos.X(b)).Max() + fold;

            var foldStartPos = Transposer.CreateImmutableVec2(foldPos, unit1.Pos.Y(b), b);
            var foldEndPos = Transposer.CreateImmutableVec2(foldPos, unit2.Pos.Y(b), b);

            var cornerRadius = fold * 0.5;

            DrawLines(ctx, cornerRadius,
                new[]
                {
                    unit1.Pos,
                    foldStartPos,
                    foldEndPos,
                    unit2.Pos
                });
        }

        private static void DrawHVLine(
            StreamGeometryContext ctx,
            in PortUnit unit1,
            in PortUnit unit2)
        {
            var (left, right) =
                unit1.Pos.X < unit2.Pos.X
                    ? (unit1, unit2)
                    : (unit2, unit1);

            var leftOffset = left.MakeOffsetPos();
            var rightOffset = right.MakeOffsetPos();
            var cornerRadius = leftOffset.FoldLength * 0.5;

            DrawLines(ctx, cornerRadius,
                new[]
                {
                    left.Pos,
                    leftOffset.OffsetPos,
                    left.IsVertical
                        ? new ImmutableVec2(rightOffset.OffsetPos.X, leftOffset.OffsetPos.Y)
                        : new ImmutableVec2(leftOffset.OffsetPos.X, rightOffset.OffsetPos.Y),
                    rightOffset.OffsetPos,
                    right.Pos
                });
        }

        internal readonly struct PortUnit
        {
            internal readonly ImmutableVec2 Pos;
            internal readonly IBiaNodeItem Item;
            internal readonly BiaNodePort Port;

            internal bool IsHorizontal => Port.Dir.IsHorizontal();

            internal bool IsVertical => Port.Dir.IsVertical();

            internal PortUnit(in ImmutableVec2 pos, IBiaNodeItem item, BiaNodePort port)
            {
                Pos = pos;
                Item = item;
                Port = port;
            }

            internal (ImmutableVec2 OffsetPos, double FoldLength) MakeOffsetPos()
            {
                double itemPortOffset;
                {
                    if (IsVertical)
                    {
                        var itemCenter = Item.AlignedPos().X + Item.Size.Width * 0.5;
                        itemPortOffset = Item.Size.Width * 0.5 - NumberHelper.Abs(itemCenter - Pos.X);
                    }
                    else
                    {
                        var itemCenter = Item.AlignedPos().Y + Item.Size.Height * 0.5;
                        itemPortOffset = Item.Size.Height * 0.5 - NumberHelper.Abs(itemCenter - Pos.Y);
                    }
                }

                var foldLength = minPortOffset + itemPortOffset;
                var foldOffset = DirVector(foldLength);

                return (Pos + foldOffset, foldLength);
            }

            private ImmutableVec2 DirVector(double length)
            {
                switch (Port.Dir)
                {
                    case BiaNodePortDir.Left:
                        return new ImmutableVec2(-length, 0);

                    case BiaNodePortDir.Top:
                        return new ImmutableVec2(0, -length);

                    case BiaNodePortDir.Right:
                        return new ImmutableVec2(+length, 0);

                    case BiaNodePortDir.Bottom:
                        return new ImmutableVec2(0, +length);

                    default:
                        throw new ArgumentOutOfRangeException(nameof(Port.Dir), Port.Dir, null);
                }
            }
        }

        private static void DrawLines(
            StreamGeometryContext ctx,
            double maxCornerRadius,
            Span<ImmutableVec2> points)
        {
            var isFirst = true;

            for (var i = 2; i != points.Length; ++i)
            {
                ref var p0 = ref points[i - 2];
                ref var p1 = ref points[i - 1];
                ref var p2 = ref points[i - 0];

                var d01 = (p0 - p1).Length;
                var d12 = (p1 - p2).Length;

                var d = (d01, d12).Min();
                var radius = (d * 0.5, maxCornerRadius).Min();

                var v01 = ImmutableVec2.SetSize(p0 - p1, radius);
                var v21 = ImmutableVec2.SetSize(p2 - p1, radius);

                var p01 = p1 + v01;
                var p21 = p1 + v21;

                var hp12 = (p1 + p2) * 0.5;

                var c0 = p01 - v01 * ControlPointRatio;
                var c1 = p21 - v21 * ControlPointRatio;

                if (isFirst)
                {
                    isFirst = false;
                    ctx.BeginFigure(Unsafe.As<ImmutableVec2, Point>(ref p0), false, false);
                }

                ctx.LineTo(Unsafe.As<ImmutableVec2, Point>(ref p01), true, false);

                ctx.BezierTo(
                    Unsafe.As<ImmutableVec2, Point>(ref c0),
                    Unsafe.As<ImmutableVec2, Point>(ref c1),
                    Unsafe.As<ImmutableVec2, Point>(ref p21),
                    true,
                    true);

                var isLastPoint = i == points.Length - 1;
                ctx.LineTo(
                    isLastPoint
                        ? Unsafe.As<ImmutableVec2, Point>(ref p2)
                        : Unsafe.As<ImmutableVec2, Point>(ref hp12),
                    true,
                    false);
            }
        }

        private static readonly double ControlPointRatio = (Math.Sqrt(2) - 1) * 4 / 3;

        private static void DrawArrow(
            StreamGeometryContext ctx,
            in Point pos1, in Point pos12, in Point pos21, in Point pos2)
        {
            var p1 = BiaNodeEditorHelper.InterpolationBezier(pos1, pos12, pos21, pos2, 0.50);
            var p2 = BiaNodeEditorHelper.InterpolationBezier(pos1, pos12, pos21, pos2, 0.45);

            const double size = 20;
            var pv = ImmutableVec2.SetSize(p1 - p2, size);
            var sv = new ImmutableVec2(-pv.Y / 1.732, pv.X / 1.732);

            var t1 = p1 + pv;
            var t2 = p1 + sv;
            var t3 = p1 - sv;

            ctx.DrawTriangle(
                Unsafe.As<ImmutableVec2, Point>(ref t1),
                Unsafe.As<ImmutableVec2, Point>(ref t2),
                Unsafe.As<ImmutableVec2, Point>(ref t3),
                false, false);
        }
    }
}