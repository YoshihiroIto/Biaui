using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace Biaui.Internals
{
    public class SharedResourceDictionary : ResourceDictionary
    {
        private static readonly Dictionary<int, ResourceDictionary> _sharedDictionaries = new Dictionary<int, ResourceDictionary>();

        private static readonly bool _isInDesignerMode;

        private Uri _sourceUri;

        static SharedResourceDictionary()
        {
            _isInDesignerMode = (bool) DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;
        }

        public new Uri Source
        {
            get => _sourceUri ?? throw new NullReferenceException();

            set
            {
                _sourceUri = value;

                if (_isInDesignerMode)
                {
                    base.Source = value;
                    return;
                }

                var hashCode = value.GetHashCode();

                if (_sharedDictionaries.TryGetValue(hashCode, out var v) == false)
                {
                    base.Source = value;

                    _sharedDictionaries.Add(hashCode, this);
                }
                else
                {
                    MergedDictionaries.Add(v);
                }
            }
        }
    }
}