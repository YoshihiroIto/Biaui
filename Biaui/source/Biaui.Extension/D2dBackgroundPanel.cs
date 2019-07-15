using System.Runtime.CompilerServices;
using System.Windows;
using Biaui.Controls.Internals;
using Biaui.Controls.NodeEditor;
using Biaui.Controls.NodeEditor.Internal;
using Biaui.Environment;
using Biaui.Interfaces;
using Biaui.Internals;
using Jewelry.Collections;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace Biaui.Extension
{
    public class D2dBackgroundPanel : D2dControl.D2dControl, IBackgroundPanel
    {
        private readonly BiaNodeEditor _parent;

        private const float LineWidth = 3.0f;
        private const float ArrowSize = 20.0f;

        public D2dBackgroundPanel(BiaNodeEditor parent)
        {
            IsAutoFrameUpdate = false;

            _parent = parent;

            ResCache.Add("RedBrush", t => new SolidColorBrush(t, new RawColor4(1.0f, 0.0f, 0.0f, 1.0f)));
            ResCache.Add("GreenBrush", t => new SolidColorBrush(t, new RawColor4(0.0f, 1.0f, 0.0f, 1.0f)));
            ResCache.Add("BlueBrush", t => new SolidColorBrush(t, new RawColor4(0.0f, 0.0f, 1.0f, 1.0f)));
            ResCache.Add("WhiteBrush", t => new SolidColorBrush(t, new RawColor4(1.0f, 1.0f, 1.0f, 1.0f)));
        }

        public new void Invalidate()
        {
            base.Invalidate();
        }

        public override void Render(DeviceContext target)
        {
            target.Clear(new RawColor4());

            if (_parent.LinksSource == null)
                return;

            var inflate = ArrowSize * _parent.ScaleTransform.ScaleX;
            var viewport = _parent.TransformRect(ActualWidth, ActualHeight);
            var lineCullingRect = new ImmutableRect(
                viewport.X - inflate,
                viewport.Y - inflate,
                viewport.Width + inflate * 2,
                viewport.Height + inflate * 2
            );

            var s = (float) _parent.Scale;
            var tx = (float) _parent.TranslateTransform.X;
            var ty = (float) _parent.TranslateTransform.Y;
            target.Transform = new RawMatrix3x2(s, 0, 0, s, tx, ty);

            var sink = new PathGeometry(target.Factory);

            using (var geom = sink.Open())
            {
                var pos0 = new RawVector2();
                var seg = new BezierSegment();

                foreach (IBiaNodeLink link in _parent.LinksSource)
                {
                    if (link.IsVisible == false)
                        continue;

                    if (link.IsLinked() == false)
                        continue;

                    var bezier = link.MakeBezierCurve();

                    var bb = _boundingBoxCache.GetOrAdd(bezier.GetHashCode(),
                        _ =>
                            BiaNodeEditorHelper.MakeBoundingBox(
                                Unsafe.As<Point, ImmutableVec2>(ref bezier.Item1),
                                Unsafe.As<Point, ImmutableVec2>(ref bezier.Item2),
                                Unsafe.As<Point, ImmutableVec2>(ref bezier.Item3),
                                Unsafe.As<Point, ImmutableVec2>(ref bezier.Item4)));

                    if (bb.IntersectsWith(lineCullingRect) == false)
                        continue;

                    pos0.X = (float) bezier.Item1.X;
                    pos0.Y = (float) bezier.Item1.Y;
                    seg.Point1.X = (float) bezier.Item2.X;
                    seg.Point1.Y = (float) bezier.Item2.Y;
                    seg.Point2.X = (float) bezier.Item3.X;
                    seg.Point2.Y = (float) bezier.Item3.Y;
                    seg.Point3.X = (float) bezier.Item4.X;
                    seg.Point3.Y = (float) bezier.Item4.Y;

                    geom.BeginFigure(pos0, FigureBegin.Hollow);
                    geom.AddBezier(seg);

                    geom.EndFigure(FigureEnd.Open);
                }

                geom.Close();

                target.DrawGeometry(sink, ResCache["WhiteBrush"] as Brush, LineWidth);
            }
        }

        private readonly LruCache<int, ImmutableRect> _boundingBoxCache = new LruCache<int, ImmutableRect>(10000, false);
    }
}