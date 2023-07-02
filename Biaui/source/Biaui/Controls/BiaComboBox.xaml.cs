using System;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using Biaui.Internals;
using Jewelry.Memory;

namespace Biaui.Controls;

public class BiaComboBox : FrameworkElement
{
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
        DependencyProperty.Register(nameof(BorderColor), typeof(ByteColor), typeof(BiaComboBox),
            new FrameworkPropertyMetadata(
                Boxes.ByteColorRed,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaComboBox) s;
                    self._BorderColor = (ByteColor) e.NewValue;
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

    public IEnumerable? ItemsSource
    {
        get => _ItemsSource;
        set
        {
            // ReSharper disable once PossibleUnintendedReferenceComparison
            if (value != _ItemsSource)
                SetValue(ItemsSourceProperty, value);
        }
    }

    private IEnumerable? _ItemsSource;

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

    public object? SelectedItem
    {
        get => _SelectedItem;
        set
        {
            if (value != _SelectedItem)
                SetValue(SelectedItemProperty, value);
        }
    }

    private object? _SelectedItem;

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

    public Brush? MarkBrush
    {
        get => _MarkBrush;
        set
        {
            if (value != _MarkBrush)
                SetValue(MarkBrushProperty, value);
        }
    }

    private Brush? _MarkBrush;

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

    public IValueConverter? ItemToStringConverter
    {
        get => _ItemToStringConverter;
        set
        {
            if (value != _ItemToStringConverter)
                SetValue(ItemToStringConverterProperty, value);
        }
    }

    private IValueConverter? _ItemToStringConverter;

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

    public object? ItemToStringConverterParameter
    {
        get => _ItemToStringConverterParameter;
        set
        {
            if (value != _ItemToStringConverterParameter)
                SetValue(ItemToStringConverterParameterProperty, value);
        }
    }

    private object? _ItemToStringConverterParameter;

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
            typeof(BiaComboBox),
            new PropertyMetadata(
                default(ICommand),
                (s, e) =>
                {
                    var self = (BiaComboBox) s;
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
            typeof(BiaComboBox),
            new PropertyMetadata(
                default(ICommand),
                (s, e) =>
                {
                    var self = (BiaComboBox) s;
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
            typeof(BiaComboBox),
            new FrameworkPropertyMetadata(
                Boxes.TextTrimmingModeStandard,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                (s, e) =>
                {
                    var self = (BiaComboBox) s;
                    self._TextTrimming = (BiaTextTrimmingMode) e.NewValue;
                }));

    #endregion

    static BiaComboBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaComboBox),
            new FrameworkPropertyMetadata(typeof(BiaComboBox)));
    }

    protected override void OnRender(DrawingContext dc)
    {
        if (ActualWidth <= 1d ||
            ActualHeight <= 1d)
            return;

        var rounder = new LayoutRounder(this);

        DrawBackground(rounder, dc);

        var isCornerRadiusZero = NumberHelper.AreCloseZero(CornerRadius);

        if (isCornerRadiusZero == false)
            dc.PushClip(
                Caches.GetClipGeom(rounder, ActualWidth, ActualHeight, CornerRadius, true));
        {
            var displayItem = ItemToStringConverter?.Convert(SelectedItem, typeof(string),
                                  ItemToStringConverterParameter, CultureInfo.CurrentUICulture)
                              ?? SelectedItem;

            if (displayItem != null)
                if (Foreground != null)
                    DefaultTextRenderer.Instance.Draw(
                        this,
                        displayItem.ToString(),
                        5d, 4d,
                        Foreground,
                        dc,
                        (1d, ActualWidth - 24d).Max(), // ▼分の幅を引く
                        TextAlignment.Left,
                        TextTrimming,
                        true);
        }
        if (isCornerRadiusZero == false)
            dc.Pop();

        // マーク
        {
            var offset = new TranslateTransform(ActualWidth - 15d, 10.5d);

            dc.PushTransform(offset);
            dc.DrawGeometry(MarkBrush, null, MarkGeom);
            dc.Pop();
        }
    }

    private void DrawBackground(in LayoutRounder rounder, DrawingContext dc)
    {
        if (NumberHelper.AreCloseZero(CornerRadius))
            dc.DrawRectangle(
                Background,
                rounder.GetBorderPen(BorderColor),
                rounder.RoundRenderRectangle(true));
        else
            dc.DrawRoundedRectangle(
                Background,
                rounder.GetBorderPen(BorderColor),
                rounder.RoundRenderRectangle(true),
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
            Discard();
            Dispatcher?.BeginInvoke(DispatcherPriority.Input, FocusThis);
        }
        else
        {
            Focus();

            _ContinuousEditingStartValue = SelectedItem;
            ShowPopup();
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

        Dispatcher?.BeginInvoke(DispatcherPriority.Input, FocusThis);

        e.Handled = true;
    }

    private bool _isDoesFindParent;
    private bool _hasScrollViewerParent;

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        if (IsEnabled == false)
            return;

        if (_isDoesFindParent == false)
        {
            _hasScrollViewerParent = this.GetParent<ScrollViewer>() != null;
            _isDoesFindParent = true;
        }

        // スクロール操作と混同するため、
        // 親にスクロールビューワーが存在すればホイール操作を無効にする
        if (_hasScrollViewerParent)
            return;

        if (e.Delta > 0)
            MoveSelectedItem(-1);

        else if (e.Delta < 0)
            MoveSelectedItem(+1);

        Dispatcher?.BeginInvoke(DispatcherPriority.Input, FocusThis);
    }

    private ListBox? _items;
    private Popup? _popup;
    private ScaleTransform? _scale;

    private void ShowPopup()
    {
        if (_popup is null)
        {
            _items = new ListBox
            {
                IsTabStop = false,
                FocusVisualStyle = null,
                Margin = new Thickness(0d, 0d, 3d, 3d),
                Effect = new DropShadowEffect
                {
                    ShadowDepth = 2d,
                    Color = Colors.Black
                }
            };

            _scale = new ScaleTransform();

            _popup = new Popup
            {
                Child = _items,
                AllowsTransparency = true,
                VerticalOffset = 1,
                StaysOpen = false,
                Focusable = false,
                RenderTransform = _scale,
                HorizontalOffset = -2d,
                PlacementTarget = this
            };

            _items.SetBinding(ItemsControl.ItemsSourceProperty,
                new Binding(nameof(ItemsSource))
                {
                    Source = this,
                    Mode = BindingMode.TwoWay
                });
            _items.SetBinding(Selector.SelectedItemProperty,
                new Binding(nameof(SelectedItem))
                {
                    Source = this,
                    Mode = BindingMode.TwoWay
                });
            _items.PreviewKeyDown += ListBoxOnPreviewKeyDown;
            _items.PreviewMouseLeftButtonDown += ListBoxOnPreviewMouseLeftButtonDown;

            _popup.Closed += PopupOnClosed;
        }

        Debug.Assert(_items is not null);
        Debug.Assert(_scale is not null);

        if (_isReqUpdateListBoxItemTemplate)
        {
            _isReqUpdateListBoxItemTemplate = false;
            SetupListBoxItemTemplate();
        }

        _items.Width = ActualWidth;

        Mouse.Capture(this, CaptureMode.SubTree);

        var s = this.CalcCompositeRenderScale();
        _scale.ScaleX = s;
        _scale.ScaleY = s;

        _popup.IsOpen = true;
        IsOpen = true;

        if (_items.SelectedItem is null)
        {
            _items.Focus();
        }
        else
        {
            var item = _items.ItemContainerGenerator.ContainerFromItem(_items.SelectedItem) as ListBoxItem;
            item?.Focus();

            // 選択アイテム位置にスクロール
            {
                var sv = _items.Descendants().OfType<ScrollViewer>().FirstOrDefault();

                if (sv != null)
                {
                    var offset = (double) _items.Items.IndexOf(_items.SelectedItem);

                    if (offset > sv.ScrollableHeight)
                        offset = sv.ScrollableHeight;

                    sv.ScrollToVerticalOffset(offset);
                }
            }
        }

        var listBoxBorder = _items.Descendants().OfType<Border>().First();
        listBoxBorder.BorderBrush = (Brush) TryFindResource("Item.SelectedActive.Border");

        StartedContinuousEditingCommand?.ExecuteIfCan(null);
    }

    private void SetupListBoxItemTemplate()
    {
        Debug.Assert(_items != null);

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

        _items.ItemTemplate = itemTemplate;
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

        SetValue();

        Dispatcher?.BeginInvoke(DispatcherPriority.Input, FocusThis);
    }

    // ReSharper disable once ConvertToNullCoalescingCompoundAssignment
    private Action FocusThis => _FocusThis ?? (_FocusThis = () => Focus());

    private Action? _FocusThis;

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

        if (e.Key == Key.Return)
        {
            SetValue();
            Focus();
        }
    }

    private void PopupOnClosed(object? sender, EventArgs e)
    {
        IsOpen = false;
    }

    private object? _ContinuousEditingStartValue;

    private void SetValue()
    {
        Debug.Assert(_popup != null);

        if (EndContinuousEditingCommand != null)
        {
            if (EndContinuousEditingCommand.CanExecute(null))
            {
                var changedValue = SelectedItem;
                SelectedItem = _ContinuousEditingStartValue;

                EndContinuousEditingCommand.Execute(null);

                SelectedItem = changedValue;
            }
        }

        Mouse.Capture(null);
        _popup.IsOpen = false;
    }

    private void Discard()
    {
        Debug.Assert(_popup != null);

        var done = false;

        if (EndContinuousEditingCommand != null)
        {
            if (EndContinuousEditingCommand.CanExecute(null))
            {
                SelectedItem = _ContinuousEditingStartValue;

                EndContinuousEditingCommand.Execute(null);

                done = true;
            }
        }

        if (done == false)
            SelectedItem = _ContinuousEditingStartValue;

        Mouse.Capture(null);
        _popup.IsOpen = false;
    }

    [SuppressMessage("ReSharper", "PossiblyImpureMethodCallOnReadonlyVariable")]
    private void MoveSelectedItem(int dir)
    {
        if (ItemsSource is null)
            return;

        using var tempItems = new TempBuffer<object>(128);

        IList? items = null;

        int itemsCount;
        int selectedIndex;
        {
            if (ItemsSource is IList list)
            {
                items = list;
                itemsCount = list.Count;
                selectedIndex = list.IndexOf(SelectedItem);
            }
            else
            {
                tempItems.AddFrom(ItemsSource);
                itemsCount = tempItems.Length;
                selectedIndex = tempItems.IndexOf(SelectedItem!);
            }
        }

        if (selectedIndex == -1)
        {
            if (itemsCount > 0)
            {
                var item = _items?.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
                item?.Focus();

                SelectedItem = items != null
                    ? items[0]
                    : tempItems[0];
            }
        }
        else
        {
            selectedIndex += dir;
            selectedIndex = (selectedIndex, 0, itemsCount - 1).Clamp();

            SelectedItem = items != null
                ? items[selectedIndex]
                : tempItems[selectedIndex];
        }
    }

    private static readonly Geometry MarkGeom = Geometry.Parse("M 0.0,0.0 L 3.5,4.0 7.0,0.0 z");
}