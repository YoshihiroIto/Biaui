using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaButton : FrameworkElement
    {
        public static readonly RoutedEvent ClickEvent =
            EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(BiaButton));

        #region Caption

        public string Caption
        {
            get => _Caption;
            set
            {
                if (value != _Caption)
                    SetValue(CaptionProperty, value);
            }
        }

        private string _Caption = default(string);

        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register(nameof(Caption), typeof(string), typeof(BiaButton),
                new PropertyMetadata(
                    default(string),
                    (s, e) =>
                    {
                        var self = (BiaButton) s;
                        self._Caption = (string) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        #region Command

        public ICommand Command
        {
            get => _Command;
            set
            {
                if (value != _Command)
                    SetValue(CommandProperty, value);
            }
        }

        private ICommand _Command = default(ICommand);

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(BiaButton),
                new PropertyMetadata(
                    default(ICommand),
                    (s, e) =>
                    {
                        var self = (BiaButton) s;
                        self._Command = (ICommand) e.NewValue;
                    }));

        #endregion

        #region CommandParameter

        public object CommandParameter
        {
            get => _CommandParameter;
            set
            {
                if (value != _CommandParameter)
                    SetValue(CommandParameterProperty, value);
            }
        }

        private object _CommandParameter = default(object);

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(BiaButton),
                new PropertyMetadata(
                    default(object),
                    (s, e) =>
                    {
                        var self = (BiaButton) s;
                        self._CommandParameter = e.NewValue;
                    }));

        #endregion

        #region Background

        public Brush Background
        {
            get => _Background;
            set
            {
                if (value != _Background)
                    SetValue(BackgroundProperty, value);
            }
        }

        private Brush _Background = default(Brush);

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(BiaButton),
                new PropertyMetadata(
                    default(Brush),
                    (s, e) =>
                    {
                        var self = (BiaButton) s;
                        self._Background = (Brush) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        #region Foreground

        public Brush Foreground
        {
            get => _Foreground;
            set
            {
                if (value != _Foreground)
                    SetValue(ForegroundProperty, value);
            }
        }

        private Brush _Foreground = default(Brush);

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(BiaButton),
                new PropertyMetadata(
                    default(Brush),
                    (s, e) =>
                    {
                        var self = (BiaButton) s;
                        self._Foreground = (Brush) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        #region CornerRadius

        public double CornerRadius
        {
            get => _CornerRadius;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value != _CornerRadius)
                    SetValue(CornerRadiusProperty, value);
            }
        }

        private double _CornerRadius = default(double);

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(double), typeof(BiaButton),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaButton) s;
                        self._CornerRadius = (double) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        #region IsPressed

        public bool IsPressed
        {
            get => _IsPressed;
            set
            {
                if (value != _IsPressed)
                    SetValue(IsPressedProperty, value);
            }
        }

        private bool _IsPressed = default(bool);

        public static readonly DependencyProperty IsPressedProperty =
            DependencyProperty.Register(nameof(IsPressed), typeof(bool), typeof(BiaButton),
                new PropertyMetadata(
                    Boxes.BoolFalse,
                    (s, e) =>
                    {
                        var self = (BiaButton) s;
                        self._IsPressed = (bool) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        #region IsPressedMouseOver

        public bool IsPressedMouseOver
        {
            get => _isPressedMouseOver;
            set
            {
                if (value != _isPressedMouseOver)
                    SetValue(IsPressedMouseOverProperty, value);
            }
        }

        private bool _isPressedMouseOver = default(bool);

        public static readonly DependencyProperty IsPressedMouseOverProperty =
            DependencyProperty.Register(nameof(IsPressedMouseOver), typeof(bool), typeof(BiaButton),
                new PropertyMetadata(
                    Boxes.BoolFalse,
                    (s, e) =>
                    {
                        var self = (BiaButton) s;
                        self._isPressedMouseOver = (bool) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        protected override void OnRender(DrawingContext dc)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;
            // ReSharper restore CompareOfFloatsByEqualityOperator

            var rect = new Rect(0.5, 0.5, ActualWidth - 1, ActualHeight - 1);
            dc.PushGuidelineSet(Caches.GetGuidelineSet(rect, 0));
            {
                // 背景
                {
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (CornerRadius == 0)
                        dc.DrawRectangle(
                            Background,
                            null,
                            ActualRectangle, null);
                    else
                        dc.DrawRoundedRectangle(
                            Background,
                            null,
                            ActualRectangle, null,
                            CornerRadius, null,
                            CornerRadius, null);
                }

                // キャプション
                var y = 4; // todo:正しく求める
                TextRenderer.Default.Draw(Caption, 0, y, Foreground, dc, ActualWidth, TextAlignment.Center);
            }
            dc.Pop();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            IsPressed = IsInMouse(e);

            CaptureMouse();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            IsPressedMouseOver = IsInMouse(e);
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

            RaiseEvent(new RoutedEventArgs(ClickEvent, this));

            if (Command != null &&
                Command.CanExecute(CommandParameter) == false)
                Command.Execute(CommandParameter);
        }

        private bool IsInMouse(MouseEventArgs e)
        {
            var pos = e.GetPosition(this);

            return
                pos.X >= 0.0 &&
                pos.X <= ActualWidth &&
                pos.Y >= 0.0 &&
                pos.Y <= ActualHeight;
        }

        private Rect ActualRectangle => new Rect(new Size(ActualWidth, ActualHeight));
    }
}