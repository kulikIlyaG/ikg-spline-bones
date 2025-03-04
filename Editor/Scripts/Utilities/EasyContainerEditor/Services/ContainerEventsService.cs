using System;
using UnityEditor;
using UnityEngine;

namespace IKGTools.Editor.EasyContainer
{
    internal sealed class ContainerEventsService
    {
        public event Action OnTick;
        public event Action<SceneView> OnTickSceneGUI;

        public ContainerEventsService()
        {
            EditorApplication.update += OnEditorUpdate;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnSceneGUI(SceneView view)
        {
            OnTickSceneGUI?.Invoke(view);
        }

        private void OnEditorUpdate()
        {
            OnTick?.Invoke();
        }
    }
}