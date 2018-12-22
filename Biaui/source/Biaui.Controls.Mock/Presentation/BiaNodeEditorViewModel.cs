using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Biaui.Controls.Mock.Foundation.Interface;
using Biaui.Controls.Mock.Foundation.Mvvm;
using Biaui.Controls.NodeEditor;
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

        #region Links

        private ObservableCollection<ILinkItem> _Links = new ObservableCollection<ILinkItem>();

        public ObservableCollection<ILinkItem> Links
        {
            get => _Links;
            set => SetProperty(ref _Links, value);
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
                var selectedNodes = new HashSet<INodeItem>(Nodes.Where(x => x.IsSelected));

                var linksWithSelectedNode = Links
                    .Where(x => selectedNodes.Contains(x.Item1) ||
                                selectedNodes.Contains(x.Item2))
                    .ToArray();

                foreach (var node in selectedNodes)
                    Nodes.Remove(node);

                foreach (var link in linksWithSelectedNode)
                    Links.Remove(link);
            });

            ClearNodesCommand = new DelegateCommand().Setup(() =>
            {
                Nodes.Clear();
                Links.Clear();
            });

            var replaceCount = 0;

            ReplaceLastNodeCommand = new DelegateCommand().Setup(() =>
            {
                if (Nodes.Count == 0)
                    return;

                var removedNode = Nodes[Nodes.Count - 1];

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

                var linksWithSelectedNode = Links
                    .Where(x => removedNode == x.Item1 ||
                                removedNode == x.Item2)
                    .ToArray();

                foreach (var link in linksWithSelectedNode)
                    Links.Remove(link);
            });

            ReplaceNodesSourceCommand = new DelegateCommand().Setup(() =>
            {
                //
                var nodes = new ObservableCollection<INodeItem>();
                MakeNodes(nodes);
                Nodes = nodes;

                //
                var links = new ObservableCollection<ILinkItem>();
                MakeLinks(links, Nodes);
                Links = links;
            });

            MakeNodes(Nodes);
            MakeLinks(Links, Nodes);
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

                INodeItem nodeItem = null;

                switch (i % 3)
                {
                    case 0:
                        nodeItem = new BasicNode
                        {
                            Title = $"Title:{i++}",
                            TitleBackground = titleBackgrounds[i % titleBackgrounds.Length],
                            Pos = new Point(x * 800 + rx, y * 800 + ry),
                        };
                        break;

                    case 1:
                        nodeItem = new ColorNode
                        {
                            Title = $"Color:{i++}",
                            TitleBackground = titleBackgrounds[i % titleBackgrounds.Length],
                            Pos = new Point(x * 800 + rx, y * 800 + ry),
                        };
                        break;

                    case 2:
                        nodeItem = new CircleNode
                        {
                            Title = $"Circle:{i++}",
                            TitleBackground = titleBackgrounds[i % titleBackgrounds.Length],
                            Pos = new Point(x * 800 + rx, y * 800 + ry),
                        };
                        break;
                }

                nodes.Add(nodeItem);
            }
        }

        private static void MakeLinks(ObservableCollection<ILinkItem> links, ObservableCollection<INodeItem> nodes)
        {
            if (nodes.Count == 0)
                return;

            for (var i = 1; i != nodes.Count; ++i)
            {
                links.Add(
                    new Link
                    {
                        Item1 = nodes[i - 1],
                        Item1PortId = "OutputA",

                        Item2 = nodes[i],
                        Item2PortId = "InputA"
                    }
                );
            }
        }
    }

    public abstract class NodeBase : ModelBase, INodeItem
    {
        public bool IsSelected
        {
            get => (_flags & Flag_IsSelected) != 0;
            set => SetFlagProperty(ref _flags, Flag_IsSelected, value);
        }

        public bool IsPreSelected
        {
            get => (_flags & Flag_IsPreSelected) != 0;
            set => SetFlagProperty(ref _flags, Flag_IsPreSelected, value);
        }

        public bool IsMouseOver
        {
            get => (_flags & Flag_IsMouseOver) != 0;
            set => SetFlagProperty(ref _flags, Flag_IsMouseOver, value);
        }

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

        private uint _flags;
        private const uint Flag_IsSelected = 1 << 0;
        private const uint Flag_IsPreSelected = 1 << 1;
        private const uint Flag_IsMouseOver = 1 << 2;

        public abstract bool IsRequireVisualTest { get; }

        public abstract BiaNodePortLayout Layout { get; }
    }

    public class BasicNode : NodeBase
    {
        public override bool IsRequireVisualTest => false;

        public override BiaNodePortLayout Layout => _Layout;

        private static readonly BiaNodePortLayout _Layout = new BiaNodePortLayout
        {
            Ports =
                new Dictionary<string, BiaNodePort>
                {
                    {
                        "InputA", new BiaNodePort
                        {
                            Id = "InputA",
                            Dir = BiaNodePortDir.Top,
                            Align = BiaNodePortAlign.Center
                        }
                    },
                    {
                        "OutputA", new BiaNodePort
                        {
                            Id = "OutputA",
                            Dir = BiaNodePortDir.Bottom,
                            Align = BiaNodePortAlign.Center
                        }
                    }
                }
        };
    }

    public class ColorNode : NodeBase
    {
        public override bool IsRequireVisualTest => false;

        public override BiaNodePortLayout Layout => _Layout;

        private static readonly BiaNodePortLayout _Layout = new BiaNodePortLayout
        {
            Ports =
                new Dictionary<string, BiaNodePort>
                {
                    {
                        "InputA", new BiaNodePort
                        {
                            Id = "InputA",
                            Dir = BiaNodePortDir.Left,
                            Align = BiaNodePortAlign.Center
                        }
                    },
                    {
                        "OutputA", new BiaNodePort
                        {
                            Id = "OutputA",
                            Offset = new Point(0, -Constants.BasicOneLineHeight * 4),
                            Dir = BiaNodePortDir.Right,
                            Align = BiaNodePortAlign.End
                        }
                    },
                    {
                        "OutputB", new BiaNodePort
                        {
                            Id = "OutputB",
                            Offset = new Point(0, -Constants.BasicOneLineHeight * 3),
                            Dir = BiaNodePortDir.Right,
                            Align = BiaNodePortAlign.End
                        }
                    },
                    {
                        "OutputC", new BiaNodePort
                        {
                            Id = "OutputC",
                            Offset = new Point(0, -Constants.BasicOneLineHeight * 2),
                            Dir = BiaNodePortDir.Right,
                            Align = BiaNodePortAlign.End
                        }
                    },
                    {
                        "OutputD", new BiaNodePort
                        {
                            Id = "OutputD",
                            Offset = new Point(0, -Constants.BasicOneLineHeight * 1),
                            Dir = BiaNodePortDir.Right,
                            Align = BiaNodePortAlign.End
                        }
                    }
                }
        };
    }

    public class CircleNode : NodeBase
    {
        public override bool IsRequireVisualTest => true;

        public override BiaNodePortLayout Layout => _Layout;

        private static readonly BiaNodePortLayout _Layout = new BiaNodePortLayout
        {
            Ports =
                new Dictionary<string, BiaNodePort>
                {
                    {
                        "InputA", new BiaNodePort
                        {
                            Id = "InputA",
                            Dir = BiaNodePortDir.Left,
                            Align = BiaNodePortAlign.Center
                        }
                    },
                    {
                        "Top", new BiaNodePort
                        {
                            Id = "Top",
                            Dir = BiaNodePortDir.Top,
                            Align = BiaNodePortAlign.Center
                        }
                    },
                    {
                        "OutputA", new BiaNodePort
                        {
                            Id = "OutputA",
                            Dir = BiaNodePortDir.Right,
                            Align = BiaNodePortAlign.Center
                        }
                    },
                    {
                        "Bottom", new BiaNodePort
                        {
                            Id = "Bottom",
                            Dir = BiaNodePortDir.Bottom,
                            Align = BiaNodePortAlign.Center
                        }
                    }
                }
        };
    }

    public class Link : ModelBase, ILinkItem
    {
        #region Item1

        private INodeItem _Item0;

        public INodeItem Item1
        {
            get => _Item0;
            set => SetProperty(ref _Item0, value);
        }

        #endregion

        #region Item1PortId

        private string _Item0PortId;

        public string Item1PortId
        {
            get => _Item0PortId;
            set => SetProperty(ref _Item0PortId, value);
        }

        #endregion

        #region Item2

        private INodeItem _Item1;

        public INodeItem Item2
        {
            get => _Item1;
            set => SetProperty(ref _Item1, value);
        }

        #endregion

        #region Item2PortId

        private string _Item1PortId;

        public string Item2PortId
        {
            get => _Item1PortId;
            set => SetProperty(ref _Item1PortId, value);
        }

        #endregion

        public object InternalData { get; set; }
    }
}