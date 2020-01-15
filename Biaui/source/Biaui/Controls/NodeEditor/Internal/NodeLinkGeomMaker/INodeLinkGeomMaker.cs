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
            ImmutableByteColor backgroundColor,
            ImmutableByteColor highlightLinkColor,
            NodeLinkGeomMakerFlags flags,
            Dictionary<(ImmutableByteColor Color, BiaNodeLinkStyle Style, bool IsHightlight), (StreamGeometry Geom, StreamGeometryContext Ctx)> outputCurves);
    }
}