using UnityEditor;
using UnityEngine.UIElements;

namespace IKGTools.SplineBones.Editor
{
    [CustomEditor(typeof(SplineBindingsData))]
    internal sealed class SplineBindingsDataEditor : UnityEditor.Editor
    {
        private SplineBindingsData _component;

        private PointBindingsEditor _pointBindingsEditor;

        private void OnEnable()
        {
            _component = target as SplineBindingsData;
            
            _pointBindingsEditor = new PointBindingsEditor(_component);
            EditorApplication.update += UpdateGUIItems;
        }

        private void OnDisable()
        {
            EditorApplication.update -= UpdateGUIItems;
            _pointBindingsEditor.OnHide();
        }

        private void UpdateGUIItems()
        {
            _pointBindingsEditor.OnGUI();
        }


        public override VisualElement CreateInspectorGUI()
        {
            VisualElement container = new VisualElement();
            
            container.Add(_pointBindingsEditor.CreateVisualElements());
            //todo add logic for create points array
            return container;
        }
    }
}