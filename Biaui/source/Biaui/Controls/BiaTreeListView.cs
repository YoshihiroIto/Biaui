using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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