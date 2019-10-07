using System.Windows;
using System.Windows.Controls;
using Biaui.Internals;

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

        static BiaEditableTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaEditableTextBlock),
                new FrameworkPropertyMetadata(typeof(BiaEditableTextBlock)));
        }
    }
}