using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace IKGTools.SplineBones.Editor
{
    internal sealed class PointBindingsEditor : BaseEditorUI
    {
        private const string FOLDOUT_LABEL_FORMAT = "Point {0:000}";
        private static readonly Color FOLDOUT_BACKGROUND_COLOR = new Color(0.2f, 0.2f, 0.2f, 0.73f);

        private readonly SplineBindingsData _data;

        private BoneWeightsEditorDrawer[] _pointWeightsEditor;

        private VisualElement _validatePointsContainer;
        private VisualElement _createPointsContainer;
        private VisualElement _pointsContainer;


        private bool[] _foldoutsState;

        public PointBindingsEditor(SplineBindingsData data)
        {
            _data = data;
            _data.OnUpdatedData += RepaintPointsContainerFromOtherEditor;
        }

        private SplineBindings Bindings => _data.Bindings;


        protected override VisualElement CreateCustomVisualElements()
        {
            _rootContainer = new VisualElement();

            _rootContainer.Add(CreateSkeletonDefinitionFieldUI());

            _rootContainer.Add(CreateValidatePointsButtonsUI());
            _rootContainer.Add(CreatePointsButtonsUI());

            _pointsContainer = new VisualElement();

            CreatePointWeightEditors();
            if (Bindings != null && Bindings.SkeletonDefinition != null)
            {
                if (IsPointsWeightCorrectForCurrentSkeleton())
                    CreatePointElementsUI();
            }

            _rootContainer.Add(_pointsContainer);

            return _rootContainer;
        }


        protected override void OnShow()
        {
            UpdateVisibleValidatePointsBindingsButtons();
            UpdateVisibleValidateManagePointsButtons();
        }


        public override void OnGUI()
        {
            if (_pointWeightsEditor == null)
                return;

            foreach (var boneWeightsEditor in _pointWeightsEditor)
            {
                boneWeightsEditor.OnGUI();
            }
        }

        private void UpdateVisibleValidateManagePointsButtons()
        {
            DisplayStyle display = DisplayStyle.None;

            if (Bindings.SkeletonDefinition != null)
                display = DisplayStyle.Flex;

            _createPointsContainer.style.display = display;
        }

        private void UpdateVisibleValidatePointsBindingsButtons()
        {
            DisplayStyle display = DisplayStyle.None;

            if (Bindings.SkeletonDefinition != null && Bindings.Points.Length > 0)
            {
                if (!IsPointsWeightCorrectForCurrentSkeleton())
                {
                    display = DisplayStyle.Flex;
                }
            }

            _validatePointsContainer.style.display = display;
        }

        private bool IsPointsWeightCorrectForCurrentSkeleton()
        {
            int bonesCount = Bindings.SkeletonDefinition.BonesCount;
            foreach (var point in Bindings.Points)
            {
                if (point.Bones.Length == bonesCount)
                    continue;

                return false;
            }

            return true;
        }

        private void RepaintPointsContainer()
        {
            _data.OnUpdatedData -= RepaintPointsContainerFromOtherEditor;
            _data.UpdateData();
            _data.OnUpdatedData += RepaintPointsContainerFromOtherEditor;

            _pointsContainer.Clear();
            CreatePointWeightEditors();
            CreatePointElementsUI();
        }

        //use for update other editor when his contains same data
        private void RepaintPointsContainerFromOtherEditor()
        {
            if (_pointsContainer != null)
                _pointsContainer.Clear();
            CreatePointWeightEditors();
            CreatePointElementsUI();
        }


        private void OnClickRemoveLastPoint()
        {
            Bindings.SetPointBindings(BindingsDataEditHelper.RemovePoint(Bindings.Points, Bindings.Points.Length - 1));

            EditorUtility.SetDirty(_data);

            RepaintPointsContainer();
        }

        private void OnClickCreatePoint()
        {
            Bindings.SetPointBindings(BindingsDataEditHelper.AddPoint(Bindings.Points,
                Bindings.SkeletonDefinition.BonesCount));

            EditorUtility.SetDirty(_data);

            RepaintPointsContainer();
        }

        private void OnClickedValidatePoints()
        {
            Bindings.SetPointBindings(BindingsDataEditHelper.ValidatePointWeightsToSkeleton(Bindings.Points,
                Bindings.SkeletonDefinition.BonesCount));

            EditorUtility.SetDirty(_data);

            UpdateVisibleValidatePointsBindingsButtons();
            RepaintPointsContainer();
        }

#region CREATE UI

        private void CreatePointWeightEditors()
        {
            if(Bindings == null)
                return;
            
            if (Bindings.Points is {Length: > 0} && IsPointsWeightCorrectForCurrentSkeleton())
            {
                _pointWeightsEditor = new BoneWeightsEditorDrawer[Bindings.Points.Length];
                Color[] colors = _data.Bindings.SkeletonDefinition.GetBonesColors();
                for (int index = 0; index < _pointWeightsEditor.Length; index++)
                    _pointWeightsEditor[index] = new BoneWeightsEditorDrawer(Bindings.Points[index], colors);
            }
            else
            {
                _pointWeightsEditor = Array.Empty<BoneWeightsEditorDrawer>();
            }
        }

        private void CreatePointElementsUI()
        {
            if (_pointsContainer == null)
                return;
            
            if (_foldoutsState == null || _foldoutsState.Length != _pointWeightsEditor.Length)
                _foldoutsState = new bool[_pointWeightsEditor.Length];
            
            for (int index = 0; index < _pointWeightsEditor.Length; index++)
            {
                Foldout weightsContainer = new Foldout
                {
                    text = string.Format(FOLDOUT_LABEL_FORMAT, index),
                    value = _foldoutsState[index]
                };
                var foldoutIndex = index;
                weightsContainer.RegisterValueChangedCallback((evt) => _foldoutsState[foldoutIndex] = evt.newValue);
                
                _pointsContainer.Add(weightsContainer);

                SetFoldoutStyles(weightsContainer);

                var weightsElement = _pointWeightsEditor[index].CreateVisualElement();
                weightsContainer.Add(weightsElement);
            }
        }

        private VisualElement CreatePointsButtonsUI()
        {
            _createPointsContainer = new VisualElement();
            _createPointsContainer.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);

            Button createPointButton = new Button()
            {
                text = "Add point"
            };
            createPointButton.clicked += OnClickCreatePoint;

            Button removePointButton = new Button()
            {
                text = "Remove last point"
            };
            removePointButton.clicked += OnClickRemoveLastPoint;

            _createPointsContainer.Add(createPointButton);
            _createPointsContainer.Add(removePointButton);

            return _createPointsContainer;
        }



        private VisualElement CreateValidatePointsButtonsUI()
        {
            _validatePointsContainer = new VisualElement();

            Button validatePointsToSkeleton = new Button()
            {
                text = "Validate points"
            };
            validatePointsToSkeleton.clicked += OnClickedValidatePoints;

            Label infoLabel = new Label("The current point binding does not match the skeleton.");

            _validatePointsContainer.Add(validatePointsToSkeleton);
            _validatePointsContainer.Add(infoLabel);
            return _validatePointsContainer;
        }


        private ObjectField CreateSkeletonDefinitionFieldUI()
        {
            ObjectField skeletonDefinitionField = new ObjectField("Skeleton")
            {
                objectType = typeof(SkeletonDefinition),
                value = Bindings.SkeletonDefinition
            };
            skeletonDefinitionField.RegisterValueChangedCallback(evt =>
            {
                Bindings.SetSkeletonDefinition(evt.newValue as SkeletonDefinition);
                EditorUtility.SetDirty(_data);

                CallRepaint();
            });
            return skeletonDefinitionField;
        }

        private void SetFoldoutStyles(VisualElement root)
        {
            float margin = 3f;
            float paddings = 3f;
            root.style.marginLeft = margin;
            root.style.marginRight = margin;
            root.style.marginTop = margin;
            root.style.marginBottom = margin;
            root.style.paddingLeft = paddings;
            root.style.paddingRight = paddings;
            root.style.paddingTop = paddings;
            root.style.paddingBottom = paddings;
            root.style.backgroundColor = FOLDOUT_BACKGROUND_COLOR;
        }

#endregion
    }
}