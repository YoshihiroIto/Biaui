using System;
using Biaui.Controls.NodeEditor;
using Biaui.Environment;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace Biaui.Extension
{
    public class D2dBackgroundPanel : D2dControl.D2dControl, IBackgroundPanel
    {
        private readonly BiaNodeEditor _parent;

        public D2dBackgroundPanel(BiaNodeEditor parent)
        {
            IsAutoFrameUpdate = false;

            _parent = parent;

            ResCache.Add("RedBrush", t => new SolidColorBrush(t, new RawColor4(1.0f, 0.0f, 0.0f, 1.0f)));
            ResCache.Add("GreenBrush", t => new SolidColorBrush(t, new RawColor4(0.0f, 1.0f, 0.0f, 1.0f)));
            ResCache.Add("BlueBrush", t => new SolidColorBrush(t, new RawColor4(0.0f, 0.0f, 1.0f, 1.0f)));
        }

        public new void Invalidate()
        {
            base.Invalidate();
        }

        public override void Render(DeviceContext target)
        {
            target.Clear(new RawColor4());

            var s = (float)_parent.Scale;
            var tx = (float)_parent.TranslateTransform.X;
            var ty = (float)_parent.TranslateTransform.Y;
            target.Transform = new RawMatrix3x2(s, 0, 0,s, tx, ty);

            var rand = new Random(0);

            var result = new PathGeometry(target.Factory);

            using (var geom = result.Open())
            {
                // geom.SetFillMode(FillMode.Alternate);

                for (var i = 0; i != 100; i++)
                {
                    geom.BeginFigure(new RawVector2
                    {
                        X = rand.Next(1000),
                        Y = rand.Next(1000),
                    }, FigureBegin.Hollow);

                    var b = new BezierSegment
                    {
                        Point1 = new RawVector2
                        {
                            X = rand.Next(1000),
                            Y = rand.Next(1000)
                        },
                        Point2 = new RawVector2
                        {
                            X = rand.Next(1000),
                            Y = rand.Next(1000)
                        },
                        Point3 = new RawVector2
                        {
                            X = rand.Next(1000),
                            Y = rand.Next(1000)
                        }
                    };

                    geom.AddBezier(b);

                    geom.EndFigure(FigureEnd.Open);
                }

                geom.Close();
            }

            target.DrawGeometry(result, ResCache["BlueBrush"] as Brush, 1);
        }
    }
}