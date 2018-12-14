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

        public BiaNodeEditorViewModel(IDisposableChecker disposableChecker) : base(disposableChecker)
        {
            var r = new Random();

            var i = 0;

            var titleBackgrounds = new[]
            {
                Brushes.Purple,
                Brushes.SeaGreen,
                Brushes.Firebrick,
                Brushes.DarkSlateGray,
                Brushes.DeepPink
            };

            for (var y = 0; y != 100; ++y)
            for (var x = 0; x != 100; ++x)
            {
                var rx = r.NextDouble() * 1024;
                var ry = r.NextDouble() * 1024;

                if ((x & 1) == 0)
                {
                    Nodes.Add(
                        new BasicNode
                        {
                            Title = $"Title:{i++}",
                            TitleBackground = titleBackgrounds[x % titleBackgrounds.Length],
                            Pos = new Point(x * 800 + rx, y * 800 + ry),
                        });
                }
                else
                {
                    Nodes.Add(
                        new ColorNode
                        {
                            Title = $"Color:{i++}",
                            TitleBackground = titleBackgrounds[x % titleBackgrounds.Length],
                            Pos = new Point(x * 800 + rx, y * 800 + ry),
                        });
                }
            }
        }
    }

    public class NodeBase : ModelBase, INodeItem
    {
        #region Title

        private string _Name;

        public string Title
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

        #region TitleBackground

        private Brush _TitleBackground;

        public Brush TitleBackground
        {
            get => _TitleBackground;
            set => SetProperty(ref _TitleBackground, value);
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

    public class BasicNode : NodeBase
    {
    }

    public class ColorNode : NodeBase
    {
        #region Red
        
        private double _Red;
        
        public double Red
        {
            get => _Red;
            set => SetProperty(ref _Red, value);
        }
        
        #endregion
        
        #region Green
        
        private double _Green;
        
        public double Green
        {
            get => _Green;
            set => SetProperty(ref _Green, value);
        }
        
        #endregion
        
        #region Blue
        
        private double _Blue;
        
        public double Blue
        {
            get => _Blue;
            set => SetProperty(ref _Blue, value);
        }
        
        #endregion
    }
}