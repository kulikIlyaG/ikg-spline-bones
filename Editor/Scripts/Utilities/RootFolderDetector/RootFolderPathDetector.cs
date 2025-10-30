using System.IO;
using UnityEngine;

namespace IKGTools.RefsDetective.Utilities
{
    internal static class RootFolderPathDetector
    {
        private static class Keys
        {
            public static readonly string BaseFolderName = "com.ikg.spline-bones";
            
            private static readonly string _assetsRootPath = "Assets/Sources/";
            private static readonly string _packagesRootPath = "Packages/";
            
            public static string AssetsRootPath => $"{_assetsRootPath}/ikg-spline-bones/";
            public static string PackagesRootPath => $"{_packagesRootPath}{BaseFolderName}/";
        }
        
        public static string GetRootFolderPath()
        {
            if (IsPackage())
            {
                return Keys.PackagesRootPath;
            }
            
            return Keys.AssetsRootPath;
        }

        private static bool IsPackage()
        {
            return PackageFolderExists(Keys.BaseFolderName);
        }

        private static bool PackageFolderExists(string packageFolderName)
        {
            string projectRoot = Directory.GetParent(Application.dataPath)?.FullName;
            
            if (projectRoot == null)
            {
                Debug.LogError($"{nameof(RootFolderPathDetector)} not found root project folder!");
                return false;
            }
            
            string cachePath = Path.Combine(projectRoot, "Library/PackageCache");

            if (!Directory.Exists(cachePath))
            {
                return false;
            }

            string[] dirs = Directory.GetDirectories(cachePath);

            foreach (string dir in dirs)
            {
                string folderName = Path.GetFileName(dir);

                if (string.IsNullOrEmpty(folderName))
                    continue;
                
                string normalized = folderName.Replace(".", "-").ToLower();
                string normalizedPart = packageFolderName.Replace(".", "-").ToLower();

                if (normalized.Contains(normalizedPart))
                {
                    return true;
                }
            }

            return false;
        }
    }
}