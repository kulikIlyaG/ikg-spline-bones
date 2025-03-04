using System;
using System.Collections.Generic;
using IKGTools.Editor.Utilities.UI.UIToolkit;
using UnityEngine;
using UnityEngine.UIElements;

namespace IKGTools.Editor.Utilities.UIToolkit
{
    public sealed class ToggleButton : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<ToggleButton, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription m_Text = new()
                {name = "text", defaultValue = "Default Text"};


            private readonly UxmlBoolAttributeDescription m_Value = new() {name = "value", defaultValue = false};

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var customElement = (ToggleButton) ve;
                customElement.SetLabel(m_Text.GetValueFromBag(bag, cc));
                customElement.SetValue(m_Value.GetValueFromBag(bag, cc));
            }
        }

        private static readonly StyleColor ENABLED_COLOR = new StyleColor(new Color32(81, 150, 84, 255));
        private static readonly StyleColor DISABLED_COLOR = new StyleColor(new Color32(85,85,85,255));
        
        
        private static readonly StyleColor ENABLED_BORDER_COLOR = new StyleColor(new Color32(93, 170, 90, 255));
        private static readonly StyleColor DISABLED_BORDER_COLOR = new StyleColor(new Color32(95, 95, 95, 255));

        private readonly Label _label;

        public bool IsInteractable => pickingMode == PickingMode.Position;

        private bool _value;

        public event Action<bool> OnChangedValue;

        public ToggleButton()
        {
            _value = false;
            
            _label = new Label("Toggle Button");
            Add(_label);

            SetupStyles();

            UpdateView();
            
            RegisterCallback<PointerUpEvent>(OnClickEvent);
        }
        
        public ToggleButton(string label, bool value, bool interactable = true)
        {
            _value = value;
            
            _label = new Label(label);
            _label.pickingMode = PickingMode.Ignore;
            Add(_label);
            
            SetupStyles();
            
            SetInteractable(interactable);

            UpdateView();
            
            RegisterCallback<PointerUpEvent>(OnClickEvent);
        }

        public string Label => _label.text;
        public bool Value => _value;

        private void SetupStyles()
        {
            style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue>{new TimeValue(0.1f)});
            style.SetBorders(GetBordersColor(), 2, _value ? 7 : 5);
            
            style.alignContent = new StyleEnum<Align>(Align.Center);
            
            _label.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
            _label.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter);
        }

        
        private void UpdateView()
        {
            style.backgroundColor = GetBackgroundColor();
            style.SetBorders(GetBordersColor(), 2, _value ? 7 : 5);
        }

        private StyleColor GetBackgroundColor()
        {
            return _value ? ENABLED_COLOR : DISABLED_COLOR;
        }

        private StyleColor GetBordersColor()
        {
            return _value ? ENABLED_BORDER_COLOR : DISABLED_BORDER_COLOR;
        }

        private void OnClickEvent(PointerUpEvent evt)
        {
            SetValue(!_value);
        }

        public void SetValue(bool value, bool muted = false)
        {
            _value = value;
            UpdateView();
            if(!muted)
                OnChangedValue?.Invoke(_value);
        }
        
        public void SetLabel(string value)
        {
            _label.text = value;
        }

        public void SetInteractable(bool interactable)
        {
            pickingMode = interactable ? PickingMode.Position : PickingMode.Ignore;
            style.opacity = new StyleFloat(interactable ? 1f : 0.5f);
        }
    }
}