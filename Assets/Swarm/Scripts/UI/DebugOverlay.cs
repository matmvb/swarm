using System.Text;
using UnityEngine;

/// <summary>
/// Overlay de desenvolvimento (F1). Mostra FPS, tempo, kills, inimigos vivos,
/// dificuldade e os números atuais do jogador — para calibrar sem adivinhar.
/// </summary>
public class DebugOverlay : MonoBehaviour
{
    [SerializeField] bool visible;
    [SerializeField] PlayerStats stats;
    [SerializeField] Health health;

    float _smoothedDelta = 0.016f;
    int _enemyCount;
    GUIStyle _style;

    void Update()
    {
        if (InputReader.WasPressed(GameKey.ToggleDebug))
            visible = !visible;

        if (!visible) return;

        _smoothedDelta = Mathf.Lerp(_smoothedDelta, Time.unscaledDeltaTime, 0.1f);
        _enemyCount = Enemy.All.Count;
    }

    void OnGUI()
    {
        if (!visible) return;

        if (_style == null)
        {
            _style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                richText = true,
                alignment = TextAnchor.UpperLeft
            };
        }

        float fps = _smoothedDelta > 0f ? 1f / _smoothedDelta : 0f;
        GameManager gameManager = GameManager.Instance;
        XpSystem xp = XpSystem.Instance;

        var text = new StringBuilder();
        text.AppendLine("<b>SWARM DEBUG</b>  (F1)");
        text.AppendLine("FPS " + fps.ToString("0") + "   frame " + (_smoothedDelta * 1000f).ToString("0.0") + " ms");

        if (gameManager != null)
        {
            text.AppendLine("tempo " + HudController.FormatTime(gameManager.Elapsed) +
                            "   restam " + HudController.FormatTime(gameManager.TimeLeft) +
                            "   kills " + gameManager.Kills +
                            "   dif x" + gameManager.Difficulty.ToString("0.00"));
            text.AppendLine("inimigos vivos " + _enemyCount);
        }

        if (xp != null)
            text.AppendLine("nível " + xp.Level + "   xp " + xp.Xp + "/" + xp.XpToNext);

        if (health != null)
            text.AppendLine("vida " + health.Current.ToString("0") + "/" + health.Max.ToString("0"));

        if (stats != null)
        {
            text.AppendLine("dano " + stats.damage.ToString("0.#") +
                            "   cadência " + stats.fireRate.ToString("0.##") + "/s" +
                            "   projéteis " + stats.projectiles +
                            "   pierce " + stats.pierce +
                            "   crit " + stats.critChance.ToString("P0"));
        }

        GUI.color = new Color(1f, 1f, 1f, 0.92f);
        GUI.Label(new Rect(16f, 12f, 760f, 240f), text.ToString(), _style);
    }
}
