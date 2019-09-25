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

        #region ChangeSelectedNodeSlotsCommand

        private ICommand _changeSelectedNodeSlotsCommand;

        public ICommand ChangeSelectedNodeSlotsCommand
        {
            get => _changeSelectedNodeSlotsCommand;
            set => SetProperty(ref _changeSelectedNodeSlotsCommand, value);
        }

        #endregion

        #region NodeSlotEnabledChecker

        private IBiaNodeSlotEnabledChecker _nodeSlotEnabledChecker = new NodeSlotEnabledChecker();

        public IBiaNodeSlotEnabledChecker NodeSlotEnabledChecker
        {
            get => _nodeSlotEnabledChecker;
            set => SetProperty(ref _nodeSlotEnabledChecker, value);
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
                    .Where(x => selectedNodes.Contains(x.ItemSlot1.Item) ||
                                selectedNodes.Contains(x.ItemSlot2.Item))
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
                    .Where(x => removedNode == x.ItemSlot1.Item ||
                                removedNode == x.ItemSlot2.Item)
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
                Debug.WriteLine($"NodeLinkStartingCommand: {NodeSlotId.ToString(e.Source.SlotId)}");
            });

            var connectedCount = 0;
            NodeLinkCompletedCommand = new DelegateCommand<NodeLinkCompletedEventArgs>().Setup(e =>
            {
                Debug.WriteLine(
                    $"NodeLinkCompletedCommand: {NodeSlotId.ToString(e.Source.SlotId)}, {NodeSlotId.ToString(e.Target.SlotId)}");

                var l = FindNodeLink(Links, e.Source, e.Target);

                if (l == null)
                {
                    Links.Add(new NodeLink
                    {
                        ItemSlot1 = e.Source,
                        ItemSlot2 = e.Target,
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

            ChangeSelectedNodeSlotsCommand = new DelegateCommand().Setup(() =>
            {
                foreach (var node in Nodes.Where(x => x.IsSelected))
                {
                    node.Slots =
                        new Dictionary<int, BiaNodeSlot>
                        {
                            {
                                NodeSlotId.Make("InputA"), new BiaNodeSlot
                                {
                                    Id = NodeSlotId.Make("InputA"),
                                    Dir = BiaNodeSlotDir.Bottom,
                                    Align = BiaNodeSlotAlign.End
                                }
                            }
                        };

                    foreach (var link in Links)
                        if (link.ItemSlot1.Item == node ||
                            link.ItemSlot2.Item == node)
                            link.Reset();
                }
            });


            MakeNodes(Nodes);
            MakeLinks(Links, Nodes);
        }

        private static IBiaNodeLink FindNodeLink(IEnumerable<IBiaNodeLink> links,
            in BiaNodeItemSlotIdPair source,
            in BiaNodeItemSlotIdPair target)
        {
            foreach (var link in links)
            {
                if (link.ItemSlot1 == source &&
                    link.ItemSlot2 == target)
                    return link;

                if (link.ItemSlot2 == source &&
                    link.ItemSlot1 == target)
                    return link;
            }

            return null;
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

                var pos = new Point(
                    BiaHasPosHelper.AlignPos(x * 800 + rx),
                    BiaHasPosHelper.AlignPos(y * 800 + ry));

                var titleBackground = titleBackgrounds[i % titleBackgrounds.Length];

                IBiaNodeItem nodeItem = null;

                switch (i % 3)
                {
                    case 0:
                        nodeItem = new BasicNode
                        {
                            Title = $"Title:{i++}",
                            TitleBackground = titleBackground,
                            Pos = pos
                        };
                        break;

                    case 1:
                        nodeItem = new ColorNode
                        {
                            Title = $"Color:{i++}",
                            TitleBackground = titleBackground,
                            Pos = pos
                        };
                        break;

                    case 2:
                        nodeItem = new CircleNode
                        {
                            Title = $"Circle:{i++}",
                            TitleBackground = titleBackground,
                            Pos = pos
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
                        ItemSlot1 = new BiaNodeItemSlotIdPair(nodes[i - 1], NodeSlotId.Make("OutputA")),
                        ItemSlot2 = new BiaNodeItemSlotIdPair(nodes[i], NodeSlotId.Make("InputA")),

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

        public abstract BiaNodePanelLayer Layer { get; }

        public Func<BiaNodeSlot, Point> MakeSlotPos { get; } = null;

        public bool CanMoveByDragging(CanMoveByDraggingArgs args) => true;

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

        #region SlotLayout

        private IReadOnlyDictionary<int, BiaNodeSlot> _Slots;

        public IReadOnlyDictionary<int, BiaNodeSlot> Slots
        {
            get => _Slots;
            set => SetProperty(ref _Slots, value);
        }

        #endregion

        public object InternalData { get; set; }
    }

    public class BasicNode : NodeBase
    {
        public override BiaNodePanelHitType HitType => BiaNodePanelHitType.Rectangle;

        public override BiaNodePanelLayer Layer => BiaNodePanelLayer.Middle;

        public BasicNode()
        {
            Slots =
                new Dictionary<int, BiaNodeSlot>
                {
                    {
                        NodeSlotId.Make("InputA"), new BiaNodeSlot
                        {
                            Id = NodeSlotId.Make("InputA"),
                            Dir = BiaNodeSlotDir.Top,
                            Align = BiaNodeSlotAlign.Center
                        }
                    },
                    {
                        NodeSlotId.Make("OutputA"), new BiaNodeSlot
                        {
                            Id = NodeSlotId.Make("OutputA"),
                            Dir = BiaNodeSlotDir.Bottom,
                            Align = BiaNodeSlotAlign.Center
                        }
                    }
                };
        }
    }

    public class ColorNode : NodeBase
    {
        public override BiaNodePanelHitType HitType => BiaNodePanelHitType.Rectangle;

        public override BiaNodePanelLayer Layer => BiaNodePanelLayer.Middle;

        public ColorNode()
        {
            Slots =
                new Dictionary<int, BiaNodeSlot>
                {
                    {
                        NodeSlotId.Make("InputA"), new BiaNodeSlot
                        {
                            Id = NodeSlotId.Make("InputA"),
                            Dir = BiaNodeSlotDir.Left,
                            Align = BiaNodeSlotAlign.Center
                        }
                    },
                    {
                        NodeSlotId.Make("Red"), new BiaNodeSlot
                        {
                            Id = NodeSlotId.Make("Red"),
                            Offset = new Point(0, -28 * 4),
                            Dir = BiaNodeSlotDir.Right,
                            Align = BiaNodeSlotAlign.End,
                            Color = Colors.Red
                        }
                    },
                    {
                        NodeSlotId.Make("Green"), new BiaNodeSlot
                        {
                            Id = NodeSlotId.Make("Green"),
                            Offset = new Point(0, -28 * 3),
                            Dir = BiaNodeSlotDir.Right,
                            Align = BiaNodeSlotAlign.End,
                            Color = Colors.LimeGreen
                        }
                    },
                    {
                        NodeSlotId.Make("Blue"), new BiaNodeSlot
                        {
                            Id = NodeSlotId.Make("Blue"),
                            Offset = new Point(0, -28 * 2),
                            Dir = BiaNodeSlotDir.Right,
                            Align = BiaNodeSlotAlign.End,
                            Color = Colors.DodgerBlue
                        }
                    },
                    {
                        NodeSlotId.Make("OutputA"), new BiaNodeSlot
                        {
                            Id = NodeSlotId.Make("OutputA"),
                            Offset = new Point(0, -28 * 1),
                            Dir = BiaNodeSlotDir.Right,
                            Align = BiaNodeSlotAlign.End
                        }
                    }
                };
        }
    }

    public class CircleNode : NodeBase
    {
        public override BiaNodePanelHitType HitType => BiaNodePanelHitType.Circle;

        public override BiaNodePanelLayer Layer => BiaNodePanelLayer.Low;

        public CircleNode()
        {
            Slots =
                new Dictionary<int, BiaNodeSlot>
                {
                    {
                        NodeSlotId.Make("InputA"), new BiaNodeSlot
                        {
                            Id = NodeSlotId.Make("InputA"),
                            Dir = BiaNodeSlotDir.Left,
                            Align = BiaNodeSlotAlign.Center
                        }
                    },
                    {
                        NodeSlotId.Make("Top"), new BiaNodeSlot
                        {
                            Id = NodeSlotId.Make("Top"),
                            Dir = BiaNodeSlotDir.Top,
                            Align = BiaNodeSlotAlign.Center
                        }
                    },
                    {
                        NodeSlotId.Make("OutputA"), new BiaNodeSlot
                        {
                            Id = NodeSlotId.Make("OutputA"),
                            Dir = BiaNodeSlotDir.Right,
                            Align = BiaNodeSlotAlign.Center
                        }
                    },
                    {
                        NodeSlotId.Make("Bottom"), new BiaNodeSlot
                        {
                            Id = NodeSlotId.Make("Bottom"),
                            Dir = BiaNodeSlotDir.Bottom,
                            Align = BiaNodeSlotAlign.Center
                        }
                    }
                };
        }
    }

    public class NodeLink : ModelBase, IBiaNodeLink
    {
        #region ItemSlot1

        private BiaNodeItemSlotIdPair _ItemSlot1;

        public BiaNodeItemSlotIdPair ItemSlot1
        {
            get => _ItemSlot1;
            set => SetProperty(ref _ItemSlot1, value);
        }

        #endregion

        #region ItemSlot2

        private BiaNodeItemSlotIdPair _ItemSlot2;

        public BiaNodeItemSlotIdPair ItemSlot2
        {
            get => _ItemSlot2;
            set => SetProperty(ref _ItemSlot2, value);
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

        public bool IsVisible => true;
    }

    public class NodeSlotEnabledChecker : IBiaNodeSlotEnabledChecker
    {
        public bool IsEnableSlot(in BiaNodeItemSlotIdPair slot)
        {
            return true;
        }

        public IEnumerable<int> Check(IBiaNodeItem target, in BiaNodeSlotEnabledCheckerArgs args)
        {
            switch (args.Timing)
            {
                case BiaNodeSlotEnableTiming.Default:
                    return target.Slots.Keys;

                case BiaNodeSlotEnableTiming.ConnectionStarting:

                    // 開始ノードが CircleNodeの場合相手もCircleNodeに限る
                    if (args.Source.Item is CircleNode)
                    {
                        return target is CircleNode
                            ? target.Slots.Keys
                            : Enumerable.Empty<int>();
                    }
                    else
                        return target.Slots.Keys;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}