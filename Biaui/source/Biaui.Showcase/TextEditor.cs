using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace Biaui.Showcase
{
    public class TextEditor : ICSharpCode.AvalonEdit.TextEditor, INotifyPropertyChanged
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextEditor),
                new PropertyMetadata((obj, args) =>
                {
                    var target = (TextEditor) obj;
                    target.Text = (string) args.NewValue;
                })
            );

        public new string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public TextEditor()
        {
            TextArea.SelectionCornerRadius = 0;
            TextArea.SelectionBorder = null;

            using var reader = new XmlTextReader(new MemoryStream(Properties.Resources.CSharp_Mode));
            SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            PropertyChanged?.Invoke(this, _TextChanged);

            base.OnTextChanged(e);
        }

        private static readonly PropertyChangedEventArgs _TextChanged = new PropertyChangedEventArgs(nameof(Text));
    }
}