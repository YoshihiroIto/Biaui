using System.Windows.Media;
using Biaui.Controls.NodeEditor;
using Biaui.Environment;
using Biaui.Interfaces;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using BezierSegment = SharpDX.Direct2D1.BezierSegment;
using Brush = SharpDX.Direct2D1.Brush;
using SolidColorBrush = SharpDX.Direct2D1.SolidColorBrush;
using PathGeometry = SharpDX.Direct2D1.PathGeometry;

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

            ResCache.Add("LinkBrush", t => Conv(t, Colors.DimGray));
            ResCache.Add("HighlightLinkBrush", t => Conv(t, _parent.HighlightLinkColor));

            SolidColorBrush Conv(RenderTarget t, Color src)
                => new SolidColorBrush(
                    t,
                    new RawColor4(src.R / 255.0f, src.G / 255.0f, src.B / 255.0f, src.A / 255.0f));
        }

        public override void Render(DeviceContext target)
        {
            target.Clear(new RawColor4());

            if (_parent.LinksSource == null)
                return;

            var s = (float) _parent.Scale;
            var tx = (float) _parent.TranslateTransform.X;
            var ty = (float) _parent.TranslateTransform.Y;
            target.Transform = new RawMatrix3x2(s, 0, 0, s, tx, ty);

            DrawCurves(ResCache["LinkBrush"] as Brush, false, target);
            DrawCurves(ResCache["HighlightLinkBrush"] as Brush, true, target);
        }

        private void DrawCurves(Brush brush, bool isHighlight, DeviceContext target)
        {
            var pos0 = new RawVector2();
            var seg = new BezierSegment();

            var sink = new PathGeometry(target.Factory);

            using (var geom = sink.Open())
            {
                foreach (IBiaNodeLink link in _parent.LinksSource)
                {
                    if (link.IsVisible == false)
                        continue;

                    if (link.IsLinked() == false)
                        continue;

                    if (link.IsHighlight() != isHighlight)
                        continue;

                    var bezier = link.MakeBezierCurve();

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

                target.DrawGeometry(sink, brush, LineWidth);
            }
        }
    }
}