using System.Windows;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal class BoxSelector : FrameworkElement
    {
        #region Rect
        
        public ImmutableRect Rect
        {
            get => _Rect;
            set
            {
                if (value != _Rect)
                    SetValue(RectProperty, value);
            }
        }
        
        private ImmutableRect _Rect;
        
        public static readonly DependencyProperty RectProperty =
            DependencyProperty.Register(nameof(Rect), typeof(ImmutableRect), typeof(BoxSelector),
                new FrameworkPropertyMetadata(
                    Boxes.ImmutableRect0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BoxSelector) s;
                        self._Rect = (ImmutableRect)e.NewValue;
                    }));
        
        #endregion

        static BoxSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BoxSelector),
                new FrameworkPropertyMetadata(typeof(BoxSelector)));
        }
        
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