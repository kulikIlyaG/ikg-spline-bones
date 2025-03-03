using UnityEngine;

namespace IKGTools.SplineBones.Editor
{
    internal static class ComponentChecker
    {
        public static bool HasComponentInHierarchy(GameObject obj, System.Type componentType, out Component foundComponent)
        {
            foundComponent = null;

            if (obj == null || componentType == null)
            {
                return false;
            }

            Transform current = obj.transform;

            while (current != null)
            {
                foundComponent = current.GetComponent(componentType);
                if (foundComponent != null)
                {
                    return true;
                }
                current = current.parent;
            }

            return false;
        }
    }
}