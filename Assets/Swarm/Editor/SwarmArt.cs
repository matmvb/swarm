#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Gera sprites geométricos com antialiasing para o visual neon.
/// Tudo branco: a cor vem do SpriteRenderer.color.
/// </summary>
public static class SwarmArt
{
    const string ShapesFolder = "Assets/Swarm/Art/Shapes";

    public static Sprite Ship => Ensure("ship", 3, 90f, 0.46f, 0f);
    public static Sprite Hex => Ensure("hex", 6, 0f, 0.46f, 0f);
    public static Sprite Diamond => Ensure("diamond", 4, 0f, 0.46f, 0f);
    public static Sprite Square => Ensure("square", 4, 45f, 0.44f, 0f);
    public static Sprite Octagon => Ensure("octagon", 8, 22.5f, 0.46f, 0f);
    public static Sprite Circle => Ensure("circle", 32, 0f, 0.44f, 0f);
    public static Sprite Ring => Ensure("ring", 32, 0f, 0.46f, 0.32f);

    public static Sprite Ensure(string name, int sides, float rotationDegrees, float radius, float hole, int size = 64)
    {
        EnsureFolder(ShapesFolder);

        string path = ShapesFolder + "/" + name + ".png";
        string absolute = Path.Combine(Application.dataPath, "Swarm/Art/Shapes/" + name + ".png");

        if (!File.Exists(absolute))
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var pixels = new Color32[size * size];
            float rotation = rotationDegrees * Mathf.Deg2Rad;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float coverage = 0f;

                    for (int sy = 0; sy < 3; sy++)
                    {
                        for (int sx = 0; sx < 3; sx++)
                        {
                            float u = (x + (sx + 0.5f) / 3f) / size;
                            float v = (y + (sy + 0.5f) / 3f) / size;
                            if (Inside(u, v, sides, rotation, radius, hole)) coverage += 1f / 9f;
                        }
                    }

                    pixels[y * size + x] = new Color32(255, 255, 255, (byte)Mathf.RoundToInt(coverage * 255f));
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply();
            File.WriteAllBytes(absolute, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = size;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    static bool Inside(float x, float y, int sides, float rotation, float radius, float hole)
    {
        float dx = x - 0.5f;
        float dy = y - 0.5f;
        float distance = Mathf.Sqrt(dx * dx + dy * dy);

        if (distance > radius) return false;
        if (hole > 0f && distance < hole) return false;
        if (sides >= 24) return true;

        float angle = Mathf.Atan2(dy, dx) - rotation;
        float sector = Mathf.PI * 2f / sides;
        float apothem = radius * Mathf.Cos(Mathf.PI / sides);
        float local = Mathf.Repeat(angle, sector) - sector * 0.5f;
        return distance * Mathf.Cos(local) <= apothem;
    }

    static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;

        string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
        string name = Path.GetFileName(path);

        if (!string.IsNullOrEmpty(parent)) EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, name);
    }
}
#endif
