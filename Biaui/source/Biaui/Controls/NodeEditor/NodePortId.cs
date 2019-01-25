using System;
using System.Collections.Concurrent;

namespace Biaui.Controls.NodeEditor
{
    public static class NodePortId
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
            if (IdToStringDic.TryGetValue(id, out var s) == false)
                throw new Exception();

            return s;
        }

        private static readonly ConcurrentDictionary<int, string> IdToStringDic = new ConcurrentDictionary<int, string>();
    }
}

