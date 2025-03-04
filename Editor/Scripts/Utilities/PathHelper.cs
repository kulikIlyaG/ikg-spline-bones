namespace IKGTools.Editor.Utilities
{
    internal static class PathHelper
    {
        public static string GetPathFor(string filePath)
        {
            return $"Packages/com.ikg.spline-bones/Editor/EditorResources/{filePath}";
        }

        public static string GetPathForEditorTexture(string texturePath)
        {
            return GetPathFor($"Textures/{texturePath}");
        }

        internal static class Keys
        {
            public static string BoneColoredIcon00 = "bone_icon_colored_00.png";
            public static string CursorAddBone = "cursor_add.png";
            public static string RemoveBoneButtonNormal = "remove_bone_button.png";
            public static string RemoveBoneButtonHovered = "remove_bone_button_hovered.png";
            public static string RemoveBoneButtonActive = "remove_bone_button_active.png";
        }
    }
}