using UnityEngine;

/// <summary>
/// Cores semânticas do combate. Mantém jogador, inimigos, boss e XP em faixas
/// distintas, reservando branco para núcleos/impactos.
/// </summary>
public static class CombatPalette
{
    public enum EnemyWeight
    {
        Small,
        Medium,
        Special
    }

    public static readonly Color Player = Hex(0x4D, 0xFF, 0xE0);
    public static readonly Color PlayerCore = Hex(0xEB, 0xF7, 0xFF);
    public static readonly Color PlayerCritical = Hex(0xFF, 0x8C, 0x2E);
    public static readonly Color PlayerMuzzle = Hex(0x4D, 0xFF, 0xE0);

    public static readonly Color EnemyProjectile = Hex(0xFF, 0x4D, 0x8D);
    public static readonly Color EnemyProjectileAccent = Hex(0xFF, 0x8C, 0x2E);
    public static readonly Color BossProjectile = Hex(0xFF, 0x26, 0x26);
    public static readonly Color BossProjectileAccent = Hex(0xFF, 0xB8, 0x4D);

    public static readonly Color Xp = Hex(0xFF, 0xD6, 0x47);
    public static readonly Color Impact = Hex(0xEB, 0xF7, 0xFF);
    public static readonly Color Warning = Hex(0xFF, 0xB8, 0x4D);

    public static Color Highlight(Color main)
    {
        return Color.Lerp(main, Impact, 0.22f);
    }

    public static Color EnemyColor(EnemyData data)
    {
        if (data == null) return EnemyProjectile;

        switch (data.displayName)
        {
            case "Drone": return Hex(0xFF, 0x4D, 0x8D);
            case "Corredor": return Hex(0xFF, 0x8C, 0x2E);
            case "Tanque": return Hex(0xBF, 0x33, 0xFF);
            case "Divisor": return Hex(0x33, 0xFF, 0xB3);
            case "Atirador": return Hex(0x59, 0xBF, 0xFF);
            case "Núcleo": return Hex(0xFF, 0x26, 0x26);
            default: return EnemyProjectile;
        }
    }

    public static EnemyWeight WeightOf(EnemyData data)
    {
        if (data == null) return EnemyWeight.Small;

        switch (data.displayName)
        {
            case "Tanque":
            case "Divisor":
            case "Atirador":
                return EnemyWeight.Medium;
            case "Núcleo":
                return EnemyWeight.Special;
            default:
                return EnemyWeight.Small;
        }
    }

    public static float ExplosionScale(EnemyWeight weight)
    {
        switch (weight)
        {
            case EnemyWeight.Medium: return 0.9f;
            case EnemyWeight.Special: return 2.4f;
            default: return 0.5f;
        }
    }

    static Color Hex(int r, int g, int b)
    {
        return new Color32((byte)r, (byte)g, (byte)b, 255);
    }
}
