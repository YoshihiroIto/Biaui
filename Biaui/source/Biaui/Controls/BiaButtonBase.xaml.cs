using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls;

public class BiaButtonBase : FrameworkElement
{
    public static readonly RoutedEvent ClickEvent =
        EventManager.RegisterRoutedEvent(nameof(Click), RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(BiaButtonBase));

    public event RoutedEventHandler Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }

    #region Command

    public ICommand? Command
    {
        get => _Command;
        set
        {
            if (value != _Command)
                SetValue(CommandProperty, value);
        }
    }

    private ICommand? _Command;

    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(BiaButtonBase),
            new PropertyMetadata(
                default(ICommand),
                (s, e) =>
                {
                    var self = (BiaButtonBase) s;

                    if (self._Command is not null)
                        self._Command.CanExecuteChanged -= self.CommandOnCanExecuteChanged;

                    self._Command = (ICommand) e.NewValue;

                    if (self._Command is not null)
                        self._Command.CanExecuteChanged += self.CommandOnCanExecuteChanged;

                    self.CommandOnCanExecuteChanged(null, EventArgs.Empty);
                }));

    private void CommandOnCanExecuteChanged(object? sender, EventArgs e)
    {
        if (Command is null)
            return;

        IsEnabled = Command.CanExecute(CommandParameter);
    }

    #endregion

    #region CommandParameter

    public object? CommandParameter
    {
        get => _CommandParameter;
        set
        {
            if (value != _CommandParameter)
                SetValue(CommandParameterProperty, value);
        }
    }

    private object? _CommandParameter;

    public static readonly DependencyProperty CommandParameterProperty =
        DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(BiaButtonBase),
            new PropertyMetadata(
                default,
                (s, e) =>
                {
                    var self = (BiaButtonBase) s;
                    self._CommandParameter = e.NewValue;
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
        DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(BiaButtonBase),
            new FrameworkPropertyMetadata(
                default(Brush),
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaButtonBase) s;
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
        DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(BiaButtonBase),
            new FrameworkPropertyMetadata(
                default(Brush),
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaButtonBase) s;
                    self._Foreground = (Brush) e.NewValue;
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

    private double _CornerRadius;

    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(nameof(CornerRadius), typeof(double), typeof(BiaButtonBase),
            new FrameworkPropertyMetadata(
                Boxes.Double0,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaButtonBase) s;
                    self._CornerRadius = (double) e.NewValue;
                }));

    #endregion

    #region IsPressed

    public bool IsPressed
    {
        get => _IsPressed;
        set
        {
            if (value != _IsPressed)
                SetValue(IsPressedProperty, Boxes.Bool(value));
        }
    }

    private bool _IsPressed;

    public static readonly DependencyProperty IsPressedProperty =
        DependencyProperty.Register(nameof(IsPressed), typeof(bool), typeof(BiaButtonBase),
            new FrameworkPropertyMetadata(
                Boxes.BoolFalse,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaButtonBase) s;
                    self._IsPressed = (bool) e.NewValue;
                }));

    #endregion

    #region IsPressedMouseOver

    public bool IsPressedMouseOver
    {
        get => _isPressedMouseOver;
        set
        {
            if (value != _isPressedMouseOver)
                SetValue(IsPressedMouseOverProperty, Boxes.Bool(value));
        }
    }

    private bool _isPressedMouseOver;

    public static readonly DependencyProperty IsPressedMouseOverProperty =
        DependencyProperty.Register(nameof(IsPressedMouseOver), typeof(bool), typeof(BiaButtonBase),
            new FrameworkPropertyMetadata(
                Boxes.BoolFalse,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaButtonBase) s;
                    self._isPressedMouseOver = (bool) e.NewValue;
                }));

    #endregion

    static BiaButtonBase()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaButtonBase),
            new FrameworkPropertyMetadata(typeof(BiaButtonBase)));
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);

        IsPressed = IsInMouse(e);

        CaptureMouse();

        e.Handled = true;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        IsPressedMouseOver = IsInMouse(e);

        e.Handled = true;
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonUp(e);

        if (IsPressed == false)
            return;

        if (IsMouseCaptured)
            ReleaseMouseCapture();

        IsPressed = false;

        if (IsInMouse(e) == false)
            return;

        Clicked();

        e.Handled = true;
    }

    private bool IsInMouse(MouseEventArgs e)
    {
        var pos = e.GetPosition(this);

        return
            pos.X >= 0d &&
            pos.X <= ActualWidth &&
            pos.Y >= 0d &&
            pos.Y <= ActualHeight;
    }

    protected virtual void Clicked()
    {
        InvokeClicked();
    }

    protected virtual void InvokeClicked()
    {
        RaiseEvent(new RoutedEventArgs(ClickEvent, this));

        Command?.ExecuteIfCan(CommandParameter);
    }
}