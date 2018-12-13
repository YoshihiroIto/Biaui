using System;
using System.Windows;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaTextBlock : FrameworkElement
    {
        #region Text

        public string Text
        {
            get => _Text;
            set
            {
                if (value != _Text)
                    SetValue(TextProperty, value);
            }
        }

        private string _Text;

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(BiaTextBlock),
                new FrameworkPropertyMetadata(
                    default(string),
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaTextBlock) s;
                        self._Text = (string) e.NewValue;
                        self.UpdateSize();
                    }));

        #endregion

        #region Foreground

        public Brush Foreground
        {
            get => _Foreground;
            set
            {
                if (value != _Foreground)
                    SetValue(ForegroundProperty, value);
            }
        }

        private Brush _Foreground;

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(BiaTextBlock),
                new PropertyMetadata(
                    default(Brush),
                    (s, e) =>
                    {
                        var self = (BiaTextBlock) s;
                        self._Foreground = (Brush) e.NewValue;
                    }));

        #endregion

        static BiaTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaTextBlock),
                new FrameworkPropertyMetadata(typeof(BiaTextBlock)));
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;

            TextRenderer.Default.Draw(
                Text,
                0, 0,
                Foreground,
                dc,
                ActualWidth,
                TextAlignment.Left
            );
        }

        private double _textWidth;

        protected override Size MeasureOverride(Size constraint)
        {
            return new Size(Math.Min(constraint.Width, _textWidth), TextRenderer.Default.FontHeight);
        }

        private void UpdateSize()
        {
            // ※  1は誤差吸収するため。
            _textWidth = + FrameworkElementHelper.RoundLayoutValue(TextRenderer.Default.CalcWidth(Text) + 1);
        }
    }
}