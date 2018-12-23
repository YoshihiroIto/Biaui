using System.Windows;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls.Internals
{
    internal interface IHasTransform
    {
        ScaleTransform Scale { get; }

        TranslateTransform Translate { get; }
    }

    internal static class HasTransformExtensions
    {
        internal static Point MakeScenePosFromControlPos(this IHasTransform self, Point pos)
            => MakeScenePosFromControlPos(self, pos.X, pos.Y);

        internal static Point MakeScenePosFromControlPos(this IHasTransform self, double x, double y)
        {
            var s = self.Scale.ScaleX;

            return new Point(
                (x - self.Translate.X) / s,
                (y - self.Translate.Y) / s);
        }

        internal static ImmutableRect MakeSceneRectFromControlPos(this IHasTransform self, double w, double h)
        {
            var s = self.Scale.ScaleX;

            return new ImmutableRect(
                -self.Translate.X / s,
                -self.Translate.Y / s,
                w / s,
                h / s);
        }
    }
}