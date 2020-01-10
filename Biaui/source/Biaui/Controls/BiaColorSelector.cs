using System;
using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaColorSelector : FrameworkElement
    {
        #region Value

        public DoubleColor Value
        {
            get => _Value;
            set
            {
                if (value.Equals(_Value) == false)
                    SetValue(ValueProperty, value);
            }
        }

        private DoubleColor _Value = DoubleColor.Zero;

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(DoubleColor), typeof(BiaColorSelector),
                new FrameworkPropertyMetadata(
                    DoubleColor.Zero,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaColorSelector) s;
                        self._Value = (DoubleColor) e.NewValue;

                        self._background = new SolidColorBrush(self._Value.Color);
                        self._background.Freeze();
                    }));

        #endregion

        #region BorderColor

        public Color BorderColor
        {
            get => _BorderColor;
            set
            {
                if (value != _BorderColor)
                    SetValue(BorderColorProperty, value);
            }
        }

        private Color _BorderColor = Colors.Transparent;

        public static readonly DependencyProperty BorderColorProperty =
            DependencyProperty.Register(nameof(BorderColor), typeof(Color), typeof(BiaColorSelector),
                new FrameworkPropertyMetadata(
                    Boxes.ColorTransparent,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaColorSelector) s;
                        self._BorderColor = (Color) e.NewValue;
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
            DependencyProperty.Register(nameof(CornerRadius), typeof(double), typeof(BiaColorSelector),
                new FrameworkPropertyMetadata(
                    Boxes.Double0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaColorSelector) s;
                        self._CornerRadius = (double) e.NewValue;
                    }));

        #endregion

        #region Choices

        public IEnumerable? Choices
        {
            get => _Choices;
            set
            {
                if (!Equals(value, _Choices))
                    SetValue(ChoicesProperty, value);
            }
        }

        private IEnumerable? _Choices;

        public static readonly DependencyProperty ChoicesProperty =
            DependencyProperty.Register(
                nameof(Choices),
                typeof(IEnumerable),
                typeof(BiaColorSelector),
                new PropertyMetadata(
                    default,
                    (s, e) =>
                    {
                        var self = (BiaColorSelector) s;
                        self._Choices = (IEnumerable) e.NewValue;
                    }));

        #endregion

        #region Columns

        public int Columns
        {
            get => _Columns;
            set
            {
                if (value != _Columns)
                    SetValue(ColumnsProperty, Boxes.Int(value));
            }
        }

        private int _Columns = 4;

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register(
                nameof(Columns),
                typeof(int),
                typeof(BiaColorSelector),
                new PropertyMetadata(
                    Boxes.Int4,
                    (s, e) =>
                    {
                        var self = (BiaColorSelector) s;
                        self._Columns = (int) e.NewValue;
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
                typeof(BiaColorSelector),
                new PropertyMetadata(
                    default(ICommand),
                    (s, e) =>
                    {
                        var self = (BiaColorSelector) s;
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
                typeof(BiaColorSelector),
                new PropertyMetadata(
                    default(ICommand),
                    (s, e) =>
                    {
                        var self = (BiaColorSelector) s;
                        self._EndContinuousEditingCommand = (ICommand) e.NewValue;
                    }));

        #endregion

        static BiaColorSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaColorSelector),
                new FrameworkPropertyMetadata(typeof(BiaColorSelector)));
        }

        private SolidColorBrush _background = Brushes.Transparent;

        protected override void OnRender(DrawingContext dc)
        {
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;

            var rect = this.RoundLayoutRenderRectangle(true);

            var borderPen = this.GetBorderPen(BorderColor);

            if (IsEnabled)
            {
                if (NumberHelper.AreCloseZero(CornerRadius))
                {
                    if (_background != null && _background.Color.A != 0xFF)
                        dc.DrawRectangle(Constants.CheckerBrush, null, rect);

                    dc.DrawRectangle(_background, borderPen, rect);
                }
                else
                {
                    if (_background != null && _background.Color.A != 0xFF)
                        dc.DrawRoundedRectangle(Constants.CheckerBrush, null, rect, CornerRadius, CornerRadius);

                    dc.DrawRoundedRectangle(_background, borderPen, rect, CornerRadius, CornerRadius);
                }
            }
            else
            {
                if (NumberHelper.AreCloseZero(CornerRadius))
                    dc.DrawRectangle(null, borderPen, rect);
                else
                    dc.DrawRoundedRectangle(null, borderPen, rect, CornerRadius, CornerRadius);
            }
        }

        private ListBox? _items;
        private Popup? _popup;
        private ScaleTransform? _scale;

        // ReSharper disable once ConvertToNullCoalescingCompoundAssignment
        private Action FocusThis => _FocusThis ?? (_FocusThis = () => Focus());
        private Action? _FocusThis;

        private bool IsOpen => _popup != null && _popup.IsOpen;

        private void ShowPopup()
        {
            if (_popup == null)
            {
                _items = new ListBox
                {
                    IsTabStop = false,
                    FocusVisualStyle = null,
                    Margin = new Thickness(0, 0, 3, 3),
                    Effect = new DropShadowEffect
                    {
                        ShadowDepth = 2,
                        Color = Colors.Black
                    }
                };

                _scale = new ScaleTransform();

                _popup = new Popup
                {
                    Child = _items,
                    AllowsTransparency = true,
                    StaysOpen = false,
                    Focusable = false,
                    RenderTransform = _scale,
                    PlacementTarget = this,
                    Placement = PlacementMode.Left,
                    HorizontalOffset = 2,
                    VerticalOffset = -4
                };

                _items.SetBinding(ItemsControl.ItemsSourceProperty,
                    new Binding(nameof(Choices))
                    {
                        Source = this,
                        Mode = BindingMode.TwoWay
                    });

                _items.SetBinding(Selector.SelectedItemProperty,
                    new Binding(nameof(Value))
                    {
                        Source = this,
                        Mode = BindingMode.TwoWay
                    });

                _items.PreviewKeyDown += ListBoxOnPreviewKeyDown;
                _items.PreviewMouseLeftButtonDown += ListBoxOnPreviewMouseLeftButtonDown;
            }

            Debug.Assert(_scale != null);
            Debug.Assert(_items != null);

            Mouse.Capture(this, CaptureMode.SubTree);

            var s = this.CalcCompositeRenderScale();
            _scale.ScaleX = s;
            _scale.ScaleY = s;

            _popup.IsOpen = true;

            if (_items.SelectedItem == null)
            {
                _items.Focus();
            }
            else
            {
                var item = _items.ItemContainerGenerator.ContainerFromItem(_items.SelectedItem) as ListBoxItem;
                item?.Focus();
            }

            StartedContinuousEditingCommand?.ExecuteIfCan(null);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (IsEnabled == false)
                return;

            if (IsOpen)
            {
                Discard();
                Dispatcher?.BeginInvoke(DispatcherPriority.Input, FocusThis);
            }
            else
            {
                Focus();

                _ContinuousEditingStartValue = Value;
                ShowPopup();
            }

            e.Handled = true;
        }

        private void ListBoxOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (IsEnabled == false)
                return;

            if (IsOpen == false)
                return;

            if (e.Key == Key.Escape)
            {
                Discard();
                Focus();
            }
        }

        private void ListBoxOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Debug.Assert(_popup != null);

            var lb = (ListBox) sender;

            var pos = Mouse.GetPosition(lb);
            var hit = VisualTreeHelper.HitTest(lb, pos);

            if (!(hit?.VisualHit is Border))
            {
                return;
            }

            SetValue();

            _popup.IsOpen = false;
            Dispatcher?.BeginInvoke(DispatcherPriority.Input, FocusThis);
        }

        private DoubleColor _ContinuousEditingStartValue;

        private void SetValue()
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
        }

        private void Discard()
        {
            Debug.Assert(_popup != null);

            var done = false;

            if (EndContinuousEditingCommand != null)
            {
                if (EndContinuousEditingCommand.CanExecute(null))
                {
                    Value = _ContinuousEditingStartValue;

                    EndContinuousEditingCommand.Execute(null);

                    done = true;
                }
            }

            if (done == false)
                Value = _ContinuousEditingStartValue;

            Mouse.Capture(null);
            _popup.IsOpen = false;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            // todo:DPI変更時に再描画が行われないため明示的に指示している。要調査。
            InvalidateVisual();

            return new Size(ActualWidth, ActualHeight);
        }
    }
}