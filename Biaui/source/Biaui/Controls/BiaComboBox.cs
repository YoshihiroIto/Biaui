﻿using System;
using System.Collections;
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

        private bool _IsReadOnly = default(bool);

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(BiaComboBox),
                new PropertyMetadata(
                    Boxes.BoolFalse,
                    (s, e) =>
                    {
                        var self = (BiaComboBox) s;
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
            DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(BiaComboBox),
                new PropertyMetadata(
                    default(Brush),
                    (s, e) =>
                    {
                        var self = (BiaComboBox) s;
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
            DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(BiaComboBox),
                new PropertyMetadata(
                    default(Brush),
                    (s, e) =>
                    {
                        var self = (BiaComboBox) s;
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
            DependencyProperty.Register(nameof(BorderColor), typeof(Color), typeof(BiaComboBox),
                new PropertyMetadata(
                    Boxes.ColorRed,
                    (s, e) =>
                    {
                        var self = (BiaComboBox) s;
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
            DependencyProperty.Register(nameof(CornerRadius), typeof(double), typeof(BiaComboBox),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaComboBox) s;
                        self._CornerRadius = (double) e.NewValue;
                        self.InvalidateVisual();
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

        private IEnumerable _ItemsSource = default(IEnumerable);

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(BiaComboBox),
                new PropertyMetadata(
                    default(IEnumerable),
                    (s, e) =>
                    {
                        var self = (BiaComboBox) s;
                        self._ItemsSource = (IEnumerable) e.NewValue;
                        self.InvalidateVisual();
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

        private object _SelectedItem = default(object);

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(BiaComboBox),
                new FrameworkPropertyMetadata(
                    default(object),
                    (s, e) =>
                    {
                        var self = (BiaComboBox) s;
                        self._SelectedItem = e.NewValue;
                        self.InvalidateVisual();
                    })
                {
                    BindsTwoWayByDefault = true,
                }
            );

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

        private Brush _MarkBrush = default(Brush);

        public static readonly DependencyProperty MarkBrushProperty =
            DependencyProperty.Register(nameof(MarkBrush), typeof(Brush), typeof(BiaComboBox),
                new PropertyMetadata(
                    default(Brush),
                    (s, e) =>
                    {
                        var self = (BiaComboBox) s;
                        self._MarkBrush = (Brush) e.NewValue;
                        self.InvalidateVisual();
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

        private bool _IsOpen = default(bool);

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

        static BiaComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaComboBox),
                new FrameworkPropertyMetadata(typeof(BiaComboBox)));
        }

        public BiaComboBox()
        {
            Unloaded += (_, __) =>
            {
                if (_popup != null)
                {
                    _listBox.PreviewKeyDown -= ListBoxOnPreviewKeyDown;
                    _listBox.PreviewMouseLeftButtonDown -= ListBoxOnPreviewMouseLeftButtonDown;
                    _popup.Closed -= PopupOnClosed;
                }
            };
        }

        private const double BorderWidth = 1.0;

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

                if (SelectedItem != null)
                {
                    dc.PushClip(Caches.GetClipGeom(ActualWidth - 2, ActualHeight, CornerRadius));
                    {
                        TextRenderer.Default.Draw(
                            SelectedItem.ToString(),
                            4.5, 3.5,
                            Foreground,
                            dc,
                            ActualWidth,
                            TextAlignment.Left
                        );
                    }
                    dc.Pop();
                }

                // マーク
                {
                    var offset = new TranslateTransform(ActualWidth - SystemParameters.VerticalScrollBarWidth, 10.5);

                    dc.PushTransform(offset);
                    dc.DrawGeometry(MarkBrush, null, _markGeom);
                    dc.Pop();
                }
            }
            dc.Pop();
        }

        private void DrawBackground(DrawingContext dc)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (CornerRadius == 0)
                dc.DrawRectangle(
                    Background,
                    Caches.GetBorderPen(BorderColor, BorderWidth),
                    ActualRectangle);
            else
                dc.DrawRoundedRectangle(
                    Background,
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

            if (IsOpen)
            {
                Mouse.Capture(null);
                _popup.IsOpen = false;
                Dispatcher.BeginInvoke(DispatcherPriority.Input, FocusThis);
                InvalidateVisual();
            }
            else
            {
                Focus();
                ShowListBox();
                InvalidateVisual();
            }
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

        private void ShowListBox()
        {
            if (_popup == null)
            {
                _listBox = new ListBox
                {
                    IsTabStop = false,
                    FocusVisualStyle = null,
                };

                _popup = new Popup
                {
                    Child = _listBox,
                    AllowsTransparency = true,
                    VerticalOffset = 2,
                    StaysOpen = false,
                    Focusable = false,
                };

                _listBox.SetBinding(ItemsControl.ItemsSourceProperty,
                    new Binding(nameof(ItemsSource)) {Source = this, Mode = BindingMode.TwoWay});
                _listBox.SetBinding(Selector.SelectedItemProperty,
                    new Binding(nameof(SelectedItem)) {Source = this, Mode = BindingMode.TwoWay});
                _listBox.PreviewKeyDown += ListBoxOnPreviewKeyDown;
                _listBox.PreviewMouseLeftButtonDown += ListBoxOnPreviewMouseLeftButtonDown;

                _popup.PlacementTarget = this;
                _popup.Closed += PopupOnClosed;
            }

            _listBox.Width = ActualWidth;

            Mouse.Capture(this, CaptureMode.SubTree);

            _popup.IsOpen = true;
            IsOpen = true;
            InvalidateVisual();

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
            InvalidateVisual();
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
                i = Math.Max(0, Math.Min(i, ia.Length - 1));

                SelectedItem = ia[i];
            }
        }

        private Rect ActualRectangle => new Rect(new Size(ActualWidth, ActualHeight));

        private static readonly Geometry _markGeom = Geometry.Parse("M 0.0,0.0 L 3.5,4.0 7.0,0.0 z");
    }
}