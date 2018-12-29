using System;
using System.Windows;
using System.Windows.Input;
using Biaui.Controls.Internals;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal class MouseOperator
    {
        internal ImmutableRect SelectionRect
        {
            get
            {
                var (left, right) = (_mouseDownPos.X, _mouseMovePos.X).MinMax();
                var (top, bottom) = (_mouseDownPos.Y, _mouseMovePos.Y).MinMax();

                return new ImmutableRect(
                    left,
                    top,
                    right - left,
                    bottom - top);
            }
        }

        private Point _mouseDownPos;
        private Point _mouseMovePos;

        private double _mouseDownScrollX;
        private double _mouseDownScrollY;
        private readonly BiaNodeEditor _target;
        private readonly IHasTransform _transformTarget;

        private enum OpType
        {
            None,

            //
            EditorScroll,
            BoxSelect,
            PanelMove,
            LinkMove,
        }

        private OpType _opType = OpType.None;

        internal bool IsOperating => _opType != OpType.None;

        internal bool IsBoxSelect => _opType == OpType.BoxSelect;

        internal bool IsPanelMove => _opType == OpType.PanelMove;

        internal bool IsLinkMove => _opType == OpType.LinkMove;

        internal bool IsMoved { get; private set; }

        internal event MouseButtonEventHandler MouseLeftButtonDown;

        internal event MouseButtonEventHandler MouseLeftButtonUp;

        internal event MouseEventHandler MouseMove;

        internal MouseOperator(BiaNodeEditor target, IHasTransform transformTarget)
        {
            _target = target;
            _transformTarget = transformTarget;

            _target.MouseLeftButtonDown += (_, e) =>
            {
                OnMouseLeftButtonDown(e, TargetType.NodeEditor);
                MouseLeftButtonDown?.Invoke(this, e);
            };

            _target.MouseLeftButtonUp += (_, e) =>
            {
                MouseLeftButtonUp?.Invoke(this, e);
                OnMouseLeftButtonUp(e);
            };

            _target.MouseMove += (_, e) =>
            {
                MouseMove?.Invoke(this, e);
                OnMouseMove(e);
            };
        }

        internal enum TargetType
        {
            NodeEditor,
            NodePanel,
            NodeLink
        }

        internal void OnMouseLeftButtonDown(MouseButtonEventArgs e, TargetType targetType)
        {
            _mouseDownScrollX = _transformTarget.Translate.X;
            _mouseDownScrollY = _transformTarget.Translate.Y;
            _mouseDownPos = e.GetPosition(_target);
            _mouseMovePos = _mouseDownPos;

            IsMoved = false;

            _target.CaptureMouse();

            // OpType
            {
                _opType = OpType.None;

                switch (targetType)
                {
                    case TargetType.NodeEditor:
                        _opType = KeyboardHelper.IsPressSpace
                            ? OpType.EditorScroll
                            : OpType.BoxSelect;
                        break;

                    case TargetType.NodePanel:
                        _opType = OpType.PanelMove;
                        break;

                    case TargetType.NodeLink:
                        _opType = OpType.LinkMove;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null);
                }
            }
        }

        internal void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            _opType = OpType.None;

            if (_target.IsMouseCaptured)
                _target.ReleaseMouseCapture();
        }

        internal void OnMouseMove(MouseEventArgs e)
        {
            if (_opType != OpType.None)
                IsMoved = true;

            switch (_opType)
            {
                case OpType.None:
                    break;

                case OpType.EditorScroll:
                    DoEditorScroll(e);
                    break;

                case OpType.BoxSelect:
                    break;

                case OpType.PanelMove:
                    DoPanelMove(e);
                    break;

                case OpType.LinkMove:
                    DoLinkMove(e);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            _mouseMovePos = e.GetPosition(_target);
        }

        private void DoEditorScroll(MouseEventArgs e)
        {
            var pos = e.GetPosition(_target);
            var diff = pos - _mouseDownPos;

            _transformTarget.Translate.X = _mouseDownScrollX + diff.X;
            _transformTarget.Translate.Y = _mouseDownScrollY + diff.Y;
        }

        internal class PanelMovingEventArgs : EventArgs
        {
            internal Vector Diff { get; set; }
        }

        internal class LinkMovingEventArgs : EventArgs
        {
            public Point MousePos { get; set; }
        }

        internal event EventHandler<PanelMovingEventArgs> PanelMoving;

        internal event EventHandler<LinkMovingEventArgs> LinkMoving;

        private readonly PanelMovingEventArgs _PanelMovingEventArgs = new PanelMovingEventArgs();
        private readonly LinkMovingEventArgs _LinkMovingEventArgs = new LinkMovingEventArgs();

        private void DoPanelMove(MouseEventArgs e)
        {
            var pos = e.GetPosition(_target);

            _PanelMovingEventArgs.Diff = (pos - _mouseMovePos) / _transformTarget.Scale.ScaleX;
            PanelMoving?.Invoke(this, _PanelMovingEventArgs);
        }

        private void DoLinkMove(MouseEventArgs e)
        {
            _LinkMovingEventArgs.MousePos = e.GetPosition(_target);
            LinkMoving?.Invoke(this, _LinkMovingEventArgs);
        }

        internal void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (IsOperating)
                return;

            var s = _transformTarget.Scale.ScaleX;

            s *= e.Delta > 0
                ? 1.25
                : 1.0 / 1.25;

            var p = e.GetPosition(_target);
            var d0 = _transformTarget.TransformPos(p.X, p.Y);

            s = (s, 0.25, 3.0).Clamp();
            _transformTarget.Scale.ScaleX = s;
            _transformTarget.Scale.ScaleY = s;

            var d1 = _transformTarget.TransformPos(p.X, p.Y);

            var diff = d1 - d0;

            _transformTarget.Translate.X += diff.X * s;
            _transformTarget.Translate.Y += diff.Y * s;
        }
    }
}