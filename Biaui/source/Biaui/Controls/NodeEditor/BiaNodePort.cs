using System.Windows;

namespace Biaui.Controls.NodeEditor
{
    public class BiaNodePort
    {
        public object Id { get; set; }

        public Point Offset { get; set; }

        public BiaNodePortAlign Align { get; set; }
    }

    public enum BiaNodePortAlign
    {
        Start,
        Center,
        End
    }
}