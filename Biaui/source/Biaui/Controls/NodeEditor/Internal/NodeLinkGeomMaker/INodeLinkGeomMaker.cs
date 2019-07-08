using System.Collections;
using System.Collections.Generic;
using System.Windows.Media;
using Biaui.Interfaces;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal.NodeLinkGeomMaker
{
    internal interface INodeLinkGeomMaker
    {
        void Make(
            IEnumerable linksSource,
            in ImmutableRect lineCullingRect,
            double alpha,
            Color backgroundColor,
            Color highlightLinkColor,
            Dictionary<(Color Color, BiaNodeLinkStyle Style, bool IsHightlight), (StreamGeometry Geom, StreamGeometryContext Ctx)> outputCurves);
    }
}