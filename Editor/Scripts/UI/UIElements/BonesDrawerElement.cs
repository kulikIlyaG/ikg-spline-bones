using IKGTools.Editor.Utilities.UI.UIToolkit;
using IKGTools.Editor.Utilities.UIToolkit;
using UnityEngine;
using UnityEngine.UIElements;

namespace IKGTools.Editor.Utilities
{
    internal sealed class BonesDrawerElement : VisualElement
    {
        private static readonly StyleColor BACKGROUND_COLOR = new StyleColor(new Color32(63, 63, 63, 255));
        private static readonly StyleColor BOUND_COLOR = new StyleColor(new Color32(34, 34, 34, 255));
        private static readonly StyleColor BORDERS_RECT_COLOR = new StyleColor(new Color32(6, 6, 6, 255));

        private Color[] _colors;
        private Vector2[] _bonesWorldPosition;
        private Vector2[] _normalizedPositions;
        private NormalizedPointElement[] _pointElements;

        private VisualElement _bounds;
        private VisualElement _rect;

        public BonesDrawerElement(Vector2[] worldPositions, Color[] colors)
        {
            _colors = colors;
            _bounds = new VisualElement();
            Add(_bounds);
            _rect = new VisualElement();
            _bounds.Add(_rect);

            SetupStylesForBackground();
            SetPositions(worldPositions);
        }

        private void SetupStylesForBackground()
        {
            style.SetBorders(BACKGROUND_COLOR, 0, 10);
            style.backgroundColor = BACKGROUND_COLOR;
            style.width = new StyleLength(Length.Percent(100));
            style.marginTop = 15;

            schedule.Execute(() =>
            {
                Debug.Log("Execute height for rect");
                style.height = resolvedStyle.width;
            });
            
            _bounds.style.SetBorders(BOUND_COLOR, 0, 5);
            _bounds.style.backgroundColor = BOUND_COLOR;
            _bounds.style.flexGrow = new StyleFloat(1f);
            
            _bounds.style.marginBottom = 15;
            _bounds.style.marginTop = 15;
            _bounds.style.marginLeft = 15;
            _bounds.style.marginRight = 15;
            
            _bounds.style.paddingBottom = 5;
            _bounds.style.paddingTop = 5;
            _bounds.style.paddingRight = 5;
            _bounds.style.paddingLeft = 5;

            _rect.style.flexGrow = 1f;
            _rect.style.SetBorders(BORDERS_RECT_COLOR, 1, 0);
        }

        public void SetPositions(Vector2[] worldPositions)
        {
            _bonesWorldPosition = worldPositions;
            if (_normalizedPositions != null && _normalizedPositions.Length != _bonesWorldPosition.Length)
            {
                _normalizedPositions = new Vector2[_bonesWorldPosition.Length];
            }else if (_normalizedPositions == null)
                _normalizedPositions = new Vector2[_bonesWorldPosition.Length];
            NormalizePositions(_bonesWorldPosition, ref _normalizedPositions);
            UpdatedBonesPositions();
        }
        
        private void UpdatedBonesPositions()
        {
            if (_pointElements != null && _normalizedPositions.Length != _pointElements.Length)
            {
                _rect.Clear();
                _pointElements = new NormalizedPointElement[_normalizedPositions.Length];
                
                for (int index = 0; index < _normalizedPositions.Length; index++)
                {
                    var element = new NormalizedPointElement(GetLabel(index), _normalizedPositions[index], GetColor(index));
                    _rect.Add(element);
                }
            }
            else
            {
                if (_pointElements == null)
                    _pointElements = new NormalizedPointElement[_normalizedPositions.Length];

                for (int index = 0; index < _pointElements.Length; index++)
                {
                    var element = _pointElements[index];
                    if (element == null)
                    {
                        element = new NormalizedPointElement(GetLabel(index), _normalizedPositions[index],GetColor(index));
                        _pointElements[index] = element;
                        _rect.Add(element);
                    }
                    else
                        element.SetNormalizedPosition(_normalizedPositions[index]);
                }
            }
            
        }

        private Color GetColor(int boneIndex)
        {
            return _colors[boneIndex];
        }

        private string GetLabel(int itemIndex)
        {
            return $"bone_{itemIndex:000}";
        }
        
        private const float EPSILON = 1e-6f;
        private void NormalizePositions(Vector2[] positions, ref Vector2[] normalizedPositions)
        {
            float minX = float.MaxValue, minY = float.MaxValue;
            float maxX = float.MinValue, maxY = float.MinValue;

            foreach (var pos in positions)
            {
                if (pos.x < minX) minX = pos.x;
                if (pos.y < minY) minY = pos.y;
                if (pos.x > maxX) maxX = pos.x;
                if (pos.y > maxY) maxY = pos.y;
            }

            Vector2 minPoint = new Vector2(minX, minY);
            Vector2 size = new Vector2(maxX - minX, maxY - minY);

            size.x = size.x == 0 ? EPSILON : size.x;
            size.y = size.y == 0 ? EPSILON : size.y;

            if (normalizedPositions.Length != positions.Length)
            {
                normalizedPositions = new Vector2[positions.Length];
            }

            for (int i = 0; i < positions.Length; i++)
            {
                normalizedPositions[i] = new Vector2(
                    (positions[i].x - minPoint.x) / size.x,
                    (positions[i].y - minPoint.y) / size.y
                );
            }
        }
    }
}