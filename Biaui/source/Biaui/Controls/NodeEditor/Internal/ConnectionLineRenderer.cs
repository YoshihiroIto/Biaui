using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Biaui.Interfaces;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal static class ConnectionLineRenderer
    {
        internal static Span<ImmutableVec2> MakeLines(
            ref Point pos1,
            ref Point pos2,
            IBiaNodeItem item1,
            IBiaNodeItem item2,
            InternalBiaNodeLinkData internalData,
            Span<ImmutableVec2> work)
        {
            var u1 = new PortUnit(Unsafe.As<Point, ImmutableVec2>(ref pos1), item1, internalData.Port1);
            var u2 = new PortUnit(Unsafe.As<Point, ImmutableVec2>(ref pos2), item2, internalData.Port2);

            if (u1.Port.Dir != u2.Port.Dir &&
                u1.IsHorizontal &&
                u2.IsHorizontal)
                return MakeDifferenceLines(u1, u2, true, work);

            if (u1.Port.Dir != u2.Port.Dir &&
                u1.IsVertical &&
                u2.IsVertical)
                return MakeDifferenceLines(u1, u2, false, work);

            if (u1.Port.Dir == u2.Port.Dir &&
                u1.Port.Dir.IsHorizontal())
                return MakeSameLines(u1, u2, true, work);

            if (u1.Port.Dir == u2.Port.Dir &&
                u1.IsVertical)
                return MakeSameLines(u1, u2, false, work);

            return MakeHVLines(u1, u2, work);
        }

        internal static void DrawLines(StreamGeometryContext ctx, Span<ImmutableVec2> points)
        {
            if (points.Length < 3)
                return;

            var isFirst = true;

            for (var i = 2; i != points.Length; ++i)
            {
                ref var p0 = ref points[i - 2];
                ref var p1 = ref points[i - 1];
                ref var p2 = ref points[i - 0];

                var d01 = (p0 - p1).Length;
                var d12 = (p1 - p2).Length;

                var d = (d01, d12).Min();

                var radius = (d * 0.5, 64.0).Min();

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

        private const double minPortOffset = 24.0;

        private static Span<ImmutableVec2> MakeDifferenceLines(
            in PortUnit unit1,
            in PortUnit unit2,
            bool isHorizontal,
            Span<ImmutableVec2> work)
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

            var startFoldPos = start.Pos.X(b) + fold;
            var foldStartPos = Transposer.CreateImmutableVec2(startFoldPos, start.Pos.Y(b), b);

            if (startFoldPos < end.Pos.X(b) - fold)
            {
                var foldEndPos = Transposer.CreateImmutableVec2(startFoldPos, end.Pos.Y(b), b);

                work[0] = start.Pos;
                work[1] = foldStartPos;
                work[2] = foldEndPos;
                work[3] = end.Pos;

                return work.Slice(0, 4);
            }
            else
            {
                var foldEndPos = Transposer.CreateImmutableVec2(end.Pos.X(b) - fold, end.Pos.Y(b), b);

                var foldV = start.Pos.Y(b) < end.Pos.Y(b)
                    ? start.Item.AlignedPos().Y(b) + start.Item.Size.Height(b) + fold
                    : end.Item.AlignedPos().Y(b) + end.Item.Size.Height(b) + fold;

                work[0] = start.Pos;
                work[1] = foldStartPos;
                work[2] = Transposer.CreateImmutableVec2(foldStartPos.X(b), foldV, b);
                work[3] = Transposer.CreateImmutableVec2(foldEndPos.X(b), foldV, b);
                work[4] = foldEndPos;
                work[5] = end.Pos;

                return work.Slice(0, 6);
            }
        }

        private static Span<ImmutableVec2> MakeSameLines(
            in PortUnit unit1,
            in PortUnit unit2,
            bool isHorizontal,
            Span<ImmutableVec2> work)
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

            work[0] = unit1.Pos;
            work[1] = foldStartPos;
            work[2] = foldEndPos;
            work[3] = unit2.Pos;

            return work.Slice(0, 4);
        }

        private static Span<ImmutableVec2> MakeHVLines(
            in PortUnit unit1,
            in PortUnit unit2,
            Span<ImmutableVec2> work)
        {
            var (left, right) =
                unit1.Pos.X < unit2.Pos.X
                    ? (unit1, unit2)
                    : (unit2, unit1);

            var leftOffset = left.MakeOffsetPos();
            var rightOffset = right.MakeOffsetPos();

            //　交差している場合
            if (left.IsHorizontal)
            {
                var (left1, left2) = (left.Pos.X < leftOffset.OffsetPos.X)
                    ? (left.Pos, leftOffset.OffsetPos)
                    : (leftOffset.OffsetPos, left.Pos);

                var (right1, right2) = (right.Pos.Y < rightOffset.OffsetPos.Y)
                    ? (right.Pos, rightOffset.OffsetPos)
                    : (rightOffset.OffsetPos, right.Pos);

                if (right1.X > left1.X &&
                    right1.X < left2.X &&
                    left1.Y > right1.Y &&
                    left1.Y < right2.Y)
                {
                    // 交差している
                    work[0] = left.Pos;
                    work[1] = new ImmutableVec2(right.Pos.X, left.Pos.Y);
                    work[2] = right.Pos;

                    return work.Slice(0, 3);
                }
            }
            else
            {
                var (left1, left2) = (left.Pos.Y < leftOffset.OffsetPos.Y)
                    ? (left.Pos, leftOffset.OffsetPos)
                    : (leftOffset.OffsetPos, left.Pos);

                var (right1, right2) = (right.Pos.X < rightOffset.OffsetPos.X)
                    ? (right.Pos, rightOffset.OffsetPos)
                    : (rightOffset.OffsetPos, right.Pos);

                if (right1.Y > left1.Y &&
                    right1.Y < left2.Y &&
                    left1.X > right1.X &&
                    left1.X < right2.X)
                {
                    // 交差している
                    work[0] = left.Pos;
                    work[1] = new ImmutableVec2(left.Pos.X, right.Pos.Y);
                    work[2] = right.Pos;

                    return work.Slice(0, 3);
                }
            }

            // 迂回必要なしの場合
            {
                if (left.IsRight)
                {
                    var r = false;
                    var c = new ImmutableVec2(right.Pos.X, left.Pos.Y);

                    if (right.IsBottom)
                        r = c.Y > rightOffset.OffsetPos.Y && c.X > leftOffset.OffsetPos.X;

                    else if (right.IsTop)
                        r = c.Y < rightOffset.OffsetPos.Y && c.X > leftOffset.OffsetPos.X;

                    if (r)
                    {
                        work[0] = left.Pos;
                        work[1] = c;
                        work[2] = right.Pos;

                        return work.Slice(0, 3);
                    }
                }

                if (right.IsLeft)
                {
                    var r = false;
                    var c = new ImmutableVec2(left.Pos.X, right.Pos.Y);

                    if (left.IsBottom)
                        r = c.Y > leftOffset.OffsetPos.Y && c.X < rightOffset.OffsetPos.X;

                    else if (left.IsTop)
                        r = c.Y < leftOffset.OffsetPos.Y && c.X < rightOffset.OffsetPos.X;

                    if (r)
                    {
                        work[0] = left.Pos;
                        work[1] = c;
                        work[2] = right.Pos;

                        return work.Slice(0, 3);
                    }
                }
            }

            work[0] = left.Pos;
            work[1] = leftOffset.OffsetPos;
            work[2] = left.IsVertical
                ? new ImmutableVec2(rightOffset.OffsetPos.X, leftOffset.OffsetPos.Y)
                : new ImmutableVec2(leftOffset.OffsetPos.X, rightOffset.OffsetPos.Y);
            work[3] = rightOffset.OffsetPos;
            work[4] = right.Pos;

            return work.Slice(0, 5);
        }

        internal readonly struct PortUnit
        {
            internal readonly ImmutableVec2 Pos;
            internal readonly IBiaNodeItem Item;
            internal readonly BiaNodePort Port;

            internal bool IsHorizontal => Port.Dir.IsHorizontal();

            internal bool IsVertical => Port.Dir.IsVertical();

            internal bool IsLeft => Port.Dir == BiaNodePortDir.Left;

            internal bool IsTop => Port.Dir == BiaNodePortDir.Top;

            internal bool IsRight => Port.Dir == BiaNodePortDir.Right;

            internal bool IsBottom => Port.Dir == BiaNodePortDir.Bottom;

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

        private static readonly double ControlPointRatio = (Math.Sqrt(2) - 1) * 4 / 3;
    }
}