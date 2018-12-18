using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal class MouseOperator
    {
        internal (Point LeftTop, Point RightBottom) SelectionRect
        {
            get
            {
                var left = Math.Min(_mouseDownPos.X, _mouseMovePos.X);
                var right = Math.Max(_mouseDownPos.X, _mouseMovePos.X);
                var top = Math.Min(_mouseDownPos.Y, _mouseMovePos.Y);
                var bottom = Math.Max(_mouseDownPos.Y, _mouseMovePos.Y);

                return (new Point(left, top), new Point(right, bottom));
            }
        }

        private Point _mouseDownPos;
        private Point _mouseMovePos;

        private double _mouseDownScrollX;
        private double _mouseDownScrollY;
        private readonly BiaNodeEditor _target;
        private readonly TranslateTransform _translate;
        private readonly ScaleTransform _scale;

        private enum OpType
        {
            None,

            //
            EditorScroll,
            BoxSelect,
            PanelMove
        }

        private OpType _opType = OpType.None;

        internal bool IsOperating => _opType != OpType.None;
        internal bool IsBoxSelect => _opType == OpType.BoxSelect;
        internal bool IsPanelMove => _opType == OpType.PanelMove;

        internal MouseOperator(BiaNodeEditor target, TranslateTransform translate, ScaleTransform scale)
        {
            _target = target;
            _translate = translate;
            _scale = scale;
        }

        internal enum TargetType
        {
            NodeEditor,
            NodePanel,
        }

        internal void OnMouseLeftButtonDown(MouseButtonEventArgs e, TargetType targetType)
        {
            _mouseDownScrollX = _translate.X;
            _mouseDownScrollY = _translate.Y;
            _mouseDownPos = e.GetPosition(_target);
            _mouseMovePos = _mouseDownPos;

            _target.CaptureMouse();

            // OpType
            {
                _opType = OpType.None;

                switch (targetType)
                {
                    case TargetType.NodeEditor:
                        _opType = KeyboardHelper.IsPressSpace ? OpType.EditorScroll : OpType.BoxSelect;

                        break;

                    case TargetType.NodePanel:
                        _opType = OpType.PanelMove;

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

                default:
                    throw new ArgumentOutOfRangeException();
            }

            _mouseMovePos = e.GetPosition(_target);
        }

        private void DoEditorScroll(MouseEventArgs e)
        {
            var pos = e.GetPosition(_target);
            var diff = pos - _mouseDownPos;

            _translate.X = _mouseDownScrollX + diff.X;
            _translate.Y = _mouseDownScrollY + diff.Y;
        }

        internal class PanelMovingEventArgs : EventArgs
        {
            internal Vector Diff { get; set; }
        }

        internal event EventHandler<PanelMovingEventArgs> PanelMoving;

        private readonly PanelMovingEventArgs _PanelMovingEventArgs = new PanelMovingEventArgs();

        private void DoPanelMove(MouseEventArgs e)
        {
            var pos = e.GetPosition(_target);

            _PanelMovingEventArgs.Diff = (pos - _mouseMovePos) / _scale.ScaleX;

            PanelMoving?.Invoke(this, _PanelMovingEventArgs);
        }

        internal void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (IsOperating)
                return;

            var s = _scale.ScaleX;

            s *= e.Delta > 0 ? 1.25 : 1.0 / 1.25;

            var p = e.GetPosition(_target);
            var d0 = _target.MakeScenePosFromControlPos(p.X, p.Y);

            s = Math.Max(Math.Min(s, 3.0), 0.25);
            _scale.ScaleX = s;
            _scale.ScaleY = s;

            var d1 = _target.MakeScenePosFromControlPos(p.X, p.Y);

            var diff = d1 - d0;

            _translate.X += diff.X * s;
            _translate.Y += diff.Y * s;

            //_target.InvalidateVisual();
        }
    }
}