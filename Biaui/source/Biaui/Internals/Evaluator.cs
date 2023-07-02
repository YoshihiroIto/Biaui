using System.Data;

namespace Biaui.Internals;

internal static class Evaluator
{
    internal static string Eval(string statement)
    {
        try
        {
            var result = _dataTable.Compute(statement,"");
            return result.ToString() ?? "";
        }
        catch
        {
            return "";
        }
    }

    private static readonly DataTable _dataTable = new ();
}
