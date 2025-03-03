using System;
using UnityEngine.UIElements;

namespace IKGTools.SplineBones.Editor
{
    internal abstract class BaseEditorUI
    {
        protected VisualElement _rootContainer;
        public event Action OnRepaintCall;
        public VisualElement CreateVisualElements()
        {
            _rootContainer = new VisualElement();
            _rootContainer.Add(CreateCustomVisualElements());
            OnShow();
            return _rootContainer;
        }

        protected abstract VisualElement CreateCustomVisualElements();
        protected virtual void OnShow(){}
        public virtual void OnGUI(){}
        public virtual void OnHide(){}

        protected void CallRepaint()
        {
            OnRepaintCall?.Invoke();
        }
    }
}