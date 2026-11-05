using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Sorteia 3 upgrades e aplica o escolhido nos stats do jogador.
/// </summary>
public class UpgradeSystem : MonoBehaviour
{
    [SerializeField] List<UpgradeData> pool = new List<UpgradeData>();

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

        float value = upgrade.percent ? upgrade.amount * 0.01f : upgrade.amount;

        switch (upgrade.stat)
        {
            case StatType.Damage:
                if (stats != null) stats.damage = upgrade.percent ? stats.damage * (1f + value) : stats.damage + value;
                break;

            case StatType.FireRate:
                if (stats != null) stats.fireRate = upgrade.percent ? stats.fireRate * (1f + value) : stats.fireRate + value;
                break;

            case StatType.MoveSpeed:
                if (stats != null) stats.moveSpeed = upgrade.percent ? stats.moveSpeed * (1f + value) : stats.moveSpeed + value;
                break;

            case StatType.MaxHealth:
                if (health != null) health.AddMaxHealth(upgrade.percent ? health.Max * value : value);
                break;

            case StatType.Projectiles:
                if (stats != null) stats.projectiles += Mathf.RoundToInt(upgrade.amount);
                break;

            case StatType.Pierce:
                if (stats != null) stats.pierce += Mathf.RoundToInt(upgrade.amount);
                break;

            case StatType.Range:
                if (stats != null) stats.range = upgrade.percent ? stats.range * (1f + value) : stats.range + value;
                break;

            case StatType.PickupRadius:
                if (stats != null) stats.pickupRadius = upgrade.percent ? stats.pickupRadius * (1f + value) : stats.pickupRadius + value;
                break;

            case StatType.CritChance:
                if (stats != null) stats.critChance = Mathf.Clamp01(stats.critChance + value);
                break;

            case StatType.ProjectileSpeed:
                if (stats != null) stats.projectileSpeed = upgrade.percent ? stats.projectileSpeed * (1f + value) : stats.projectileSpeed + value;
                break;

            case StatType.Heal:
                if (health != null) health.Heal(health.Max * Mathf.Max(0.01f, value));
                break;
        }
    }
}
