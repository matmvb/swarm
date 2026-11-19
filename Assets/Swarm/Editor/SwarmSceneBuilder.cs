#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Gerador do protótipo: cria sprites, dados (ScriptableObjects),
/// a cena completa e liga todas as referências.
///
/// Menu: SWARM > Montar Protótipo Completo
/// </summary>
public static class SwarmSceneBuilder
{
    const string ROOT = "Assets/Swarm";
    const string SCENE_PATH = ROOT + "/Scenes/Swarm.unity";

    static Sprite _white;
    static Material _spriteMaterial;
    static Sprite _ship, _hex, _diamond, _square, _octagon, _circle, _ring, _grid;
    static Font _font;

    [MenuItem("SWARM/Montar Protótipo Completo", priority = 0)]
    public static void BuildAll()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        bool confirmed = EditorUtility.DisplayDialog(
            "SWARM",
            "Isso (re)cria os assets e a cena do protótipo em:\n\n" + SCENE_PATH + "\n\nContinuar?",
            "Sim, montar", "Cancelar");

        if (!confirmed) return;

        BuildProject();

        EditorUtility.DisplayDialog("SWARM", "Pronto!\n\nA cena foi criada e aberta.\nAperte Play.", "OK");
    }

    /// <summary>
    /// Mesma coisa, sem diálogos. Use em batch:
    /// Unity.exe -batchmode -quit -projectPath "..." -executeMethod SwarmSceneBuilder.BuildAllBatch
    /// </summary>
    public static void BuildAllBatch()
    {
        BuildProject();
    }

    /// <summary>Gera e valida em uma única execução (batch).</summary>
    public static void BuildAndValidateBatch()
    {
        BuildProject();
        ValidateScene();
    }

    static void BuildProject()
    {
        // A cena precisa ser criada ANTES dos assets: NewScene dispara um refresh
        // que recarrega ScriptableObjects e invalida as instâncias em memória
        // (referências viram fileID: 0 na cena salva).
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        EnsureFolders();
        _white = EnsureWhiteSprite();
        _spriteMaterial = EnsureSpriteMaterial();
        _ship = SwarmArt.Ship;
        _hex = SwarmArt.Hex;
        _diamond = SwarmArt.Diamond;
        _square = SwarmArt.Square;
        _octagon = SwarmArt.Octagon;
        _circle = SwarmArt.Circle;
        _ring = SwarmArt.Ring;
        _grid = SwarmArt.Grid();
        _font = null;

        List<string> enemyPaths = EnsureEnemyData();
        List<string> upgradePaths = EnsureUpgradeData();
        List<string> characterPaths = EnsureCharacterData();
        AssetDatabase.SaveAssets();

        List<EnemyData> enemies = LoadAssets<EnemyData>(enemyPaths);
        List<UpgradeData> upgrades = LoadAssets<UpgradeData>(upgradePaths);
        List<CharacterData> characters = LoadAssets<CharacterData>(characterPaths);
        AssignEnemySprites(enemies);
        LinkPrerequisites();
        AssetDatabase.SaveAssets();

        GameObject cameraObject = CreateCamera();
        CreateBackgroundGrid(cameraObject.transform);
        GameObject player = CreatePlayer(out PlayerStats stats, out Health health, out XpSystem xpSystem, out UpgradeSystem upgradeSystem);

        SetRef(cameraObject.GetComponent<CameraFollow>(), "target", player.transform);

        GameObject projectileTemplate = CreateProjectileTemplate();
        GameObject orbTemplate = CreateOrbTemplate();
        GameObject bossProjectileTemplate = CreateBossProjectileTemplate();
        GameObject burstTemplate = CreateBurstTemplate();

        var thruster = player.AddComponent<ThrusterTrail>();
        SetRef(thruster, "visual", player.transform.Find("Visual"));
        SetRef(thruster, "template", burstTemplate.GetComponent<BurstParticle>());

        var muzzleFlashObject = new GameObject("MuzzleFlash");
        var muzzleFlash = muzzleFlashObject.AddComponent<MuzzleFlash>();
        SetRef(muzzleFlash, "template", burstTemplate.GetComponent<BurstParticle>());
        List<Enemy> enemyTemplates = CreateEnemyTemplates(enemies);

        if (enemyTemplates.Count >= 6)
        {
            var splitter = enemyTemplates[3].gameObject.AddComponent<SplitOnDeath>();
            SetRef(splitter, "fragmentTemplate", enemyTemplates[0]);

            var shooter = enemyTemplates[4].gameObject.AddComponent<RangedEnemy>();
            SetRef(shooter, "projectilePrefab", bossProjectileTemplate.GetComponent<EnemyProjectile>());
        }

        Enemy bossTemplate = null;
        if (enemyTemplates.Count > 0)
        {
            bossTemplate = enemyTemplates[enemyTemplates.Count - 1];
            enemyTemplates.RemoveAt(enemyTemplates.Count - 1);
        }

        if (bossTemplate != null)
        {
            var boss = bossTemplate.gameObject.AddComponent<BossController>();
            SetRef(boss, "projectilePrefab", bossProjectileTemplate.GetComponent<EnemyProjectile>());
        }

        SetRef(player.GetComponent<Weapon>(), "projectilePrefab", projectileTemplate.GetComponent<Projectile>());
        SetRef(xpSystem, "orbPrefab", orbTemplate.GetComponent<XpOrb>());
        SetRef(xpSystem, "upgrades", upgradeSystem);
        SetList(upgradeSystem, "pool", ToObjectList(upgrades));

        CreateSpawner(player.transform, enemyTemplates, bossTemplate);
        HudController hud = CreateHud(stats, health, xpSystem, upgradeSystem, characters);
        SetRef(xpSystem, "hud", hud);

        GameObject manager = CreateManager();
        CreateSfx();
        CreateGlobalVolume();

        var burstObject = new GameObject("DeathBurst");
        var deathBurst = burstObject.AddComponent<DeathBurst>();
        SetRef(deathBurst, "template", burstTemplate.GetComponent<BurstParticle>());

        var runLogger = manager.AddComponent<RunLogger>();
        SetRef(runLogger, "xp", xpSystem);
        SetRef(runLogger, "upgrades", upgradeSystem);

        var debugObject = new GameObject("DebugOverlay");
        var debugOverlay = debugObject.AddComponent<DebugOverlay>();
        SetRef(debugOverlay, "stats", stats);
        SetRef(debugOverlay, "health", health);

        EnsureFolder(ROOT + "/Scenes");
        EditorSceneManager.SaveScene(scene, SCENE_PATH);

        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(SCENE_PATH, true) };
        AssetDatabase.Refresh();

        Debug.Log("[SWARM] Protótipo criado/atualizado em " + SCENE_PATH);
    }

    [MenuItem("SWARM/Abrir Cena do Protótipo", priority = 1)]
    public static void OpenScene()
    {
        if (!string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(SCENE_PATH))) EditorSceneManager.OpenScene(SCENE_PATH);
        else EditorUtility.DisplayDialog("SWARM", "Cena não existe. Rode 'Montar Protótipo Completo'.", "OK");
    }

    // ─────────────────────────────────────────────────────────────
    // CENA
    // ─────────────────────────────────────────────────────────────

    static GameObject CreateCamera()
    {
        var go = new GameObject("Main Camera");
        go.tag = "MainCamera";
        go.transform.position = new Vector3(0f, 0f, -10f);

        var camera = go.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 6f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.04f, 0.055f, 0.10f, 1f);
        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = 100f;

        // Câmera criada por script não vem com AudioListener: sem isso, o jogo fica mudo.
        go.AddComponent<AudioListener>();

        var cameraData = go.GetComponent<UniversalAdditionalCameraData>();
        if (cameraData == null) cameraData = go.AddComponent<UniversalAdditionalCameraData>();
        cameraData.renderPostProcessing = true;

        go.AddComponent<CameraFollow>();
        go.AddComponent<ScreenshotCapture>();
        return go;
    }

    static void CreateBackgroundGrid(Transform follow)
    {
        var go = new GameObject("BackgroundGrid");
        go.transform.position = new Vector3(0f, 0f, 1f);

        var sprite = go.AddComponent<SpriteRenderer>();
        sprite.sprite = _grid;
        sprite.color = new Color(0.18f, 0.30f, 0.55f, 0.35f);
        sprite.drawMode = SpriteDrawMode.Tiled;
        sprite.size = new Vector2(40f, 24f);
        sprite.tileMode = SpriteTileMode.Continuous;
        sprite.sortingOrder = -100;
        ApplySpriteMaterial(sprite);

        var grid = go.AddComponent<BackgroundGrid>();
        SetRef(grid, "follow", follow);
    }

    static GameObject CreatePlayer(out PlayerStats stats, out Health health, out XpSystem xp, out UpgradeSystem upgrades)    {
        var go = new GameObject("Player");
        go.tag = "Player";
        go.transform.position = Vector3.zero;
        go.transform.localScale = Vector3.one * 0.8f;

        var visual = new GameObject("Visual");
        visual.transform.SetParent(go.transform, false);

        var sprite = visual.AddComponent<SpriteRenderer>();
        sprite.sprite = _ship;
        sprite.color = new Color(0.30f, 1f, 0.88f);
        sprite.sortingOrder = 10;
        ApplySpriteMaterial(sprite);

        var body = go.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;

        var collider = go.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.5f;

        var muzzle = new GameObject("Muzzle");
        muzzle.transform.SetParent(visual.transform, false);
        muzzle.transform.localPosition = new Vector3(0f, 0.55f, 0f);

        stats = go.AddComponent<PlayerStats>();
        health = go.AddComponent<Health>();
        var controller = go.AddComponent<PlayerController>();
        var weapon = go.AddComponent<Weapon>();
        xp = go.AddComponent<XpSystem>();
        upgrades = go.AddComponent<UpgradeSystem>();
        SetRef(upgrades, "playerHealth", health);
        var orbital = go.AddComponent<OrbitalWeapon>();
        SetRef(orbital, "orbSprite", _circle);
        go.AddComponent<PlayerRegen>();

        var playerVisual = go.AddComponent<PlayerVisual>();
        SetRef(playerVisual, "visual", visual.transform);

        SetRef(controller, "stats", stats);
        SetRef(controller, "health", health);
        SetRef(weapon, "stats", stats);
        SetRef(weapon, "muzzle", muzzle.transform);

        return go;
    }

    static GameObject CreateProjectileTemplate()
    {
        var go = new GameObject("ProjectileTemplate");
        go.transform.position = new Vector3(0f, -20f, 0f);
        go.transform.localScale = Vector3.one * 0.22f;

        var sprite = go.AddComponent<SpriteRenderer>();
        sprite.sprite = _diamond;
        sprite.color = new Color(0.6f, 1f, 1f);
        sprite.sortingOrder = 5;
        ApplySpriteMaterial(sprite);

        var body = go.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        var collider = go.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.5f;

        var trail = go.AddComponent<TrailRenderer>();
        trail.time = 0.08f;
        trail.startWidth = 0.14f;
        trail.endWidth = 0f;
        trail.sharedMaterial = _spriteMaterial;
        trail.startColor = new Color(0.6f, 1f, 1f, 0.85f);
        trail.endColor = new Color(0.6f, 1f, 1f, 0f);
        trail.sortingOrder = 4;
        trail.minVertexDistance = 0.03f;
        trail.textureMode = LineTextureMode.Stretch;

        go.AddComponent<Projectile>();
        go.SetActive(false);
        return go;
    }

    static GameObject CreateOrbTemplate()
    {
        var go = new GameObject("XpOrbTemplate");
        go.transform.position = new Vector3(0f, -25f, 0f);
        go.transform.localScale = Vector3.one * 0.3f;

        var sprite = go.AddComponent<SpriteRenderer>();
        sprite.sprite = _circle;
        sprite.color = new Color(0.4f, 1f, 0.5f);
        sprite.sortingOrder = 4;
        ApplySpriteMaterial(sprite);

        go.AddComponent<XpOrb>();
        go.SetActive(false);
        return go;
    }

    static GameObject CreateBossProjectileTemplate()
    {
        var go = new GameObject("BossProjectileTemplate");
        go.transform.position = new Vector3(6f, -20f, 0f);
        go.transform.localScale = Vector3.one * 0.35f;

        var sprite = go.AddComponent<SpriteRenderer>();
        sprite.sprite = _circle;
        sprite.color = new Color(1f, 0.35f, 0.55f);
        sprite.sortingOrder = 6;
        ApplySpriteMaterial(sprite);

        var body = go.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        var collider = go.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.5f;

        go.AddComponent<EnemyProjectile>();
        go.SetActive(false);
        return go;
    }

    static List<Enemy> CreateEnemyTemplates(List<EnemyData> data)
    {
        var result = new List<Enemy>();
        var parent = new GameObject("Templates").transform;

        for (int i = 0; i < data.Count; i++)
        {
            EnemyData enemyData = data[i];

            var go = new GameObject("Enemy_" + enemyData.displayName);
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(0f, -30f - i * 3f, 0f);

            var sprite = go.AddComponent<SpriteRenderer>();
            sprite.sprite = enemyData.sprite != null ? enemyData.sprite : _white;
            sprite.color = enemyData.color;
            sprite.sortingOrder = 3;
            ApplySpriteMaterial(sprite);

            var body = go.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;

            var collider = go.AddComponent<CircleCollider2D>();
            collider.isTrigger = false;
            collider.radius = 0.5f;

            var enemy = go.AddComponent<Enemy>();
            SetRef(enemy, "data", enemyData);

            go.SetActive(false);
            result.Add(enemy);
        }

        return result;
    }

    static void CreateSpawner(Transform target, List<Enemy> templates, Enemy bossTemplate)
    {
        var go = new GameObject("EnemySpawner");
        var spawner = go.AddComponent<EnemySpawner>();

        SetRef(spawner, "target", target);
        SetList(spawner, "templates", ToObjectList(templates));
        SetRef(spawner, "bossTemplate", bossTemplate);
    }

    static GameObject CreateManager()
    {
        var go = new GameObject("GameManager");
        go.AddComponent<GameManager>();
        return go;
    }

    static void CreateSfx()
    {
        var go = new GameObject("Sfx");
        go.AddComponent<Sfx>();
        go.AddComponent<HitStop>();
    }

    static GameObject CreateBurstTemplate()
    {
        var go = new GameObject("BurstParticleTemplate");
        go.transform.position = new Vector3(0f, -35f, 0f);
        go.transform.localScale = Vector3.one * 0.2f;

        var sprite = go.AddComponent<SpriteRenderer>();
        sprite.sprite = _circle;
        sprite.color = Color.white;
        sprite.sortingOrder = 7;
        ApplySpriteMaterial(sprite);

        go.AddComponent<BurstParticle>();
        go.SetActive(false);
        return go;
    }

    static void CreateGlobalVolume()
    {
        EnsureFolder(ROOT + "/Settings");

        string path = ROOT + "/Settings/NeonProfile.asset";
        var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(path);

        if (profile == null)
        {
            profile = ScriptableObject.CreateInstance<VolumeProfile>();
            AssetDatabase.CreateAsset(profile, path);

            var bloom = profile.Add<Bloom>(true);
            bloom.intensity.Override(0.75f);
            bloom.threshold.Override(0.55f);
            bloom.scatter.Override(0.65f);

            var vignette = profile.Add<Vignette>(true);
            vignette.intensity.Override(0.25f);
            vignette.smoothness.Override(0.4f);

            EditorUtility.SetDirty(profile);
        }

        var go = new GameObject("Global Volume");
        var volume = go.AddComponent<Volume>();
        volume.isGlobal = true;
        volume.priority = 0f;
        volume.sharedProfile = profile;
    }

    // ─────────────────────────────────────────────────────────────
    // HUD
    // ─────────────────────────────────────────────────────────────

    static HudController CreateHud(PlayerStats stats, Health health, XpSystem xp, UpgradeSystem upgrades, List<CharacterData> characters)
    {
        var canvasObject = new GameObject("HUD", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
        var canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        var hud = canvasObject.AddComponent<HudController>();
        Image damageFlash = MakeFlash(canvasObject.transform);

        Text timer = MakeText(canvasObject.transform, "Timer", new Vector2(0.5f, 1f), new Vector2(0f, -40f), new Vector2(420f, 60f), 40, TextAnchor.MiddleCenter);
        Text kills = MakeText(canvasObject.transform, "Kills", new Vector2(0f, 1f), new Vector2(150f, -40f), new Vector2(280f, 50f), 28, TextAnchor.MiddleLeft);
        Text level = MakeText(canvasObject.transform, "Level", new Vector2(1f, 1f), new Vector2(-150f, -40f), new Vector2(280f, 50f), 28, TextAnchor.MiddleRight);
        Text hpText = MakeText(canvasObject.transform, "HpText", new Vector2(0f, 1f), new Vector2(150f, -95f), new Vector2(280f, 40f), 22, TextAnchor.MiddleLeft);
        Text hint = MakeText(canvasObject.transform, "Hint", new Vector2(0.5f, 1f), new Vector2(0f, -82f), new Vector2(1000f, 30f), 18, TextAnchor.MiddleCenter);
        hint.text = "WASD move  ·  tiro automático  ·  1/2/3 escolhe upgrade";
        hint.color = new Color(1f, 1f, 1f, 0.4f);

        Image hpFill = MakeBar(canvasObject.transform, "HpBar", new Vector2(0f, 1f), new Vector2(20f, -125f), new Vector2(260f, 18f), new Color(1f, 0.25f, 0.35f));
        Image xpFill = MakeBar(canvasObject.transform, "XpBar", new Vector2(0.5f, 0f), new Vector2(0f, 18f), new Vector2(1200f, 10f), new Color(0.4f, 1f, 0.5f));
        GameObject bossBar = MakeBossBar(canvasObject.transform, out Image bossFill, out Text bossLabel);
        Text bossToast = MakeText(canvasObject.transform, "BossToast", new Vector2(0.5f, 0.5f), new Vector2(0f, 120f), new Vector2(1000f, 50f), 34, TextAnchor.MiddleCenter);
        bossToast.color = new Color(1f, 0.75f, 0.3f);
        bossToast.gameObject.SetActive(false);

        GameObject levelUpPanel = MakePanel(canvasObject.transform, "LevelUpPanel", new Color(0f, 0f, 0f, 0.82f));
        Text levelUpTitle = MakeText(levelUpPanel.transform, "LevelUpTitle", new Vector2(0.5f, 0.5f), new Vector2(0f, 220f), new Vector2(1400f, 80f), 46, TextAnchor.MiddleCenter);
        Text levelUpOptions = MakeText(levelUpPanel.transform, "LevelUpOptions", new Vector2(0.5f, 0.5f), new Vector2(0f, -40f), new Vector2(1100f, 380f), 30, TextAnchor.MiddleCenter);
        levelUpOptions.verticalOverflow = VerticalWrapMode.Overflow;

        GameObject gameOverPanel = MakePanel(canvasObject.transform, "GameOverPanel", new Color(0.02f, 0.02f, 0.05f, 0.92f));
        Text gameOverText = MakeText(gameOverPanel.transform, "GameOverText", new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1100f, 640f), 34, TextAnchor.MiddleCenter);
        gameOverText.verticalOverflow = VerticalWrapMode.Overflow;

        GameObject characterPanel = MakePanel(canvasObject.transform, "CharacterSelectPanel", new Color(0.02f, 0.02f, 0.05f, 0.96f));
        Text characterTitle = MakeText(characterPanel.transform, "CharacterTitle", new Vector2(0.5f, 0.5f), new Vector2(0f, 300f), new Vector2(1400f, 80f), 46, TextAnchor.MiddleCenter);
        Text characterOptions = MakeText(characterPanel.transform, "CharacterOptions", new Vector2(0.5f, 0.5f), new Vector2(0f, -50f), new Vector2(1100f, 560f), 26, TextAnchor.MiddleCenter);
        characterOptions.verticalOverflow = VerticalWrapMode.Overflow;

        var characterSelect = canvasObject.AddComponent<CharacterSelect>();
        SetRef(characterSelect, "panel", characterPanel);
        SetRef(characterSelect, "title", characterTitle);
        SetRef(characterSelect, "options", characterOptions);
        SetList(characterSelect, "characters", ToObjectList(characters));
        SetRef(characterSelect, "stats", stats);
        SetRef(characterSelect, "health", health);

        GameObject optionsPanel = MakePanel(canvasObject.transform, "OptionsPanel", new Color(0.02f, 0.02f, 0.05f, 0.97f));
        Text optionsTitle = MakeText(optionsPanel.transform, "OptionsTitle", new Vector2(0.5f, 0.5f), new Vector2(0f, 260f), new Vector2(1400f, 80f), 46, TextAnchor.MiddleCenter);
        Text optionsText = MakeText(optionsPanel.transform, "OptionsText", new Vector2(0.5f, 0.5f), new Vector2(0f, -20f), new Vector2(1100f, 380f), 30, TextAnchor.MiddleCenter);
        optionsText.verticalOverflow = VerticalWrapMode.Overflow;

        var optionsMenu = canvasObject.AddComponent<OptionsMenu>();
        SetRef(optionsMenu, "panel", optionsPanel);
        SetRef(optionsMenu, "title", optionsTitle);
        SetRef(optionsMenu, "options", optionsText);
        SetRef(optionsMenu, "characterSelect", characterSelect);

        SetRef(hud, "timerText", timer);
        SetRef(hud, "killsText", kills);
        SetRef(hud, "levelText", level);
        SetRef(hud, "hpText", hpText);
        SetRef(hud, "hudHint", hint);
        SetRef(hud, "hpFill", hpFill);
        SetRef(hud, "xpFill", xpFill);
        SetRef(hud, "damageFlash", damageFlash);
        SetRef(hud, "levelUpPanel", levelUpPanel);
        SetRef(hud, "levelUpTitle", levelUpTitle);
        SetRef(hud, "levelUpOptions", levelUpOptions);
        SetRef(hud, "gameOverPanel", gameOverPanel);
        SetRef(hud, "gameOverText", gameOverText);
        SetRef(hud, "bossBar", bossBar);
        SetRef(hud, "bossFill", bossFill);
        SetRef(hud, "bossLabel", bossLabel);
        SetRef(hud, "bossToast", bossToast);
        SetRef(hud, "xp", xp);
        SetRef(hud, "playerHealth", health);
        SetRef(hud, "upgrades", upgrades);
        SetRef(hud, "stats", stats);

        return hud;
    }

    static Text MakeText(Transform parent, string name, Vector2 anchor, Vector2 position, Vector2 size, int fontSize, TextAnchor alignment)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);

        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        var text = go.GetComponent<Text>();
        text.font = UIFont();
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        text.text = name;
        text.raycastTarget = false;

        return text;
    }

    static Image MakeFlash(Transform parent)
    {
        var go = new GameObject("DamageFlash", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);

        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var image = go.GetComponent<Image>();
        image.sprite = _white;
        image.color = new Color(1f, 0.10f, 0.15f, 0f);
        image.raycastTarget = false;

        return image;
    }

    static Image MakeBar(Transform parent, string name, Vector2 anchor, Vector2 position, Vector2 size, Color color)    {
        var background = new GameObject(name, typeof(RectTransform), typeof(Image));
        background.transform.SetParent(parent, false);

        var backgroundRect = background.GetComponent<RectTransform>();
        backgroundRect.anchorMin = anchor;
        backgroundRect.anchorMax = anchor;
        backgroundRect.pivot = new Vector2(anchor.x, anchor.y);
        backgroundRect.anchoredPosition = position;
        backgroundRect.sizeDelta = size;

        var backgroundImage = background.GetComponent<Image>();
        backgroundImage.sprite = _white;
        backgroundImage.color = new Color(0f, 0f, 0f, 0.55f);
        backgroundImage.raycastTarget = false;

        var fillObject = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        fillObject.transform.SetParent(background.transform, false);

        var fillRect = fillObject.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        var fill = fillObject.GetComponent<Image>();
        fill.sprite = _white;
        fill.color = color;
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillAmount = 1f;
        fill.raycastTarget = false;

        return fill;
    }

    static GameObject MakeBossBar(Transform parent, out Image fill, out Text label)
    {
        var container = new GameObject("BossBar", typeof(RectTransform), typeof(Image));
        container.transform.SetParent(parent, false);

        var rect = container.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -130f);
        rect.sizeDelta = new Vector2(760f, 22f);

        var background = container.GetComponent<Image>();
        background.sprite = _white;
        background.color = new Color(0f, 0f, 0f, 0.65f);
        background.raycastTarget = false;

        var fillObject = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        fillObject.transform.SetParent(container.transform, false);

        var fillRect = fillObject.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        fill = fillObject.GetComponent<Image>();
        fill.sprite = _white;
        fill.color = new Color(1f, 0.20f, 0.28f);
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillAmount = 1f;
        fill.raycastTarget = false;

        label = MakeText(parent, "BossLabel", new Vector2(0.5f, 1f), new Vector2(0f, -106f), new Vector2(760f, 24f), 20, TextAnchor.MiddleCenter);
        label.text = "NÚCLEO";
        label.color = new Color(1f, 0.45f, 0.5f);

        container.SetActive(false);
        label.gameObject.SetActive(false);

        return container;
    }

    static GameObject MakePanel(Transform parent, string name, Color color)    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);

        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var image = go.GetComponent<Image>();
        image.sprite = _white;
        image.color = color;
        image.raycastTarget = false;

        go.SetActive(false);
        return go;
    }

    static Font UIFont()
    {
        if (_font != null) return _font;

        try { _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); } catch { }
        if (_font == null)
        {
            try { _font = Resources.GetBuiltinResource<Font>("Arial.ttf"); } catch { }
        }

        return _font;
    }

    // ─────────────────────────────────────────────────────────────
    // VALIDAÇÃO
    // ─────────────────────────────────────────────────────────────

    [MenuItem("SWARM/Validar Cena do Protótipo", priority = 2)]
    public static void ValidateSceneMenu()
    {
        int problems = ValidateScene();
        EditorUtility.DisplayDialog("SWARM",
            problems == 0 ? "Cena OK: nenhum problema encontrado." : problems + " problema(s). Veja o Console.",
            "OK");
    }

    public static void ValidateSceneBatch()
    {
        ValidateScene();
    }

    /// <summary>Confere se todas as referências obrigatórias da cena estão ligadas.</summary>
    public static int ValidateScene()
    {
        int problems = 0;

        if (string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(SCENE_PATH)))
        {
            Debug.LogError("[SWARM] Cena não existe: " + SCENE_PATH);
            return 1;
        }

        EditorSceneManager.OpenScene(SCENE_PATH);

        PlayerController player = Object.FindFirstObjectByType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("[SWARM] Player não encontrado");
            problems++;
        }
        else
        {
            problems += CheckRefs(player, "stats", "health");
            problems += CheckComponents<Rigidbody2D>(player.gameObject, "Rigidbody2D");
            problems += CheckComponents<CircleCollider2D>(player.gameObject, "CircleCollider2D");
            problems += CheckComponents<OrbitalWeapon>(player.gameObject, "OrbitalWeapon");
            problems += CheckComponents<PlayerRegen>(player.gameObject, "PlayerRegen");

            Weapon weapon = player.GetComponent<Weapon>();
            if (weapon == null) { Debug.LogError("[SWARM] Weapon ausente no Player"); problems++; }
            else problems += CheckRefs(weapon, "stats", "muzzle", "projectilePrefab");

            XpSystem xp = player.GetComponent<XpSystem>();
            if (xp == null) { Debug.LogError("[SWARM] XpSystem ausente no Player"); problems++; }
            else problems += CheckRefs(xp, "orbPrefab", "upgrades", "hud");

            UpgradeSystem upgrades = player.GetComponent<UpgradeSystem>();
            if (upgrades == null) { Debug.LogError("[SWARM] UpgradeSystem ausente no Player"); problems++; }
            else
            {
                problems += CheckArray(upgrades, "pool", 20);
                problems += CheckRefs(upgrades, "playerHealth");
            }
        }

        CameraFollow cameraFollow = Object.FindFirstObjectByType<CameraFollow>();
        if (cameraFollow == null) { Debug.LogError("[SWARM] CameraFollow ausente"); problems++; }
        else problems += CheckRefs(cameraFollow, "target");

        EnemySpawner spawner = Object.FindFirstObjectByType<EnemySpawner>();
        if (spawner == null)
        {
            Debug.LogError("[SWARM] EnemySpawner ausente");
            problems++;
        }
        else
        {
            problems += CheckRefs(spawner, "target", "bossTemplate");
            problems += CheckArray(spawner, "templates", 5);
        }

        HudController hud = Object.FindFirstObjectByType<HudController>();
        if (hud == null)
        {
            Debug.LogError("[SWARM] HudController ausente");
            problems++;
        }
        else
        {
            problems += CheckRefs(hud, "timerText", "killsText", "levelText", "hpText", "hudHint",
                "hpFill", "xpFill", "levelUpPanel", "levelUpTitle", "levelUpOptions",
                "gameOverPanel", "gameOverText", "bossBar", "bossFill", "bossLabel", "bossToast", "damageFlash",
                "xp", "playerHealth", "upgrades", "stats");
        }

        CharacterSelect characterSelect = Object.FindFirstObjectByType<CharacterSelect>();
        if (characterSelect == null)
        {
            Debug.LogError("[SWARM] CharacterSelect ausente");
            problems++;
        }
        else
        {
            problems += CheckRefs(characterSelect, "panel", "title", "options", "stats", "health");
            problems += CheckArray(characterSelect, "characters", 3);
        }

        DeathBurst deathBurst = Object.FindFirstObjectByType<DeathBurst>();
        if (deathBurst == null)
        {
            Debug.LogError("[SWARM] DeathBurst ausente");
            problems++;
        }
        else
        {
            problems += CheckRefs(deathBurst, "template");
        }

        BurstParticle[] burstParticles = Object.FindObjectsByType<BurstParticle>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (burstParticles.Length != 1)
        {
            Debug.LogError("[SWARM] Esperado 1 template de partícula, achei " + burstParticles.Length);
            problems++;
        }
        foreach (BurstParticle particle in burstParticles)
        {
            if (particle.gameObject.activeSelf)
            {
                Debug.LogError("[SWARM] Template de partícula deveria estar inativo");
                problems++;
            }
        }

        HitStop hitStop = Object.FindFirstObjectByType<HitStop>();
        if (hitStop == null)
        {
            Debug.LogError("[SWARM] HitStop ausente");
            problems++;
        }

        ScreenshotCapture screenshot = Object.FindFirstObjectByType<ScreenshotCapture>();
        if (screenshot == null)
        {
            Debug.LogError("[SWARM] ScreenshotCapture ausente");
            problems++;
        }

        BackgroundGrid backgroundGrid = Object.FindFirstObjectByType<BackgroundGrid>();
        if (backgroundGrid == null)
        {
            Debug.LogError("[SWARM] BackgroundGrid ausente");
            problems++;
        }
        else
        {
            problems += CheckRefs(backgroundGrid, "follow");
        }

        PlayerVisual playerVisual = Object.FindFirstObjectByType<PlayerVisual>();
        if (playerVisual == null)
        {
            Debug.LogError("[SWARM] PlayerVisual ausente");
            problems++;
        }
        else
        {
            problems += CheckRefs(playerVisual, "visual");
        }

        MuzzleFlash muzzleFlash = Object.FindFirstObjectByType<MuzzleFlash>();
        if (muzzleFlash == null)
        {
            Debug.LogError("[SWARM] MuzzleFlash ausente");
            problems++;
        }
        else
        {
            problems += CheckRefs(muzzleFlash, "template");
        }

        ThrusterTrail thruster = Object.FindFirstObjectByType<ThrusterTrail>();
        if (thruster == null)
        {
            Debug.LogError("[SWARM] ThrusterTrail ausente");
            problems++;
        }
        else
        {
            problems += CheckRefs(thruster, "visual", "template");
        }

        OptionsMenu optionsMenu = Object.FindFirstObjectByType<OptionsMenu>();
        if (optionsMenu == null)
        {
            Debug.LogError("[SWARM] OptionsMenu ausente");
            problems++;
        }
        else
        {
            problems += CheckRefs(optionsMenu, "panel", "title", "options", "characterSelect");
        }

        SplitOnDeath splitOnDeath = Object.FindFirstObjectByType<SplitOnDeath>(FindObjectsInactive.Include);
        if (splitOnDeath == null)
        {
            Debug.LogError("[SWARM] SplitOnDeath ausente");
            problems++;
        }
        else
        {
            problems += CheckRefs(splitOnDeath, "fragmentTemplate");
        }

        RangedEnemy rangedEnemy = Object.FindFirstObjectByType<RangedEnemy>(FindObjectsInactive.Include);
        if (rangedEnemy == null)
        {
            Debug.LogError("[SWARM] RangedEnemy ausente");
            problems++;
        }
        else
        {
            problems += CheckRefs(rangedEnemy, "projectilePrefab");
        }

        Enemy[] enemies = Object.FindObjectsByType<Enemy>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (enemies.Length != 6) { Debug.LogError("[SWARM] Esperados 6 templates de inimigo, achei " + enemies.Length); problems++; }

        foreach (Enemy enemy in enemies)
        {
            problems += CheckRefs(enemy, "data");
            if (enemy.GetComponent<Rigidbody2D>() == null) { Debug.LogError("[SWARM] " + enemy.name + " sem Rigidbody2D"); problems++; }
            if (enemy.GetComponent<Collider2D>() == null) { Debug.LogError("[SWARM] " + enemy.name + " sem Collider2D"); problems++; }
            if (enemy.gameObject.activeSelf) { Debug.LogError("[SWARM] Template " + enemy.name + " deveria estar inativo"); problems++; }
        }

        Projectile[] projectiles = Object.FindObjectsByType<Projectile>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (projectiles.Length != 1) { Debug.LogError("[SWARM] Esperado 1 template de projétil, achei " + projectiles.Length); problems++; }
        foreach (Projectile projectile in projectiles)
        {
            if (projectile.GetComponent<Rigidbody2D>() == null || projectile.GetComponent<Collider2D>() == null)
            {
                Debug.LogError("[SWARM] Template de projétil sem Rigidbody2D/Collider2D");
                problems++;
            }
            if (projectile.gameObject.activeSelf) { Debug.LogError("[SWARM] Template de projétil deveria estar inativo"); problems++; }
        }

        XpOrb[] orbs = Object.FindObjectsByType<XpOrb>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (orbs.Length != 1) { Debug.LogError("[SWARM] Esperado 1 template de orbe, achei " + orbs.Length); problems++; }
        foreach (XpOrb orb in orbs)
        {
            if (orb.gameObject.activeSelf) { Debug.LogError("[SWARM] Template de orbe deveria estar inativo"); problems++; }
        }

        bool inBuildSettings = false;
        foreach (EditorBuildSettingsScene entry in EditorBuildSettings.scenes)
            if (entry.path == SCENE_PATH && entry.enabled) { inBuildSettings = true; break; }

        if (!inBuildSettings) { Debug.LogError("[SWARM] Cena não está no Build Settings"); problems++; }

        RunLogger runLogger = Object.FindFirstObjectByType<RunLogger>();
        if (runLogger == null) { Debug.LogError("[SWARM] RunLogger ausente"); problems++; }
        else problems += CheckRefs(runLogger, "xp", "upgrades");

        DebugOverlay overlay = Object.FindFirstObjectByType<DebugOverlay>();
        if (overlay == null) { Debug.LogError("[SWARM] DebugOverlay ausente"); problems++; }
        else problems += CheckRefs(overlay, "stats", "health");

        BossController boss = Object.FindFirstObjectByType<BossController>(FindObjectsInactive.Include);
        if (boss == null)
        {
            Debug.LogError("[SWARM] BossController ausente");
            problems++;
        }
        else
        {
            problems += CheckRefs(boss, "projectilePrefab");
        }

        EnemyProjectile[] enemyProjectiles = Object.FindObjectsByType<EnemyProjectile>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (enemyProjectiles.Length != 1)
        {
            Debug.LogError("[SWARM] Esperado 1 template de projétil inimigo, achei " + enemyProjectiles.Length);
            problems++;
        }

        foreach (EnemyProjectile projectile in enemyProjectiles)
        {
            if (projectile.GetComponent<Rigidbody2D>() == null || projectile.GetComponent<Collider2D>() == null)
            {
                Debug.LogError("[SWARM] Projétil inimigo sem Rigidbody2D/Collider2D");
                problems++;
            }
            if (projectile.gameObject.activeSelf)
            {
                Debug.LogError("[SWARM] Template de projétil inimigo deveria estar inativo");
                problems++;
            }
        }

        Debug.Log("[SWARM] Validação concluída: " + problems + " problema(s).");
        return problems;
    }

    static int CheckRefs(Component component, params string[] fields)
    {
        int problems = 0;
        var serialized = new SerializedObject(component);

        foreach (string field in fields)
        {
            SerializedProperty property = serialized.FindProperty(field);

            if (property == null)
            {
                Debug.LogError("[SWARM] " + component.GetType().Name + "." + field + " não existe");
                problems++;
            }
            else if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null)
            {
                Debug.LogError("[SWARM] " + component.GetType().Name + "." + field + " está NULL");
                problems++;
            }
        }

        return problems;
    }

    static int CheckArray(Component component, string field, int expected)
    {
        var serialized = new SerializedObject(component);
        SerializedProperty property = serialized.FindProperty(field);

        if (property == null)
        {
            Debug.LogError("[SWARM] Campo " + field + " não existe em " + component.GetType().Name);
            return 1;
        }

        if (property.arraySize != expected)
        {
            Debug.LogError("[SWARM] " + component.GetType().Name + "." + field +
                           " tem " + property.arraySize + " itens (esperado " + expected + ")");
            return 1;
        }

        for (int i = 0; i < property.arraySize; i++)
        {
            if (property.GetArrayElementAtIndex(i).objectReferenceValue == null)
            {
                Debug.LogError("[SWARM] " + component.GetType().Name + "." + field + "[" + i + "] está NULL");
                return 1;
            }
        }

        return 0;
    }

    static int CheckComponents<T>(GameObject go, string label) where T : Component
    {
        if (go.GetComponent<T>() != null) return 0;
        Debug.LogError("[SWARM] " + go.name + " sem " + label);
        return 1;
    }

    // ─────────────────────────────────────────────────────────────
    // DADOS
    // ─────────────────────────────────────────────────────────────

    static List<string> EnsureEnemyData()
    {
        var list = new List<string>();

        list.Add(EnsureAsset<EnemyData>(ROOT + "/Data/Enemies/Drone.asset", data =>
        {
            data.displayName = "Drone";
            data.maxHealth = 20f;
            data.moveSpeed = 2f;
            data.contactDamage = 6f;
            data.xp = 1;
            data.minTime = 0f;
            data.weight = 60f;
            data.sprite = _hex;
            data.color = new Color(1f, 0.30f, 0.45f);
            data.scale = 0.7f;
            data.maxAliveOfType = 0;
        }));

        list.Add(EnsureAsset<EnemyData>(ROOT + "/Data/Enemies/Corredor.asset", data =>
        {
            data.displayName = "Corredor";
            data.maxHealth = 12f;
            data.moveSpeed = 3.6f;
            data.contactDamage = 4f;
            data.xp = 1;
            data.minTime = 45f;
            data.weight = 30f;
            data.sprite = _diamond;
            data.color = new Color(1f, 0.55f, 0.20f);
            data.scale = 0.55f;
            data.maxAliveOfType = 0;
        }));

        list.Add(EnsureAsset<EnemyData>(ROOT + "/Data/Enemies/Tanque.asset", data =>
        {
            data.displayName = "Tanque";
            data.maxHealth = 90f;
            data.moveSpeed = 1.3f;
            data.contactDamage = 14f;
            data.xp = 3;
            data.minTime = 120f;
            data.weight = 20f;
            data.sprite = _square;
            data.color = new Color(0.75f, 0.20f, 1f);
            data.scale = 1.4f;
            data.maxAliveOfType = 0;
        }));

        list.Add(EnsureAsset<EnemyData>(ROOT + "/Data/Enemies/Divisor.asset", data =>
        {
            data.displayName = "Divisor";
            data.maxHealth = 45f;
            data.moveSpeed = 1.6f;
            data.contactDamage = 8f;
            data.xp = 2;
            data.minTime = 75f;
            data.weight = 15f;
            data.sprite = _ring;
            data.color = new Color(0.20f, 1f, 0.70f);
            data.scale = 1.0f;
            data.maxAliveOfType = 0;
        }));

        list.Add(EnsureAsset<EnemyData>(ROOT + "/Data/Enemies/Atirador.asset", data =>
        {
            data.displayName = "Atirador";
            data.maxHealth = 35f;
            data.moveSpeed = 1.4f;
            data.contactDamage = 6f;
            data.xp = 3;
            data.minTime = 150f;
            data.weight = 12f;
            data.sprite = _circle;
            data.color = new Color(0.35f, 0.75f, 1f);
            data.scale = 0.8f;
            data.maxAliveOfType = 0;
        }));

        list.Add(EnsureAsset<EnemyData>(ROOT + "/Data/Enemies/Nucleo.asset", data =>
        {
            data.displayName = "Núcleo";
            data.maxHealth = 1200f;
            data.moveSpeed = 1f;
            data.contactDamage = 15f;
            data.xp = 40;
            data.minTime = 300f;
            data.weight = 4f;
            data.sprite = _octagon;
            data.color = new Color(1f, 0.15f, 0.15f);
            data.scale = 2.2f;
            data.maxAliveOfType = 1;
        }));

        return list;
    }

    static List<string> EnsureCharacterData()
    {
        var list = new List<string>();

        list.Add(EnsureAsset<CharacterData>(ROOT + "/Data/Characters/Tanque.asset", character =>
        {
            character.displayName = "TANQUE";
            character.description = "+50% vida  ·  -15% velocidade";
            character.color = new Color(1f, 0.55f, 0.20f);
            character.maxHealthMultiplier = 1.5f;
            character.moveSpeedMultiplier = 0.85f;
            character.damageMultiplier = 1f;
            character.fireRateMultiplier = 1f;
        }));

        list.Add(EnsureAsset<CharacterData>(ROOT + "/Data/Characters/Veloz.asset", character =>
        {
            character.displayName = "VELOZ";
            character.description = "-20% vida  ·  +25% velocidade  ·  +10% cadência";
            character.color = new Color(0.40f, 1f, 0.45f);
            character.maxHealthMultiplier = 0.8f;
            character.moveSpeedMultiplier = 1.25f;
            character.damageMultiplier = 1f;
            character.fireRateMultiplier = 1.1f;
        }));

        list.Add(EnsureAsset<CharacterData>(ROOT + "/Data/Characters/Atirador.asset", character =>
        {
            character.displayName = "ATIRADOR";
            character.description = "-10% vida  ·  +25% dano  ·  -10% cadência";
            character.color = new Color(1f, 0.90f, 0.35f);
            character.maxHealthMultiplier = 0.9f;
            character.moveSpeedMultiplier = 1f;
            character.damageMultiplier = 1.25f;
            character.fireRateMultiplier = 0.9f;
        }));

        return list;
    }

    static void LinkPrerequisites()
    {
        Link("OrbeDenso", "OrbeOrbital");
        Link("Execucao", "MiraFria");
        ConfigureExtras();
    }

    /// <summary>Diferencia os pares redundantes com um segundo efeito.</summary>
    static void ConfigureExtras()
    {
        AddExtra("NucleoInstavel", StatType.CritChance, 5f, true, "+25% de dano e +5% de crítico");
        AddExtra("CalibrePesado", StatType.Pierce, 1f, false, "+20% velocidade do projétil e +1 perfuração");
        AddExtra("CascaExtra", StatType.Regen, 0.3f, false, "+40 de vida máxima e +0,3 vida/s");
        AddExtra("ImaForte", StatType.MoveSpeed, 5f, true, "+1 de raio de coleta e +5% de velocidade");
        AddExtra("Execucao", StatType.CritChance, 5f, true, "+0,5 multiplicador de crítico e +5% de crítico");
    }

    static void AddExtra(string file, StatType stat, float amount, bool percent, string description)
    {
        var upgrade = AssetDatabase.LoadAssetAtPath<UpgradeData>(ROOT + "/Data/Upgrades/" + file + ".asset");
        if (upgrade == null) return;

        upgrade.description = description;

        bool exists = false;
        for (int i = 0; i < upgrade.extraEffects.Count; i++)
        {
            if (upgrade.extraEffects[i] != null && upgrade.extraEffects[i].stat == stat)
            {
                exists = true;
                break;
            }
        }

        if (!exists)
            upgrade.extraEffects.Add(new UpgradeEffect { stat = stat, amount = amount, percent = percent });

        EditorUtility.SetDirty(upgrade);
    }

    static void Link(string targetFile, string prerequisiteFile)
    {
        var target = AssetDatabase.LoadAssetAtPath<UpgradeData>(ROOT + "/Data/Upgrades/" + targetFile + ".asset");
        var prerequisite = AssetDatabase.LoadAssetAtPath<UpgradeData>(ROOT + "/Data/Upgrades/" + prerequisiteFile + ".asset");

        if (target != null && prerequisite != null && target.prerequisite == null)
        {
            target.prerequisite = prerequisite;
            EditorUtility.SetDirty(target);
        }
    }

    static void AssignEnemySprites(List<EnemyData> enemies)    {
        Sprite[] sprites = { _hex, _diamond, _square, _ring, _circle, _octagon };

        for (int i = 0; i < enemies.Count && i < sprites.Length; i++)
        {
            if (enemies[i] == null || enemies[i].sprite != null) continue;
            enemies[i].sprite = sprites[i];
            EditorUtility.SetDirty(enemies[i]);
        }
    }

    static List<string> EnsureUpgradeData()
    {
        var list = new List<string>
        {
            Upgrade("MunicaoPesada", "Munição Pesada", "+20% de dano", StatType.Damage, 20f, true, 5),
            Upgrade("GatilhoRapido", "Gatilho Rápido", "+15% de cadência", StatType.FireRate, 15f, true, 5),
            Upgrade("TiroDuplo", "Tiro Duplo", "+1 projétil", StatType.Projectiles, 1f, false, 3),
            Upgrade("Perfuracao", "Perfuração", "+1 perfuração", StatType.Pierce, 1f, false, 3),
            Upgrade("PernaLeve", "Perna Leve", "+12% velocidade", StatType.MoveSpeed, 12f, true, 5),
            Upgrade("Blindagem", "Blindagem", "+25 de vida máxima", StatType.MaxHealth, 25f, false, 5),
            Upgrade("Ima", "Ímã", "+40% raio de coleta", StatType.PickupRadius, 40f, true, 3),
            Upgrade("MiraFria", "Mira Fria", "+5% de crítico", StatType.CritChance, 5f, true, 4),
            Upgrade("Alcance", "Alcance", "+15% de alcance", StatType.Range, 15f, true, 3),
            Upgrade("ProjetilVeloz", "Projétil Veloz", "+15% velocidade do projétil", StatType.ProjectileSpeed, 15f, true, 3),
            Upgrade("Reparo", "Reparo", "Cura 30% da vida máxima", StatType.Heal, 30f, true, 5),
            Upgrade("NucleoInstavel", "Núcleo Instável", "+25% de dano (raro e forte)", StatType.Damage, 25f, true, 2),
            Upgrade("OrbeOrbital", "Orbe Orbital", "+1 orbe girando ao seu redor", StatType.Orbitals, 1f, false, 3),
            Upgrade("OrbeDenso", "Orbe Denso", "+25% de dano dos orbes", StatType.OrbDamage, 25f, true, 3),
            Upgrade("Placas", "Placas", "+10% de armadura", StatType.Armor, 10f, true, 3),
            Upgrade("Nanorrobos", "Nanorrobôs", "+0,5 vida por segundo", StatType.Regen, 0.5f, false, 3),
            Upgrade("ImaForte", "Ímã Forte", "+1 de raio de coleta", StatType.PickupRadius, 1f, false, 3),
            Upgrade("CalibrePesado", "Calibre Pesado", "+20% velocidade do projétil", StatType.ProjectileSpeed, 20f, true, 3),
            Upgrade("Execucao", "Execução", "+0,5 multiplicador de crítico", StatType.CritDamage, 0.5f, false, 2),
            Upgrade("CascaExtra", "Casca Extra", "+40 de vida máxima", StatType.MaxHealth, 40f, false, 5)
        };

        return list;
    }

    static string Upgrade(string file, string title, string description, StatType stat, float amount, bool percent, int maxStacks)
    {
        return EnsureAsset<UpgradeData>(ROOT + "/Data/Upgrades/" + file + ".asset", upgrade =>
        {
            upgrade.title = title;
            upgrade.description = description;
            upgrade.stat = stat;
            upgrade.amount = amount;
            upgrade.percent = percent;
            upgrade.maxStacks = maxStacks;
        });
    }

    static List<T> LoadAssets<T>(List<string> paths) where T : UnityEngine.Object
    {
        var list = new List<T>(paths.Count);
        foreach (string path in paths)
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null) Debug.LogError("[SWARM] Asset não carregou: " + path);
            else list.Add(asset);
        }
        return list;
    }

    static string EnsureAsset<T>(string path, System.Action<T> configure) where T : ScriptableObject
    {
        EnsureFolder(Path.GetDirectoryName(path).Replace('\\', '/'));

        if (AssetDatabase.LoadAssetAtPath<T>(path) != null) return path;

        T asset = ScriptableObject.CreateInstance<T>();
        configure?.Invoke(asset);
        AssetDatabase.CreateAsset(asset, path);
        return path;
    }

    // ─────────────────────────────────────────────────────────────
    // UTIL
    // ─────────────────────────────────────────────────────────────

    static void SetRef(UnityEngine.Object target, string fieldName, UnityEngine.Object value)
    {
        var serialized = new SerializedObject(target);
        SerializedProperty property = serialized.FindProperty(fieldName);

        if (property == null)
        {
            Debug.LogWarning("[SWARM] Campo não encontrado: " + fieldName + " em " + target);
            return;
        }

        property.objectReferenceValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    static void SetList(UnityEngine.Object target, string fieldName, List<UnityEngine.Object> values)
    {
        var serialized = new SerializedObject(target);
        SerializedProperty property = serialized.FindProperty(fieldName);

        if (property == null)
        {
            Debug.LogWarning("[SWARM] Lista não encontrada: " + fieldName + " em " + target);
            return;
        }

        property.arraySize = values.Count;
        for (int i = 0; i < values.Count; i++)
            property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];

        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    static List<UnityEngine.Object> ToObjectList<T>(List<T> source) where T : UnityEngine.Object
    {
        var list = new List<UnityEngine.Object>(source.Count);
        for (int i = 0; i < source.Count; i++) list.Add(source[i]);
        return list;
    }

    static void EnsureFolders()
    {
        EnsureFolder(ROOT);
        EnsureFolder(ROOT + "/Art");
        EnsureFolder(ROOT + "/Data");
        EnsureFolder(ROOT + "/Data/Enemies");
        EnsureFolder(ROOT + "/Data/Upgrades");
        EnsureFolder(ROOT + "/Scenes");
        EnsureFolder(ROOT + "/Settings");
    }

    static void EnsureFolder(string path)
    {
        if (string.IsNullOrEmpty(path) || AssetDatabase.IsValidFolder(path)) return;

        string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
        string name = Path.GetFileName(path);

        if (!string.IsNullOrEmpty(parent)) EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, name);
    }

    static void ApplySpriteMaterial(SpriteRenderer renderer)
    {
        if (renderer != null && _spriteMaterial != null)
            renderer.sharedMaterial = _spriteMaterial;
    }

    /// <summary>
    /// Material unlit para os sprites. Sem isso, no URP 2D os sprites usam o
    /// material "Lit" e ficam pretos quando a cena não tem Global Light 2D.
    /// </summary>
    static Material EnsureSpriteMaterial()
    {
        string path = ROOT + "/Art/SpriteUnlit.mat";
        var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null) return existing;

        Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        if (shader == null) return null;

        var material = new Material(shader) { name = "SpriteUnlit" };
        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    static Sprite EnsureWhiteSprite()
    {
        EnsureFolder(ROOT + "/Art");

        string assetPath = ROOT + "/Art/white.png";
        string absolutePath = Path.Combine(Application.dataPath, "Swarm/Art/white.png");

        if (!File.Exists(absolutePath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));

            var texture = new Texture2D(4, 4, TextureFormat.RGBA32, false);
            var pixels = new Color32[16];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = new Color32(255, 255, 255, 255);
            texture.SetPixels32(pixels);
            texture.Apply();

            File.WriteAllBytes(absolutePath, texture.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        }

        var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 4f;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
    }
}
#endif
