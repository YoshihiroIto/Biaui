using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;

namespace D2dControl
{ 
    public class ResourceCache
    {
        private readonly Dictionary<int, Func<RenderTarget, object>> _generators =
            new Dictionary<int, Func<RenderTarget, object>>();

        private readonly Dictionary<int, object?> _resources = new Dictionary<int, object?>();
        private RenderTarget? _renderTarget;

        public RenderTarget RenderTarget
        {
            get => _renderTarget ?? throw new NullReferenceException();
            set
            {
                _renderTarget = value;
                UpdateResources();
            }
        }

        public int Count => _resources.Count;

        public object? this[int key] => _resources[key];

        public Dictionary<int, object?>.KeyCollection Keys => _resources.Keys;

        public Dictionary<int, object?>.ValueCollection Values => _resources.Values;

        public void Add(int key, Func<RenderTarget, object> gen)
        {
            if (_resources.TryGetValue(key, out var resOld))
            {
                Disposer.SafeDispose(ref resOld);
                _generators.Remove(key);
                _resources.Remove(key);
            }

            if (_renderTarget == null)
            {
                _generators.Add(key, gen);
                _resources.Add(key, null);
            }
            else
            {
                var res = gen(_renderTarget);
                _generators.Add(key, gen);
                _resources.Add(key, res);
            }
        }

        public void Clear()
        {
            foreach (var key in _resources.Keys)
            {
                var res = _resources[key];
                Disposer.SafeDispose(ref res);
            }

            _generators.Clear();
            _resources.Clear();
        }

        public bool ContainsKey(int key)
        {
            return _resources.ContainsKey(key);
        }

        public Dictionary<int, object?>.Enumerator GetEnumerator()
        {
            return _resources.GetEnumerator();
        }

        public bool Remove(int key)
        {
            if (_resources.TryGetValue(key, out var res))
            {
                Disposer.SafeDispose(ref res);
                _generators.Remove(key);
                _resources.Remove(key);
                return true;
            }

            return false;
        }

        public bool TryGetValue(int key, out object? res)
        {
            return _resources.TryGetValue(key, out res);
        }

        private void UpdateResources()
        {
            if (_renderTarget == null)
                return;

            foreach (var g in _generators)
            {
                var key = g.Key;
                var gen = g.Value;
                var res = gen(_renderTarget);

                if (_resources.TryGetValue(key, out var resOld))
                {
                    Disposer.SafeDispose(ref resOld);
                    _resources.Remove(key);
                }

                _resources.Add(key, res);
            }
        }
    }
}