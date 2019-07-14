using System.Windows.Controls;
using Biaui.Controls.NodeEditor;
using Biaui.Controls.NodeEditor.Internal;
using Biaui.Environment;

namespace Biaui
{
    public static class BiauiEnvironment
    {
        public static IBackgroundPanelGenerator BackgroundPanelGenerator { get; private set; }
            = new DefaultBackgroundPanelGenerator();

        public static void RegisterBackgroundPanelGenerator(IBackgroundPanelGenerator generator)
        {
            BackgroundPanelGenerator = generator;
        }

        private class DefaultBackgroundPanelGenerator : IBackgroundPanelGenerator
        {
            public IBackgroundPanel Generate(BiaNodeEditor parent)
                => new DefaultBackgroundPanel(parent);
        }
    }
}