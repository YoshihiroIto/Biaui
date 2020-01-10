using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaTextBox : FrameworkElement
    {
        #region Text

        public string? Text
        {
            get => _Text;
            set
            {
                if (value != _Text)
                    SetValue(TextProperty, value);
            }
        }

        private string? _Text;

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(BiaTextBox),
                new FrameworkPropertyMetadata(
                    default(string),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaTextBox) s;
                        self._Text = (string) e.NewValue;
                    }));

        #endregion

        #region Watermark

        public string? Watermark
        {
            get => _Watermark;
            set
            {
                if (value != _Watermark)
                    SetValue(WatermarkProperty, value);
            }
        }

        private string? _Watermark;

        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register(nameof(Watermark), typeof(string), typeof(BiaTextBox),
                new FrameworkPropertyMetadata(
                    default(string),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaTextBox) s;
                        self._Watermark = (string) e.NewValue;
                    }));

        #endregion

        #region WatermarkForeground

        public Brush? WatermarkForeground
        {
            get => _WatermarkForeground;
            set
            {
                if (value != _WatermarkForeground)
                    SetValue(WatermarkForegroundProperty, value);
            }
        }

        private Brush? _WatermarkForeground;

        public static readonly DependencyProperty WatermarkForegroundProperty =
            DependencyProperty.Register(nameof(WatermarkForeground), typeof(Brush), typeof(BiaTextBox),
                new FrameworkPropertyMetadata(
                    default(Brush),
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaTextBox) s;
                        self._WatermarkForeground = (Brush) e.NewValue;
                    }));

        #endregion

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
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(BiaTextBox),
                new FrameworkPropertyMetadata(
                    Boxes.BoolFalse,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaTextBox) s;
                        self._IsReadOnly = (bool) e.NewValue;
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
            DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(BiaTextBox),
                new FrameworkPropertyMetadata(
                    default(Brush),
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaTextBox) s;
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
            DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(BiaTextBox),
                new FrameworkPropertyMetadata(
                    default(Brush),
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaTextBox) s;
                        self._Foreground = (Brush) e.NewValue;
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

        private Color _BorderColor = Colors.Red;

        public static readonly DependencyProperty BorderColorProperty =
            DependencyProperty.Register(nameof(BorderColor), typeof(Color), typeof(BiaTextBox),
                new FrameworkPropertyMetadata(
                    Boxes.ColorRed,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaTextBox) s;
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
            DependencyProperty.Register(nameof(CornerRadius), typeof(double), typeof(BiaTextBox),
                new FrameworkPropertyMetadata(
                    Boxes.Double0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaTextBox) s;
                        self._CornerRadius = (double) e.NewValue;
                    }));

        #endregion

        static BiaTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaTextBox),
                new FrameworkPropertyMetadata(typeof(BiaTextBox)));
        }

        public BiaTextBox()
        {
            IsEnabledChanged += (_, __) => InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;

            DrawBackground(dc);

            var isCornerRadiusZero = NumberHelper.AreCloseZero(CornerRadius);

            if (isCornerRadiusZero == false)
                dc.PushClip(Caches.GetClipGeom(this, ActualWidth, ActualHeight, CornerRadius, true));
            {
                if (_isEditing == false &&
                    string.IsNullOrEmpty(TargetText) &&
                    Watermark != null &&
                    WatermarkForeground != null)
                    TextRenderer.Default.Draw(
                        this,
                        Watermark,
                        4.5, 3.5,
                        WatermarkForeground,
                        dc,
                        (1.0, ActualWidth - 9).Max(),
                        TextAlignment.Left
                    );

                if (TargetText != null &&
                    Foreground != null)
                    TextRenderer.Default.Draw(
                        this,
                        TargetText,
                        4.5, 3.5,
                        Foreground,
                        dc,
                        (1.0, ActualWidth - 9).Max(),
                        TextAlignment.Left
                    );
            }
            if (isCornerRadiusZero == false)
                dc.Pop();
        }

        private string? TargetText => _isEditing ? _textBox?.Text : Text;

        private void DrawBackground(DrawingContext dc)
        {
            var brush = Background;

            if (NumberHelper.AreCloseZero(CornerRadius))
                dc.DrawRectangle(
                    brush,
                    this.GetBorderPen(BorderColor),
                    this.RoundLayoutRenderRectangle(true));
            else
                dc.DrawRoundedRectangle(
                    brush,
                    this.GetBorderPen(BorderColor),
                    this.RoundLayoutRenderRectangle(true),
                    CornerRadius,
                    CornerRadius);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (IsEnabled == false)
                return;

            if (_isEditing)
            {
                if (this.IsInActualSize(e.GetPosition(this)) == false)
                {
                    FinishEditing(true);
                    ReleaseMouseCapture();

                    Win32Helper.mouse_event(Win32Helper.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                }
            }
            else
            {
                Focus();
                ShowEditBox();
            }

            e.Handled = true;
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

        private TextBox? _textBox;
        private bool _isEditing;

        private void ShowEditBox()
        {
            if (_textBox == null)
            {
                _textBox = new TextBox
                {
                    IsTabStop = false,
                    IsUndoEnabled = false,
                    FocusVisualStyle = null,
                    SelectionLength = 0,
                };

                _textBox.TextChanged += TextBox_OnTextChanged;
                _textBox.PreviewKeyDown += TextBox_OnPreviewKeyDown;
                _textBox.PreviewMouseDown += TextBox_OnPreviewMouseDown;
            }

            _textBox.Width = ActualWidth;
            _textBox.Height = ActualHeight;
            _textBox.IsReadOnly = IsReadOnly;
            _textBox.Text = Text;

            AddVisualChild(_textBox);
            InvalidateMeasure();

            _textBox.SelectAll();
            _textBox.Focus();
            _textBox.CaptureMouse();

            _isEditing = true;
        }

        private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            InvalidateVisual();
        }

        private void TextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Tab:
                {
                    FinishEditing(true);

                    var t = Keyboard.Modifiers == ModifierKeys.Shift
                        ? Caches.PreviousTraversalRequest
                        : Caches.NextTraversalRequest;
                    MoveFocus(t);

                    e.Handled = true;

                    break;
                }

                case Key.Return:
                {
                    FinishEditing(true);

                    Dispatcher?.BeginInvoke(DispatcherPriority.Input, (Action) (() => Focus()));

                    e.Handled = true;
                    break;
                }

                case Key.Escape:
                {
                    FinishEditing(false);

                    Dispatcher?.BeginInvoke(DispatcherPriority.Input, (Action) (() => Focus()));

                    e.Handled = true;
                    break;
                }
            }
        }

        private void TextBox_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // 自コントロール上であれば、終了させない
            var pos = e.GetPosition(this);
            var rect = this.RoundLayoutRenderRectangle(false);
            if (rect.Contains(pos))
                return;

            if (_isEditing)
                FinishEditing(true);
        }

        private void FinishEditing(bool isEdit)
        {
            if (isEdit)
            {
                Text = _textBox?.Text;
                GetBindingExpression(TextProperty)?.UpdateSource();
            }

            RemoveVisualChild(_textBox);
            _isEditing = false;
            InvalidateVisual();

            if (IsMouseCaptured)
                ReleaseMouseCapture();
        }

        protected override int VisualChildrenCount
            => _isEditing ? 1 : 0;

        protected override Visual? GetVisualChild(int index)
            => _textBox;

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

            return base.MeasureOverride(availableSize);
        }
    }
}