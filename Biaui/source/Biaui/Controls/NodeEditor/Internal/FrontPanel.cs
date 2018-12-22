using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Biaui.Interfaces;

namespace Biaui.Controls.NodeEditor.Internal
{
    internal class FrontPanel : Canvas
    {
        private readonly IHasTransform _transform;

        static FrontPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrontPanel),
                new FrameworkPropertyMetadata(typeof(FrontPanel)));
        }

        internal FrontPanel(IHasTransform transform)
        {
            IsHitTestVisible = false;

            _transform = transform;

            _transform.Translate.Changed += (_, __) => InvalidateVisual();
            _transform.Scale.Changed += (_, __) => InvalidateVisual();
        }

        internal class PostRenderEventArgs : EventArgs
        {
            internal DrawingContext DrawingContext { get; set; }
        }

        internal event EventHandler<PostRenderEventArgs> PostRender;

        private static readonly PostRenderEventArgs _PostArgs = new PostRenderEventArgs();

        protected override void OnRender(DrawingContext dc)
        {
            if (ActualWidth <= 1 ||
                ActualHeight <= 1)
                return;

            dc.PushTransform(_transform.Translate);
            dc.PushTransform(_transform.Scale);
            {
                _PostArgs.DrawingContext = dc;
                PostRender?.Invoke(this, _PostArgs);
            }
            dc.Pop();
            dc.Pop();
        }
    }
}