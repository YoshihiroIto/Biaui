using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Biaui.Controls
{
    public class BiaEditableTextBlock : Control
    {
        #region IsEditing
        
        public bool IsEditing
        {
            get => _IsEditing;
            set
            {
                if (value != _IsEditing)
                    SetValue(IsEditingProperty, Boxes.Bool(value));
            }
        }
        
        private bool _IsEditing;
        
        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.Register(
                nameof(IsEditing),
                typeof(bool),
                typeof(BiaEditableTextBlock),
                new PropertyMetadata(
                    Boxes.BoolFalse,
                    (s, e) =>
                    {
                        var self = (BiaEditableTextBlock) s;
                        self._IsEditing = (bool)e.NewValue;
                    }));
        
        #endregion

        #region Text
        
        public string? Text
        {
            get => _Text;
            set
            {
                if (value != _Text)
                    SetValue(TextProperty, value);
            }
        }
        
        private string? _Text;
        
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(BiaEditableTextBlock),
                new PropertyMetadata(
                    default,
                    (s, e) =>
                    {
                        var self = (BiaEditableTextBlock) s;
                        self._Text = (string)e.NewValue;
                    }));
        
        #endregion

        #region Watermark

        public string? Watermark
        {
            get => _Watermark;
            set
            {
                if (value != _Watermark)
                    SetValue(WatermarkProperty, value);
            }
        }

        private string? _Watermark;

        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register(nameof(Watermark), typeof(string), typeof(BiaEditableTextBlock),
                new FrameworkPropertyMetadata(
                    default(string),
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaEditableTextBlock) s;
                        self._Watermark = (string) e.NewValue;
                    }));

        #endregion

        #region WatermarkForeground

        public Brush? WatermarkForeground
        {
            get => _WatermarkForeground;
            set
            {
                if (value != _WatermarkForeground)
                    SetValue(WatermarkForegroundProperty, value);
            }
        }

        private Brush? _WatermarkForeground;

        public static readonly DependencyProperty WatermarkForegroundProperty =
            DependencyProperty.Register(nameof(WatermarkForeground), typeof(Brush), typeof(BiaEditableTextBlock),
                new FrameworkPropertyMetadata(
                    default(Brush),
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    (s, e) =>
                    {
                        var self = (BiaEditableTextBlock) s;
                        self._WatermarkForeground = (Brush) e.NewValue;
                    }));

        #endregion

        static BiaEditableTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaEditableTextBlock),
                new FrameworkPropertyMetadata(typeof(BiaEditableTextBlock)));
        }
    }
}