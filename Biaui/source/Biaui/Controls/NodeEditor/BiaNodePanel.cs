using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Biaui.NodeEditor;

namespace Biaui.Controls.NodeEditor
{
    public class BiaNodePanel : Thumb
    {
        private static readonly Random _random = new Random();

        public BiaNodePanel()
        {
            Style = null;

            var r = (byte) (0x80 + _random.Next(0x7F));
            var g = (byte) (0x80 + _random.Next(0x7F));
            var b = (byte) (0x80 + _random.Next(0x7F));

            var brush = new SolidColorBrush(Color.FromRgb(r, g, b));
            brush.Freeze();

            Background = brush;
            BorderBrush = Brushes.Red;

            MouseEnter += (s, __) => ((BiaNodePanel) s).BorderThickness = new Thickness(4);
            MouseLeave += (s, __) => ((BiaNodePanel) s).BorderThickness = new Thickness(0);

            DragDelta += OnDragDelta;
        }

        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var i = (INodeItem)DataContext;

            i.Pos = new Point(i.Pos.X + e.HorizontalChange, i.Pos.Y + e.VerticalChange);
        }
    }
}
