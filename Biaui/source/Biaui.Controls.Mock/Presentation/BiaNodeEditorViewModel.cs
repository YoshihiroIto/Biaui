using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        private ObservableCollection<IBiaNodeItem> _Nodes = new ObservableCollection<IBiaNodeItem>();

        public ObservableCollection<IBiaNodeItem> Nodes
        {
            get => _Nodes;
            set => SetProperty(ref _Nodes, value);
        }

        #endregion

        #region Links

        private ObservableCollection<IBiaNodeLink> _Links = new ObservableCollection<IBiaNodeLink>();

        public ObservableCollection<IBiaNodeLink> Links
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

        #region NodeLinkStartingCommand

        private ICommand _NodeLinkStartingCommand;

        public ICommand NodeLinkStartingCommand
        {
            get => _NodeLinkStartingCommand;
            set => SetProperty(ref _NodeLinkStartingCommand, value);
        }

        #endregion

        #region NodeLinkCompletedCommand

        private ICommand _NodeLinkCompletedCommand;

        public ICommand NodeLinkCompletedCommand
        {
            get => _NodeLinkCompletedCommand;
            set => SetProperty(ref _NodeLinkCompletedCommand, value);
        }

        #endregion

        #region NodePortEnabledChecker
        
        private IBiaNodePortEnabledChecker _NodePortEnabledChecker = new NodePortEnabledChecker();
        
        public IBiaNodePortEnabledChecker NodePortEnabledChecker
        {
            get => _NodePortEnabledChecker;
            set => SetProperty(ref _NodePortEnabledChecker, value);
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
                var selectedNodes = new HashSet<IBiaNodeItem>(Nodes.Where(x => x.IsSelected));

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
                        : (IBiaNodeItem) new BasicNode
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
                var nodes = new ObservableCollection<IBiaNodeItem>();
                MakeNodes(nodes);
                Nodes = nodes;

                //
                var links = new ObservableCollection<IBiaNodeLink>();
                MakeLinks(links, Nodes);
                Links = links;
            });

            NodeLinkStartingCommand = new DelegateCommand<NodeLinkStartingEventArgs>().Setup(e =>
            {
                Debug.WriteLine($"NodeLinkStartingCommand: {NodePortId.ToString(e.SourcePortId)}");
            });

            var connectedCount = 0;
            NodeLinkCompletedCommand = new DelegateCommand<NodeLinkCompletedEventArgs>().Setup(e =>
            {
                Debug.WriteLine(
                    $"NodeLinkCompletedCommand: {NodePortId.ToString(e.SourcePortId)}, {NodePortId.ToString(e.TargetPortId)}");

                var l = FindNodeLink(Links,
                    e.SourceNodeItem,
                    e.SourcePortId,
                    e.TargetNodeItem,
                    e.TargetPortId
                );

                if (l == null)
                {
                    Links.Add(new NodeLink
                    {
                        Item1 = e.SourceNodeItem,
                        Item1PortId = e.SourcePortId,
                        Item2 = e.TargetNodeItem,
                        Item2PortId = e.TargetPortId,
                        Color = Colors.LimeGreen,
                        Style = (connectedCount & 1) == 0
                            ? BiaNodeLinkStyle.None | BiaNodeLinkStyle.Arrow
                            : BiaNodeLinkStyle.DashedLine | BiaNodeLinkStyle.Arrow
                    });

                    ++connectedCount;
                }
                else
                {
                    Links.Remove(l);
                }
            });

            MakeNodes(Nodes);
            MakeLinks(Links, Nodes);
        }

        private static IBiaNodeLink FindNodeLink(IEnumerable<IBiaNodeLink> links,
            IBiaNodeItem sourceNodeItem,
            int sourcePortId,
            IBiaNodeItem targetNodeItem,
            int targetPortId
        )
        {
            return links?.AsParallel().FirstOrDefault(l =>
            {
                if (l.Item1 == sourceNodeItem &&
                    l.Item1PortId == sourcePortId &&
                    l.Item2 == targetNodeItem &&
                    l.Item2PortId == targetPortId)
                    return true;

                if (l.Item2 == sourceNodeItem &&
                    l.Item2PortId == sourcePortId &&
                    l.Item1 == targetNodeItem &&
                    l.Item1PortId == targetPortId)
                    return true;

                return false;
            });
        }

        private static void MakeNodes(ObservableCollection<IBiaNodeItem> nodes)
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

                IBiaNodeItem nodeItem = null;

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

        private static void MakeLinks(ObservableCollection<IBiaNodeLink> links,
            ObservableCollection<IBiaNodeItem> nodes)
        {
            if (nodes.Count == 0)
                return;

            for (var i = 1; i != nodes.Count; ++i)
            {
                links.Add(
                    new NodeLink
                    {
                        Item1 = nodes[i - 1],
                        Item1PortId = NodePortId.Make("OutputA"),

                        Item2 = nodes[i],
                        Item2PortId = NodePortId.Make("InputA"),

                        Color = Colors.DeepPink,
                        Style = (i & 1) == 0
                            ? BiaNodeLinkStyle.Arrow
                            : BiaNodeLinkStyle.None
                    }
                );
            }
        }
    }

    public abstract class NodeBase : ModelBase, IBiaNodeItem
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

        public abstract BiaNodePanelHitType HitType { get; }

        public abstract BiaNodePortLayout Layout { get; }

        public object InternalData { get; set; }
    }

    public class BasicNode : NodeBase
    {
        public override BiaNodePanelHitType HitType => BiaNodePanelHitType.Rectangle;

        public override BiaNodePortLayout Layout => _Layout;

        private static readonly BiaNodePortLayout _Layout = new BiaNodePortLayout
        {
            Ports =
                new Dictionary<int, BiaNodePort>
                {
                    {
                        NodePortId.Make("InputA"), new BiaNodePort
                        {
                            Id = NodePortId.Make("InputA"),
                            Dir = BiaNodePortDir.Top,
                            Align = BiaNodePortAlign.Center
                        }
                    },
                    {
                        NodePortId.Make("OutputA"), new BiaNodePort
                        {
                            Id = NodePortId.Make("OutputA"),
                            Dir = BiaNodePortDir.Bottom,
                            Align = BiaNodePortAlign.Center
                        }
                    }
                }
        };
    }

    public class ColorNode : NodeBase
    {
        public override BiaNodePanelHitType HitType => BiaNodePanelHitType.Rectangle;

        public override BiaNodePortLayout Layout => _Layout;

        private static readonly BiaNodePortLayout _Layout = new BiaNodePortLayout
        {
            Ports =
                new Dictionary<int, BiaNodePort>
                {
                    {
                        NodePortId.Make("InputA"), new BiaNodePort
                        {
                            Id = NodePortId.Make("InputA"),
                            Dir = BiaNodePortDir.Left,
                            Align = BiaNodePortAlign.Center
                        }
                    },
                    {
                        NodePortId.Make("Red"), new BiaNodePort
                        {
                            Id = NodePortId.Make("Red"),
                            Offset = new Point(0, -28 * 4),
                            Dir = BiaNodePortDir.Right,
                            Align = BiaNodePortAlign.End,
                            Color = Colors.Red
                        }
                    },
                    {
                        NodePortId.Make("Green"), new BiaNodePort
                        {
                            Id = NodePortId.Make("Green"),
                            Offset = new Point(0, -28 * 3),
                            Dir = BiaNodePortDir.Right,
                            Align = BiaNodePortAlign.End,
                            Color = Colors.LimeGreen
                        }
                    },
                    {
                        NodePortId.Make("Blue"), new BiaNodePort
                        {
                            Id = NodePortId.Make("Blue"),
                            Offset = new Point(0, -28 * 2),
                            Dir = BiaNodePortDir.Right,
                            Align = BiaNodePortAlign.End,
                            Color = Colors.DodgerBlue
                        }
                    },
                    {
                        NodePortId.Make("OutputA"), new BiaNodePort
                        {
                            Id = NodePortId.Make("OutputA"),
                            Offset = new Point(0, -28 * 1),
                            Dir = BiaNodePortDir.Right,
                            Align = BiaNodePortAlign.End
                        }
                    }
                }
        };
    }

    public class CircleNode : NodeBase
    {
        public override BiaNodePanelHitType HitType => BiaNodePanelHitType.Circle;

        public override BiaNodePortLayout Layout => _Layout;

        private static readonly BiaNodePortLayout _Layout = new BiaNodePortLayout
        {
            Ports =
                new Dictionary<int, BiaNodePort>
                {
                    {
                        NodePortId.Make("InputA"), new BiaNodePort
                        {
                            Id = NodePortId.Make("InputA"),
                            Dir = BiaNodePortDir.Left,
                            Align = BiaNodePortAlign.Center
                        }
                    },
                    {
                        NodePortId.Make("Top"), new BiaNodePort
                        {
                            Id = NodePortId.Make("Top"),
                            Dir = BiaNodePortDir.Top,
                            Align = BiaNodePortAlign.Center
                        }
                    },
                    {
                        NodePortId.Make("OutputA"), new BiaNodePort
                        {
                            Id = NodePortId.Make("OutputA"),
                            Dir = BiaNodePortDir.Right,
                            Align = BiaNodePortAlign.Center
                        }
                    },
                    {
                        NodePortId.Make("Bottom"), new BiaNodePort
                        {
                            Id = NodePortId.Make("Bottom"),
                            Dir = BiaNodePortDir.Bottom,
                            Align = BiaNodePortAlign.Center
                        }
                    }
                }
        };
    }

    public class NodeLink : ModelBase, IBiaNodeLink
    {
        #region Item1

        private IBiaNodeItem _Item1;

        public IBiaNodeItem Item1
        {
            get => _Item1;
            set => SetProperty(ref _Item1, value);
        }

        #endregion

        #region Item1PortId

        private int _Item1PortId;

        public int Item1PortId
        {
            get => _Item1PortId;
            set => SetProperty(ref _Item1PortId, value);
        }

        #endregion

        #region Item2

        private IBiaNodeItem _Item2;

        public IBiaNodeItem Item2
        {
            get => _Item2;
            set => SetProperty(ref _Item2, value);
        }

        #endregion

        #region Item2PortId

        private int _Item2PortId;

        public int Item2PortId
        {
            get => _Item2PortId;
            set => SetProperty(ref _Item2PortId, value);
        }

        #endregion

        #region Color

        private Color _Color;

        public Color Color
        {
            get => _Color;
            set => SetProperty(ref _Color, value);
        }

        #endregion

        #region Style

        private BiaNodeLinkStyle _Style;

        public BiaNodeLinkStyle Style
        {
            get => _Style;
            set => SetProperty(ref _Style, value);
        }

        #endregion

        public object InternalData { get; set; }
    }

    public class NodePortEnabledChecker : IBiaNodePortEnabledChecker
    {
        public IEnumerable<int> Check(IBiaNodeItem target, in BiaNodePortEnabledCheckerArgs args)
        {
            switch (args.Timing)
            {
                case BiaNodePortEnableTiming.Default:
                    return target.Layout.Ports.Keys;

                case BiaNodePortEnableTiming.ConnectionStarting:
                    // 開始ノードが CircleNodeの場合相手もCircleNodeに限る
                    if (args.SourceItem is CircleNode)
                    {
                        return target is CircleNode
                            ? target.Layout.Ports.Keys
                            : Enumerable.Empty<int>();
                    }
                    else
                        return target.Layout.Ports.Keys;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
