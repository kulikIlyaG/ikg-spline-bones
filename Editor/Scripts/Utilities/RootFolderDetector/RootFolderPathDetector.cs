using UnityEditor.PackageManager;

namespace IKGTools.RefsDetective.Utilities
{
    internal static class RootFolderPathDetector
    {
        private const string DevelopAssetsRootPath = "Assets/Sources/ikg-spline-bones/";

        public static string GetRootFolderPath()
        {
            PackageInfo packageInfo = PackageInfo.FindForAssembly(typeof(RootFolderPathDetector).Assembly);
            return packageInfo != null ? $"{packageInfo.assetPath}/" : DevelopAssetsRootPath;
        }
    }
}