using System.Collections;

namespace Biaui.Interfaces;

public interface IBiaHasChildren
{
    IEnumerable Children { get; }
}