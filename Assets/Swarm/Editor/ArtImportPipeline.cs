#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
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
    const int MaxTextureSize = 2048;

    class Spec
    {
        public string path;
        public int frameWidth;
        public int frameHeight;
        public int frames;
        public int ppu;
        public int sheetWidth;
        public int sheetHeight;
    }

    public static int LastErrorCount { get; private set; }

    [MenuItem("SWARM/Arte/Aplicar import settings", priority = 20)]
    public static void ApplyAllMenu()
    {
        int count = ApplyAll();
        string message = count + " sprites reimportados.";

        if (LastErrorCount > 0)
            message += "\n\n" + LastErrorCount + " erro(s) de integridade. Veja o Console.";

        EditorUtility.DisplayDialog("SWARM", message, "OK");
    }

    public static void ApplyAllBatch()
    {
        int count = ApplyAll();

        if (count == 0 || LastErrorCount > 0)
            EditorApplication.Exit(1);
    }

    [MenuItem("SWARM/Arte/Validar PNGs", priority = 21)]
    public static void ValidateAllMenu()
    {
        int errors = ValidateAll();
        EditorUtility.DisplayDialog("SWARM",
            errors == 0 ? "PNGs válidos." : errors + " erro(s) de integridade. Veja o Console.",
            "OK");
    }

    public static void ValidateAllBatch()
    {
        int errors = ValidateAll();
        if (errors > 0) EditorApplication.Exit(1);
    }

    public static int ValidateAll()
    {
        Dictionary<string, Spec> specs = ParseManifest();
        if (specs == null || specs.Count == 0)
        {
            Debug.LogError("[SWARM] Manifesto de arte vazio ou inválido: " + ManifestPath);
            return 1;
        }

        int errors = 0;

        foreach (KeyValuePair<string, Spec> pair in specs)
        {
            string path = Root + "/" + pair.Value.path;

            if (!File.Exists(path))
            {
                Debug.LogError("[SWARM] PNG ausente: " + path);
                errors++;
                continue;
            }

            if (!ValidatePng(path, pair.Value, out string error, out string warning))
            {
                Debug.LogError("[SWARM] PNG inválido: " + path + " — " + error);
                errors++;
                continue;
            }

            if (!string.IsNullOrEmpty(warning))
                Debug.LogWarning("[SWARM] PNG: " + path + " — " + warning);
        }

        LastErrorCount = errors;
        Debug.Log("[SWARM] Validação de PNGs: " + specs.Count + " verificados, " + errors + " erro(s).");
        return errors;
    }

    public static int ApplyAll()
    {
        Dictionary<string, Spec> specs = ParseManifest();
        if (specs == null || specs.Count == 0)
        {
            Debug.LogError("[SWARM] Manifesto de arte vazio ou inválido: " + ManifestPath);
            LastErrorCount = 1;
            return 0;
        }

        int applied = 0;
        int errors = 0;

        foreach (KeyValuePair<string, Spec> pair in specs)
        {
            string path = Root + "/" + pair.Value.path;

            if (!File.Exists(path))
            {
                Debug.LogError("[SWARM] PNG ausente: " + path);
                errors++;
                continue;
            }

            if (!ValidatePng(path, pair.Value, out string error, out string warning))
            {
                Debug.LogError("[SWARM] PNG inválido: " + path + " — " + error);
                errors++;
                continue;
            }

            if (!string.IsNullOrEmpty(warning))
                Debug.LogWarning("[SWARM] PNG: " + path + " — " + warning);

            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                Debug.LogError("[SWARM] Sem TextureImporter: " + path);
                errors++;
                continue;
            }

            ApplyImporter(importer, pair.Value);
            applied++;
        }

        LastErrorCount = errors;

        AssetDatabase.Refresh();
        AssignEnemySprites(specs);
        AssignCharacterSprites(specs);
        AssetDatabase.SaveAssets();

        Debug.Log("[SWARM] Arte reimportada e vinculada: " + applied + " sprites; " + errors + " erro(s).");
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
        importer.maxTextureSize = MaxTextureSize;
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

    // ─────────────────────────────────────────────────────────────
    // VALIDAÇÃO PNG
    // ─────────────────────────────────────────────────────────────

    static bool ValidatePng(string path, Spec spec, out string error, out string warning)
    {
        error = null;
        warning = null;

        byte[] bytes;

        try
        {
            bytes = File.ReadAllBytes(path);
        }
        catch (Exception exception)
        {
            error = "não foi possível ler o arquivo (" + exception.Message + ")";
            return false;
        }

        if (bytes.Length < 20)
        {
            error = "arquivo pequeno demais para ser PNG";
            return false;
        }

        if (bytes[0] != 0x89 || bytes[1] != 0x50 || bytes[2] != 0x4E || bytes[3] != 0x47 ||
            bytes[4] != 0x0D || bytes[5] != 0x0A || bytes[6] != 0x1A || bytes[7] != 0x0A)
        {
            error = "assinatura PNG inválida";
            return false;
        }

        int width = 0;
        int height = 0;
        int bitDepth = 0;
        int colorType = 0;
        int interlace = 0;
        bool sawHeader = false;
        bool sawEnd = false;
        int idatCount = 0;
        var idat = new List<byte>();

        int position = 8;

        while (position < bytes.Length)
        {
            if (position + 12 > bytes.Length)
            {
                error = "chunk truncado no offset " + position;
                return false;
            }

            uint length = ReadUInt32(bytes, position);
            if (length > int.MaxValue || position + 12 + (int)length > bytes.Length)
            {
                error = "tamanho de chunk inválido no offset " + position;
                return false;
            }

            string type = Encoding.ASCII.GetString(bytes, position + 4, 4);
            int dataStart = position + 8;
            int dataLength = (int)length;
            int crcStart = dataStart + dataLength;

            uint storedCrc = ReadUInt32(bytes, crcStart);
            uint calculatedCrc = Crc32(bytes, position + 4, 4 + dataLength);

            if (storedCrc != calculatedCrc)
            {
                error = "CRC inválido no chunk " + type + " (offset " + position + ")";
                return false;
            }

            if (type == "IHDR")
            {
                if (sawHeader)
                {
                    error = "IHDR duplicado";
                    return false;
                }

                if (length != 13)
                {
                    error = "IHDR com tamanho inválido (" + length + ")";
                    return false;
                }

                sawHeader = true;
                width = ReadInt32(bytes, dataStart);
                height = ReadInt32(bytes, dataStart + 4);
                bitDepth = bytes[dataStart + 8];
                colorType = bytes[dataStart + 9];

                int compression = bytes[dataStart + 10];
                int filter = bytes[dataStart + 11];
                interlace = bytes[dataStart + 12];

                if (compression != 0)
                {
                    error = "método de compressão não suportado: " + compression;
                    return false;
                }

                if (filter != 0)
                {
                    error = "método de filtro IHDR não suportado: " + filter;
                    return false;
                }
            }
            else if (type == "IDAT")
            {
                idatCount++;
                for (int i = 0; i < dataLength; i++) idat.Add(bytes[dataStart + i]);
            }
            else if (type == "IEND")
            {
                sawEnd = true;
                if (length != 0)
                {
                    error = "IEND com dados";
                    return false;
                }
            }

            position = crcStart + 4;
        }

        if (!sawHeader)
        {
            error = "IHDR ausente";
            return false;
        }

        if (!sawEnd)
        {
            error = "IEND ausente";
            return false;
        }

        if (idatCount == 0)
        {
            error = "IDAT ausente";
            return false;
        }

        if (width <= 0 || height <= 0)
        {
            error = "dimensões inválidas: " + width + "x" + height;
            return false;
        }

        if (width > MaxTextureSize || height > MaxTextureSize)
        {
            error = "dimensões acima de " + MaxTextureSize + "px: " + width + "x" + height;
            return false;
        }

        if (bitDepth != 8)
        {
            error = "bit depth não suportado: " + bitDepth + " (esperado 8)";
            return false;
        }

        if (colorType != 6 && colorType != 4)
        {
            error = "color type não suportado: " + colorType + " (esperado 6/RGBA ou 4/GA)";
            return false;
        }

        if (interlace != 0)
        {
            error = "PNG entrelaçado não é suportado pelo pipeline";
            return false;
        }

        int expectedWidth = spec.frameWidth * spec.frames;
        int expectedHeight = spec.frameHeight;

        if (width != expectedWidth || height != expectedHeight)
        {
            error = "dimensões não batem com o manifesto: arquivo " + width + "x" + height +
                    ", esperado " + expectedWidth + "x" + expectedHeight;
            return false;
        }

        if (spec.sheetWidth > 0 && spec.sheetWidth != width)
        {
            error = "sheetWidth do manifesto (" + spec.sheetWidth + ") não bate com o PNG (" + width + ")";
            return false;
        }

        if (spec.sheetHeight > 0 && spec.sheetHeight != height)
        {
            error = "sheetHeight do manifesto (" + spec.sheetHeight + ") não bate com o PNG (" + height + ")";
            return false;
        }

        int bytesPerPixel = colorType == 6 ? 4 : 2;
        int stride = width * bytesPerPixel;
        long expectedRawLength = (long)height * (1 + stride);

        if (idat.Count <= 6)
        {
            error = "stream zlib curto demais";
            return false;
        }

        byte[] raw;

        try
        {
            using (var compressed = new MemoryStream(idat.ToArray(), 2, idat.Count - 6, false))
            using (var deflate = new DeflateStream(compressed, CompressionMode.Decompress))
            using (var output = new MemoryStream())
            {
                deflate.CopyTo(output);
                raw = output.ToArray();
            }
        }
        catch (Exception exception)
        {
            error = "zlib/deflate inválido (" + exception.Message + ")";
            return false;
        }

        if (raw.Length != expectedRawLength)
        {
            error = "scanlines com tamanho inesperado: " + raw.Length + " bytes, esperado " + expectedRawLength;
            return false;
        }

        bool hasTransparentPixel = false;

        for (int y = 0; y < height; y++)
        {
            int rowStart = y * (1 + stride);
            int filter = raw[rowStart];

            if (filter > 4)
            {
                error = "filtro de scanline inválido na linha " + y + ": " + filter;
                return false;
            }

            if (colorType == 6)
            {
                for (int x = 0; x < width; x++)
                {
                    if (raw[rowStart + 1 + x * 4 + 3] < 255)
                    {
                        hasTransparentPixel = true;
                        break;
                    }
                }
            }
            else
            {
                for (int x = 0; x < width; x++)
                {
                    if (raw[rowStart + 1 + x * 2 + 1] < 255)
                    {
                        hasTransparentPixel = true;
                        break;
                    }
                }
            }
        }

        if (!hasTransparentPixel)
            warning = "sem pixels transparentes; confirme se o fundo deveria ser opaco";

        return true;
    }

    static uint ReadUInt32(byte[] bytes, int offset)
    {
        return ((uint)bytes[offset] << 24) |
               ((uint)bytes[offset + 1] << 16) |
               ((uint)bytes[offset + 2] << 8) |
               bytes[offset + 3];
    }

    static int ReadInt32(byte[] bytes, int offset)
    {
        return (bytes[offset] << 24) |
               (bytes[offset + 1] << 16) |
               (bytes[offset + 2] << 8) |
               bytes[offset + 3];
    }

    static uint Crc32(byte[] bytes, int offset, int count)
    {
        uint crc = 0xFFFFFFFF;

        for (int i = 0; i < count; i++)
        {
            crc ^= bytes[offset + i];

            for (int bit = 0; bit < 8; bit++)
                crc = (crc >> 1) ^ (0xEDB88320u & (uint)-(int)(crc & 1u));
        }

        return ~crc;
    }

    // ─────────────────────────────────────────────────────────────
    // VÍNCULO DE DADOS
    // ─────────────────────────────────────────────────────────────

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
        UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
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
                ppu = IntField(body, "ppu"),
                sheetWidth = IntField(body, "sheetWidth"),
                sheetHeight = IntField(body, "sheetHeight")
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
