using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Biaui.Controls.Mock.Foundation.Interface;
using Biaui.Controls.Mock.Foundation.Mvvm;
using Biaui.Interfaces;

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

        #region AddNodeCommand

        private ICommand _addNodeCommand;

        public ICommand AddNodeCommand
        {
            get => _addNodeCommand;
            set => SetProperty(ref _addNodeCommand, value);
        }

        #endregion

        #region RemoveSelectedNodesCommand

        private ICommand _RemoveSelectedNodesCommand;

        public ICommand RemoveSelectedNodesCommand
        {
            get => _RemoveSelectedNodesCommand;
            set => SetProperty(ref _RemoveSelectedNodesCommand, value);
        }

        #endregion

        #region ClearNodesCommand

        private ICommand _ClearNodesCommand;

        public ICommand ClearNodesCommand
        {
            get => _ClearNodesCommand;
            set => SetProperty(ref _ClearNodesCommand, value);
        }

        #endregion

        #region ReplaceLastNodeCommand

        private ICommand _ReplaceLastNodeCommand;

        public ICommand ReplaceLastNodeCommand
        {
            get => _ReplaceLastNodeCommand;
            set => SetProperty(ref _ReplaceLastNodeCommand, value);
        }

        #endregion

        #region ReplaceNodesSourceCommand

        private ICommand _replaceNodesSourceCommand;

        public ICommand ReplaceNodesSourceCommand
        {
            get => _replaceNodesSourceCommand;
            set => SetProperty(ref _replaceNodesSourceCommand, value);
        }

        #endregion

        public BiaNodeEditorViewModel(IDisposableChecker disposableChecker) : base(disposableChecker)
        {
            var addCount = 0;

            AddNodeCommand = new DelegateCommand().Setup(() =>
            {
                Nodes.Add(
                    new BasicNode
                    {
                        Title = $"Add:{addCount++}",
                        TitleBackground = Brushes.RoyalBlue,
                        Pos = new Point(0, 0),
                    });
            });

            RemoveSelectedNodesCommand = new DelegateCommand().Setup(() =>
            {
                foreach (var node in Nodes.Where(x => x.IsSelected).ToArray())
                    Nodes.Remove(node);
            });

            ClearNodesCommand = new DelegateCommand().Setup(() => { Nodes.Clear(); });

            var replaceCount = 0;

            ReplaceLastNodeCommand = new DelegateCommand().Setup(() =>
            {
                if (Nodes.Count == 0)
                    return;

                Nodes[Nodes.Count - 1] =
                    (replaceCount & 1) == 0
                        ? new ColorNode
                        {
                            Title = $"Replace:{replaceCount++}",
                            TitleBackground = Brushes.MediumVioletRed,
                            Pos = new Point(0, 0),
                        }
                        : (INodeItem) new BasicNode
                        {
                            Title = $"Replace:{replaceCount++}",
                            TitleBackground = Brushes.DarkGreen,
                            Pos = new Point(0, 0),
                        };
            });

            ReplaceNodesSourceCommand = new DelegateCommand().Setup(() =>
            {
                var nodes = new ObservableCollection<INodeItem>();
                MakeNodes(nodes);

                Nodes = nodes;
            });

            MakeNodes(Nodes);
        }

        private static void MakeNodes(ObservableCollection<INodeItem> nodes)
        {
            var titleBackgrounds = new[]
            {
                Brushes.Purple,
                Brushes.SeaGreen,
                Brushes.Firebrick,
                Brushes.DarkSlateGray,
                Brushes.DeepPink
            };

            var r = new Random();
            var i = 0;

            for (var y = 0; y != 100; ++y)
            for (var x = 0; x != 100; ++x)
            {
#if true
                var rx = r.NextDouble() * 1024;
                var ry = r.NextDouble() * 1024;
#else
                var rx = 0.0;
                var ry = 0.0;
#endif


                switch (i % 3)
                {
                    case 0:
                        nodes.Add(
                            new BasicNode
                            {
                                Title = $"Title:{i++}",
                                TitleBackground = titleBackgrounds[i % titleBackgrounds.Length],
                                Pos = new Point(x * 800 + rx, y * 800 + ry),
                            });
                        break;

                    case 1:
                        nodes.Add(
                            new ColorNode
                            {
                                Title = $"Color:{i++}",
                                TitleBackground = titleBackgrounds[i % titleBackgrounds.Length],
                                Pos = new Point(x * 800 + rx, y * 800 + ry),
                            });
                        break;

                    case 2:
                        nodes.Add(
                            new CircleNode
                            {
                                Title = $"Circle:{i++}",
                                TitleBackground = titleBackgrounds[i % titleBackgrounds.Length],
                                Pos = new Point(x * 800 + rx, y * 800 + ry),
                            });
                        break;
                }
            }
        }
    }

    public class NodeBase : ModelBase
    {
        #region Title

        private string _Name;

        public string Title
        {
            get => _Name;
            set => SetProperty(ref _Name, value);
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

    public class BasicNode : NodeBase, INodeItem
    {
        public bool IsRequireVisualTest => false;
    }

    public class ColorNode : NodeBase, INodeItem
    {
        public bool IsRequireVisualTest => false;

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

    public class CircleNode : NodeBase, INodeItem
    {
        public bool IsRequireVisualTest => true;
    }
}