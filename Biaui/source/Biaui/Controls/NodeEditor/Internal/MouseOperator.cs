using System;
using System.Windows;
using System.Windows.Input;
using Biaui.Controls.Internals;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal class MouseOperator
    {
        internal ImmutableRect_double SelectionRect
        {
            get
            {
                var (left, right) = (_mouseDownPos.X, _mouseMovePos.X).MinMax();
                var (top, bottom) = (_mouseDownPos.Y, _mouseMovePos.Y).MinMax();

                return new ImmutableRect_double(
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
        private readonly FrameworkElement _target;
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

        internal event MouseButtonEventHandler PrePreviewMouseLeftButtonDown;
        internal event MouseButtonEventHandler PostPreviewMouseLeftButtonDown;

        internal event MouseButtonEventHandler PrePreviewMouseLeftButtonUp;
        internal event MouseButtonEventHandler PostPreviewMouseLeftButtonUp;

        internal event MouseButtonEventHandler PreMouseLeftButtonDown;
        internal event MouseButtonEventHandler PostMouseLeftButtonDown;

        internal event MouseButtonEventHandler PreMouseLeftButtonUp;
        internal event MouseButtonEventHandler PostMouseLeftButtonUp;

        internal event MouseEventHandler PreMouseMove;
        internal event MouseEventHandler PostMouseMove;

        internal event MouseWheelEventHandler PreMouseWheel;
        internal event MouseWheelEventHandler PostMouseWheel;

        internal void InvokePostMouseLeftButtonDown(MouseButtonEventArgs e) => PostMouseLeftButtonDown?.Invoke(this, e);

        internal MouseOperator(FrameworkElement target, IHasTransform transformTarget)
        {
            _target = target;
            _transformTarget = transformTarget;

            _target.PreviewMouseLeftButtonDown += (_, e) =>
            {
                PrePreviewMouseLeftButtonDown?.Invoke(this, e);
                PostPreviewMouseLeftButtonDown?.Invoke(this, e);
            };

            _target.PreviewMouseLeftButtonUp += (_, e) =>
            {
                PrePreviewMouseLeftButtonUp?.Invoke(this, e);
                PostPreviewMouseLeftButtonUp?.Invoke(this, e);
            };

            _target.MouseLeftButtonDown += (_, e) =>
            {
                PreMouseLeftButtonDown?.Invoke(this, e);
                OnMouseLeftButtonDown(e, TargetType.NodeEditor);
                PostMouseLeftButtonDown?.Invoke(this, e);
            };

            _target.MouseLeftButtonUp += (_, e) =>
            {
                PreMouseLeftButtonUp?.Invoke(this, e);
                OnMouseLeftButtonUp(e);
                PostMouseLeftButtonUp?.Invoke(this, e);
            };

            _target.MouseMove += (_, e) =>
            {
                PreMouseMove?.Invoke(this, e);
                OnMouseMove(e);
                PostMouseMove?.Invoke(this, e);
            };

            _target.MouseWheel += (_, e) =>
            {
                PreMouseWheel?.Invoke(this, e);
                OnMouseWheel(e);
                PostMouseWheel?.Invoke(this, e);
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
            _mouseDownScrollX = _transformTarget.TranslateTransform.X;
            _mouseDownScrollY = _transformTarget.TranslateTransform.Y;
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
                        _opType = KeyboardHelper.IsPressShift
                            ? OpType.EditorScroll
                            : OpType.BoxSelect;
                        break;

                    case TargetType.NodePanel:
                        _opType = OpType.PanelMove;
                        PanelBeginMoving?.Invoke(this, EventArgs.Empty);
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
            if (_opType == OpType.PanelMove)
                PanelEndMoving?.Invoke(this, EventArgs.Empty);

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

            _transformTarget.TranslateTransform.X = _mouseDownScrollX + diff.X;
            _transformTarget.TranslateTransform.Y = _mouseDownScrollY + diff.Y;
        }

        internal class PanelMovingEventArgs : EventArgs
        {
            internal Vector Diff { get; set; }
        }

        internal class LinkMovingEventArgs : EventArgs
        {
            public Point MousePos { get; set; }
        }

        internal event EventHandler PanelBeginMoving;
        internal event EventHandler PanelEndMoving;
        internal event EventHandler<PanelMovingEventArgs> PanelMoving;

        internal event EventHandler<LinkMovingEventArgs> LinkMoving;

        private readonly PanelMovingEventArgs _PanelMovingEventArgs = new PanelMovingEventArgs();
        private readonly LinkMovingEventArgs _LinkMovingEventArgs = new LinkMovingEventArgs();

        private void DoPanelMove(MouseEventArgs e)
        {
            var pos = e.GetPosition(_target);

            _PanelMovingEventArgs.Diff = (pos - _mouseMovePos) / _transformTarget.ScaleTransform.ScaleX;
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

            var s = _transformTarget.ScaleTransform.ScaleX;

            s *= e.Delta > 0
                ? 1.25
                : 1.0 / 1.25;

            s = (s, Constants.NodeEditor_MinScale, Constants.NodeEditor_MaxScale).Clamp();

            var p = e.GetPosition(_target);

            _transformTarget.SetTransform(s, p.X, p.Y);
        }
    }
}