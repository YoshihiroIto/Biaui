﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls;

// https://stackoverflow.com/questions/324641/how-to-make-the-contents-of-a-round-cornered-border-be-also-round-cornered
public class BiaClippingBorder : Border
{
    protected override void OnRender(DrawingContext dc)
    {
        OnApplyChildClip();
        base.OnRender(dc);
    }

    public override UIElement? Child
    {
        get => base.Child;

        set
        {
            if (Child == value)
                return;

            Child?.SetValue(ClipProperty, _oldClip);

            _oldClip = value?.ReadLocalValue(ClipProperty);

            base.Child = value;
        }
    }

    static BiaClippingBorder()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaClippingBorder),
            new FrameworkPropertyMetadata(typeof(BiaClippingBorder)));
    }

    protected virtual void OnApplyChildClip()
    {
        var child = Child;

        if (child is null)
            return;

        var key = HashCodeMaker.Make(
            child.RenderSize.Width, child.RenderSize.Height,
            CornerRadius.BottomLeft, CornerRadius.BottomRight,
            CornerRadius.TopLeft, CornerRadius.TopRight);

        if (_clipRectCache.TryGetValue(key, out var clipRect) == false)
        {
            var isSame =
                NumberHelper.AreClose(CornerRadius.TopLeft, CornerRadius.TopRight) &&
                NumberHelper.AreClose(CornerRadius.TopRight, CornerRadius.BottomRight) &&
                NumberHelper.AreClose(CornerRadius.BottomRight, CornerRadius.BottomLeft) &&
                NumberHelper.AreClose(CornerRadius.BottomLeft, CornerRadius.TopLeft);

            clipRect = isSame
                ? MakeRoundRectangleGeometrySameCorner(new Rect(child.RenderSize), CornerRadius, BorderThickness)
                : MakeRoundRectangleGeometry(new Rect(child.RenderSize), CornerRadius, BorderThickness);

            _clipRectCache.Add(key, clipRect);
        }

        child.Clip = clipRect;
    }

    private object? _oldClip;

    private static readonly Dictionary<long, Geometry> _clipRectCache = new ();

    private static Geometry MakeRoundRectangleGeometrySameCorner(Rect baseRect, CornerRadius cornerRadius,
        Thickness borderThickness)
    {
        var radius = (0d, cornerRadius.TopLeft - borderThickness.Left * 0.5d).Max();

        var clipRect = new RectangleGeometry
        {
            RadiusX = radius,
            RadiusY = radius,
            Rect = baseRect
        };

        clipRect.Freeze();

        return clipRect;
    }

    // https://wpfspark.wordpress.com/2011/06/08/clipborder-a-wpf-border-that-clips/
    private static Geometry MakeRoundRectangleGeometry(Rect baseRect, CornerRadius cornerRadius, Thickness borderThickness)
    {
        if (cornerRadius.TopLeft < double.Epsilon)
            cornerRadius.TopLeft = 0d;

        if (cornerRadius.TopRight < double.Epsilon)
            cornerRadius.TopRight = 0d;

        if (cornerRadius.BottomLeft < double.Epsilon)
            cornerRadius.BottomLeft = 0d;

        if (cornerRadius.BottomRight < double.Epsilon)
            cornerRadius.BottomRight = 0d;

        var leftHalf = borderThickness.Left * 0.5d;
        if (leftHalf < double.Epsilon)
            leftHalf = 0d;

        var topHalf = borderThickness.Top * 0.5d;
        if (topHalf < double.Epsilon)
            topHalf = 0d;

        var rightHalf = borderThickness.Right * 0.5d;
        if (rightHalf < double.Epsilon)
            rightHalf = 0d;

        var bottomHalf = borderThickness.Bottom * 0.5d;
        if (bottomHalf < double.Epsilon)
            bottomHalf = 0d;

        var topLeftRect = new Rect(
            baseRect.Location.X,
            baseRect.Location.Y,
            (0d, cornerRadius.TopLeft - leftHalf).Max(),
            (0d, cornerRadius.TopLeft - rightHalf).Max());

        var topRightRect = new Rect(
            baseRect.Location.X + baseRect.Width - cornerRadius.TopRight + rightHalf,
            baseRect.Location.Y,
            (0d, cornerRadius.TopRight - rightHalf).Max(),
            (0d, cornerRadius.TopRight - topHalf).Max());

        var bottomRightRect = new Rect(
            baseRect.Location.X + baseRect.Width - cornerRadius.BottomRight + rightHalf,
            baseRect.Location.Y + baseRect.Height - cornerRadius.BottomRight + bottomHalf,
            (0d, cornerRadius.BottomRight - rightHalf).Max(),
            (0d, cornerRadius.BottomRight - bottomHalf).Max());

        var bottomLeftRect = new Rect(
            baseRect.Location.X,
            baseRect.Location.Y + baseRect.Height - cornerRadius.BottomLeft + bottomHalf,
            (0d, cornerRadius.BottomLeft - leftHalf).Max(),
            (0d, cornerRadius.BottomLeft - bottomHalf).Max());

        if (topLeftRect.Right > topRightRect.Left)
        {
            var newWidth = topLeftRect.Width / (topLeftRect.Width + topRightRect.Width) * baseRect.Width;

            topLeftRect = new Rect(
                topLeftRect.Location.X,
                topLeftRect.Location.Y,
                newWidth,
                topLeftRect.Height);

            topRightRect = new Rect(
                baseRect.Left + newWidth,
                topRightRect.Location.Y,
                (0d, baseRect.Width - newWidth).Max(),
                topRightRect.Height);
        }

        if (topRightRect.Bottom > bottomRightRect.Top)
        {
            var newHeight = topRightRect.Height / (topRightRect.Height + bottomRightRect.Height) * baseRect.Height;

            topRightRect = new Rect(
                topRightRect.Location.X,
                topRightRect.Location.Y,
                topRightRect.Width,
                newHeight);

            bottomRightRect = new Rect(
                bottomRightRect.Location.X,
                baseRect.Top + newHeight,
                bottomRightRect.Width,
                (0d, baseRect.Height - newHeight).Max());
        }

        if (bottomRightRect.Left < bottomLeftRect.Right)
        {
            var newWidth = bottomLeftRect.Width / (bottomLeftRect.Width + bottomRightRect.Width) * baseRect.Width;

            bottomLeftRect = new Rect(
                bottomLeftRect.Location.X,
                bottomLeftRect.Location.Y,
                newWidth,
                bottomLeftRect.Height);

            bottomRightRect = new Rect(
                baseRect.Left + newWidth,
                bottomRightRect.Location.Y,
                (0d, baseRect.Width - newWidth).Max(),
                bottomRightRect.Height);
        }

        if (bottomLeftRect.Top < topLeftRect.Bottom)
        {
            var newHeight = topLeftRect.Height / (topLeftRect.Height + bottomLeftRect.Height) * baseRect.Height;

            topLeftRect = new Rect(
                topLeftRect.Location.X,
                topLeftRect.Location.Y,
                topLeftRect.Width,
                newHeight);

            bottomLeftRect = new Rect(
                bottomLeftRect.Location.X,
                baseRect.Top + newHeight,
                bottomLeftRect.Width,
                (0d, baseRect.Height - newHeight).Max());
        }

        var clipRect = new StreamGeometry();

        using (var context = clipRect.Open())
        {
            context.BeginFigure(topLeftRect.BottomLeft, true, true);

            context.ArcTo(topLeftRect.TopRight, topLeftRect.Size, 0d, false, SweepDirection.Clockwise,
                true, true);

            context.LineTo(topRightRect.TopLeft, true, true);
            context.ArcTo(topRightRect.BottomRight, topRightRect.Size, 0d, false, SweepDirection.Clockwise,
                true, true);

            context.LineTo(bottomRightRect.TopRight, true, true);
            context.ArcTo(bottomRightRect.BottomLeft, bottomRightRect.Size, 0d, false, SweepDirection.Clockwise,
                true, true);

            context.LineTo(bottomLeftRect.BottomRight, true, true);
            context.ArcTo(bottomLeftRect.TopLeft, bottomLeftRect.Size, 0d, false, SweepDirection.Clockwise,
                true, true);
        }

        clipRect.Freeze();

        return clipRect;
    }
}