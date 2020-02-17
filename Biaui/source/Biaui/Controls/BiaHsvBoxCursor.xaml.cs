using System.Windows;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaHsvBoxCursor : FrameworkElement
    {
        #region BorderColor

        public ByteColor BorderColor
        {
            get => _BorderColor;
            set
            {
                if (value != _BorderColor)
                    SetValue(BorderColorProperty, value);
            }
        }

        private ByteColor _BorderColor = ByteColor.Red;

        public static readonly DependencyProperty BorderColorProperty =
            DependencyProperty.Register(nameof(BorderColor), typeof(ByteColor), typeof(BiaHsvBoxCursor),
                new FrameworkPropertyMetadata(
                    Boxes.ByteColorRed,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaHsvBoxCursor) s;
                        self._BorderColor = (ByteColor) e.NewValue;
                    }));

        #endregion

        #region Hue

        public double Hue
        {
            get => _Hue;
            set
            {
                if (NumberHelper.AreClose(value, _Hue) == false)
                    SetValue(HueProperty, Boxes.Double(value));
            }
        }

        private double _Hue;

        public static readonly DependencyProperty HueProperty =
            DependencyProperty.Register(nameof(Hue), typeof(double), typeof(BiaHsvBoxCursor),
                new FrameworkPropertyMetadata(
                    Boxes.Double0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaHsvBoxCursor) s;
                        self._Hue = (double) e.NewValue;
                    }));

        #endregion

        #region Saturation

        public double Saturation
        {
            get => _Saturation;
            set
            {
                if (NumberHelper.AreClose(value, _Saturation) == false)
                    SetValue(SaturationProperty, Boxes.Double(value));
            }
        }

        private double _Saturation;

        public static readonly DependencyProperty SaturationProperty =
            DependencyProperty.Register(nameof(Saturation), typeof(double), typeof(BiaHsvBoxCursor),
                new FrameworkPropertyMetadata(
                    Boxes.Double0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaHsvBoxCursor) s;
                        self._Saturation = (double) e.NewValue;
                    }));

        #endregion

        #region IsReadOnly

        public bool IsReadOnly
        {
            get => _IsReadOnly;
            set
            {
                if (value != _IsReadOnly)
                    SetValue(IsReadOnlyProperty, Boxes.Bool(value));
            }
        }

        private bool _IsReadOnly;

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(BiaHsvBoxCursor),
                new FrameworkPropertyMetadata(
                    Boxes.BoolFalse,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaHsvBoxCursor) s;
                        self._IsReadOnly = (bool) e.NewValue;
                    }));

        #endregion

        static BiaHsvBoxCursor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaHsvBoxCursor),
                new FrameworkPropertyMetadata(typeof(BiaHsvBoxCursor)));
        }

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly PropertyChangeNotifier _isEnabledChangeNotifier;

        public BiaHsvBoxCursor()
        {
            IsHitTestVisible = false;

            _isEnabledChangeNotifier = new PropertyChangeNotifier(this, IsEnabledProperty);
            _isEnabledChangeNotifier.ValueChanged += (_, __) => InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;
            
            var rounder = new LayoutRounder(this);

            // Border
            {
                var p = rounder.GetBorderPen(BorderColor);

                var w = rounder.RoundLayoutValue(ActualWidth - 0.5);
                var h = rounder.RoundLayoutValue(ActualHeight - 0.5);
                var z = rounder.RoundLayoutValue(0.5);

                var p0 = new Point(z, z);
                var p1 = new Point(w, z);
                var p2 = new Point(z, h);
                var p3 = new Point(w, h);

                var p0A = new Point(z - 0.5, z);
                var p1A = new Point(w + 0.5, z);
                var p2A = new Point(z - 0.5, h);
                var p3A = new Point(w + 0.5, h);

                dc.DrawLine(p, p0A, p1A);
                dc.DrawLine(p, p1, p3);
                dc.DrawLine(p, p3A, p2A);
                dc.DrawLine(p, p2, p0);
            }

            // Cursor
            this.DrawPointCursor(rounder, dc, MakeCursorRenderPos(rounder), IsEnabled, IsReadOnly);
        }

        private ImmutableVec2_double MakeCursorRenderPos(in LayoutRounder rounder) =>
            MakeCursorRenderPos(rounder, ActualWidth, ActualHeight, Hue, Saturation);

        internal static ImmutableVec2_double MakeCursorRenderPos(
            in LayoutRounder rounder,
            double actualWidth,
            double actualHeight,
            double hue,
            double saturation)
        {
            hue = NumberHelper.Clamp01(hue);
            saturation = NumberHelper.Clamp01(saturation);

            var bw = rounder.RoundLayoutValue(FrameworkElementExtensions.BorderWidth);
            var w = rounder.RoundLayoutValue(actualWidth - bw * 2);
            var h = rounder.RoundLayoutValue(actualHeight - bw * 2);

            var x = hue * w + bw;
            var y = (1 - saturation) * h + bw;

            return new ImmutableVec2_double(rounder.RoundLayoutValue(x), rounder.RoundLayoutValue(y));
        }
    }
}