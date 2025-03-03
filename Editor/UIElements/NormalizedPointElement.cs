using IKGTools.SplineBones.Editor.Utilities.UIToolkit;
using UnityEngine;
using UnityEngine.UIElements;

namespace IKGTools.SplineBones.Editor
{
    internal sealed class NormalizedPointElement : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<NormalizedPointElement, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription m_Text = new()
                {name = "text", defaultValue = "Default Text"};
            private readonly UxmlFloatAttributeDescription m_Horizontal =  new()
                {name = "X Position", defaultValue = 0};
            private readonly UxmlFloatAttributeDescription m_Vertical =  new()
                {name = "Y Position", defaultValue = 0};


            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var customElement = (NormalizedPointElement) ve;
                customElement.SetLabel(m_Text.GetValueFromBag(bag, cc));
                customElement.SetNormalizedPosition(new Vector2(Mathf.Clamp01(m_Horizontal.GetValueFromBag(bag, cc)), Mathf.Clamp01(m_Vertical.GetValueFromBag(bag, cc))));
            }
        }

        private readonly Color _color = new Color32(255, 141, 61, 255);
        private readonly Label _label;
        private readonly VisualElement _point;

        private Vector2 _normalizedPosition;

        public NormalizedPointElement()
        {
            _point = new VisualElement();
            Add(_point);
            
            _label = new Label("label");
            Add(_label);
            
            SetupStyles();
        }
        
        public NormalizedPointElement(string label, Vector2 normalizedPosition, Color color)
        {
            _color = color;
            _point = new VisualElement();
            Add(_point);
            
            _label = new Label("label");
            Add(_label);
            
            SetupStyles();
            SetNormalizedPosition(normalizedPosition);
            SetLabel(label);
        }

        private void SetupStyles()
        {
            style.transformOrigin = new StyleTransformOrigin(new TransformOrigin(50f, 50f, 0f));
            style.flexGrow = new StyleFloat(0f);
            style.alignItems = new StyleEnum<Align>(Align.Center);
            style.justifyContent = new StyleEnum<Justify>(Justify.Center);
            style.width = 0;
            style.height = 0;
            style.position = new StyleEnum<Position>(Position.Absolute);

            _point.style.position = new StyleEnum<Position>(Position.Absolute);
            _point.style.left = -2.5f;
            _point.style.bottom = -2.5f;
            _point.style.flexGrow = 0f;
            _point.style.width = 5;
            _point.style.height = 5;
            _point.style.backgroundColor = new StyleColor(_color);
            _point.style.SetBorders(new StyleColor(Color.clear), 0, 100);

            _label.style.position = new StyleEnum<Position>(Position.Absolute);
            _label.style.bottom = -15f;

            schedule.Execute(() =>
            {
                Debug.Log("Executed");
                _label.style.left = -(_label.resolvedStyle.width/2f);
            });
            _label.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
            _label.style.fontSize = 8;
            _label.style.color = new StyleColor(_color);
            _label.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter);
            
            _label.style.paddingBottom = 0f;
            _label.style.paddingTop = 0f;
            _label.style.paddingLeft = 0f;
            _label.style.paddingRight = 0f;
            
            _label.style.marginBottom = 0f;
            _label.style.marginTop = 0f;
            _label.style.marginLeft = 0f;
            _label.style.marginRight = 0f;
        }

        public void SetNormalizedPosition(Vector2 position)
        {
            _normalizedPosition = position;
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            style.left = new Length(_normalizedPosition.x * 100, LengthUnit.Percent);
            style.bottom = new Length(_normalizedPosition.y * 100, LengthUnit.Percent);
        }

        private void SetLabel(string label)
        {
            _label.text = label;
        }
    }
}