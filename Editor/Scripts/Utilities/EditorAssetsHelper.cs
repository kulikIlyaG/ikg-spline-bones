using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IKGTools.Editor.Utilities
{
    [InitializeOnLoad]
    internal static class EditorAssetsHelper
    {
        private static Dictionary<string, Object> _loaded;

        static EditorAssetsHelper()
        {
            _loaded = new Dictionary<string, Object>(50);
        }

        public static T Get<T>(string path) where T : Object
        {
            if (!_loaded.ContainsKey(path))
            {
                T load = AssetDatabase.LoadAssetAtPath<T>(path);
                _loaded.Add(path, load);
            }

            return _loaded[path] as T;
        }
    }
}