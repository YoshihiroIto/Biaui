using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaTreeListView : Control
    {
        #region ItemsSource
        
        public IEnumerable ItemsSource
        {
            get => _ItemsSource;
            set
            {
                if (!Equals(value, _ItemsSource))
                    SetValue(ItemsSourceProperty, value);
            }
        }
        
        private IEnumerable _ItemsSource;
        
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(BiaTreeListView),
                new PropertyMetadata(
                    default,
                    (s, e) =>
                    {
                        var self = (BiaTreeListView) s;
                        self._ItemsSource = (IEnumerable)e.NewValue;
                    }));
        
        #endregion

        #region Columns
        
        public GridViewColumnCollection Columns
        {
            get => _Columns;
            set
            {
                if (value != _Columns)
                    SetValue(ColumnsProperty, value);
            }
        }
        
        private GridViewColumnCollection _Columns;
        
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register(
                nameof(Columns),
                typeof(GridViewColumnCollection),
                typeof(BiaTreeListView),
                new PropertyMetadata(
                    default,
                    (s, e) =>
                    {
                        var self = (BiaTreeListView) s;
                        self._Columns = (GridViewColumnCollection)e.NewValue;
                    }));
        
        #endregion

        #region ItemTemplate
        
        public DataTemplate ItemTemplate
        {
            get => _ItemTemplate;
            set
            {
                if (value != _ItemTemplate)
                    SetValue(ItemTemplateProperty, value);
            }
        }
        
        private DataTemplate _ItemTemplate;
        
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(
                nameof(ItemTemplate),
                typeof(DataTemplate),
                typeof(BiaTreeListView),
                new PropertyMetadata(
                    default,
                    (s, e) =>
                    {
                        var self = (BiaTreeListView) s;
                        self._ItemTemplate = (DataTemplate)e.NewValue;
                    }));
        
        #endregion

        #region ItemContainerStyle
        
        public Style ItemContainerStyle
        {
            get => _ItemContainerStyle;
            set
            {
                if (value != _ItemContainerStyle)
                    SetValue(ItemContainerStyleProperty, value);
            }
        }
        
        private Style _ItemContainerStyle;
        
        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register(
                nameof(ItemContainerStyle),
                typeof(Style),
                typeof(BiaTreeListView),
                new PropertyMetadata(
                    default,
                    (s, e) =>
                    {
                        var self = (BiaTreeListView) s;
                        self._ItemContainerStyle = (Style)e.NewValue;
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
            DependencyProperty.Register(
                nameof(SelectedItem),
                typeof(object),
                typeof(BiaTreeListView),
                new PropertyMetadata(
                    default,
                    (s, e) =>
                    {
                        var self = (BiaTreeListView) s;
                        self._SelectedItem = e.NewValue;
                    }));
        
        #endregion

        #region SelectedItems
        
        public IList SelectedItems
        {
            get => _SelectedItems;
            set
            {
                if (!Equals(value, _SelectedItems))
                    SetValue(SelectedItemsProperty, value);
            }
        }
        
        private IList _SelectedItems;
        
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(
                nameof(SelectedItems),
                typeof(IList),
                typeof(BiaTreeListView),
                new PropertyMetadata(
                    default,
                    (s, e) =>
                    {
                        var self = (BiaTreeListView) s;
                        self._SelectedItems = (IList)e.NewValue;
                    }));
        
        #endregion

        #region AlternationCount
        
        public int AlternationCount
        {
            get => _AlternationCount;
            set
            {
                if (value != _AlternationCount)
                    SetValue(AlternationCountProperty, value);
            }
        }
        
        private int _AlternationCount;
        
        public static readonly DependencyProperty AlternationCountProperty =
            DependencyProperty.Register(
                nameof(AlternationCount),
                typeof(int),
                typeof(BiaTreeListView),
                new PropertyMetadata(
                    Boxes.Int0,
                    (s, e) =>
                    {
                        var self = (BiaTreeListView) s;
                        self._AlternationCount = (int)e.NewValue;
                    }));
        
        #endregion

        static BiaTreeListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaTreeListView),
                new FrameworkPropertyMetadata(typeof(BiaTreeListView)));
        }

        public BiaTreeListView()
        {
            Loaded += (_, __) =>
            {
                var treeView = this.Descendants<BiaTreeView>().First();
                var treeViewSv = treeView.Descendants<ScrollViewer>().First();
                var headerSv = this.Descendants<ScrollViewer>().First(x => x.Name == "HeaderScrollViewer");

                treeViewSv.ScrollChanged += (___, e) =>
                {
                    headerSv.Width = e.ViewportWidth;
                    headerSv.ScrollToHorizontalOffset(e.HorizontalOffset);
                };
            };
        }
    }
}