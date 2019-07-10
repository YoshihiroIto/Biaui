using System.Windows;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls.Internals
{
    internal interface IHasTransform
    {
        ScaleTransform ScaleTransform { get; }

        TranslateTransform TranslateTransform { get; }
    }

    internal static class HasTransformExtensions
    {
        internal static Point TransformPos(this IHasTransform self, double x, double y)
        {
            var s = self.ScaleTransform.ScaleX;

            return new Point(
                (x - self.TranslateTransform.X) / s,
                (y - self.TranslateTransform.Y) / s);
        }

        internal static ImmutableRect TransformRect(this IHasTransform self, double w, double h)
        {
            var s = self.ScaleTransform.ScaleX;

            return new ImmutableRect(
                -self.TranslateTransform.X / s,
                -self.TranslateTransform.Y / s,
                w / s,
                h / s);
        }

        internal static ImmutableRect TransformRect(this IHasTransform self, in ImmutableRect rect)
        {
            var s = self.ScaleTransform.ScaleX;

            return new ImmutableRect(
                (rect.X - self.TranslateTransform.X) / s,
                (rect.Y - self.TranslateTransform.Y) / s,
                rect.Width / s,
                rect.Height / s);
        }

        internal static void SetTransform(this IHasTransform self, double scale, double centerX, double centerY)
        {
            var d0 = self.TransformPos(centerX, centerY);

            self.ScaleTransform.ScaleX = scale;
            self.ScaleTransform.ScaleY = scale;

            var d1 = self.TransformPos(centerX, centerY);

            var diff = d1 - d0;

            self.TranslateTransform.X += diff.X * scale;
            self.TranslateTransform.Y += diff.Y * scale;
        }
    }
}