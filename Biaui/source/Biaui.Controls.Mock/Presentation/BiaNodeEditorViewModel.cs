using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using Biaui.Controls.Mock.Foundation.Interface;
using Biaui.Controls.Mock.Foundation.Mvvm;
using Biaui.NodeEditor;

namespace Biaui.Controls.Mock.Presentation
{
    public class BiaNodeEditorViewModel : ViewModelBase
    {
        #region Nodes

        private ObservableCollection<INodeItem> _Nodes = new ObservableCollection<INodeItem>();

        public ObservableCollection<INodeItem> Nodes
        {
            get => _Nodes;
            set => SetProperty(ref _Nodes, value);
        }

        #endregion

        #region Scale

        private double _Scale = 1;

        public double Scale
        {
            get => _Scale;
            set => SetProperty(ref _Scale, value);
        }

        #endregion

        public BiaNodeEditorViewModel(IDisposableChecker disposableChecker) : base(disposableChecker)
        {
            var r = new Random();

            var i = 0;

            var titleColors = new[]
            {
                Colors.Purple,
                Colors.SeaGreen,
                Colors.Firebrick,
                Colors.DarkSlateGray,
                Colors.DeepPink
            };

            for (var y = 0; y != 100; ++y)
            for (var x = 0; x != 100; ++x)
            {
                var rx = r.NextDouble() * 1024;
                var ry = r.NextDouble() * 1024;

                Nodes.Add(
                    new Node
                    {
                        Name = $"Name:{i++}",
                        TitleColor = titleColors[x % titleColors.Length],
                        Pos = new Point(x * 800 + rx, y * 800 + ry),
                        Size = new Size(32*8, 32*13)
                    });
            }
        }
    }

    public class Node : ModelBase, INodeItem
    {
        #region Name
        
        private string _Name;
        
        public string Name
        {
            get => _Name;
            set => SetProperty(ref _Name, value);
        }
        
        #endregion

        #region IsSelected

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        #endregion

        #region IsPreSelected

        private bool _IsPreSelected;

        public bool IsPreSelected
        {
            get => _IsPreSelected;
            set => SetProperty(ref _IsPreSelected, value);
        }

        #endregion

        #region TitleColor
        
        private Color _TitleColor;
        
        public Color TitleColor
        {
            get => _TitleColor;
            set => SetProperty(ref _TitleColor, value);
        }
        
        #endregion

        #region Pos

        private Point _Pos;

        public Point Pos
        {
            get => _Pos;
            set => SetProperty(ref _Pos, value);
        }

        #endregion

        #region Size

        private Size _Size;

        public Size Size
        {
            get => _Size;
            set => SetProperty(ref _Size, value);
        }

        #endregion
    }
}