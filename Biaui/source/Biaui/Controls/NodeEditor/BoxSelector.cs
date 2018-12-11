using System.Windows;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor
{
    internal class BoxSelector : FrameworkElement
    {
        #region Rect
        
        public Rect Rect
        {
            get => _Rect;
            set
            {
                if (value != _Rect)
                    SetValue(RectProperty, value);
            }
        }
        
        private Rect _Rect = default(Rect);
        
        public static readonly DependencyProperty RectProperty =
            DependencyProperty.Register(nameof(Rect), typeof(Rect), typeof(BoxSelector),
                new FrameworkPropertyMetadata(
                    Boxes.Rect0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BoxSelector) s;
                        self._Rect = (Rect)e.NewValue;
                    }));
        
        #endregion
        
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (Rect.Width <= 0 || Rect.Height <= 0)
                return;

            var b = Caches.GetSolidColorBrush(Color.FromArgb(0x3F, 0x41, 0x69, 0xE1));
            var p = this.GetBorderPen(Color.FromArgb(0xFF, 0x41, 0x69, 0xE1));

            dc.DrawRectangle(b, p, FrameworkElementHelper.RoundLayoutRect(Rect));
        }
    }
}