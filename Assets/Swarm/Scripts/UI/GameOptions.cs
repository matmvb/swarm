using System.Text;
using UnityEngine;

/// <summary>
/// Opções persistidas: volume master e tela cheia. O reset limpa todo o save
/// (Núcleos, meta e opções).
/// </summary>
public static class GameOptions
{
    const string VolumeKey = "opt.volume";
    const string FullscreenKey = "opt.fullscreen";

    public static float Volume
    {
        get => PlayerPrefs.GetFloat(VolumeKey, 0.8f);
        set
        {
            PlayerPrefs.SetFloat(VolumeKey, Mathf.Clamp01(value));
            PlayerPrefs.Save();
            Apply();
        }
    }

    public static bool Fullscreen
    {
        get => PlayerPrefs.GetInt(FullscreenKey, 1) == 1;
        set
        {
            PlayerPrefs.SetInt(FullscreenKey, value ? 1 : 0);
            PlayerPrefs.Save();
            Apply();
        }
    }

    public static void Apply()
    {
        AudioListener.volume = Volume;
        Screen.fullScreen = Fullscreen;
    }

    public static void ToggleFullscreen()
    {
        Fullscreen = !Fullscreen;
    }

    public static void ResetSave()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Apply();
    }

    public static string Describe(bool confirmReset)
    {
        var builder = new StringBuilder();

        builder.AppendLine("OPÇÕES");
        builder.AppendLine();
        builder.Append("Volume      ").Append(Mathf.RoundToInt(Volume * 100f)).Append("%   [- / +]").AppendLine();
        builder.Append("Tela cheia  ").Append(Fullscreen ? "SIM" : "NÃO").Append("   [F]").AppendLine();
        builder.AppendLine();
        builder.AppendLine(confirmReset ? "Resetar save   [R]  ← CONFIRME" : "Resetar save   [R]");
        builder.AppendLine();
        builder.Append("[O] voltar");

        return builder.ToString();
    }
}
