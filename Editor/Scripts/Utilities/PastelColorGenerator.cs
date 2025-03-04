using UnityEngine;

namespace IKGTools.Editor.Utilities
{
    internal static class PastelColorGenerator
    {
        public static Color[] GeneratePastelColors(int count, float min, float max)
        {
            Color[] colors = new Color[count];
            for (int i = 0; i < count; i++)
            {
                colors[i] = GenerateUniquePastelColor(colors, min, max);
            }
            return colors;
        }

        public static Color GenerateUniquePastelColor(Color[] existingColors, float min, float max)
        {
            Color newColor;
            bool isUnique;
            int attempts = 0;
            do
            {
                newColor = GeneratePastelColor(min, max);
                isUnique = true;
                foreach (var color in existingColors)
                {
                    if (color != default && Vector3.Distance(new Vector3(color.r, color.g, color.b), new Vector3(newColor.r, newColor.g, newColor.b)) < 0.25f)
                    {
                        isUnique = false;
                        break;
                    }
                }
                attempts++;
            } while (!isUnique && attempts < 100);
            return newColor;
        }

        private static Color GeneratePastelColor(float min, float max)
        {
            float r = Random.Range(min, max);
            float g = Random.Range(min, max);
            float b = Random.Range(min, max);
            return new Color((r + 1f) * 0.5f, (g + 1f) * 0.5f, (b + 1f) * 0.5f);
        }
    }
}