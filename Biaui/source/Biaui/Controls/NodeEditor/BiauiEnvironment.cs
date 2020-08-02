using Biaui.Controls.NodeEditor.Environment;
using Biaui.Controls.NodeEditor.Internal;

namespace Biaui.Controls.NodeEditor
{
    public static class BiauiEnvironment
    {
        public static IBackgroundPanelGenerator BackgroundPanelGenerator
        {
            get => _BackgroundPanelGenerator ??= new DefaultBackgroundPanelGenerator();
            private set => _BackgroundPanelGenerator = value;
        }
        private static IBackgroundPanelGenerator? _BackgroundPanelGenerator;

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