using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaTextBox : FrameworkElement
    {
        #region Text

        public string Text
        {
            get => _Text;
            set
            {
                if (value != _Text)
                    SetValue(TextProperty, value);
            }
        }

        private string _Text = default(string);

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(BiaTextBox),
                new PropertyMetadata(
                    default(string),
                    (s, e) =>
                    {
                        var self = (BiaTextBox) s;
                        self._Text = (string) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        #region IsReadOnly

        public bool IsReadOnly
        {
            get => _IsReadOnly;
            set
            {
                if (value != _IsReadOnly)
                    SetValue(IsReadOnlyProperty, value);
            }
        }

        private bool _IsReadOnly = default(bool);

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(BiaTextBox),
                new PropertyMetadata(
                    Boxes.BoolFalse,
                    (s, e) =>
                    {
                        var self = (BiaTextBox) s;
                        self._IsReadOnly = (bool) e.NewValue;
                        self.InvalidateVisual();
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
            DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(BiaTextBox),
                new PropertyMetadata(
                    default(Brush),
                    (s, e) =>
                    {
                        var self = (BiaTextBox) s;
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
            DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(BiaTextBox),
                new PropertyMetadata(
                    default(Brush),
                    (s, e) =>
                    {
                        var self = (BiaTextBox) s;
                        self._Foreground = (Brush) e.NewValue;
                        self.InvalidateVisual();
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
                new PropertyMetadata(
                    Boxes.ColorRed,
                    (s, e) =>
                    {
                        var self = (BiaTextBox) s;
                        self._BorderColor = (Color) e.NewValue;
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
            DependencyProperty.Register(nameof(CornerRadius), typeof(double), typeof(BiaTextBox),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaTextBox) s;
                        self._CornerRadius = (double) e.NewValue;
                        self.InvalidateVisual();
                    }));

        #endregion

        static BiaTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaTextBox),
                new FrameworkPropertyMetadata(typeof(BiaTextBox)));
        }

        public BiaTextBox()
        {
            Unloaded += (_, __) =>
            {
                if (_popup != null)
                {
                    _popup.Closed -= PopupOnClosed;
                    _popup.PreviewKeyDown -= PopupOnPreviewKeyDown;
                    _textBox.TextChanged -= TextBoxOnTextChanged;
                }
            };
        }

        protected override void OnRender(DrawingContext dc)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;
            // ReSharper restore CompareOfFloatsByEqualityOperator

            var rect = new Rect(0.5, 0.5, ActualWidth - 1, ActualHeight - 1);
            dc.PushGuidelineSet(Caches.GetGuidelineSet(rect, BorderWidth));
            {
                DrawBackground(dc);

                dc.PushClip(Caches.GetClipGeom(ActualWidth - 2, ActualHeight, CornerRadius));
                {
                    TextRenderer.Default.Draw(
                        _isInPopup ? _textBox.Text : Text,
                        4.5, 3.5,
                        Foreground,
                        dc,
                        ActualWidth,
                        TextAlignment.Left
                    );
                }
                dc.Pop();
            }
            dc.Pop();
        }

        private const double BorderWidth = 1.0;

        private void DrawBackground(DrawingContext dc)
        {
            var brush = _isInPopup ? _textBox.Background : Background;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (CornerRadius == 0)
                dc.DrawRectangle(
                    brush,
                    Caches.GetBorderPen(BorderColor, BorderWidth),
                    ActualRectangle);
            else
                dc.DrawRoundedRectangle(
                    brush,
                    Caches.GetBorderPen(BorderColor, BorderWidth),
                    ActualRectangle,
                    CornerRadius,
                    CornerRadius);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (IsEnabled == false)
                return;

            Focus();
            ShowEditBox();
            InvalidateVisual();

            Dispatcher.BeginInvoke(DispatcherPriority.Input, (Action) MouseSimulator.DownLeftMouseButton);
        }

#if false
// todo:ポップアップが閉じない問題を修正
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            ShowEditBox();
        }
#endif

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (IsEnabled == false)
                return;

            if (e.Key == Key.Return)
                ShowEditBox();
        }

        private TextBox _textBox;
        private Popup _popup;
        private bool _isInPopup;

        private void ShowEditBox()
        {
            if (_textBox == null)
            {
                _textBox = new TextBox
                {
                    IsTabStop = false,
                    IsUndoEnabled = false,
                    FocusVisualStyle = null
                };

                _popup = new Popup
                {
                    Child = _textBox,
                    AllowsTransparency = true,
                    StaysOpen = false,
                };

                _textBox.TextChanged += TextBoxOnTextChanged;
                _popup.Closed += PopupOnClosed;
                _popup.PreviewKeyDown += PopupOnPreviewKeyDown;
            }

            _textBox.Width = ActualWidth;
            _textBox.Height = ActualHeight;
            _textBox.IsReadOnly = IsReadOnly;
            _textBox.Text = Text;
            _textBox.SelectionLength = 0;

            _popup.PlacementTarget = this;
            _popup.VerticalOffset = -ActualHeight;

            _popup.IsOpen = true;
            _isInPopup = true;
            InvalidateVisual();

            Win32Helper.SetFocus(WpfHelper.GetHwnd(_popup));
            _textBox.Focus();
        }

        private void PopupOnClosed(object sender, EventArgs e)
        {
            Text = _textBox.Text;

            _isInPopup = false;
            InvalidateVisual();
        }

        private void TextBoxOnTextChanged(object sender, TextChangedEventArgs e)
        {
            InvalidateVisual();
        }

        private void PopupOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Tab:
                    Dispatcher.BeginInvoke(DispatcherPriority.Input, (Action) MoveFocus);
                    break;

                case Key.Return:
                    Dispatcher.BeginInvoke(DispatcherPriority.Input, (Action) ClosePopup);
                    break;
            }

            void ClosePopup()
            {
                Text = _textBox.Text;
                ((Popup) sender).IsOpen = false;
            }

            void MoveFocus()
            {
                Text = _textBox.Text;
                ((Popup) sender).IsOpen = false;

                var dir = Keyboard.Modifiers == ModifierKeys.Shift
                    ? FocusNavigationDirection.Previous
                    : FocusNavigationDirection.Next;

                this.MoveFocus(new TraversalRequest(dir));
            }
        }

        private Rect ActualRectangle => new Rect(new Size(ActualWidth, ActualHeight));
    }
}