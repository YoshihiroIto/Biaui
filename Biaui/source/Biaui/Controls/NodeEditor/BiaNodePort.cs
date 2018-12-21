using System.Windows;
using Biaui.Interfaces;

namespace Biaui.Controls.NodeEditor
{
    public class BiaNodePort
    {
        public string Id { get; set; }

        public Point Offset { get; set; }

        public BiaNodePortDir Dir { get; set; }

        public BiaNodePortAlign Align { get; set; }
    }

    public enum BiaNodePortAlign
    {
        Start,
        Center,
        End
    }
}