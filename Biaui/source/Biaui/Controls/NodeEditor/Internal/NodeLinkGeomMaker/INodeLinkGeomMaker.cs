using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Media;
using Biaui.Interfaces;
using Biaui.Internals;

namespace Biaui.Controls.NodeEditor.Internal.NodeLinkGeomMaker
{
    [Flags]
    internal enum NodeLinkGeomMakerFlags
    {
        None = 0,
        RequireArrow = 1 << 0
    }

    internal interface INodeLinkGeomMaker
    {
        void Make(
            IEnumerable linksSource,
            in ImmutableRect_double lineCullingRect,
            double alpha,
            Color backgroundColor,
            Color highlightLinkColor,
            NodeLinkGeomMakerFlags flags,
            Dictionary<(Color Color, BiaNodeLinkStyle Style, bool IsHightlight), (StreamGeometry Geom, StreamGeometryContext Ctx)> outputCurves);
    }
}