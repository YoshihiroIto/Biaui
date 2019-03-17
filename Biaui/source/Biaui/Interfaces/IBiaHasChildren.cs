using System.Collections;
using System.Collections.Specialized;

namespace Biaui.Interfaces
{
    public interface IBiaHasChildren
    {
        IEnumerable Children { get; }
    }
}