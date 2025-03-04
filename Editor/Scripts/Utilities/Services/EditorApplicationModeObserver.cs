using System;
using UnityEditor;
using UnityEngine;

namespace IKGTools.Editor.Services
{
    public sealed class EditorApplicationModeObserver
    {
        private bool _isPlaying;
        
        public event Action<bool> OnChangedPlayMode;

        public EditorApplicationModeObserver()
        {
            _isPlaying = Application.isPlaying;
            EditorApplication.update += OnEditorUpdate;
        }

        private void OnEditorUpdate()
        {
            if (Application.isPlaying != _isPlaying)
            {
                _isPlaying = Application.isPlaying;
                OnChangedPlayMode?.Invoke(_isPlaying);
            }
        }
    }
}