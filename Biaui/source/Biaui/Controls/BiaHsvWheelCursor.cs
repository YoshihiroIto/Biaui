using System;
using System.Windows;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaHsvWheelCursor : FrameworkElement
    {
        #region BorderColor

        public Color BorderColor
        {
            get => _BorderColor;
            set
            {
                if (value != _BorderColor)
                    SetValue(BorderColorProperty, value);
            }
        }

        private Color _BorderColor = Colors.Red;

        public static readonly DependencyProperty BorderColorProperty =
            DependencyProperty.Register(nameof(BorderColor), typeof(Color), typeof(BiaHsvWheelCursor),
                new FrameworkPropertyMetadata(
                    Boxes.ColorRed,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaHsvWheelCursor) s;
                        self._BorderColor = (Color) e.NewValue;
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
            DependencyProperty.Register(nameof(Hue), typeof(double), typeof(BiaHsvWheelCursor),
                new FrameworkPropertyMetadata(
                    Boxes.Double0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaHsvWheelCursor) s;
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
            DependencyProperty.Register(nameof(Saturation), typeof(double), typeof(BiaHsvWheelCursor),
                new FrameworkPropertyMetadata(
                    Boxes.Double0,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaHsvWheelCursor) s;
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
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(BiaHsvWheelCursor),
                new FrameworkPropertyMetadata(
                    Boxes.BoolFalse,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaHsvWheelCursor) s;
                        self._IsReadOnly = (bool) e.NewValue;
                    }));

        #endregion

        static BiaHsvWheelCursor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaHsvWheelCursor),
                new FrameworkPropertyMetadata(typeof(BiaHsvWheelCursor)));
        }

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly PropertyChangeNotifier _isEnabledChangeNotifier;

        public BiaHsvWheelCursor()
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

            // Cursor
            this.DrawPointCursor(dc, CursorRenderPos, IsEnabled, IsReadOnly);
        }

        private ImmutableVec2 CursorRenderPos =>
            MakeCursorRenderPos(this, ActualWidth, ActualHeight, Hue, Saturation);

        internal static ImmutableVec2 MakeCursorRenderPos(
            Visual visual,
            double actualWidth,
            double actualHeight,
            double hue,
            double saturation)
        {
            hue = NumberHelper.Clamp01(hue);
            saturation = NumberHelper.Clamp01(saturation);

            var bw = visual.RoundLayoutValue(FrameworkElementExtensions.BorderWidth);
            var w = visual.RoundLayoutValue(actualWidth - bw * 2);
            var h = visual.RoundLayoutValue(actualHeight - bw * 2);

            var r = hue * 2.0 * Math.PI;

            var (cx, cy) = MakeAspectRatioCorrection(actualWidth, actualHeight);

            var x = bw + Math.Cos(r) * saturation * (w * 0.5) / cx + w * 0.5;
            var y = bw + Math.Sin(r) * saturation * (h * 0.5) / cy + h * 0.5;

            return new ImmutableVec2(visual.RoundLayoutValue(x), visual.RoundLayoutValue(y));
        }

        internal static (double X, double Y) MakeAspectRatioCorrection(double actualWidth, double actualHeight)
            => actualWidth > actualHeight
                ? (actualWidth / actualHeight, 1.0)
                : (1.0, actualHeight / actualWidth);
    }
}