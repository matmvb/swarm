using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tela de escolha do piloto no início da run. Pausa o jogo até o jogador escolher.
/// </summary>
public class CharacterSelect : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] Text title;
    [SerializeField] Text options;
    [SerializeField] List<CharacterData> characters = new List<CharacterData>();
    [SerializeField] PlayerStats stats;
    [SerializeField] Health health;

    bool _choosing;

    void Start()
    {
        if (GameManager.Instance == null) return;
        if (characters == null || characters.Count == 0) return;

        _choosing = true;

        if (panel != null) panel.SetActive(true);
        if (title != null) title.text = "ESCOLHA SEU PILOTO";
        if (options != null) options.text = BuildOptions();

        Time.timeScale = 0f;
    }

    void Update()
    {
        if (!_choosing) return;

        if (TryBuyMeta()) return;

        int index = -1;
        if (InputReader.WasPressed(GameKey.Digit1)) index = 0;
        else if (InputReader.WasPressed(GameKey.Digit2)) index = 1;
        else if (InputReader.WasPressed(GameKey.Digit3)) index = 2;

        if (index < 0 || index >= characters.Count) return;
        Choose(index);
    }

    bool TryBuyMeta()
    {
        for (int i = 0; i < MetaProgression.Upgrades.Length && i < 3; i++)
        {
            GameKey key = i == 0 ? GameKey.Digit4 : i == 1 ? GameKey.Digit5 : GameKey.Digit6;
            if (!InputReader.WasPressed(key)) continue;

            if (MetaProgression.Buy(MetaProgression.Upgrades[i]))
            {
                if (options != null) options.text = BuildOptions();
                if (Sfx.Instance != null) Sfx.Instance.PlayPickup();
            }

            return true;
        }

        return false;
    }

    string BuildOptions()
    {
        var builder = new StringBuilder();

        builder.AppendLine("<b>PILOTOS</b>");
        builder.AppendLine();

        for (int i = 0; i < characters.Count; i++)
        {
            CharacterData character = characters[i];
            if (character == null) continue;

            builder.Append('[').Append(i + 1).Append("]  ").Append(character.displayName).Append('\n');
            builder.Append("      ").Append(character.description).Append('\n');
        }

        builder.AppendLine();
        builder.Append("<b>NÚCLEOS: ").Append(SaveSystem.Cores).AppendLine("</b>");

        for (int i = 0; i < MetaProgression.Upgrades.Length; i++)
        {
            MetaUpgrade upgrade = MetaProgression.Upgrades[i];
            int level = MetaProgression.GetLevel(upgrade);

            builder.Append('[').Append(i + 4).Append("] +5% ").Append(upgrade.title);
            builder.Append("  (").Append(level).Append('/').Append(upgrade.maxLevel).Append(')');

            if (level >= upgrade.maxLevel) builder.Append("  MAX");
            else builder.Append("  custo ").Append(MetaProgression.GetCost(upgrade));

            builder.AppendLine();
        }

        return builder.ToString().TrimEnd();
    }

    void Choose(int index)
    {
        CharacterData character = characters[index];

        MetaProgression.ApplyAll(stats, health);
        if (character != null) Apply(character);

        _choosing = false;
        if (panel != null) panel.SetActive(false);
        Time.timeScale = 1f;

        if (character != null) Debug.Log("[SWARM] Piloto escolhido: " + character.displayName);
    }

    void Apply(CharacterData character)
    {
        if (stats != null)
        {
            stats.moveSpeed *= character.moveSpeedMultiplier;
            stats.damage *= character.damageMultiplier;
            stats.fireRate *= character.fireRateMultiplier;

            var sprite = stats.GetComponent<SpriteRenderer>();
            if (sprite != null) sprite.color = character.color;
        }

        if (health != null) health.ScaleMaxHealth(character.maxHealthMultiplier);
    }
}
