#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Aplica o manifesto de arte nos PNGs e liga os sprites de naves/inimigos.
/// Menu: SWARM > Arte > Aplicar import settings.
/// Batch: Unity.exe -executeMethod ArtImportPipeline.ApplyAllBatch
/// </summary>
public static class ArtImportPipeline
{
    const string Root = "Assets/Swarm/Art/Sprites";
    const string ManifestPath = Root + "/manifest.json";

    class Spec
    {
        public string path;
        public int frameWidth;
        public int frameHeight;
        public int frames;
        public int ppu;
    }

    [MenuItem("SWARM/Arte/Aplicar import settings", priority = 20)]
    public static void ApplyAllMenu()
    {
        int count = ApplyAll();
        EditorUtility.DisplayDialog("SWARM",
            count > 0 ? count + " sprites reimportados." : "Nenhum sprite encontrado. Veja o Console.",
            "OK");
    }

    public static void ApplyAllBatch()
    {
        int count = ApplyAll();
        if (count == 0) EditorApplication.Exit(1);
    }

    public static int ApplyAll()
    {
        Dictionary<string, Spec> specs = ParseManifest();
        if (specs == null || specs.Count == 0)
        {
            Debug.LogError("[SWARM] Manifesto de arte vazio ou inválido: " + ManifestPath);
            return 0;
        }

        int applied = 0;

        foreach (KeyValuePair<string, Spec> pair in specs)
        {
            string path = Root + "/" + pair.Value.path;
            if (!File.Exists(path))
            {
                Debug.LogWarning("[SWARM] Sprite ausente: " + path);
                continue;
            }

            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                Debug.LogWarning("[SWARM] Sem TextureImporter: " + path);
                continue;
            }

            ApplyImporter(importer, pair.Value);
            applied++;
        }

        AssetDatabase.Refresh();
        AssignEnemySprites(specs);
        AssignCharacterSprites(specs);
        AssetDatabase.SaveAssets();

        Debug.Log("[SWARM] Arte reimportada e vinculada: " + applied + " sprites.");
        return applied;
    }

    static void ApplyImporter(TextureImporter importer, Spec spec)
    {
        var settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);
        settings.spriteMeshType = SpriteMeshType.FullRect;
        settings.spriteExtrude = 0;
        settings.spritePivot = new Vector2(0.5f, 0.5f);
        importer.SetTextureSettings(settings);

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = spec.frames > 1 ? SpriteImportMode.Multiple : SpriteImportMode.Single;
        importer.spritePixelsPerUnit = spec.ppu;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.maxTextureSize = 2048;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.sRGBTexture = true;
        importer.textureShape = TextureImporterShape.Texture2D;

        if (spec.frames > 1)
        {
            var metas = new SpriteMetaData[spec.frames];

            for (int i = 0; i < spec.frames; i++)
            {
                metas[i] = new SpriteMetaData
                {
                    name = Path.GetFileNameWithoutExtension(spec.path) + "_" + i,
                    rect = new Rect(i * spec.frameWidth, 0f, spec.frameWidth, spec.frameHeight),
                    alignment = (int)SpriteAlignment.Center,
                    pivot = new Vector2(0.5f, 0.5f)
                };
            }

#pragma warning disable 0618
            // TextureImporter.spritesheet ainda é o caminho estável para slices
            // de sprite sheets neste pipeline de editor.
            importer.spritesheet = metas;
#pragma warning restore 0618
        }

        importer.SaveAndReimport();
    }

    static void AssignEnemySprites(Dictionary<string, Spec> specs)
    {
        var map = new Dictionary<string, string>
        {
            { "Drone", "enemy_esporo_idle" },
            { "Corredor", "enemy_dardo_idle" },
            { "Tanque", "enemy_casco_idle" },
            { "Divisor", "enemy_celula_idle" },
            { "Atirador", "enemy_olho_idle" },
            { "Núcleo", "enemy_nucleo_idle" }
        };

        string[] paths = Directory.GetFiles("Assets/Swarm/Data/Enemies", "*.asset");

        for (int i = 0; i < paths.Length; i++)
        {
            var data = AssetDatabase.LoadAssetAtPath<EnemyData>(paths[i].Replace('\\', '/'));
            if (data == null) continue;

            if (!map.TryGetValue(data.displayName, out string key)) continue;
            if (!specs.TryGetValue(key, out Spec spec)) continue;

            Sprite sprite = LoadFirstSprite(Root + "/" + spec.path);
            if (sprite == null)
            {
                Debug.LogWarning("[SWARM] Sprite de inimigo não carregou: " + key);
                continue;
            }

            data.sprite = sprite;
            data.color = Color.white;
            EditorUtility.SetDirty(data);
        }
    }

    static void AssignCharacterSprites(Dictionary<string, Spec> specs)
    {
        var map = new Dictionary<string, string>
        {
            { "TANQUE", "ship_baluarte_idle" },
            { "VELOZ", "ship_vetor_idle" },
            { "ATIRADOR", "ship_lanca_idle" }
        };

        string[] paths = Directory.GetFiles("Assets/Swarm/Data/Characters", "*.asset");

        for (int i = 0; i < paths.Length; i++)
        {
            var data = AssetDatabase.LoadAssetAtPath<CharacterData>(paths[i].Replace('\\', '/'));
            if (data == null) continue;

            if (!map.TryGetValue(data.displayName, out string key)) continue;
            if (!specs.TryGetValue(key, out Spec spec)) continue;

            Sprite sprite = LoadFirstSprite(Root + "/" + spec.path);
            if (sprite == null)
            {
                Debug.LogWarning("[SWARM] Sprite de nave não carregou: " + key);
                continue;
            }

            data.shipSprite = sprite;
            EditorUtility.SetDirty(data);
        }
    }

    static Sprite LoadFirstSprite(string path)
    {
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
        Sprite first = null;

        for (int i = 0; i < assets.Length; i++)
        {
            if (assets[i] is Sprite sprite)
            {
                if (first == null || string.CompareOrdinal(sprite.name, first.name) < 0)
                    first = sprite;
            }
        }

        return first != null ? first : AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    static Dictionary<string, Spec> ParseManifest()
    {
        var result = new Dictionary<string, Spec>();
        if (!File.Exists(ManifestPath)) return result;

        string json = File.ReadAllText(ManifestPath);
        MatchCollection entries = Regex.Matches(json, "\"(?<key>[^\"]+)\"\\s*:\\s*\\{(?<body>[^}]*)\\}", RegexOptions.Singleline);

        foreach (Match entry in entries)
        {
            string key = entry.Groups["key"].Value;
            string body = entry.Groups["body"].Value;

            var spec = new Spec
            {
                path = StringField(body, "path"),
                frameWidth = IntField(body, "frameWidth"),
                frameHeight = IntField(body, "frameHeight"),
                frames = IntField(body, "frames"),
                ppu = IntField(body, "ppu")
            };

            if (string.IsNullOrEmpty(spec.path) || spec.frameWidth <= 0 || spec.frameHeight <= 0 ||
                spec.frames <= 0 || spec.ppu <= 0)
            {
                Debug.LogWarning("[SWARM] Entrada inválida no manifesto: " + key);
                continue;
            }

            result[key] = spec;
        }

        return result;
    }

    static string StringField(string body, string name)
    {
        Match match = Regex.Match(body, "\"" + name + "\"\\s*:\\s*\"([^\"]*)\"");
        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    static int IntField(string body, string name)
    {
        Match match = Regex.Match(body, "\"" + name + "\"\\s*:\\s*(\\d+)");
        return match.Success ? int.Parse(match.Groups[1].Value) : 0;
    }
}
#endif
