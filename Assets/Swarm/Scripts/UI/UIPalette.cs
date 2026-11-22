using UnityEngine;

/// <summary>
/// Paleta única da UI. Qualquer cor nova deve sair daqui para manter consistência.
/// </summary>
public static class UIPalette
{
    public static readonly Color Accent = new Color(0.30f, 1.00f, 0.88f);
    public static readonly Color AccentSoft = new Color(0.65f, 1.00f, 0.95f);
    public static readonly Color Text = new Color(0.92f, 0.97f, 1.00f);
    public static readonly Color Muted = new Color(0.56f, 0.64f, 0.72f);
    public static readonly Color Danger = new Color(1.00f, 0.30f, 0.35f);
    public static readonly Color Warning = new Color(1.00f, 0.75f, 0.30f);
    public static readonly Color Exp = new Color(0.40f, 1.00f, 0.50f);
    public static readonly Color Panel = new Color(0.02f, 0.03f, 0.06f, 0.94f);
    public static readonly Color PanelSolid = new Color(0.02f, 0.03f, 0.06f, 0.98f);
}
