using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor
{
    internal class MouseOperator
    {
        private double _mouseDownScrollX;
        private double _mouseDownScrollY;
        private Point _mouseDownMousePos;

        private Point _oldMouseDownMousePos;


        private readonly BiaNodeEditor _target;
        private readonly TranslateTransform _translate;
        private readonly ScaleTransform _scale;

        private enum OpType
        {
            None,
            //
            EditorScroll,
            PanelMove
        }

        private OpType _opType = OpType.None;

        internal bool IsOperating => _opType != OpType.None;

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
            _mouseDownMousePos = e.GetPosition(_target);
            _oldMouseDownMousePos = _mouseDownMousePos;

            _target.CaptureMouse();

            // OpType
            {
                _opType = OpType.None;
                if (KeyboardHelper.IsPressSpace)
                    _opType = OpType.EditorScroll;

                else if (targetType == TargetType.NodePanel)
                    _opType = OpType.PanelMove;
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

                case OpType.PanelMove:
                    DoPanelMove(e);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            _oldMouseDownMousePos = e.GetPosition(_target);
        }

        private void DoEditorScroll(MouseEventArgs e)
        {
            var pos = e.GetPosition(_target);
            var diff = pos - _mouseDownMousePos;

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

            _PanelMovingEventArgs.Diff = (pos - _oldMouseDownMousePos) / _scale.ScaleX;

            PanelMoving?.Invoke(this, _PanelMovingEventArgs);
        }

        internal void OnMouseWheel(MouseWheelEventArgs e)
        {
            var s = _scale.ScaleX;

            s *= e.Delta > 0 ? 1.25 : 1.0 / 1.25;

            var p = e.GetPosition(_target);
            var d0 = ScenePosFromControlPos(p);

            s = Math.Max(Math.Min(s, 3.0), 0.25);
            _scale.ScaleX = s;
            _scale.ScaleY = s;

            var d1 = ScenePosFromControlPos(p);

            var diff = d1 - d0;

            _translate.X += diff.X * s;
            _translate.Y += diff.Y * s;
        }

        private Point ScenePosFromControlPos(Point pos)
            => new Point(
                (pos.X - _translate.X) / _scale.ScaleX,
                (pos.Y - _translate.Y) / _scale.ScaleY);
    }
}