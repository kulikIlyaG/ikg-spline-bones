using System;
using IKGTools.Editor.Utilities.UI.UIToolkit;
using UnityEngine;
using UnityEngine.UIElements;

namespace IKGTools.Editor.Utilities.UIToolkit
{
    public sealed class CustomSliderElement : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<CustomSliderElement, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription m_Text = new()
                {name = "text", defaultValue = "Default Text"};

            private readonly UxmlColorAttributeDescription m_Color = new()
                {name = "color", defaultValue = new Color32(90, 90, 90, 255)};

            private readonly UxmlFloatAttributeDescription m_Value = new() {name = "value", defaultValue = 1f};

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var customElement = (CustomSliderElement) ve;
                customElement.SetNameLabel(m_Text.GetValueFromBag(bag, cc));
                customElement.SetColor(m_Color.GetValueFromBag(bag, cc));
                customElement.SetValue(m_Value.GetValueFromBag(bag, cc));
            }
        }

        private const float LOWER_VALUE_TO_DARK_VALUE_LABEL = 0.9f;
        private const float LOWER_VALUE_TO_DARK_NAME_LABEL = 0.1f;

        private const float BORDERS_COLOR_PERCENT = 0.25f;
        private const float BACKGROUND_COLOR_PERCENT = 0.5f;
        private const float DARK_LABEL_COLOR_PERCENT = 0.35f;
        private const float FILL_COLOR_PERCENT = 0.9f;
        private const int ADD_HANDLE_COLOR = 155;
        private const int ADD_LABEL_COLOR_BRIGHT = 155;


        private readonly VisualElement _fill;
        private readonly VisualElement _handle;
        private readonly Label _nameLabel;
        private readonly Label _valueLabel;
        

        private Color _color = new Color32(90, 90, 90, 255);
        private float _value = 0f;


        private bool _setUpperNameColor;
        private bool _setUpperValueColor;


        public event Action OnPointerDown, OnPointerUp;
        public event Action<float> OnValueChanged;


        public CustomSliderElement()
        {
            _fill = new VisualElement();
            Add(_fill);

            _handle = new VisualElement();
            _fill.Add(_handle);

            _nameLabel = new Label("Name");
            Add(_nameLabel);

            _valueLabel = new Label();
            Add(_valueLabel);

            RegisterCallbacks();
            SetupStyles();
            UpdateColor();
            
            UpdateValueLabel();
        }

        public CustomSliderElement(string name, float startValue, Color color)
        {
            _fill = new VisualElement();
            Add(_fill);

            _handle = new VisualElement();
            _fill.Add(_handle);

            _nameLabel = new Label("Name");
            Add(_nameLabel);

            _valueLabel = new Label("0%");
            Add(_valueLabel);

            RegisterCallbacks();
            SetupStyles();
            UpdateColor();

            SetValue(startValue);
            SetNameLabel(name);
            SetColor(color);
        }

        public string NameLabel => _nameLabel.text;

        public float Value => _value;

        public Color Color => _color;
        
        public bool IsSelected { get; private set; }

        public void SetValue(float value)
        {
            _value = Mathf.Clamp01(value);
            
            UpdateHandlePosition();
            UpdateValueLabel();

            UpdateValueLabelColor();
            UpdateNameLabelColor();

            OnValueChanged?.Invoke(_value);
        }

        public void SetNameLabel(string value)
        {
            _nameLabel.text = value;
        }

        public void SetColor(Color color)
        {
            _color = color;
            UpdateColor();
        }

        private void RegisterCallbacks()
        {
            _handle.RegisterCallback<PointerDownEvent>(OnPointerDownEvent);
            _handle.RegisterCallback<PointerMoveEvent>(OnPointerMoveEvent);
            _handle.RegisterCallback<PointerUpEvent>(OnPointerUpEvent);
        }
        
        private void UpdateValueByEvent(PointerMoveEvent evt)
        {
            float mouseX = evt.position.x;
            float sliderX = worldBound.x;
            float relativeX = mouseX - sliderX;
            float normalizedValue = Mathf.Clamp01(relativeX / resolvedStyle.width);

            SetValue(normalizedValue);
        }

        private void UpdateValueLabel()
        {
            _valueLabel.text = $"{Mathf.RoundToInt(_value * 100)}%";
        }

        private void UpdateHandlePosition()
        {
            _fill.style.flexGrow = _value;
        }

#region DRAG

        private void OnPointerDownEvent(PointerDownEvent evt)
        {
            IsSelected = true;
            evt.target.CapturePointer(evt.pointerId);
            OnPointerDown?.Invoke();
            _handle.style.SetBorders(new StyleColor(Color.white), 1, 0);
        }

        private void OnPointerMoveEvent(PointerMoveEvent evt)
        {
            if (evt.target.HasPointerCapture(evt.pointerId))
                UpdateValueByEvent(evt);
        }

        private void OnPointerUpEvent(PointerUpEvent evt)
        {
            _handle.ReleasePointer(evt.pointerId);
            _handle.style.SetBorders(new StyleColor(Color.clear), 0, 0);
            IsSelected = false;
            OnPointerUp?.Invoke();
        }

#endregion
        
#region COLORS

        private void UpdateColor()
        {
            StyleColor bordersColor = new StyleColor(MultiplyColorPreservingAlpha(_color, BORDERS_COLOR_PERCENT));
            StyleColor backgroundColor = new StyleColor(MultiplyColorPreservingAlpha(_color, BACKGROUND_COLOR_PERCENT));
            StyleColor fillColor = new StyleColor(MultiplyColorPreservingAlpha(_color, FILL_COLOR_PERCENT));
            StyleColor handleColor = new StyleColor(AddColorPreservingAlpha(_color, ADD_HANDLE_COLOR));

            StyleColor nameLabelColor = GetNameColorLabelByValue();
            StyleColor valueLabelColor = GetValueColorLabelByValue();


            style.backgroundColor = backgroundColor;
            style.borderBottomColor = bordersColor;
            style.borderTopColor = bordersColor;
            style.borderLeftColor = bordersColor;
            style.borderRightColor = bordersColor;

            _fill.style.backgroundColor = fillColor;
            _handle.style.backgroundColor = handleColor;
            _nameLabel.style.color = nameLabelColor;
            _valueLabel.style.color = valueLabelColor;
        }


        private void UpdateNameLabelColor()
        {
            if (_value > LOWER_VALUE_TO_DARK_NAME_LABEL)
            {
                if (!_setUpperNameColor)
                {
                    _nameLabel.style.color = GetNameColorLabelByValue();
                    _setUpperNameColor = true;
                }
            }
            else
            {
                if (_setUpperNameColor)
                {
                    _nameLabel.style.color = GetNameColorLabelByValue();
                    _setUpperNameColor = false;
                }
            }
        }


        private void UpdateValueLabelColor()
        {
            if (_value > LOWER_VALUE_TO_DARK_VALUE_LABEL)
            {
                if (!_setUpperValueColor)
                {
                    _valueLabel.style.color = GetValueColorLabelByValue();
                    _setUpperValueColor = true;
                }
            }
            else
            {
                if (_setUpperValueColor)
                {
                    _valueLabel.style.color = GetValueColorLabelByValue();
                    _setUpperValueColor = false;
                }
            }
        }

        private StyleColor GetValueColorLabelByValue()
        {
            if (_value > LOWER_VALUE_TO_DARK_VALUE_LABEL)
                return new StyleColor(MultiplyColorPreservingAlpha(_color, DARK_LABEL_COLOR_PERCENT));

            return new StyleColor(AddColorPreservingAlpha(_color, ADD_LABEL_COLOR_BRIGHT));
        }

        private StyleColor GetNameColorLabelByValue()
        {
            if (_value > LOWER_VALUE_TO_DARK_NAME_LABEL)
                return new StyleColor(MultiplyColorPreservingAlpha(_color, DARK_LABEL_COLOR_PERCENT));

            return new StyleColor(AddColorPreservingAlpha(_color, ADD_LABEL_COLOR_BRIGHT));
        }

        Color32 MultiplyColorPreservingAlpha(Color32 originalColor, float multiplier)
        {
            byte r = (byte) Mathf.Clamp(originalColor.r * multiplier, 0, 255);
            byte g = (byte) Mathf.Clamp(originalColor.g * multiplier, 0, 255);
            byte b = (byte) Mathf.Clamp(originalColor.b * multiplier, 0, 255);

            return new Color32(r, g, b, originalColor.a);
        }

        Color32 AddColorPreservingAlpha(Color32 originalColor, int add)
        {
            byte r = (byte) Mathf.Clamp(originalColor.r + add, 0, 255);
            byte g = (byte) Mathf.Clamp(originalColor.g + add, 0, 255);
            byte b = (byte) Mathf.Clamp(originalColor.b + add, 0, 255);

            return new Color32(r, g, b, originalColor.a);
        }

#endregion

#region STYLES
        private void SetupStyles()
        {
            SetupMainStyles();
            SetupFillStyles();
            SetupHandleStyles();
            SetupNameLabelStyles();
            SetupValueLabelStyles();
        }

        private void SetupValueLabelStyles()
        {
            _valueLabel.pickingMode = PickingMode.Ignore;
            _valueLabel.style.position = new StyleEnum<Position>(Position.Absolute);

            _valueLabel.style.right = 10f;
            _valueLabel.style.top = new StyleLength(Length.Percent(50));
            _valueLabel.style.translate = new StyleTranslate(new Translate(0, Length.Percent(-50)));

            _valueLabel.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleRight);
            _valueLabel.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);

            _valueLabel.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);

            _valueLabel.style.maxWidth = new StyleLength(Length.Percent(40));
            _valueLabel.style.maxHeight = new StyleLength(Length.Percent(100));
            _valueLabel.style.whiteSpace = new StyleEnum<WhiteSpace>(WhiteSpace.Normal);
            _valueLabel.style.overflow = new StyleEnum<Overflow>(Overflow.Hidden);
        }


        private void SetupNameLabelStyles()
        {
            _nameLabel.pickingMode = PickingMode.Ignore;
            _nameLabel.style.position = new StyleEnum<Position>(Position.Absolute);

            _nameLabel.style.left = 10f;
            _nameLabel.style.top = new StyleLength(Length.Percent(50));
            _nameLabel.style.translate = new StyleTranslate(new Translate(0, Length.Percent(-50)));

            _nameLabel.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);

            _nameLabel.style.maxWidth = new StyleLength(Length.Percent(70));
            _nameLabel.style.maxHeight = new StyleLength(Length.Percent(100));
            _nameLabel.style.whiteSpace = new StyleEnum<WhiteSpace>(WhiteSpace.Normal);
            _nameLabel.style.overflow = new StyleEnum<Overflow>(Overflow.Hidden);
        }

        private void SetupHandleStyles()
        {
            _handle.name = $"Handle";
            _handle.pickingMode = PickingMode.Position;
            _handle.style.flexGrow = 0f;
            _handle.style.width = 5f;
        }

        private void SetupFillStyles()
        {
            _fill.name = $"Fill";
            _fill.pickingMode = PickingMode.Ignore;
            _fill.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            _fill.style.justifyContent = new StyleEnum<Justify>(Justify.FlexEnd);
        }

        private void SetupMainStyles()
        {
            pickingMode = PickingMode.Position;

            style.flexGrow = 1f;
            style.height = 32f;
            style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            style.marginBottom = 3f;
            style.marginTop = 3f;
            style.marginRight = 3f;
            style.marginLeft = 3f;

            style.borderBottomWidth = 2f;
            style.borderTopWidth = 2f;
            style.borderRightWidth = 2f;
            style.borderLeftWidth = 2f;
        }

#endregion
    }
}