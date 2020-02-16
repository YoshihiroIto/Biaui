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
        private readonly IHasScalerRange _scalerRange;

        private enum OperationType
        {
            None,

            //
            EditorScroll,
            BoxSelect,
            PanelMove,
            LinkMove,
        }

        private OperationType _Operation = OperationType.None;

        private OperationType Operation
        {
            get => _Operation;
            set
            {
                if (_Operation == value)
                    return;

                _Operation = value;
                OperationChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        internal bool IsOperating => Operation != OperationType.None;

        internal bool IsEditorScroll => Operation == OperationType.EditorScroll;

        internal bool IsBoxSelect => Operation == OperationType.BoxSelect;

        internal bool IsPanelMove => Operation == OperationType.PanelMove;

        internal bool IsLinkMove => Operation == OperationType.LinkMove;

        internal bool IsMoved { get; private set; }

        internal event MouseButtonEventHandler? PrePreviewMouseLeftButtonDown;

        internal event MouseButtonEventHandler? PostPreviewMouseLeftButtonDown;

        internal event MouseButtonEventHandler? PrePreviewMouseLeftButtonUp;

        internal event MouseButtonEventHandler? PostPreviewMouseLeftButtonUp;

        internal event MouseButtonEventHandler? PreMouseLeftButtonDown;

        internal event MouseButtonEventHandler? PostMouseLeftButtonDown;

        internal event MouseButtonEventHandler? PreMouseLeftButtonUp;

        internal event MouseButtonEventHandler? PostMouseLeftButtonUp;

        internal event MouseEventHandler? PreMouseMove;

        internal event MouseEventHandler? PostMouseMove;

        internal event MouseWheelEventHandler? PreMouseWheel;

        internal event MouseWheelEventHandler? PostMouseWheel;

        internal event EventHandler OperationChanged;

        internal void InvokePostMouseLeftButtonDown(MouseButtonEventArgs e) => PostMouseLeftButtonDown?.Invoke(this, e);

        internal MouseOperator(FrameworkElement target, IHasTransform transformTarget, IHasScalerRange scalerRange)
        {
            _target = target;
            _transformTarget = transformTarget;
            _scalerRange = scalerRange;

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

            _target.MouseDown += (_, e) =>
            {
                if (e.ChangedButton == MouseButton.Middle)
                    OnMouseMiddleButtonDown(e, TargetType.NodeEditor);
            };

            _target.MouseUp += (_, e) =>
            {
                if (e.ChangedButton == MouseButton.Middle)
                    OnMouseMiddleButtonUp(e);
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
                Operation = OperationType.None;

                switch (targetType)
                {
                    case TargetType.NodeEditor:
                        Operation = KeyboardHelper.IsPressShift
                            ? OperationType.EditorScroll
                            : OperationType.BoxSelect;
                        break;

                    case TargetType.NodePanel:
                        Operation = OperationType.PanelMove;
                        PanelBeginMoving?.Invoke(this, EventArgs.Empty);
                        break;

                    case TargetType.NodeLink:
                        Operation = OperationType.LinkMove;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null);
                }
            }
        }

        internal void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (Operation == OperationType.PanelMove)
                PanelEndMoving?.Invoke(this, EventArgs.Empty);

            Operation = OperationType.None;

            if (_target.IsMouseCaptured)
                _target.ReleaseMouseCapture();
        }

        internal void OnMouseMove(MouseEventArgs e)
        {
            if (Operation != OperationType.None)
                IsMoved = true;

            switch (Operation)
            {
                case OperationType.None:
                    break;

                case OperationType.EditorScroll:
                    DoEditorScroll(e);
                    break;

                case OperationType.BoxSelect:
                    break;

                case OperationType.PanelMove:
                    DoPanelMove(e);
                    break;

                case OperationType.LinkMove:
                    DoLinkMove(e);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            _mouseMovePos = e.GetPosition(_target);
        }

        internal void OnMouseMiddleButtonDown(MouseButtonEventArgs e, TargetType targetType)
        {
            _mouseDownScrollX = _transformTarget.TranslateTransform.X;
            _mouseDownScrollY = _transformTarget.TranslateTransform.Y;
            _mouseDownPos = e.GetPosition(_target);
            _mouseMovePos = _mouseDownPos;

            IsMoved = false;

            _target.CaptureMouse();

            // OpType
            {
                Operation = targetType switch
                {
                    TargetType.NodeEditor => OperationType.EditorScroll,
                    _ => OperationType.None
                };
            }
        }

        internal void OnMouseMiddleButtonUp(MouseButtonEventArgs e)
        {
            OnMouseLeftButtonUp(e);
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

        internal event EventHandler? PanelBeginMoving;

        internal event EventHandler? PanelEndMoving;

        internal event EventHandler<PanelMovingEventArgs>? PanelMoving;

        internal event EventHandler<LinkMovingEventArgs>? LinkMoving;

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

            s = (s, _scalerRange.ScalerMinimum, _scalerRange.ScalerMaximum).Clamp();

            var p = e.GetPosition(_target);

            _transformTarget.SetTransform(s, p.X, p.Y);
        }
    }
}