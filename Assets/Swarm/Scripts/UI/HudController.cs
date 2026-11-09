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

    [Header("Level Up")]
    [SerializeField] GameObject levelUpPanel;
    [SerializeField] Text levelUpTitle;
    [SerializeField] Text levelUpOptions;

    [Header("Game Over")]
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] Text gameOverText;

    [Header("Referências")]
    [SerializeField] XpSystem xp;
    [SerializeField] Health playerHealth;
    [SerializeField] UpgradeSystem upgrades;
    [SerializeField] PlayerStats stats;

    UpgradeData[] _options;

    public bool LevelUpOpen { get; private set; }

    void Start()
    {
        if (levelUpPanel != null) levelUpPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (xp != null) xp.OnChanged += RefreshXp;
        if (playerHealth != null) playerHealth.Changed += OnHealthChanged;
        if (GameManager.Instance != null) GameManager.Instance.OnRunEnded += ShowGameOver;

        RefreshHp();
        RefreshXp();
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

        if (timerText != null) timerText.text = FormatTime(gameManager.Elapsed);
        if (killsText != null) killsText.text = "KILLS " + gameManager.Kills;

        if (hudHint != null && hudHint.enabled && gameManager.Elapsed > 10f)
            hudHint.enabled = false;

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
        }
    }

    void OnHealthChanged(float current, float max)
    {
        RefreshHp();
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

        if (levelText != null)
            levelText.text = "NÍVEL " + xp.Level;
    }

    public void ShowLevelUp(UpgradeData[] options)
    {
        _options = options;
        LevelUpOpen = true;

        if (Sfx.Instance != null) Sfx.Instance.PlayLevelUp();

        if (levelUpPanel != null) levelUpPanel.SetActive(true);
        if (levelUpTitle != null)
            levelUpTitle.text = "NÍVEL " + (xp != null ? xp.Level : 1) + " — ESCOLHA UM UPGRADE";

        if (levelUpOptions != null)
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
        if (_options != null && index < _options.Length && upgrades != null)
            upgrades.Apply(_options[index], stats, playerHealth);

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

        builder.Append("\nNúcleos  +").Append(SaveSystem.LastCoresEarned).Append('\n');
        builder.Append("Total    ").Append(SaveSystem.Cores).Append('\n');
        builder.Append("\n[R] JOGAR DE NOVO");

        gameOverText.text = builder.ToString();
    }

    public static string FormatTime(float seconds)
    {
        int total = Mathf.Max(0, Mathf.FloorToInt(seconds));
        return (total / 60).ToString("00") + ":" + (total % 60).ToString("00");
    }
}
