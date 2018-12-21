using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaComboBox : FrameworkElement
    {
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

        private Brush _Background;

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(BiaComboBox),
                new FrameworkPropertyMetadata(
                    default(Brush),
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaComboBox) s;
                        self._Background = (Brush) e.NewValue;
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

        private Brush _Foreground;

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(BiaComboBox),
                new FrameworkPropertyMetadata(
                    default(Brush),
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaComboBox) s;
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
            DependencyProperty.Register(nameof(BorderColor), typeof(Color), typeof(BiaComboBox),
                new FrameworkPropertyMetadata(
                    Boxes.ColorRed,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaComboBox) s;
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
                    SetValue(CornerRadiusProperty, value);
            }
        }

        private double _CornerRadius;

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(double), typeof(BiaComboBox),
                new FrameworkPropertyMetadata(
                    Boxes.Double0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaComboBox) s;
                        self._CornerRadius = (double) e.NewValue;
                    }));

        #endregion

        #region ItemsSource

        public IEnumerable ItemsSource
        {
            get => _ItemsSource;
            set
            {
                // ReSharper disable once PossibleUnintendedReferenceComparison
                if (value != _ItemsSource)
                    SetValue(ItemsSourceProperty, value);
            }
        }

        private IEnumerable _ItemsSource;

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(BiaComboBox),
                new FrameworkPropertyMetadata(
                    default(IEnumerable),
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaComboBox) s;
                        self._ItemsSource = (IEnumerable) e.NewValue;
                    }));

        #endregion

        #region SelectedItem

        public object SelectedItem
        {
            get => _SelectedItem;
            set
            {
                if (value != _SelectedItem)
                    SetValue(SelectedItemProperty, value);
            }
        }

        private object _SelectedItem;

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(BiaComboBox),
                new FrameworkPropertyMetadata(
                    default,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaComboBox) s;
                        self._SelectedItem = e.NewValue;
                    }));

        #endregion

        #region MarkBrush

        public Brush MarkBrush
        {
            get => _MarkBrush;
            set
            {
                if (value != _MarkBrush)
                    SetValue(MarkBrushProperty, value);
            }
        }

        private Brush _MarkBrush;

        public static readonly DependencyProperty MarkBrushProperty =
            DependencyProperty.Register(nameof(MarkBrush), typeof(Brush), typeof(BiaComboBox),
                new FrameworkPropertyMetadata(
                    default(Brush),
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaComboBox) s;
                        self._MarkBrush = (Brush) e.NewValue;
                    }));

        #endregion

        #region IsOpen

        public bool IsOpen
        {
            get => _IsOpen;
            set
            {
                if (value != _IsOpen)
                    SetValue(IsOpenProperty, Boxes.Bool(value));
            }
        }

        private bool _IsOpen;

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(BiaComboBox),
                new PropertyMetadata(
                    Boxes.BoolFalse,
                    (s, e) =>
                    {
                        var self = (BiaComboBox) s;
                        self._IsOpen = (bool) e.NewValue;
                    }));

        #endregion

        #region ItemToStringConverter

        public IValueConverter ItemToStringConverter
        {
            get => _ItemToStringConverter;
            set
            {
                if (value != _ItemToStringConverter)
                    SetValue(ItemToStringConverterProperty, value);
            }
        }

        private IValueConverter _ItemToStringConverter;

        public static readonly DependencyProperty ItemToStringConverterProperty =
            DependencyProperty.Register(nameof(ItemToStringConverter), typeof(IValueConverter), typeof(BiaComboBox),
                new PropertyMetadata(
                    default(IValueConverter),
                    (s, e) =>
                    {
                        var self = (BiaComboBox) s;
                        self._ItemToStringConverter = (IValueConverter) e.NewValue;
                        self._isReqUpdateListBoxItemTemplate = true;
                    }));

        private bool _isReqUpdateListBoxItemTemplate = true;

        #endregion

        #region ItemToStringConverterParameter

        public object ItemToStringConverterParameter
        {
            get => _ItemToStringConverterParameter;
            set
            {
                if (value != _ItemToStringConverterParameter)
                    SetValue(ItemToStringConverterParameterProperty, value);
            }
        }

        private object _ItemToStringConverterParameter;

        public static readonly DependencyProperty ItemToStringConverterParameterProperty =
            DependencyProperty.Register(nameof(ItemToStringConverterParameter), typeof(object), typeof(BiaComboBox),
                new PropertyMetadata(
                    default,
                    (s, e) =>
                    {
                        var self = (BiaComboBox) s;
                        self._ItemToStringConverterParameter = e.NewValue;
                        self._isReqUpdateListBoxItemTemplate = true;
                    }));

        #endregion

        static BiaComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaComboBox),
                new FrameworkPropertyMetadata(typeof(BiaComboBox)));
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;

            DrawBackground(dc);

            var isCornerRadiusZero = NumberHelper.AreCloseZero(CornerRadius);

            if (isCornerRadiusZero == false)
                dc.PushClip(
                    Caches.GetClipGeom(ActualWidth, ActualHeight, CornerRadius, true));
            {
                var displayItem = ItemToStringConverter?.Convert(SelectedItem, typeof(string),
                                      ItemToStringConverterParameter, CultureInfo.CurrentUICulture)
                                  ?? SelectedItem;

                if (displayItem != null)
                    TextRenderer.Default.Draw(
                        displayItem.ToString(),
                        4.5, 3.5,
                        Foreground,
                        dc,
                        ActualWidth,
                        TextAlignment.Left
                    );
            }
            if (isCornerRadiusZero == false)
                dc.Pop();

            // マーク
            {
                var offset = new TranslateTransform(ActualWidth - SystemParameters.VerticalScrollBarWidth, 10.5);

                dc.PushTransform(offset);
                dc.DrawGeometry(MarkBrush, null, _markGeom);
                dc.Pop();
            }
        }

        private void DrawBackground(DrawingContext dc)
        {
            if (NumberHelper.AreCloseZero(CornerRadius))
                dc.DrawRectangle(
                    Background,
                    this.GetBorderPen(BorderColor),
                    this.RoundLayoutActualRectangle(true));
            else
                dc.DrawRoundedRectangle(
                    Background,
                    this.GetBorderPen(BorderColor),
                    this.RoundLayoutActualRectangle(true),
                    CornerRadius,
                    CornerRadius);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (IsEnabled == false)
                return;

            if (IsOpen)
            {
                Mouse.Capture(null);
                _popup.IsOpen = false;
                Dispatcher.BeginInvoke(DispatcherPriority.Input, FocusThis);
            }
            else
            {
                Focus();
                ShowListBox();
            }

            e.Handled = true;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (IsEnabled == false)
                return;

            if (e.Key == Key.Up)
                MoveSelectedItem(-1);

            else if (e.Key == Key.Down)
                MoveSelectedItem(+1);

            Dispatcher.BeginInvoke(DispatcherPriority.Input, FocusThis);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (IsEnabled == false)
                return;

            if (e.Delta > 0)
                MoveSelectedItem(-1);

            else if (e.Delta < 0)
                MoveSelectedItem(+1);

            Dispatcher.BeginInvoke(DispatcherPriority.Input, FocusThis);
        }

        private ListBox _listBox;
        private Popup _popup;
        private ScaleTransform _scale;

        private void ShowListBox()
        {
            if (_popup == null)
            {
                _listBox = new ListBox
                {
                    IsTabStop = false,
                    FocusVisualStyle = null,
                };

                _scale = new ScaleTransform();

                _popup = new Popup
                {
                    Child = _listBox,
                    AllowsTransparency = true,
                    VerticalOffset = 2,
                    StaysOpen = false,
                    Focusable = false,
                    RenderTransform = _scale,
                    PlacementTarget = this
                };

                _listBox.SetBinding(ItemsControl.ItemsSourceProperty,
                    new Binding(nameof(ItemsSource)) {Source = this, Mode = BindingMode.TwoWay});
                _listBox.SetBinding(Selector.SelectedItemProperty,
                    new Binding(nameof(SelectedItem)) {Source = this, Mode = BindingMode.TwoWay});
                _listBox.PreviewKeyDown += ListBoxOnPreviewKeyDown;
                _listBox.PreviewMouseLeftButtonDown += ListBoxOnPreviewMouseLeftButtonDown;

                _popup.Closed += PopupOnClosed;
            }

            if (_isReqUpdateListBoxItemTemplate)
            {
                _isReqUpdateListBoxItemTemplate = false;
                SetupListBoxItemTemplate();
            }

            _listBox.Width = ActualWidth;

            Mouse.Capture(this, CaptureMode.SubTree);

            var s = this.CalcCompositeRenderScale();
            _scale.ScaleX = s;
            _scale.ScaleY = s;

            _popup.IsOpen = true;
            IsOpen = true;

            if (_listBox.SelectedItem == null)
            {
                _listBox.Focus();
            }
            else
            {
                var item = _listBox.ItemContainerGenerator.ContainerFromItem(_listBox.SelectedItem) as ListBoxItem;
                item?.Focus();

                // 選択アイテム位置にスクロール
                {
                    var sv = _listBox.Descendants<ScrollViewer>().FirstOrDefault();

                    if (sv != null)
                    {
                        var offset = (double) _listBox.Items.IndexOf(_listBox.SelectedItem);

                        if (offset > sv.ScrollableHeight)
                            offset = sv.ScrollableHeight;

                        sv.ScrollToVerticalOffset(offset);
                    }
                }
            }
        }

        private void SetupListBoxItemTemplate()
        {
            var itemTemplate = new DataTemplate();
            {
                var textBlock = new FrameworkElementFactory(typeof(BiaTextBlock));

                textBlock.SetBinding(
                    BiaTextBlock.TextProperty,
                    new Binding
                    {
                        Converter = ItemToStringConverter,
                        ConverterParameter = ItemToStringConverterParameter,
                        ConverterCulture = CultureInfo.CurrentUICulture,
                    });

                itemTemplate.VisualTree = textBlock;
            }
            _listBox.ItemTemplate = itemTemplate;
        }

        private void ListBoxOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var lb = (ListBox) sender;

            var pos = Mouse.GetPosition(lb);
            var hit = VisualTreeHelper.HitTest(lb, pos);

            var parent = (hit?.VisualHit as FrameworkElement)?.TemplatedParent;

            while (parent != null)
            {
                if (parent.GetType() == typeof(ScrollBar))
                    return;

                parent = (parent as FrameworkElement)?.TemplatedParent;
            }

            _popup.IsOpen = false;
            Dispatcher.BeginInvoke(DispatcherPriority.Input, FocusThis);
        }

        private Action FocusThis => () => Focus();

        private void ListBoxOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (IsEnabled == false)
                return;

            if (IsOpen == false)
                return;

            if (e.Key == Key.Return || e.Key == Key.Escape)
            {
                _popup.IsOpen = false;
                Focus();
            }
        }

        private void PopupOnClosed(object sender, EventArgs e)
        {
            IsOpen = false;
        }

        private void MoveSelectedItem(int dir)
        {
            if (ItemsSource == null)
                return;

            var ia = ItemsSource.Cast<object>().ToArray();
            var i = Array.IndexOf(ia, SelectedItem);

            if (i == -1)
            {
                if (ia.Length > 0)
                {
                    var item = _listBox.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
                    item?.Focus();

                    SelectedItem = ia[0];
                }
            }
            else
            {
                i = i + dir;
                i = (i, 0, ia.Length - 1).Clamp();

                SelectedItem = ia[i];
            }
        }

        private static readonly Geometry _markGeom = Geometry.Parse("M 0.0,0.0 L 3.5,4.0 7.0,0.0 z");
    }
}