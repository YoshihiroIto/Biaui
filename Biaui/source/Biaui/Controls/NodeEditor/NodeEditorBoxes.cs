using System;
using System.Runtime.CompilerServices;

namespace Biaui.Controls.NodeEditor
{
    public static class NodeEditorBoxes
    {
        public static readonly object NodeLinkStyleAxisAlign = BiaNodeEditorNodeLinkStyle.AxisAlign;
        public static readonly object NodeLinkStyleBezierCurve = BiaNodeEditorNodeLinkStyle.BezierCurve;
        
        public static readonly object ScaleSliderLocationLeft = BiaNodeEditorScaleSliderLocation.Left;
        public static readonly object ScaleSliderLocationLeftTop = BiaNodeEditorScaleSliderLocation.LeftTop;
        public static readonly object ScaleSliderLocationTopLeft = BiaNodeEditorScaleSliderLocation.TopLeft;
        public static readonly object ScaleSliderLocationTop = BiaNodeEditorScaleSliderLocation.Top;
        public static readonly object ScaleSliderLocationTopRight = BiaNodeEditorScaleSliderLocation.TopRight;
        public static readonly object ScaleSliderLocationRightTop = BiaNodeEditorScaleSliderLocation.RightTop;
        public static readonly object ScaleSliderLocationRight = BiaNodeEditorScaleSliderLocation.Right;
        public static readonly object ScaleSliderLocationRightBottom = BiaNodeEditorScaleSliderLocation.RightBottom;
        public static readonly object ScaleSliderLocationBottomRight = BiaNodeEditorScaleSliderLocation.BottomRight;
        public static readonly object ScaleSliderLocationBottom = BiaNodeEditorScaleSliderLocation.Bottom;
        public static readonly object ScaleSliderLocationBottomLeft = BiaNodeEditorScaleSliderLocation.BottomLeft;
        public static readonly object ScaleSliderLocationLeftBottom = BiaNodeEditorScaleSliderLocation.LeftBottom;
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object NodeLinkStyle(BiaNodeEditorNodeLinkStyle i)
        {
            return i switch
            {
                BiaNodeEditorNodeLinkStyle.AxisAlign => NodeLinkStyleAxisAlign,
                BiaNodeEditorNodeLinkStyle.BezierCurve => NodeLinkStyleBezierCurve,
                _ => throw new ArgumentOutOfRangeException(nameof(i), i, null)
            };
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object NodeEditorScaleSliderLocation(BiaNodeEditorScaleSliderLocation v)
        {
            return v switch
            {
                BiaNodeEditorScaleSliderLocation.Left => ScaleSliderLocationLeft,
                BiaNodeEditorScaleSliderLocation.LeftTop => ScaleSliderLocationLeftTop,
                BiaNodeEditorScaleSliderLocation.TopLeft => ScaleSliderLocationTopLeft,
                BiaNodeEditorScaleSliderLocation.Top => ScaleSliderLocationTop,
                BiaNodeEditorScaleSliderLocation.TopRight => ScaleSliderLocationTopRight,
                BiaNodeEditorScaleSliderLocation.RightTop => ScaleSliderLocationRightTop,
                BiaNodeEditorScaleSliderLocation.Right => ScaleSliderLocationRight,
                BiaNodeEditorScaleSliderLocation.RightBottom => ScaleSliderLocationRightBottom,
                BiaNodeEditorScaleSliderLocation.BottomRight => ScaleSliderLocationBottomRight,
                BiaNodeEditorScaleSliderLocation.Bottom => ScaleSliderLocationBottom,
                BiaNodeEditorScaleSliderLocation.BottomLeft => ScaleSliderLocationBottomLeft,
                BiaNodeEditorScaleSliderLocation.LeftBottom => ScaleSliderLocationLeftBottom,
                _ => throw new ArgumentOutOfRangeException(nameof(v), v, null)
            };
        }
    }
}