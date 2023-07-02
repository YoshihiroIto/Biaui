﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Biaui.Internals;

namespace Biaui.Controls;

public enum BiaNumberEditorMode
{
    Simple,
    WideRange
}

public class BiaNumberEditor : FrameworkElement
{
    #region IsReadOnly

    public bool IsReadOnly
    {
        get => _IsReadOnly;
        set
        {
            if (value != _IsReadOnly)
                SetValue(IsReadOnlyProperty, Boxes.Bool(value));
        }
    }

    private bool _IsReadOnly;

    public static readonly DependencyProperty IsReadOnlyProperty =
        DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(BiaNumberEditor),
            new FrameworkPropertyMetadata(
                Boxes.BoolFalse,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaNumberEditor) s;
                    self._IsReadOnly = (bool) e.NewValue;
                }));

    #endregion

    #region BorderColor

    public ByteColor BorderColor
    {
        get => _BorderColor;
        set
        {
            if (value != _BorderColor)
                SetValue(BorderColorProperty, value);
        }
    }

    private ByteColor _BorderColor = ByteColor.Red;

    public static readonly DependencyProperty BorderColorProperty =
        DependencyProperty.Register(nameof(BorderColor), typeof(ByteColor), typeof(BiaNumberEditor),
            new FrameworkPropertyMetadata(
                Boxes.ByteColorRed,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaNumberEditor) s;
                    self._BorderColor = (ByteColor) e.NewValue;
                }));

    #endregion

    #region SliderBrush

    public Brush? SliderBrush
    {
        get => _SliderBrush;
        set
        {
            if (value != _SliderBrush)
                SetValue(SliderBrushProperty, value);
        }
    }

    private Brush? _SliderBrush = Brushes.GreenYellow;

    public static readonly DependencyProperty SliderBrushProperty =
        DependencyProperty.Register(nameof(SliderBrush), typeof(Brush), typeof(BiaNumberEditor),
            new FrameworkPropertyMetadata(
                Brushes.GreenYellow,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaNumberEditor) s;
                    self._SliderBrush = (Brush) e.NewValue;
                }));

    #endregion

    #region Value

    public double Value
    {
        get => _Value;
        set
        {
            if (NumberHelper.AreClose(value, _Value) == false)
                SetValue(ValueProperty, Boxes.Double(value));
        }
    }

    private double _Value;

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(double), typeof(BiaNumberEditor),
            new FrameworkPropertyMetadata(
                Boxes.Double0,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    // 最小値・最大値でクランプして保存する
                    var self = (BiaNumberEditor) s;
                    self._Value = self.ClampValue((double) e.NewValue);

                    self.ValueChanged?.Invoke(self, EventArgs.Empty);
                }));

    #endregion

    #region Caption

    public string? Caption
    {
        get => _Caption;
        set
        {
            if (value != _Caption)
                SetValue(CaptionProperty, value);
        }
    }

    private string? _Caption;

    public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register(nameof(Caption), typeof(string), typeof(BiaNumberEditor),
            new FrameworkPropertyMetadata(
                default(string),
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaNumberEditor) s;
                    self._Caption = e.NewValue?.ToString() ?? "";
                }));

    #endregion

    #region SliderMinimum

    public double SliderMinimum
    {
        get => _SliderMinimum;
        set
        {
            if (NumberHelper.AreClose(value, _SliderMinimum) == false)
                SetValue(SliderMinimumProperty, Boxes.Double(value));
        }
    }

    private double _SliderMinimum = double.MinValue;

    public static readonly DependencyProperty SliderMinimumProperty =
        DependencyProperty.Register(nameof(SliderMinimum), typeof(double), typeof(BiaNumberEditor),
            new FrameworkPropertyMetadata(
                Boxes.DoubleMin,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaNumberEditor) s;
                    self._SliderMinimum = (double) e.NewValue;
                }));

    #endregion

    #region SliderMaximum

    public double SliderMaximum
    {
        get => _SliderMaximum;
        set
        {
            if (NumberHelper.AreClose(value, _SliderMaximum) == false)
                SetValue(SliderMaximumProperty, Boxes.Double(value));
        }
    }

    private double _SliderMaximum = double.MaxValue;

    public static readonly DependencyProperty SliderMaximumProperty =
        DependencyProperty.Register(nameof(SliderMaximum), typeof(double), typeof(BiaNumberEditor),
            new FrameworkPropertyMetadata(
                Boxes.DoubleMax,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaNumberEditor) s;
                    self._SliderMaximum = (double) e.NewValue;
                }));

    #endregion

    #region Minimum

    public double Minimum
    {
        get => _Minimum;
        set
        {
            if (NumberHelper.AreClose(value, _Minimum) == false)
                SetValue(MinimumProperty, Boxes.Double(value));
        }
    }

    private double _Minimum = double.MinValue;

    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(BiaNumberEditor),
            new FrameworkPropertyMetadata(
                Boxes.DoubleMin,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    // 変更後の最小値でValueをクランプする

                    var self = (BiaNumberEditor) s;
                    self._Minimum = (double) e.NewValue;

                    self.Value = self.ClampValue(self.Value);
                }));

    #endregion

    #region Maximum

    public double Maximum
    {
        get => _Maximum;
        set
        {
            if (NumberHelper.AreClose(value, _Maximum) == false)
                SetValue(MaximumProperty, Boxes.Double(value));
        }
    }

    private double _Maximum = double.MaxValue;

    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(BiaNumberEditor),
            new FrameworkPropertyMetadata(
                Boxes.DoubleMax,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    // 変更後の最大値でValueをクランプする

                    var self = (BiaNumberEditor) s;
                    self._Maximum = (double) e.NewValue;

                    self.Value = self.ClampValue(self.Value);
                }));

    #endregion

    #region DisplayFormat

    public string DisplayFormat
    {
        get => _DisplayFormat;
        set
        {
            if (value != _DisplayFormat)
                SetValue(DisplayFormatProperty, value);
        }
    }

    private string _DisplayFormat = "F3";

    public static readonly DependencyProperty DisplayFormatProperty =
        DependencyProperty.Register(nameof(DisplayFormat), typeof(string), typeof(BiaNumberEditor),
            new FrameworkPropertyMetadata(
                "F3",
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaNumberEditor) s;
                    self._DisplayFormat = e.NewValue?.ToString() ?? "";
                }));

    #endregion

    #region UnitString

    public string UnitString
    {
        get => _UnitString;
        set
        {
            if (value != _UnitString)
                SetValue(UnitStringProperty, value);
        }
    }

    private string _UnitString = "";

    public static readonly DependencyProperty UnitStringProperty =
        DependencyProperty.Register(nameof(UnitString), typeof(string), typeof(BiaNumberEditor),
            new FrameworkPropertyMetadata(
                "",
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaNumberEditor) s;
                    self._UnitString = e.NewValue?.ToString() ?? "";
                }));

    #endregion

    #region Background

    public Brush? Background
    {
        get => _Background;
        set
        {
            if (value != _Background)
                SetValue(BackgroundProperty, value);
        }
    }

    private Brush? _Background;

    public static readonly DependencyProperty BackgroundProperty =
        DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(BiaNumberEditor),
            new FrameworkPropertyMetadata(
                default(Brush),
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaNumberEditor) s;
                    self._Background = (Brush) e.NewValue;
                }));

    #endregion

    #region Foreground

    public Brush? Foreground
    {
        get => _Foreground;
        set
        {
            if (value != _Foreground)
                SetValue(ForegroundProperty, value);
        }
    }

    private Brush? _Foreground;

    public static readonly DependencyProperty ForegroundProperty =
        DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(BiaNumberEditor),
            new FrameworkPropertyMetadata(
                default(Brush),
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaNumberEditor) s;
                    self._Foreground = (Brush) e.NewValue;
                }));

    #endregion

    #region CaptionForeground

    public Brush? CaptionForeground
    {
        get => _CaptionForeground;
        set
        {
            if (value != _CaptionForeground)
                SetValue(CaptionForegroundProperty, value);
        }
    }

    private Brush? _CaptionForeground;

    public static readonly DependencyProperty CaptionForegroundProperty =
        DependencyProperty.Register(nameof(CaptionForeground), typeof(Brush), typeof(BiaNumberEditor),
            new FrameworkPropertyMetadata(
                default(Brush),
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaNumberEditor) s;
                    self._CaptionForeground = (Brush) e.NewValue;
                }));

    #endregion

    #region Padding

    public Thickness Padding
    {
        get => _Padding;
        set
        {
            if (value != _Padding)
                SetValue(PaddingProperty, value);
        }
    }

    private Thickness _Padding;

    public static readonly DependencyProperty PaddingProperty =
        DependencyProperty.Register(nameof(Padding), typeof(Thickness), typeof(BiaNumberEditor),
            new FrameworkPropertyMetadata(
                Boxes.Thickness0,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaNumberEditor) s;
                    self._Padding = (Thickness) e.NewValue;
                }));

    #endregion

    #region Mode

    public BiaNumberEditorMode Mode
    {
        get => _Mode;
        set
        {
            if (value != _Mode)
                SetValue(ModeProperty, Boxes.NumberEditorMode(value));
        }
    }

    private BiaNumberEditorMode _Mode = BiaNumberEditorMode.Simple;

    public static readonly DependencyProperty ModeProperty =
        DependencyProperty.Register(nameof(Mode), typeof(BiaNumberEditorMode), typeof(BiaNumberEditor),
            new FrameworkPropertyMetadata(
                Boxes.NumberModeSimple,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaNumberEditor) s;
                    self._Mode = (BiaNumberEditorMode) e.NewValue;
                }));

    #endregion

    #region Increment

    public double Increment
    {
        get => _Increment;
        set
        {
            if (NumberHelper.AreClose(value, _Increment) == false)
                SetValue(IncrementProperty, Boxes.Double(value));
        }
    }

    private double _Increment = 1.0;

    public static readonly DependencyProperty IncrementProperty =
        DependencyProperty.Register(nameof(Increment), typeof(double), typeof(BiaNumberEditor),
            new FrameworkPropertyMetadata(
                Boxes.Double1,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaNumberEditor) s;
                    self._Increment = (double) e.NewValue;
                }));

    #endregion

    #region CornerRadius

    public double CornerRadius
    {
        get => _CornerRadius;
        set
        {
            if (NumberHelper.AreClose(value, _CornerRadius) == false)
                SetValue(CornerRadiusProperty, Boxes.Double(value));
        }
    }

    private double _CornerRadius = Constants.BasicCornerRadiusPrim;

    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(nameof(CornerRadius), typeof(double), typeof(BiaNumberEditor),
            new FrameworkPropertyMetadata(
                Boxes.BasicCornerRadiusPrim,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaNumberEditor) s;
                    self._CornerRadius = (double) e.NewValue;
                }));

    #endregion

    #region IsVisibleBorder

    public bool IsVisibleBorder
    {
        get => _IsVisibleBorder;
        set
        {
            if (value != _IsVisibleBorder)
                SetValue(IsVisibleBorderProperty, Boxes.Bool(value));
        }
    }

    private bool _IsVisibleBorder = true;

    public static readonly DependencyProperty IsVisibleBorderProperty =
        DependencyProperty.Register(nameof(IsVisibleBorder), typeof(bool), typeof(BiaNumberEditor),
            new FrameworkPropertyMetadata(
                Boxes.BoolTrue,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaNumberEditor) s;
                    self._IsVisibleBorder = (bool) e.NewValue;
                }));

    #endregion

    #region StartedContinuousEditingCommand

    public ICommand? StartedContinuousEditingCommand
    {
        get => _StartedContinuousEditingCommand;
        set
        {
            if (value != _StartedContinuousEditingCommand)
                SetValue(StartedContinuousEditingCommandProperty, value);
        }
    }

    private ICommand? _StartedContinuousEditingCommand;

    public static readonly DependencyProperty StartedContinuousEditingCommandProperty =
        DependencyProperty.Register(
            nameof(StartedContinuousEditingCommand),
            typeof(ICommand),
            typeof(BiaNumberEditor),
            new PropertyMetadata(
                default(ICommand),
                (s, e) =>
                {
                    var self = (BiaNumberEditor) s;
                    self._StartedContinuousEditingCommand = (ICommand) e.NewValue;
                }));

    #endregion

    #region EndContinuousEditingCommand

    public ICommand? EndContinuousEditingCommand
    {
        get => _EndContinuousEditingCommand;
        set
        {
            if (value != _EndContinuousEditingCommand)
                SetValue(EndContinuousEditingCommandProperty, value);
        }
    }

    private ICommand? _EndContinuousEditingCommand;

    public static readonly DependencyProperty EndContinuousEditingCommandProperty =
        DependencyProperty.Register(
            nameof(EndContinuousEditingCommand),
            typeof(ICommand),
            typeof(BiaNumberEditor),
            new PropertyMetadata(
                default(ICommand),
                (s, e) =>
                {
                    var self = (BiaNumberEditor) s;
                    self._EndContinuousEditingCommand = (ICommand) e.NewValue;
                }));

    #endregion
        
    #region TextTrimming

    public BiaTextTrimmingMode TextTrimming
    {
        get => _TextTrimming;
        set
        {
            if (value != _TextTrimming)
                SetValue(TextTrimmingProperty, Boxes.TextTrimming(value));
        }
    }

    private BiaTextTrimmingMode _TextTrimming = BiaTextTrimmingMode.Standard;

    public static readonly DependencyProperty TextTrimmingProperty =
        DependencyProperty.Register(
            nameof(TextTrimming),
            typeof(BiaTextTrimmingMode),
            typeof(BiaNumberEditor),
            new FrameworkPropertyMetadata(
                Boxes.TextTrimmingModeStandard,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaNumberEditor) s;
                    self._TextTrimming = (BiaTextTrimmingMode) e.NewValue;
                }));

    #endregion
        
    #region CaptionAlignment
        
    public TextAlignment CaptionAlignment
    {
        get => _CaptionAlignment;
        set
        {
            if (value != _CaptionAlignment)
                SetValue(CaptionAlignmentProperty, Boxes.TextAlignment(value));
        }
    }
        
    private TextAlignment _CaptionAlignment = TextAlignment.Left;
        
    public static readonly DependencyProperty CaptionAlignmentProperty =
        DependencyProperty.Register(
            nameof(CaptionAlignment),
            typeof(TextAlignment),
            typeof(BiaNumberEditor),
            new FrameworkPropertyMetadata(
                Boxes.TextAlignmentLeft,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaNumberEditor) s;
                    self._CaptionAlignment = (TextAlignment)e.NewValue;
                }));
        
    #endregion
        
    #region ValueAlignment
        
    public TextAlignment ValueAlignment
    {
        get => _ValueAlignment;
        set
        {
            if (value != _ValueAlignment)
                SetValue(CaptionAlignmentProperty, Boxes.TextAlignment(value));
        }
    }
        
    private TextAlignment _ValueAlignment = TextAlignment.Right;
        
    public static readonly DependencyProperty ValueAlignmentProperty =
        DependencyProperty.Register(
            nameof(ValueAlignment),
            typeof(TextAlignment),
            typeof(BiaNumberEditor),
            new FrameworkPropertyMetadata(
                Boxes.TextAlignmentRight,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaNumberEditor) s;
                    self._ValueAlignment = (TextAlignment)e.NewValue;
                }));
        
    #endregion

    // ReSharper disable once EventNeverSubscribedTo.Global
    public event EventHandler? ValueChanged;

    static BiaNumberEditor()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaNumberEditor),
            new FrameworkPropertyMetadata(typeof(BiaNumberEditor)));
    }

    public BiaNumberEditor()
    {
        IsEnabledChanged += (_, __) => InvalidateVisual();
    }

    protected override void OnRender(DrawingContext dc)
    {
        if (ActualWidth <= 1 ||
            ActualHeight <= 1)
            return;
            
        var rounder = new LayoutRounder(this);

        DrawBackground(rounder, dc);

        var isCornerRadiusZero = NumberHelper.AreCloseZero(CornerRadius);

        if (isCornerRadiusZero == false)
            dc.PushClip(
                Caches.GetClipGeom(rounder, ActualWidth, ActualHeight, CornerRadius, IsVisibleBorder));
        {
            if (Mode == BiaNumberEditorMode.Simple)
                DrawSlider(rounder, dc);

            DrawText(dc);

            if (IsMouseOver &&
                IsEnabled &&
                IsReadOnly == false &&
                NumberHelper.AreCloseZero(Increment) == false)
                DrawSpin(dc);
        }
        if (isCornerRadiusZero == false)
            dc.Pop();

        if (IsVisibleBorder)
            DrawBorder(rounder, dc);
    }

    private void DrawBackground(in LayoutRounder rounder, DrawingContext dc)
    {
        var brush = _isEditing
            ? _textBox?.Background
            : Background;

        if (brush is null)
            return;

        if (NumberHelper.AreCloseZero(CornerRadius))
            dc.DrawRectangle(
                brush,
                null,
                rounder.RoundRenderRectangle(IsVisibleBorder));
        else
            dc.DrawRoundedRectangle(
                brush,
                null,
                rounder.RoundRenderRectangle(IsVisibleBorder),
                CornerRadius,
                CornerRadius);
    }

    private void DrawBorder(in LayoutRounder rounder, DrawingContext dc)
    {
        if (NumberHelper.AreCloseZero(CornerRadius))
            dc.DrawRectangle(
                Brushes.Transparent,
                rounder.GetBorderPen(BorderColor),
                rounder.RoundRenderRectangle(IsVisibleBorder)
            );
        else
            dc.DrawRoundedRectangle(
                Brushes.Transparent,
                rounder.GetBorderPen(BorderColor),
                rounder.RoundRenderRectangle(IsVisibleBorder),
                CornerRadius,
                CornerRadius);
    }

    private void DrawSlider(in LayoutRounder rounder, DrawingContext dc)
    {
        if (SliderWidth <= 0.0f)
            return;

        var brush = _isEditing
            ? _textBox?.Background
            : SliderBrush;

        if (brush is null)
            return;

        var w = (UiValue - ActualSliderMinimum) * rounder.RoundRenderWidth(IsVisibleBorder) / SliderWidth;
        var r = rounder.RoundRenderRectangle(IsVisibleBorder);
        r.Width = (rounder.RoundLayoutValue(w), 0.0).Max();

        dc.DrawRectangle(brush, null, r);
    }

    private const double SpinWidth = 14.0;

    private void DrawText(DrawingContext dc)
    {
        var offsetY = (ActualHeight - Constants.BasicOneLineHeight) * 0.5;

        var textWidth = ActualWidth - Padding.Left - Padding.Right - SpinWidth * 2;
        var textX = Padding.Left + SpinWidth;
        var textY = Padding.Top + offsetY;
            
        if (Caption != null &&
            CaptionForeground != null)
            DefaultTextRenderer.Instance.Draw(
                this,
                Caption,
                textX,
                textY,
                CaptionForeground,
                dc,
                textWidth,
                CaptionAlignment,
                TextTrimming,
                false);

        if (UiValueString != null &&
            Foreground != null)
        {
            DefaultTextRenderer.Instance.Draw(
                this,
                UiValueString,
                textX,
                textY,
                Foreground,
                dc,
                textWidth,
                ValueAlignment,
                BiaTextTrimmingMode.None,
                false);
        }
    }

    private static readonly Brush? _moBrush = Application.Current.TryFindResource("AccentBrushKey") as Brush;

    private void DrawSpin(DrawingContext dc)
    {
        if (_DecSpinGeom is null)
            SetupSpinGeom();
            
        var spinBackground = Caches.GetSolidColorBrush(new ByteColor(0x40, 0x00, 0x00, 0x00));
            
        var offsetY = 8 + (ActualHeight - Constants.BasicOneLineHeight) * 0.5;

        {
            var offsetX = 5.0;

            if (_mouseOverType == MouseOverType.DecSpin)
                dc.DrawRectangle(spinBackground, null, new Rect(0, 0, SpinWidth, ActualHeight));

            var key = HashCodeMaker.Make(offsetX, offsetY);

            if (_translateTransformCache.TryGetValue(key, out var tt) == false)
            {
                tt = new TranslateTransform(offsetX, offsetY);
                _translateTransformCache.Add(key, tt);
            }

            dc.PushTransform(tt);
            dc.DrawGeometry(
                _mouseOverType == MouseOverType.DecSpin
                    ? _moBrush
                    : Foreground, null, _DecSpinGeom);
            dc.Pop();
        }

        {
            var offsetX = ActualWidth - 5.0 * 2 + 1;

            if (_mouseOverType == MouseOverType.IncSpin)
                dc.DrawRectangle(spinBackground, null,
                    new Rect(ActualWidth - SpinWidth, 0, SpinWidth, ActualHeight));

            var key = HashCodeMaker.Make(offsetX, offsetY);

            if (_translateTransformCache.TryGetValue(key, out var tt) == false)
            {
                tt = new TranslateTransform(offsetX, offsetY);
                _translateTransformCache.Add(key, tt);
            }

            dc.PushTransform(tt);
            dc.DrawGeometry(
                _mouseOverType == MouseOverType.IncSpin
                    ? _moBrush
                    : Foreground, null, _IncSpinGeom);
            dc.Pop();
        }
    }

    private enum MouseOverType
    {
        None,
        DecSpin,
        IncSpin,
        Slider
    }

    private bool _isMouseDown;
    private bool _isMouseMoved;
    private Point _oldPos;
    private Point _mouseDownPos;
    private MouseOverType _mouseOverTypeOnMouseDown;
    private MouseOverType _mouseOverType;

    private bool _isContinuousEdited;
    private double _ContinuousEditingStartValue;

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);

        _oldPos = e.GetPosition(this);
        _mouseDownPos = _oldPos;

        if (IsReadOnly)
            return;

        _isMouseDown = true;
        _isMouseMoved = false;
        _mouseOverTypeOnMouseDown = MakeMouseOverType(e);
        _isContinuousEdited = false;
        _ContinuousEditingStartValue = Value;

        if (_mouseOverTypeOnMouseDown == MouseOverType.Slider)
            CaptureMouse();

        e.Handled = true;
    }

    private const double ClickPlayWidth = 4;

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        var newMouseOverType = MakeMouseOverType(e);

        if (_mouseOverType != newMouseOverType)
        {
            _mouseOverType = newMouseOverType;
            InvalidateVisual();
        }

        if (_isMouseDown == false)
        {
            Cursor = _mouseOverType == MouseOverType.Slider && IsReadOnly == false ? Cursors.SizeWE : Cursors.Arrow;
            return;
        }

        if (IsReadOnly)
            return;

        // Down直後マウス移動ない場合でもOnMouseMoveが呼ばれることがあるため、判定する。
        var currentPos = e.GetPosition(this);
        if (currentPos == _oldPos)
            return;

        if (_isMouseMoved == false)
            if (Math.Abs(currentPos.X - _mouseDownPos.X) <= ClickPlayWidth)
                return;

        if (_mouseOverTypeOnMouseDown != MouseOverType.Slider)
            return;

        var oldValue = Value;
        var newValue = Value;

        switch (Mode)
        {
            case BiaNumberEditorMode.Simple:
                {
                    // 0から1
                    var xr = (currentPos.X, 0.0, ActualWidth).Clamp() / ActualWidth;
                    newValue = SliderWidth * xr + ActualSliderMinimum;
                    break;
                }

            case BiaNumberEditorMode.WideRange:
                {
                    // Ctrl押下中は５倍速い
                    var s = KeyboardHelper.IsPressControl
                        ? 5.0
                        : 1.0;
                    var w = currentPos.X - _oldPos.X;
                    var v = oldValue + s * w * Increment;

                    newValue = (v, ActualSliderMinimum, ActualSliderMaximum).Clamp();

                    break;
                }
        }

        if (NumberHelper.AreClose(newValue, oldValue) == false)
        {
            if (_isContinuousEdited == false)
            {
                _isContinuousEdited = true;

                StartedContinuousEditingCommand?.ExecuteIfCan(null);
            }

            Value = newValue;
        }

        _mouseOverType = MouseOverType.Slider;
        _isMouseMoved = true;
        _oldPos = currentPos;

        Cursor = _mouseOverType == MouseOverType.Slider && IsReadOnly == false ? Cursors.SizeWE : Cursors.Arrow;

        e.Handled = true;
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonUp(e);

        if (IsReadOnly == false)
        {
            if (_mouseOverTypeOnMouseDown == MouseOverType.Slider)
                ReleaseMouseCapture();
        }

        _isMouseDown = false;

        if (_isMouseMoved)
        {
            if (_isContinuousEdited)
            {
                if (EndContinuousEditingCommand != null)
                {
                    if (EndContinuousEditingCommand.CanExecute(null))
                    {
                        var changedValue = Value;
                        Value = _ContinuousEditingStartValue;

                        EndContinuousEditingCommand.Execute(null);

                        Value = changedValue;
                    }
                }

                _isContinuousEdited = false;
            }
        }
        else
        {
            var p = e.GetPosition(this);

            if (Math.Abs(p.X - _mouseDownPos.X) <= ClickPlayWidth)
            {
                // Ctrl押下中は５倍速い
                var inc = KeyboardHelper.IsPressControl
                    ? Increment * 5
                    : Increment;

                if (p.X <= SpinWidth && IsReadOnly == false)
                    AddValue(-inc);
                else if (p.X >= ActualWidth - SpinWidth && IsReadOnly == false)
                    AddValue(inc);
                else
                    ShowEditBox();
            }
        }

        _mouseOverType = MakeMouseOverType(e);

        e.Handled = true;
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);

        if (_isMouseDown)
        {
            _isMouseDown = false;
            ReleaseMouseCapture();
            this.ResetMouseClipping();
        }

        _mouseOverType = MouseOverType.None;

        e.Handled = true;
    }

    private MouseOverType MakeMouseOverType(MouseEventArgs e)
    {
        var x = e.GetPosition(this).X;

        if (x < 0)
            return MouseOverType.None;

        if (x >= ActualWidth)
            return MouseOverType.None;

        if (x <= SpinWidth)
            return MouseOverType.DecSpin;

        if (x >= ActualWidth - SpinWidth)
            return MouseOverType.IncSpin;

        return MouseOverType.Slider;
    }

    private TextBox? _textBox;
    private bool _isEditing;

    private void AddValue(double i)
    {
        var v = Value + i;

        Value = ClampValue(v);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (IsEnabled == false)
            return;

        if (e.Key == Key.Return)
        {
            ShowEditBox();
            e.Handled = true;
        }
    }

    private void ShowEditBox()
    {
        if (_isEditing)
            return;

        if (_textBox is null)
        {
            _textBox = new TextBox
            {
                IsTabStop = false,
                IsUndoEnabled = false,
                FocusVisualStyle = null,
                SelectionLength = 0
            };

            _textBox.TextChanged += TextBox_OnTextChanged;
            _textBox.LostFocus += TextBox_OnLostFocus;
            _textBox.PreviewKeyDown += TextBox_OnPreviewKeyDown;
            _textBox.PreviewMouseDown += TextBox_OnPreviewMouseDown;
        }

        _textBox.Width = ActualWidth;
        _textBox.Height = ActualHeight;
        _textBox.IsReadOnly = IsReadOnly;
        _textBox.Text = FormattedValueString;

        AddVisualChild(_textBox);
        InvalidateMeasure();

        _textBox.SelectAll();
        _textBox.Focus();
        _textBox.CaptureMouse();

        _isEditing = true;
    }

    private void TextBox_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        var rounder = new LayoutRounder(this);
            
        // 自コントロール上であれば、終了させない
        var pos = e.GetPosition(this);
        var rect = rounder.RoundRenderRectangle(false);
        if (rect.Contains(pos))
            return;

        if (_isEditing)
            FinishEditing(true);
    }

    private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        InvalidateVisual();
    }

    private void TextBox_OnLostFocus(object sender, RoutedEventArgs e)
    {
        if (_isEditing)
            FinishEditing(true);
    }

    private void TextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Tab:
                {
                    Debug.Assert(_textBox != null);

                    var v = MakeValueFromString(_textBox.Text);

                    if (v.Result == MakeValueResult.Continue)
                        _textBox.Text = v.Value.ToString(DisplayFormat);
                    else
                        FinishEditing(v.Result == MakeValueResult.Ok);

                    var t = Keyboard.Modifiers == ModifierKeys.Shift
                        ? Caches.PreviousTraversalRequest
                        : Caches.NextTraversalRequest;
                    MoveFocus(t);

                    e.Handled = true;
                    break;
                }

            case Key.Return:
                {
                    Debug.Assert(_textBox != null);

                    var v = MakeValueFromString(_textBox.Text);

                    if (v.Result == MakeValueResult.Continue)
                        _textBox.Text = v.Value.ToString(DisplayFormat);
                    else
                    {
                        FinishEditing(v.Result == MakeValueResult.Ok);
                        Dispatcher?.BeginInvoke(DispatcherPriority.Input, (Action) (() => Focus()));
                    }

                    e.Handled = true;
                    break;
                }

            case Key.Escape:
                {
                    Debug.Assert(_textBox != null);

                    _textBox.Text = FormattedValueString;
                    FinishEditing(false);

                    Dispatcher?.BeginInvoke(DispatcherPriority.Input, (Action) (() => Focus()));

                    e.Handled = true;
                    break;
                }
        }
    }

    private void FinishEditing(bool isEdit)
    {
        Debug.Assert(_textBox != null);

        if (isEdit)
        {
            var v = MakeValueFromString(_textBox.Text);
            if (v.Result == MakeValueResult.Ok ||
                v.Result == MakeValueResult.Continue)
                Value = v.Value;
        }

        _textBox.ReleaseMouseCapture();

        RemoveVisualChild(_textBox);
        _isEditing = false;
        InvalidateVisual();
    }

    protected override int VisualChildrenCount
        => _isEditing
            ? 1
            : 0;

    protected override Visual GetVisualChild(int index)
        => _textBox ?? throw new NullReferenceException();

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (_isEditing)
            _textBox?.Arrange(new Rect(new Point(0, 0), _textBox.DesiredSize));

        return base.ArrangeOverride(finalSize);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (_isEditing)
            _textBox?.Measure(new Size(ActualWidth, ActualHeight));

        // todo:DPI変更時に再描画が行われないため明示的に指示している。要調査。
        InvalidateVisual();

        return base.MeasureOverride(availableSize);
    }

    private enum MakeValueResult
    {
        Ok,
        Cancel,
        Continue
    }

    private (MakeValueResult Result, double Value) MakeValueFromString(string src)
    {
        if (double.TryParse(src, out var v))
            return (MakeValueResult.Ok, ClampValue(v));

        if (double.TryParse(Evaluator.Eval(src), out v))
            return (MakeValueResult.Continue, ClampValue(v));

        if (double.TryParse(Evaluator.Eval(src), out v))
            return (MakeValueResult.Continue, ClampValue(v));

        return (MakeValueResult.Cancel, default);
    }

    private static void SetupSpinGeom()
    {
        {
            _DecSpinGeom = new StreamGeometry();

            using (var ctx = _DecSpinGeom.Open())
            {
                ctx.DrawTriangle(
                    new Point(0, 4),
                    new Point(4, 0),
                    new Point(4, 8),
                    true,
                    true);
            }
                
            _DecSpinGeom.Freeze();
        }
            
        {
            _IncSpinGeom = new StreamGeometry();

            using (var ctx = _IncSpinGeom.Open())
            {
                ctx.DrawTriangle(
                    new Point(4, 4),
                    new Point(0, 0),
                    new Point(0, 8),
                    true,
                    true);
            }
                
            _IncSpinGeom.Freeze();
        }
    }

    private double ClampValue(double v)
        => (v, ActualMinimum, ActualMaximum).Clamp();

    private string FormattedValueString => Value.ToString(DisplayFormat);

    private double SliderWidth => Math.Abs(SliderMaximum - SliderMinimum);

    private double ActualSliderMinimum => (SliderMinimum, SliderMaximum).Min();

    private double ActualSliderMaximum => (SliderMinimum, SliderMaximum).Max();

    private double ActualMinimum => (Minimum, Maximum).Min();

    private double ActualMaximum => (Minimum, Maximum).Max();

    private string? UiValueString
    {
        get
        {
            if (_isEditing == false)
                return Concat(FormattedValueString, UnitString);

            var v = MakeValueFromString(_textBox?.Text ?? "");

            return
                v.Result == MakeValueResult.Ok || v.Result == MakeValueResult.Continue
                    ? Concat(v.Value.ToString(DisplayFormat), UnitString)
                    : Concat(FormattedValueString, UnitString);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string Concat(string a, string b)
        => string.IsNullOrEmpty(b)
            ? a
            : a + b;

    private double UiValue
    {
        get
        {
            if (_isEditing == false)
                return Value;

            var v = MakeValueFromString(_textBox?.Text ?? "");

            return
                v.Result == MakeValueResult.Ok || v.Result == MakeValueResult.Continue
                    ? v.Value
                    : Value;
        }
    }

    private static StreamGeometry? _DecSpinGeom;
    private static StreamGeometry? _IncSpinGeom;

    private static readonly Dictionary<long, TranslateTransform> _translateTransformCache = new Dictionary<long, TranslateTransform>();
}