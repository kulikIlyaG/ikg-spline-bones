using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

namespace IKGTools.SplineBones.Editor
{
    internal sealed class BoneAndSplineInfoDrawer
    {
        private static readonly Color32 POINT_LABEL_COLOR = new Color32(255, 61, 160, 255);
        
        private readonly SkeletonComponent _skeleton;
        private readonly SplineBindingsData _bindingsData;
        private readonly Spline _spline;

        private readonly GUIStyle _pointStyle;
        private GUIStyle[] _boneStyles;

        public BoneAndSplineInfoDrawer(SkeletonComponent skeleton, SplineBindingsData bindingsData, Spline spline)
        {
            _skeleton = skeleton;
            _bindingsData = bindingsData;
            _spline = spline;
            
            _pointStyle = new GUIStyle();
            _pointStyle.normal.textColor = POINT_LABEL_COLOR;
            _pointStyle.alignment = TextAnchor.LowerLeft;
            _pointStyle.fontStyle = FontStyle.Bold;
            
            ValidateBoneStyles();
        }
        
        private void ValidateBoneStyles()
        {
            if (_boneStyles == null || _boneStyles.Length != _bindingsData.Bindings.SkeletonDefinition.BonesCount)
            {
                var colors = _bindingsData.Bindings.SkeletonDefinition.GetBonesColors();
                _boneStyles = new GUIStyle[colors.Length];
                for (int index = 0; index < _boneStyles.Length; index++)
                {
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = colors[index];
                    style.normal.background = EditorGUIUtility.whiteTexture;
                    style.alignment = TextAnchor.UpperRight;
                    style.fontStyle = FontStyle.Bold;
                    _boneStyles[index] = style;
                }
            }
        }
        
        public void Draw()
        {
            var bones = _skeleton.Bones;
            if (bones is {Length: > 0})
            {
                ValidateBoneStyles();
                Color defaultGUIcolor = GUI.color;
                for (var index = 0; index < bones.Length; index++)
                {
                    var bone = bones[index];
                    if (bone != null && bone.Origin != null)
                    {
                        Vector3 bonePosition = bone.Origin.position;

                        GUI.color = _boneStyles[index].normal.textColor;
                        Handles.Label(bonePosition, bone.Origin.name, _boneStyles[index]);
                    }
                }

                GUI.color = defaultGUIcolor;
            }

            if (_spline != null)
            {
                DrawPointsLabels();
                DrawPointsTangents();
            }
        }

        private void DrawPointsTangents()
        {
            int pointsCount = _spline.GetPointCount();
            var defaultHandlesColor = Handles.color;
            for (int index = 0; index < pointsCount; index++)
            {
                if(_skeleton == null)
                    break;

                Vector3 point = _spline.GetPosition(index);
                Vector3 leftTangents = point + _spline.GetLeftTangent(index);
                Vector3 rightTangents = point + _spline.GetRightTangent(index);
                Vector3 pointPosition = _skeleton.transform.TransformPoint(point);
                Vector3 tangentIn = _skeleton.transform.TransformPoint(leftTangents);
                Vector3 tangentOut = _skeleton.transform.TransformPoint(rightTangents);

                Handles.color = new Color(1f, 0.32f, 0.44f);
                Handles.DrawLine(pointPosition, tangentIn);
                Handles.DrawLine(pointPosition, tangentOut);
            }

            Handles.color = defaultHandlesColor;
        }

        private void DrawPointsLabels()
        {
            int pointsCount = _spline.GetPointCount();
            for (int index = 0; index < pointsCount; index++)
            {
                if(_skeleton == null)
                    break;
                Vector3 pointPosition = _skeleton.transform.TransformPoint(_spline.GetPosition(index));
                    
                string pointName = $"point_{index:000}";
                Handles.Label(pointPosition, pointName, _pointStyle);
            }
        }
    }
}