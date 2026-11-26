using UnityEngine;

/// <summary>
/// Paleta única da UI e dos VFX. Qualquer cor nova deve sair daqui para manter consistência.
/// </summary>
public static class UIPalette
{
    public static readonly Color Accent = new Color(0.30f, 1.00f, 0.88f);
    public static readonly Color AccentSoft = new Color(0.65f, 1.00f, 0.95f);
    public static readonly Color Text = new Color(0.92f, 0.97f, 1.00f);
    public static readonly Color Muted = new Color(0.56f, 0.64f, 0.72f);
    public static readonly Color Danger = new Color(1.00f, 0.24f, 0.30f);
    public static readonly Color Warning = new Color(1.00f, 0.72f, 0.30f);
    public static readonly Color Exp = new Color(1.00f, 0.84f, 0.28f);
    public static readonly Color Panel = new Color(0.02f, 0.03f, 0.06f, 1f);
    public static readonly Color PanelSolid = new Color(0.02f, 0.03f, 0.06f, 1f);
    public static readonly Color Void = new Color(0.0196f, 0.0274f, 0.0588f);

    /// <summary>Cor de categoria do card de upgrade, derivada do efeito principal.</summary>
    public static Color ForStat(StatType stat)
    {
        switch (stat)
        {
            case StatType.Damage:
            case StatType.FireRate:
            case StatType.Projectiles:
            case StatType.Pierce:
            case StatType.CritChance:
            case StatType.CritDamage:
            case StatType.ProjectileSpeed:
                return Accent;

            case StatType.MaxHealth:
            case StatType.Armor:
            case StatType.Regen:
            case StatType.Heal:
                return Exp;

            default:
                return Warning;
        }
    }
}
