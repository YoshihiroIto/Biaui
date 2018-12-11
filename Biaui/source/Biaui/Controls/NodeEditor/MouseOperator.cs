using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor
{
    internal class MouseOperator
    {
        internal bool IsScrolling { get; private set; }

        private double _mouseDownScrollX;
        private double _mouseDownScrollY;
        private Point _mouseDownMousePos;

        private readonly BiaNodeEditor _target;
        private readonly TranslateTransform _translate;
        private readonly ScaleTransform _scale;

        internal MouseOperator(BiaNodeEditor target, TranslateTransform translate, ScaleTransform scale)
        {
            _target = target;
            _translate = translate;
            _scale = scale;
        }

        internal void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            _mouseDownScrollX = _translate.X;
            _mouseDownScrollY = _translate.Y;
            _mouseDownMousePos = e.GetPosition(_target);

            IsScrolling = (Win32Helper.GetAsyncKeyState(Win32Helper.VK_SPACE) & 0x8000) != 0;

            _target.CaptureMouse();
        }

        internal void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            IsScrolling = false;

            if (_target.IsMouseCaptured)
                _target.ReleaseMouseCapture();
        }

        internal void OnMouseMove(MouseEventArgs e)
        {
            if (IsScrolling == false)
                return;

            var pos = e.GetPosition(_target);
            var diff = pos - _mouseDownMousePos;

            _translate.X = _mouseDownScrollX + diff.X;
            _translate.Y = _mouseDownScrollY + diff.Y;
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