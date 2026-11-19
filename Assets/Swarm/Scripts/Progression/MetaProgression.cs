using UnityEngine;

/// <summary>Uma melhoria permanente comprada com Núcleos.</summary>
public class MetaUpgrade
{
    public readonly string id;
    public readonly string title;
    public readonly string description;
    public readonly int baseCost;
    public readonly float costGrowth;
    public readonly int maxLevel;

    public MetaUpgrade(string id, string title, string description, int baseCost, float costGrowth, int maxLevel)
    {
        this.id = id;
        this.title = title;
        this.description = description;
        this.baseCost = baseCost;
        this.costGrowth = costGrowth;
        this.maxLevel = maxLevel;
    }
}

/// <summary>
/// Meta-progressão: gasta Núcleos entre runs em bônus permanentes.
/// Os níveis ficam no PlayerPrefs; o bônus é aplicado no início da run.
/// </summary>
public static class MetaProgression
{
    public const float BonusPerLevel = 0.05f;

    public static readonly MetaUpgrade[] Upgrades =
    {
        new MetaUpgrade("damage", "Dano", "+5% de dano permanente", 50, 1.5f, 20),
        new MetaUpgrade("health", "Casco", "+5% de vida permanente", 50, 1.5f, 20),
        new MetaUpgrade("speed", "Motor", "+5% de velocidade permanente", 50, 1.5f, 20)
    };

    public static int GetLevel(MetaUpgrade upgrade)
    {
        return GetLevel(upgrade.id);
    }

    public static int GetLevel(string id)
    {
        return PlayerPrefs.GetInt("meta." + id, 0);
    }

    public static int GetCost(MetaUpgrade upgrade)
    {
        int level = GetLevel(upgrade);
        return Mathf.RoundToInt(upgrade.baseCost * Mathf.Pow(upgrade.costGrowth, level));
    }

    public static bool CanBuy(MetaUpgrade upgrade)
    {
        return GetLevel(upgrade) < upgrade.maxLevel && SaveSystem.Cores >= GetCost(upgrade);
    }

    public static bool Buy(MetaUpgrade upgrade)
    {
        if (!CanBuy(upgrade)) return false;

        int level = GetLevel(upgrade);
        SaveSystem.Cores -= GetCost(upgrade);
        PlayerPrefs.SetInt("meta." + upgrade.id, level + 1);
        PlayerPrefs.Save();
        return true;
    }

    /// <summary>Aplica os bônus permanentes nos stats base do jogador.</summary>
    public static void ApplyAll(PlayerStats stats, Health health)
    {
        float damage = 1f + BonusPerLevel * PlayerPrefs.GetInt("meta.damage", 0);
        float speed = 1f + BonusPerLevel * PlayerPrefs.GetInt("meta.speed", 0);
        float life = 1f + BonusPerLevel * PlayerPrefs.GetInt("meta.health", 0);

        if (stats != null)
        {
            stats.damage *= damage;
            stats.moveSpeed *= speed;
        }

        if (health != null) health.ScaleMaxHealth(life);
    }
}
