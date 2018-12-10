using System;
using System.Collections.ObjectModel;
using System.Windows;
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


            for (var y = 0; y != 100; ++y)
            for (var x = 0; x != 100; ++x)
            {
                var rx = r.NextDouble() * 800;
                var ry = r.NextDouble() * 800;

                Nodes.Add(
                    new Node
                    {
                        Pos = new Point(x * 800 + rx, y * 800 + ry),
                        Size = new Size(200, 400)
                    });
            }
        }
    }

    public class Node : ModelBase, INodeItem
    {
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