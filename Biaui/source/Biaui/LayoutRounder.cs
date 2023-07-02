using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui;

public readonly ref struct LayoutRounder
{
    public readonly double DpiScale;
    
    private readonly FrameworkElement _element;
    private readonly double _inverseDpiScale;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LayoutRounder(FrameworkElement element)
    {
        _element = element;
        DpiScale = element.PixelsPerDip();
        _inverseDpiScale = 1d / DpiScale;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Pen GetBorderPen(ByteColor color)
        => Caches.GetPen(color, RoundLayoutValue(FrameworkElementExtensions.BorderWidth));
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double RoundRenderWidth(bool isWithBorder)
    {
        return isWithBorder
            ? RoundLayoutValue(_element.RenderSize.Width - FrameworkElementExtensions.BorderWidth)
            : RoundLayoutValue(_element.RenderSize.Width);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double RoundRenderHeight(bool isWithBorder)
    {
        return isWithBorder
            ? RoundLayoutValue(_element.RenderSize.Height - FrameworkElementExtensions.BorderWidth)
            : RoundLayoutValue(_element.RenderSize.Height);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Rect RoundRenderRectangle(bool isWithBorder)
    {
        // ReSharper disable ConditionIsAlwaysTrueOrFalse
        if (isWithBorder)
        {
            return new Rect(
                RoundLayoutValue(FrameworkElementExtensions.BorderHalfWidth),
                RoundLayoutValue(FrameworkElementExtensions.BorderHalfWidth),
                RoundRenderWidth(isWithBorder),
                RoundRenderHeight(isWithBorder));
        }
        else
        {
            return new Rect(0, 0, RoundRenderWidth(isWithBorder),
                RoundRenderHeight(isWithBorder));
        }
        // ReSharper restore ConditionIsAlwaysTrueOrFalse
    } 
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ImmutableRect_double RoundLayoutRect(in ImmutableRect_double rect)
    {
        return new ImmutableRect_double(
            RoundLayoutValue(rect.X),
            RoundLayoutValue(rect.Y),
            RoundLayoutValue(rect.Width),
            RoundLayoutValue(rect.Height));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Rect RoundLayoutRect(double x, double y, double w, double h)
    {
        return new Rect(
            RoundLayoutValue(x),
            RoundLayoutValue(y),
            RoundLayoutValue(w),
            RoundLayoutValue(h));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Rect RoundLayoutRect(Rect rect)
    {
        return new Rect(
            RoundLayoutValue(rect.X),
            RoundLayoutValue(rect.Y),
            RoundLayoutValue(rect.Width),
            RoundLayoutValue(rect.Height));
    }
    
    public double RoundLayoutValue(double value)
    {
        double newValue;

        if (NumberHelper.AreClose(DpiScale, 1d) == false)
        {
            newValue = Math.Round(value * DpiScale) * _inverseDpiScale;

            if (double.IsNaN(newValue) ||
                double.IsInfinity(newValue) ||
                NumberHelper.AreClose(newValue, double.MaxValue))
            {
                newValue = value;
            }
        }
        else
        {
            newValue = value;
        }

        return newValue;
    }
}
