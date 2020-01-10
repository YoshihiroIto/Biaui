using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Biaui.Controls.NodeEditor
{
    public static class NodeSlotId
    {
        public static int Make(string v)
        {
             var id = v.GetHashCode();

            if(IdToStringDic.ContainsKey(id) == false)
                 IdToStringDic[id] = v;

             return id;
        }

        public static string ToString(int id)
        {
            if (IdToStringDic.TryGetValue(id, out var v) == false)
                throw new Exception();

            Debug.Assert(v != null);

            return v;
        }

        private static readonly ConcurrentDictionary<int, string> IdToStringDic = new ConcurrentDictionary<int, string>();
    }
}

