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

        int index = -1;
        if (InputReader.WasPressed(GameKey.Digit1)) index = 0;
        else if (InputReader.WasPressed(GameKey.Digit2)) index = 1;
        else if (InputReader.WasPressed(GameKey.Digit3)) index = 2;

        if (index < 0 || index >= characters.Count) return;
        Choose(index);
    }

    string BuildOptions()
    {
        var builder = new StringBuilder();

        for (int i = 0; i < characters.Count; i++)
        {
            CharacterData character = characters[i];
            if (character == null) continue;

            builder.Append('[').Append(i + 1).Append("]  ").Append(character.displayName).Append('\n');
            builder.Append("      ").Append(character.description).Append("\n\n");
        }

        return builder.ToString().TrimEnd();
    }

    void Choose(int index)
    {
        CharacterData character = characters[index];
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
