using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace Biaui.Internals
{
    public class SharedResourceDictionary : ResourceDictionary
    {
        private static readonly Dictionary<Uri, ResourceDictionary> _sharedDictionaries = new Dictionary<Uri, ResourceDictionary>();

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

                if (_sharedDictionaries.TryGetValue(value, out var v) == false)
                {
                    base.Source = value;

                    _sharedDictionaries.Add(value, this);
                }
                else
                {
                    MergedDictionaries.Add(v);
                }
            }
        }
    }
}