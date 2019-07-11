using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Biaui.Interfaces;
using Biaui.Internals;
using Jewelry.Collections;

namespace Biaui.Controls.NodeEditor.Internal.NodeLinkGeomMaker
{
    internal class BezierCurveStyleMakeNodeGeomLink : INodeLinkGeomMaker
    {
        private readonly double _arrowSize;

        internal BezierCurveStyleMakeNodeGeomLink(double arrowSize)
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
            foreach (IBiaNodeLink link in linksSource)
            {
                if (link.IsVisible == false)
                    continue;

                if (link.InternalData().Slot1 == null || link.InternalData().Slot2 == null)
                    continue;

                var item1 = link.ItemSlot1.Item;
                var item2 = link.ItemSlot2.Item;
                var pos1 = item1.MakeSlotPos(link.InternalData().Slot1);
                var pos2 = item2.MakeSlotPos(link.InternalData().Slot2);
                var pos1C = BiaNodeEditorHelper.MakeBezierControlPoint(pos1, link.InternalData().Slot1.Dir);
                var pos2C = BiaNodeEditorHelper.MakeBezierControlPoint(pos2, link.InternalData().Slot2.Dir);

                // ReSharper disable AccessToModifiedClosure
                var bb = _boundingBoxCache.GetOrAdd(
                    (pos1, pos2, link.InternalData().Slot1.Dir, link.InternalData().Slot2.Dir),
                    x => BiaNodeEditorHelper.MakeBoundingBox(
                        Unsafe.As<Point, ImmutableVec2>(ref pos1),
                        Unsafe.As<Point, ImmutableVec2>(ref pos1C),
                        Unsafe.As<Point, ImmutableVec2>(ref pos2C),
                        Unsafe.As<Point, ImmutableVec2>(ref pos2)));
                // ReSharper restore AccessToModifiedClosure

                if (bb.IntersectsWith(lineCullingRect) == false)
                    continue;

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

                curve.Ctx.BeginFigure(pos1, false, false);
                curve.Ctx.BezierTo(
                    pos1C,
                    pos2C,
                    pos2,
                    true,
                    true);

                // 矢印
                if ((flags & NodeLinkGeomMakerFlags.RequireArrow) != 0 &&
                    (link.Style & BiaNodeLinkStyle.Arrow) != 0)
                {
                    DrawArrow(
                        curve.Ctx,
                        Unsafe.As<Point, ImmutableVec2>(ref pos1),
                        Unsafe.As<Point, ImmutableVec2>(ref pos1C),
                        Unsafe.As<Point, ImmutableVec2>(ref pos2C),
                        Unsafe.As<Point, ImmutableVec2>(ref pos2));
                }
            }
        }

        private static readonly LruCache<ValueTuple<Point, Point, BiaNodeSlotDir, BiaNodeSlotDir>, ImmutableRect> _boundingBoxCache =
            new LruCache<(Point, Point, BiaNodeSlotDir, BiaNodeSlotDir), ImmutableRect>(10000, false);

        private void DrawArrow(
            StreamGeometryContext ctx,
            in ImmutableVec2 p1,
            in ImmutableVec2 c1,
            in ImmutableVec2 c2,
            in ImmutableVec2 p2)
        {
            var b1X = BiaNodeEditorHelper.Bezier(p1.X, c1.X, c2.X, p2.X, 0.5001);
            var b1Y = BiaNodeEditorHelper.Bezier(p1.Y, c1.Y, c2.Y, p2.Y, 0.5001);
            var b2X = BiaNodeEditorHelper.Bezier(p1.X, c1.X, c2.X, p2.X, 0.5);
            var b2Y = BiaNodeEditorHelper.Bezier(p1.Y, c1.Y, c2.Y, p2.Y, 0.5);

            var sx = b1X - b2X;
            var sy = b1Y - b2Y;
            var r = Math.Atan2(sy, sx) + Math.PI * 0.5;
            var m = (Math.Sin(r), Math.Cos(r));

            var l1 = new ImmutableVec2(_arrowSize / 1.732, _arrowSize / 1.732 * 2);
            var l2 = new ImmutableVec2(-_arrowSize / 1.732, _arrowSize / 1.732 * 2);

            var t1 = (p1 + p2) * 0.5;
            var t2 = Rotate(m, l1) + t1;
            var t3 = Rotate(m, l2) + t1;

            ctx.DrawTriangle(
                Unsafe.As<ImmutableVec2, Point>(ref t1),
                Unsafe.As<ImmutableVec2, Point>(ref t2),
                Unsafe.As<ImmutableVec2, Point>(ref t3),
                false, false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ImmutableVec2 Rotate(in ValueTuple<double, double> m, in ImmutableVec2 pos)
            => new ImmutableVec2(
                pos.X * m.Item2 - pos.Y * m.Item1,
                pos.X * m.Item1 + pos.Y * m.Item2);
    }
}