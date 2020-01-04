using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;

namespace D2dControl
{ 
    public class ResourceCache
    {
        private readonly Dictionary<int, Func<RenderTarget, object>> generators =
            new Dictionary<int, Func<RenderTarget, object>>();

        private readonly Dictionary<int, object> resources = new Dictionary<int, object>();
        private RenderTarget renderTarget;

        public RenderTarget RenderTarget
        {
            get => renderTarget;
            set
            {
                renderTarget = value;
                UpdateResources();
            }
        }

        public int Count => resources.Count;

        public object this[int key] => resources[key];

        public Dictionary<int, object>.KeyCollection Keys => resources.Keys;

        public Dictionary<int, object>.ValueCollection Values => resources.Values;

        public void Add(int key, Func<RenderTarget, object> gen)
        {
            if (resources.TryGetValue(key, out var resOld))
            {
                Disposer.SafeDispose(ref resOld);
                generators.Remove(key);
                resources.Remove(key);
            }

            if (renderTarget == null)
            {
                generators.Add(key, gen);
                resources.Add(key, null);
            }
            else
            {
                var res = gen(renderTarget);
                generators.Add(key, gen);
                resources.Add(key, res);
            }
        }

        public void Clear()
        {
            foreach (var key in resources.Keys)
            {
                var res = resources[key];
                Disposer.SafeDispose(ref res);
            }

            generators.Clear();
            resources.Clear();
        }

        public bool ContainsKey(int key)
        {
            return resources.ContainsKey(key);
        }

        public Dictionary<int, object>.Enumerator GetEnumerator()
        {
            return resources.GetEnumerator();
        }

        public bool Remove(int key)
        {
            if (resources.TryGetValue(key, out var res))
            {
                Disposer.SafeDispose(ref res);
                generators.Remove(key);
                resources.Remove(key);
                return true;
            }

            return false;
        }

        public bool TryGetValue(int key, out object res)
        {
            return resources.TryGetValue(key, out res);
        }

        private void UpdateResources()
        {
            if (renderTarget == null)
                return;

            foreach (var g in generators)
            {
                var key = g.Key;
                var gen = g.Value;
                var res = gen(renderTarget);

                if (resources.TryGetValue(key, out var resOld))
                {
                    Disposer.SafeDispose(ref resOld);
                    resources.Remove(key);
                }

                resources.Add(key, res);
            }
        }
    }
}