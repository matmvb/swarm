using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Sorteia 3 upgrades e aplica o escolhido (principal + efeitos secundários).
/// </summary>
public class UpgradeSystem : MonoBehaviour
{
    [SerializeField] List<UpgradeData> pool = new List<UpgradeData>();
    [SerializeField] Health playerHealth;

    readonly Dictionary<UpgradeData, int> _taken = new Dictionary<UpgradeData, int>();

    /// <summary>Retorna até <paramref name="count"/> upgrades distintos e disponíveis.</summary>
    public UpgradeData[] Roll(int count)
    {
        var candidates = new List<UpgradeData>(pool.Count);

        for (int i = 0; i < pool.Count; i++)
        {
            UpgradeData upgrade = pool[i];
            if (upgrade == null) continue;

            _taken.TryGetValue(upgrade, out int taken);
            if (taken >= upgrade.maxStacks) continue;

            // Cura com a vida quase cheia é escolha desperdiçada.
            if (upgrade.stat == StatType.Heal && playerHealth != null &&
                playerHealth.Current >= playerHealth.Max * 0.9f)
                continue;

            if (upgrade.prerequisite != null)
            {
                _taken.TryGetValue(upgrade.prerequisite, out int prerequisiteTaken);
                if (prerequisiteTaken <= 0) continue;
            }

            candidates.Add(upgrade);
        }

        int amount = Mathf.Min(count, candidates.Count);
        var result = new UpgradeData[amount];

        for (int i = 0; i < amount; i++)
        {
            int index = Random.Range(0, candidates.Count);
            result[i] = candidates[index];
            candidates.RemoveAt(index);
        }

        // Primeira escolha da run: garante ao menos uma opção ofensiva.
        if (_taken.Count == 0 && result.Length > 0 && !ContainsOffensive(result))
        {
            int offensiveIndex = candidates.FindIndex(IsOffensive);
            if (offensiveIndex >= 0) result[result.Length - 1] = candidates[offensiveIndex];
        }

        return result;
    }

    /// <summary>Resumo legível da build, para o CSV de balanceamento.</summary>
    public string BuildSummary()
    {
        var builder = new StringBuilder();

        foreach (KeyValuePair<UpgradeData, int> pair in _taken)
        {
            if (pair.Key == null || pair.Value <= 0) continue;
            if (builder.Length > 0) builder.Append(", ");
            builder.Append(pair.Key.title).Append(" x").Append(pair.Value);
        }

        return builder.ToString();
    }

    public void Apply(UpgradeData upgrade, PlayerStats stats, Health health)
    {
        if (upgrade == null) return;

        _taken.TryGetValue(upgrade, out int taken);
        _taken[upgrade] = taken + 1;

        ApplyEffect(upgrade.stat, upgrade.amount, upgrade.percent, stats, health);

        if (upgrade.extraEffects == null) return;

        for (int i = 0; i < upgrade.extraEffects.Count; i++)
        {
            UpgradeEffect effect = upgrade.extraEffects[i];
            if (effect != null) ApplyEffect(effect.stat, effect.amount, effect.percent, stats, health);
        }
    }

    static void ApplyEffect(StatType stat, float amount, bool percent, PlayerStats stats, Health health)
    {
        float value = percent ? amount * 0.01f : amount;

        switch (stat)
        {
            case StatType.Damage:
                if (stats != null) stats.damage = percent ? stats.damage * (1f + value) : stats.damage + value;
                break;

            case StatType.FireRate:
                if (stats != null) stats.fireRate = percent ? stats.fireRate * (1f + value) : stats.fireRate + value;
                break;

            case StatType.MoveSpeed:
                if (stats != null) stats.moveSpeed = percent ? stats.moveSpeed * (1f + value) : stats.moveSpeed + value;
                break;

            case StatType.MaxHealth:
                if (health != null) health.AddMaxHealth(percent ? health.Max * value : value);
                break;

            case StatType.Projectiles:
                if (stats != null) stats.projectiles += Mathf.RoundToInt(amount);
                break;

            case StatType.Pierce:
                if (stats != null) stats.pierce += Mathf.RoundToInt(amount);
                break;

            case StatType.Range:
                if (stats != null) stats.range = percent ? stats.range * (1f + value) : stats.range + value;
                break;

            case StatType.PickupRadius:
                if (stats != null) stats.pickupRadius = percent ? stats.pickupRadius * (1f + value) : stats.pickupRadius + value;
                break;

            case StatType.CritChance:
                if (stats != null) stats.critChance = Mathf.Clamp01(stats.critChance + value);
                break;

            case StatType.ProjectileSpeed:
                if (stats != null) stats.projectileSpeed = percent ? stats.projectileSpeed * (1f + value) : stats.projectileSpeed + value;
                break;

            case StatType.Heal:
                if (health != null) health.Heal(health.Max * Mathf.Max(0.01f, value));
                break;

            case StatType.Orbitals:
                if (stats != null) stats.orbitals = Mathf.Clamp(stats.orbitals + Mathf.RoundToInt(amount), 0, 8);
                break;

            case StatType.OrbDamage:
                if (stats != null) stats.orbitalDamage = percent ? stats.orbitalDamage * (1f + value) : stats.orbitalDamage + value;
                break;

            case StatType.Armor:
                if (stats != null)
                {
                    stats.armor = Mathf.Clamp(stats.armor + (percent ? value : amount), 0f, 0.6f);
                    if (health != null) health.damageTakenMultiplier = 1f - stats.armor;
                }
                break;

            case StatType.Regen:
                if (stats != null) stats.regen += amount;
                break;

            case StatType.CritDamage:
                if (stats != null) stats.critMultiplier += amount;
                break;
        }
    }

    static bool IsOffensive(UpgradeData upgrade)
    {
        return upgrade.stat == StatType.Damage
            || upgrade.stat == StatType.FireRate
            || upgrade.stat == StatType.Projectiles;
    }

    static bool ContainsOffensive(UpgradeData[] upgrades)
    {
        for (int i = 0; i < upgrades.Length; i++)
            if (upgrades[i] != null && IsOffensive(upgrades[i])) return true;

        return false;
    }
}
