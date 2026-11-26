using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HUD, tela de level up e tela de fim.
/// O level up congela o jogo (timeScale = 0) para dar peso à escolha.
/// </summary>
public class HudController : MonoBehaviour
{
    [Header("Textos")]
    [SerializeField] Text timerText;
    [SerializeField] Text killsText;
    [SerializeField] Text levelText;
    [SerializeField] Text hpText;
    [SerializeField] Text hudHint;

    [Header("Barras")]
    [SerializeField] Image hpFill;
    [SerializeField] Image xpFill;
    [SerializeField] Image damageFlash;

    [Header("Level Up")]
    [SerializeField] GameObject levelUpPanel;
    [SerializeField] Text levelUpTitle;
    [SerializeField] Text levelUpOptions;
    [SerializeField] UpgradeCardUI[] levelUpCards;

    [Header("Game Over")]
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] Text gameOverText;

    [Header("Boss")]
    [SerializeField] GameObject bossBar;
    [SerializeField] Image bossFill;
    [SerializeField] Text bossLabel;
    [SerializeField] Text bossToast;

    [Header("Referências")]
    [SerializeField] XpSystem xp;
    [SerializeField] Health playerHealth;
    [SerializeField] UpgradeSystem upgrades;
    [SerializeField] PlayerStats stats;

    UpgradeData[] _options;
    bool _bossWasVisible;
    float _bossToastTimer;
    float _hudClock;
    int _shownSeconds = -1;
    int _shownKills = -1;
    int _shownLevel = -1;
    float _flashAlpha;
    float _lastHealth = -1f;

    public bool LevelUpOpen { get; private set; }

    void Start()
    {
        if (levelUpPanel != null) levelUpPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (bossToast != null) bossToast.gameObject.SetActive(false);

        if (xp != null) xp.OnChanged += RefreshXp;
        if (playerHealth != null) playerHealth.Changed += OnHealthChanged;
        if (GameManager.Instance != null) GameManager.Instance.OnRunEnded += ShowGameOver;

        RefreshHp();
        RefreshXp();
        if (playerHealth != null) _lastHealth = playerHealth.Current;
    }

    void OnDestroy()
    {
        if (xp != null) xp.OnChanged -= RefreshXp;
        if (playerHealth != null) playerHealth.Changed -= OnHealthChanged;
        if (GameManager.Instance != null) GameManager.Instance.OnRunEnded -= ShowGameOver;
    }

    void Update()
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager == null) return;

        // HUD a 10 Hz e só quando o valor muda: evita TextGenerator/UGUI rebuild
        // e alocação de string a cada frame (era a maior fonte de GC).
        _hudClock -= Time.unscaledDeltaTime;
        if (_hudClock <= 0f)
        {
            _hudClock = 0.1f;

            int seconds = Mathf.FloorToInt(gameManager.Elapsed);
            if (timerText != null && seconds != _shownSeconds)
            {
                _shownSeconds = seconds;
                timerText.text = FormatTime(seconds);
            }

            if (killsText != null && gameManager.Kills != _shownKills)
            {
                _shownKills = gameManager.Kills;
                killsText.text = "KILLS " + gameManager.Kills;
            }
        }

        if (hudHint != null && hudHint.enabled && gameManager.Elapsed > 10f)
            hudHint.enabled = false;

        UpdateBossBar();

        if (bossToast != null && bossToast.gameObject.activeSelf)
        {
            _bossToastTimer -= Time.unscaledDeltaTime;
            if (_bossToastTimer <= 0f) bossToast.gameObject.SetActive(false);
        }

        if (damageFlash != null && _flashAlpha > 0f)
        {
            _flashAlpha = Mathf.Max(0f, _flashAlpha - Time.unscaledDeltaTime * 2f);
            damageFlash.color = new Color(UIPalette.Danger.r, UIPalette.Danger.g, UIPalette.Danger.b, _flashAlpha);
        }

        if (LevelUpOpen)
        {
            if (InputReader.WasPressed(GameKey.Digit1)) Choose(0);
            else if (InputReader.WasPressed(GameKey.Digit2)) Choose(1);
            else if (InputReader.WasPressed(GameKey.Digit3)) Choose(2);
            return;
        }

        if (!gameManager.IsRunning && gameOverPanel != null && gameOverPanel.activeSelf)
        {
            if (InputReader.WasPressed(GameKey.Restart)) gameManager.Restart();
            else if (InputReader.WasPressed(GameKey.MainMenuKey)) gameManager.Restart(false);
        }
    }

    void OnHealthChanged(float current, float max)
    {
        if (_lastHealth >= 0f && current < _lastHealth - 0.01f)
            _flashAlpha = 0.22f;

        _lastHealth = current;
        RefreshHp();
    }

    void UpdateBossBar()
    {
        BossController boss = BossController.Active;
        bool visible = boss != null;

        if (_bossWasVisible && !visible && BossController.LastFightWon)
            ShowBossToast();

        _bossWasVisible = visible;

        if (bossBar != null && bossBar.activeSelf != visible)
            bossBar.SetActive(visible);

        if (bossLabel != null && bossLabel.gameObject.activeSelf != visible)
            bossLabel.gameObject.SetActive(visible);

        if (visible && bossFill != null)
            bossFill.fillAmount = boss.Health01;
    }

    void ShowBossToast()
    {
        if (bossToast == null) return;

        bossToast.text = "NÚCLEO DESTRUÍDO  ·  " + BossController.LastFightDuration.ToString("0.0") + " s";
        bossToast.gameObject.SetActive(true);
        _bossToastTimer = 4f;

        if (Sfx.Instance != null) Sfx.Instance.PlayBossDown();
    }

    void RefreshHp()
    {
        if (playerHealth == null) return;

        if (hpFill != null)
            hpFill.fillAmount = playerHealth.Max > 0f ? playerHealth.Current / playerHealth.Max : 0f;

        if (hpText != null)
            hpText.text = Mathf.CeilToInt(playerHealth.Current) + " / " + Mathf.CeilToInt(playerHealth.Max);
    }

    void RefreshXp()
    {
        if (xp == null) return;

        if (xpFill != null)
            xpFill.fillAmount = xp.XpToNext > 0 ? (float)xp.Xp / xp.XpToNext : 0f;

        if (levelText != null && xp.Level != _shownLevel)
        {
            _shownLevel = xp.Level;
            levelText.text = "NÍVEL " + xp.Level;
        }
    }

    public void ShowLevelUp(UpgradeData[] options)
    {
        _options = options;
        LevelUpOpen = true;

        if (Sfx.Instance != null) Sfx.Instance.PlayLevelUp();

        if (levelUpPanel != null) levelUpPanel.SetActive(true);
        if (levelUpTitle != null)
            levelUpTitle.text = "NÍVEL " + (xp != null ? xp.Level : 1) + " — ESCOLHA UM UPGRADE";

        if (levelUpCards != null && levelUpCards.Length > 0)
        {
            if (levelUpOptions != null) levelUpOptions.text = "";

            for (int i = 0; i < levelUpCards.Length; i++)
            {
                if (levelUpCards[i] == null) continue;

                if (options != null && i < options.Length && options[i] != null)
                    levelUpCards[i].Bind(options[i], i, UIPalette.ForStat(options[i].stat));
                else
                    levelUpCards[i].Clear();
            }

            if ((options == null || options.Length == 0) && levelUpOptions != null)
                levelUpOptions.text = "Sem upgrades disponíveis.  [1] continuar";
        }
        else if (levelUpOptions != null)
        {
            if (options == null || options.Length == 0)
            {
                levelUpOptions.text = "Sem upgrades disponíveis.\n\n[1] continuar";
            }
            else
            {
                var builder = new StringBuilder();
                for (int i = 0; i < options.Length; i++)
                {
                    builder.Append('[').Append(i + 1).Append("]  ").Append(options[i].title).Append('\n');
                    builder.Append("      ").Append(options[i].description).Append("\n\n");
                }
                levelUpOptions.text = builder.ToString().TrimEnd();
            }
        }

        Time.timeScale = 0f;
    }

    void Choose(int index)
    {
        if (_options != null && _options.Length > 0)
        {
            // Tecla de uma opção que não existe não deve consumir o level up.
            if (index >= _options.Length) return;
            if (upgrades != null) upgrades.Apply(_options[index], stats, playerHealth);
        }

        _options = null;
        LevelUpOpen = false;

        if (levelUpPanel != null) levelUpPanel.SetActive(false);

        if (xp != null) xp.ConsumePendingLevelUp();
        else Time.timeScale = 1f;
    }

    void ShowGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (gameOverText == null) return;

        GameManager gameManager = GameManager.Instance;
        var builder = new StringBuilder();

        builder.Append(gameManager != null && gameManager.LastRunVictory ? "VITÓRIA\n\n" : "FIM DA RUN\n\n");
        if (gameManager != null)
        {
            builder.Append("Tempo    ").Append(FormatTime(gameManager.Elapsed)).Append('\n');
            builder.Append("Kills    ").Append(gameManager.Kills).Append('\n');
        }
        if (xp != null) builder.Append("Nível    ").Append(xp.Level).Append('\n');

        if (BossController.LastFightWon)
            builder.Append("Boss     ").Append(BossController.LastFightDuration.ToString("0.0")).Append(" s\n");

        builder.Append("\nNúcleos  +").Append(SaveSystem.LastCoresEarned).Append('\n');
        builder.Append("Total    ").Append(SaveSystem.Cores).Append('\n');
        builder.Append("\n[R] JOGAR DE NOVO\n[M] MENU PRINCIPAL");

        gameOverText.text = builder.ToString();
    }

    public static string FormatTime(float seconds)
    {
        return FormatTime(Mathf.Max(0, Mathf.FloorToInt(seconds)));
    }

    public static string FormatTime(int totalSeconds)
    {
        int total = Mathf.Max(0, totalSeconds);
        return (total / 60).ToString("00") + ":" + (total % 60).ToString("00");
    }
}
