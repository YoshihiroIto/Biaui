using System.Windows;
using System.Windows.Media;

namespace Biaui.Controls.NodeEditor
{
    public class BiaNodePort
    {
        public int Id { get; set; }

        public Point Offset { get; set; }

        public BiaNodePortDir Dir { get; set; }

        public BiaNodePortAlign Align { get; set; }

        public Color Color { get; set; } = Colors.White;
    }

    public enum BiaNodePortAlign
    {
        Start,
        Center,
        End
    }

    public enum BiaNodePortDir
    {
        Left,
        Top,
        Right,
        Bottom
    }
}