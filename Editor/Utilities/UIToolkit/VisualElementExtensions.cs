using UnityEngine.UIElements;

namespace IKGTools.SplineBones.Editor.Utilities.UIToolkit
{
    public static class VisualElementExtensions
    {
        public static bool TryRemoveClass(this VisualElement element, string className)
        {
            if (element.ClassListContains(className))
            {
                element.RemoveFromClassList(className);
                return true;
            }

            return false;
        }

        public static bool TryAddClass(this VisualElement element, string className)
        {
            if (!element.ClassListContains(className))
            {
                element.AddToClassList(className);
                return true;
            }

            return false;
        }
        
        public static void SetBorders(this IStyle style,  StyleColor borderColor, int borderWidth, int radius)
        {
            style.borderBottomColor = borderColor;
            style.borderTopColor = borderColor;
            style.borderLeftColor = borderColor;
            style.borderRightColor = borderColor;
            
            style.borderBottomWidth = borderWidth;
            style.borderTopWidth = borderWidth;
            style.borderLeftWidth = borderWidth;
            style.borderRightWidth = borderWidth;
            
            style.borderBottomRightRadius = radius;
            style.borderBottomLeftRadius = radius;
            style.borderTopRightRadius = radius;
            style.borderTopLeftRadius = radius;
        }
    }
}