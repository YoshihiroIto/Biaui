using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Biaui.Interfaces;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal.NodeLinkGeomMaker
{
    internal class AxisAlignStyleNodeLinkGeomMaker : INodeLinkGeomMaker
    {
        private readonly double _arrowSize;

        internal AxisAlignStyleNodeLinkGeomMaker(double arrowSize)
        {
            _arrowSize = arrowSize;
        }

        public void Make(
            IEnumerable linksSource,
            in ImmutableRect lineCullingRect,
            double alpha,
            Color backgroundColor,
            Color highlightLinkColor,
            NodeLinkGeomMakerFlags flags,
            Dictionary<(Color Color, BiaNodeLinkStyle Style, bool IsHightlight), (StreamGeometry Geom, StreamGeometryContext Ctx)> outputCurves)
        {
            Span<ImmutableVec2> work = stackalloc ImmutableVec2[10];

            foreach (IBiaNodeLink link in linksSource)
            {
                if (link.IsVisible == false)
                    continue;

                if (link.InternalData().Slot1 == null || link.InternalData().Slot2 == null)
                    continue;

                var item1 = link.ItemSlot1.Item;
                var item2 = link.ItemSlot2.Item;
                var pos1 = item1.MakeSlotPosDefault(link.InternalData().Slot1);
                var pos2 = item2.MakeSlotPosDefault(link.InternalData().Slot2);

                var lines = LinkLineRenderer.MakeLines(ref pos1, ref pos2, item1, item2, link.InternalData(), work);

                // 線分ごとにバウンディングボックス判定、折れ丸は無視
                if (IsHitLines(lineCullingRect, lines) == false)
                    continue;
                // 
                var isHighlight = item1.IsSelected || item1.IsPreSelected || item1.IsMouseOver ||
                                  item2.IsSelected || item2.IsPreSelected || item2.IsMouseOver;

                var color = ColorHelper.Lerp(alpha, backgroundColor, isHighlight ? highlightLinkColor : link.Color);
                var key = (color, link.Style, isHighlight);

                if (outputCurves.TryGetValue(key, out var curve) == false)
                {
                    var geom = new StreamGeometry
                    {
                        FillRule = FillRule.Nonzero
                    };
                    var ctx = geom.Open();

                    curve = (geom, ctx);
                    outputCurves.Add(key, curve);
                }

                LinkLineRenderer.DrawLines(curve.Ctx, lines);

                // 矢印
                if ((flags & NodeLinkGeomMakerFlags.RequireArrow) != 0 &&
                    (link.Style & BiaNodeLinkStyle.Arrow) != 0)
                {
                    DrawArrow(
                        curve.Ctx,
                        lines,
                        Unsafe.As<Point, ImmutableVec2>(ref pos1));
                }
            }
        }

        private void DrawArrow(
            StreamGeometryContext ctx,
            Span<ImmutableVec2> lines,
            in ImmutableVec2 startPos)
        {
            if (lines.Length < 2)
                return;

            var isPosDir = lines[0] == startPos;

            var span = FindLongestSpan(lines);

            var (edge1, edge2) =
                isPosDir
                    ? (lines[span], lines[span - 1])
                    : (lines[span - 1], lines[span]);

            var pos = (edge1 + edge2) * 0.5;

            var pv = ImmutableVec2.SetSize(edge1 - edge2, _arrowSize);
            var sv = new ImmutableVec2(-pv.Y / 1.732, pv.X / 1.732);

            var t1 = pos + pv;
            var t2 = pos + sv;
            var t3 = pos - sv;

            ctx.DrawTriangle(
                Unsafe.As<ImmutableVec2, Point>(ref t1),
                Unsafe.As<ImmutableVec2, Point>(ref t2),
                Unsafe.As<ImmutableVec2, Point>(ref t3),
                false, false);
        }

        private static int FindLongestSpan(Span<ImmutableVec2> lines)
        {
            var maxLength = -1.0;
            var maxIndex = 1;

            for (var i = 1; i != lines.Length; ++i)
            {
                var p1 = lines[i - 1];
                var p2 = lines[i];

                var l = (p1 - p2).LengthSq;

                if (l > maxLength)
                {
                    maxLength = l;
                    maxIndex = i;
                }
            }

            return maxIndex;
        }

        private static bool IsHitLines(in ImmutableRect rect, Span<ImmutableVec2> points)
        {
            if (points.Length <= 1)
                return false;

            for (var i = 1; i != points.Length; ++i)
            {
                ref var p0 = ref points[i - 1];
                ref var p1 = ref points[i - 0];

                // 垂直線
                if (NumberHelper.AreClose(p0.X, p1.X))
                {
                    if (p0.X < rect.X)
                        continue;

                    if (p0.X > rect.X + rect.Width)
                        continue;

                    if (p0.Y > rect.Y &&
                        p0.Y < rect.Y + rect.Height)
                        return true;

                    if (p1.Y > rect.Y &&
                        p1.Y < rect.Y + rect.Height)
                        return true;

                    if (p0.Y < rect.Y &&
                        p1.Y > rect.Y + rect.Height)
                        return true;

                    if (p1.Y < rect.Y &&
                        p0.Y > rect.Y + rect.Height)
                        return true;
                }
                // 水平線
                else
                {
                    if (p0.Y < rect.Y)
                        continue;

                    if (p0.Y > rect.Y + rect.Height)
                        continue;

                    if (p0.X > rect.X &&
                        p0.X < rect.X + rect.Width)
                        return true;

                    if (p1.X > rect.X &&
                        p1.X < rect.X + rect.Width)
                        return true;

                    if (p0.X < rect.X &&
                        p1.X > rect.X + rect.Width)
                        return true;

                    if (p1.X < rect.X &&
                        p0.X > rect.X + rect.Width)
                        return true;
                }
            }

            return false;
        }
    }
}