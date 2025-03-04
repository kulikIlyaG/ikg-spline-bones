using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Diagnostics;
using System.Linq;
using IKGTools.Editor.Utilities.UIToolkit;

namespace IKGTools.SplineBones.Editor
{
    public class BoneWeightsEditorDrawer
    {
        private const float EPSILON = 0.0001f;

        private const int DOUBLE_CLICK_THRESHOLD_MS = 250;

        private const string BONE_LABEL_FORMAT = "Bone {0:000}";
        private const string TOTAL_WEIGHT_LABEL_FORMAT = "Total Weight: {0}";
        
        private static readonly Color HIGHLIGHT_TOTAL_WEIGHT_LABEL_COLOR = new Color(1f, 0.4f, 0.24f);
        
        private readonly PointBinding _pointBinding;
        private readonly Color[] _colors;
        private readonly float[] _lastWeights;
        
        
        private CustomSliderElement[] _sliders;
        private Label _totalWeightLabel;
        
        public event Action OnUserBeganChangeValues;
        public event Action OnUserEndChangeValues;
        
        public BoneWeightsEditorDrawer(PointBinding pointBinding, Color[] colors)
        {
            _pointBinding = pointBinding;
            _colors = colors;
            _lastWeights = new float[pointBinding.Bones.Length];
            UpdateCachedWeights();
        }


        public VisualElement CreateVisualElement()
        {
            var weightsContainer = new VisualElement();

            _sliders = new CustomSliderElement[_pointBinding.Bones.Length];
            for (int index = 0; index < _pointBinding.Bones.Length; index++)
            {
                var boneSlider = CreateBoneWeightSlider(index, _pointBinding.Bones[index].BoneIndex);
                weightsContainer.Add(boneSlider);
                _sliders[index] = boneSlider;
            }

            _totalWeightLabel = new Label();
            UpdateTotalWeightLabel();
            weightsContainer.Add(_totalWeightLabel);
            
            return weightsContainer;
        }

        private long _lastClickTime;
        private CustomSliderElement CreateBoneWeightSlider(int indexInArray, int boneIndex)
        {
            BoneWeight bone = _pointBinding.Bones[indexInArray];
            Color color = _colors[indexInArray % _colors.Length];
            
            var customSlider = new CustomSliderElement(string.Format(BONE_LABEL_FORMAT, boneIndex), bone.Weight, color);
            
            customSlider.RegisterCallback<PointerDownEvent>((evt) =>
            {
                long currentTime = Stopwatch.GetTimestamp();
                long elapsedTime = (currentTime - _lastClickTime) * 1000 / Stopwatch.Frequency; // Перевод в миллисекунды

                if (elapsedTime <= DOUBLE_CLICK_THRESHOLD_MS)
                {
                    OnDoubleClick(indexInArray);
                }

                _lastClickTime = currentTime;
            });
            
            customSlider.OnPointerDown += () =>
            {
                OnUserBeganChangeValues?.Invoke();
            };

            customSlider.OnValueChanged += (newValue) =>
            {
                if (HasWeightsChanged(indexInArray, newValue))
                {
                    AdjustWeights(indexInArray, newValue);
                    UpdateCachedWeights();
                }
            };

            customSlider.OnPointerUp += () =>
            {
                OnUserEndChangeValues?.Invoke();
            };

            return customSlider;
        }

        private void OnDoubleClick(int index)
        {
            float currentWeight = _pointBinding.Bones[index].Weight;
            if (currentWeight >= 1f)
                AdjustWeights(index, 0f);
            else if (currentWeight <= 0f)
                AdjustWeights(index, 1f);
        }

        private void UpdateCachedWeights()
        {
            for (int i = 0; i < _pointBinding.Bones.Length; i++)
                _lastWeights[i] = _pointBinding.Bones[i].Weight;
        }

        private bool HasWeightsChanged(int index, float newWeight)
        {
            return Mathf.Abs(newWeight - _lastWeights[index]) > EPSILON;
        }
        
        private void UpdateTotalWeightLabel()
        {
            float totalWeight = _pointBinding.Bones.Sum(b => b.Weight);
            _totalWeightLabel.text = string.Format(TOTAL_WEIGHT_LABEL_FORMAT, $"{totalWeight:P0}");
            
            if (Mathf.Abs(totalWeight - 1f) > EPSILON)
                _totalWeightLabel.style.color = Color.white;
            else
                _totalWeightLabel.style.color = HIGHLIGHT_TOTAL_WEIGHT_LABEL_COLOR;
        }
        
        public void OnGUI()
        {
            if(_sliders == null)
                return;
            for (var index = 0; index < _sliders.Length; index++)
            {
                var slider = _sliders[index];
                if(slider.IsSelected)
                    continue;
                slider.SetValue(_pointBinding.Bones[index].Weight);
            }
            UpdateTotalWeightLabel();
        }
        
        private void AdjustWeights(int changedIndex, float targetWeight)
        {
            float currentWeight = _pointBinding.Bones[changedIndex].Weight;

            if (targetWeight < currentWeight)
            {
                _pointBinding.Bones[changedIndex].SetWeight(targetWeight);
                return;
            }

            float totalWeight = _pointBinding.Bones.Sum(b => b.Weight);
            float newTotalWeight = totalWeight - currentWeight + targetWeight;

            if (newTotalWeight <= 1f + EPSILON)
            {
                _pointBinding.Bones[changedIndex].SetWeight(targetWeight);
                return;
            }

            float excess = newTotalWeight - 1f;
            float otherWeightsSum = totalWeight - currentWeight;

            if (otherWeightsSum < EPSILON)
            {
                _pointBinding.Bones[changedIndex].SetWeight(1f);
                return;
            }

            float scaleFactor = (otherWeightsSum - excess) / otherWeightsSum;
            _pointBinding.Bones[changedIndex].SetWeight(targetWeight);

            for (int i = 0; i < _pointBinding.Bones.Length; i++)
            {
                if (i != changedIndex && _pointBinding.Bones[i].Weight > 0)
                {
                    float newWeight = Mathf.Max(0, _pointBinding.Bones[i].Weight * scaleFactor);
                    _pointBinding.Bones[i].SetWeight(newWeight);
                }
            }
        }
    }
}