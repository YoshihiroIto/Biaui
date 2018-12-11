using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal class GridPanel : Canvas
    {
        private readonly TranslateTransform _translate;
        private readonly ScaleTransform _scale;

        internal GridPanel(TranslateTransform translate, ScaleTransform scale)
        {
            _translate = translate;
            _scale = scale;

            _translate.Changed += (_, __) => InvalidateVisual();
            _scale.Changed += (_, __) => InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            const double unit = 1024;

            var p = this.GetBorderPen(Color.FromRgb(0x37, 0x37, 0x40));

            var s = _scale.ScaleX;
            var tx = _translate.X;
            var ty = _translate.Y;

            var bx = FrameworkElementHelper.RoundLayoutValue(ActualWidth);
            var by = FrameworkElementHelper.RoundLayoutValue(ActualHeight);

            for (var h = 0;; ++h)
            {
                var x = (h * unit) * s + tx;

                x = FrameworkElementHelper.RoundLayoutValue(x);

                if (x < 0) continue;
                if (x > ActualWidth) break;

                dc.DrawLine(p, new Point(x, 0), new Point(x, by));
            }

            for (var h = 0;; --h)
            {
                var x = (h * unit) * s + tx;

                x = FrameworkElementHelper.RoundLayoutValue(x);

                if (x > ActualWidth) continue;
                if (x < 0) break;

                dc.DrawLine(p, new Point(x, 0), new Point(x, by));
            }

            for (var v = 0;; ++v)
            {
                var y = (v * unit) * s + ty;

                y = FrameworkElementHelper.RoundLayoutValue(y);

                if (y < 0) continue;
                if (y > ActualHeight) break;

                dc.DrawLine(p, new Point(0, y), new Point(bx, y));
            }


            for (var v = 0;; --v)
            {
                var y = (v * unit) * s + ty;

                y = FrameworkElementHelper.RoundLayoutValue(y);

                if (y > ActualHeight) continue;
                if (y < 0) break;

                dc.DrawLine(p, new Point(0, y), new Point(bx, y));
            }
        }
    }
}