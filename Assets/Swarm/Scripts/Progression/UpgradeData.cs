using UnityEngine;

public enum StatType
{
    Damage,
    FireRate,
    MoveSpeed,
    MaxHealth,
    Projectiles,
    Pierce,
    Range,
    PickupRadius,
    CritChance,
    ProjectileSpeed,
    Heal,
    Orbitals,
    OrbDamage,
    Armor,
    Regen,
    CritDamage
}

/// <summary>
/// Um upgrade oferecido no level up. Crie novos no menu
/// Assets > Create > SWARM > Upgrade.
/// </summary>
[CreateAssetMenu(menuName = "SWARM/Upgrade", fileName = "NovoUpgrade")]
public class UpgradeData : ScriptableObject
{
    public string title = "Upgrade";

    [TextArea(1, 3)]
    public string description = "+10% de algo";

    public StatType stat = StatType.Damage;

    [Tooltip("Se percent = true, 20 significa +20%. Se false, 20 é valor absoluto.")]
    public float amount = 10f;

    public bool percent = true;

    [Min(1)]
    public int maxStacks = 5;
}
