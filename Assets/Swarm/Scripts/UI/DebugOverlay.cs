using System.Text;
using UnityEngine;

/// <summary>
/// Overlay de desenvolvimento (F1). Mostra FPS, tempo, kills, inimigos vivos,
/// dificuldade e os números atuais do jogador — para calibrar sem adivinhar.
/// O texto é reconstruído a 5 Hz e desenhado só no Repaint para não alocar a 60 Hz.
/// </summary>
public class DebugOverlay : MonoBehaviour
{
    [SerializeField] bool visible;
    [SerializeField] PlayerStats stats;
    [SerializeField] Health health;

    readonly StringBuilder _builder = new StringBuilder(512);

    float _smoothedDelta = 0.016f;
    float _textClock;
    int _enemyCount;
    GUIContent _content;
    GUIStyle _style;

    void Update()
    {
        if (InputReader.WasPressed(GameKey.ToggleDebug))
            visible = !visible;

        if (!visible) return;

        _smoothedDelta = Mathf.Lerp(_smoothedDelta, Time.unscaledDeltaTime, 0.1f);
        _enemyCount = Enemy.All.Count;

        _textClock -= Time.unscaledDeltaTime;
        if (_textClock > 0f) return;

        _textClock = 0.2f;

        if (_content == null) _content = new GUIContent();
        _content.text = BuildText();
    }

    string BuildText()
    {
        float fps = _smoothedDelta > 0f ? 1f / _smoothedDelta : 0f;
        GameManager gameManager = GameManager.Instance;
        XpSystem xp = XpSystem.Instance;

        _builder.Clear();
        _builder.AppendLine("<b>SWARM DEBUG</b>  (F1)");
        _builder.Append("FPS ").Append(fps.ToString("0"))
                .Append("   frame ").Append((_smoothedDelta * 1000f).ToString("0.0")).AppendLine(" ms");

        if (gameManager != null)
        {
            _builder.Append("tempo ").Append(HudController.FormatTime(gameManager.Elapsed))
                    .Append("   restam ").Append(HudController.FormatTime(gameManager.TimeLeft))
                    .Append("   kills ").Append(gameManager.Kills)
                    .Append("   dif x").Append(gameManager.Difficulty.ToString("0.00"))
                    .AppendLine();
            _builder.Append("inimigos vivos ").Append(_enemyCount).AppendLine();
        }

        if (xp != null)
            _builder.Append("nível ").Append(xp.Level).Append("   xp ").Append(xp.Xp).Append('/').Append(xp.XpToNext).AppendLine();

        if (health != null)
            _builder.Append("vida ").Append(health.Current.ToString("0")).Append('/').Append(health.Max.ToString("0")).AppendLine();

        if (stats != null)
        {
            _builder.Append("dano ").Append(stats.damage.ToString("0.#"))
                    .Append("   cadência ").Append(stats.fireRate.ToString("0.##")).Append("/s")
                    .Append("   projéteis ").Append(stats.projectiles)
                    .Append("   pierce ").Append(stats.pierce)
                    .Append("   crit ").Append(stats.critChance.ToString("P0"))
                    .AppendLine();
        }

        return _builder.ToString();
    }

    void OnGUI()
    {
        if (!visible) return;
        if (Event.current == null || Event.current.type != EventType.Repaint) return;
        if (_content == null) return;

        if (_style == null)
        {
            _style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                richText = true,
                alignment = TextAnchor.UpperLeft
            };
        }

        GUI.color = new Color(1f, 1f, 1f, 0.92f);
        GUI.Label(new Rect(16f, 12f, 820f, 240f), _content, _style);
    }
}
