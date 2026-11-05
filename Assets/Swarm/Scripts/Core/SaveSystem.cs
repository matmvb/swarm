using UnityEngine;

/// <summary>
/// Save mínimo em PlayerPrefs. Guarda Núcleos e recordes entre sessões.
/// </summary>
public static class SaveSystem
{
    const string CoresKey = "swarm.cores";
    const string BestKillsKey = "swarm.bestKills";
    const string BestTimeKey = "swarm.bestTime";

    /// <summary>Quanto a última run rendeu (usado na tela de fim).</summary>
    public static int LastCoresEarned { get; set; }

    public static int Cores
    {
        get => PlayerPrefs.GetInt(CoresKey, 0);
        set
        {
            PlayerPrefs.SetInt(CoresKey, Mathf.Max(0, value));
            PlayerPrefs.Save();
        }
    }

    public static int BestKills => PlayerPrefs.GetInt(BestKillsKey, 0);
    public static float BestTime => PlayerPrefs.GetFloat(BestTimeKey, 0f);

    public static void AddCores(int amount)
    {
        if (amount <= 0) return;
        Cores += amount;
    }

    public static void ReportRun(int kills, float elapsed)
    {
        if (kills > BestKills) PlayerPrefs.SetInt(BestKillsKey, kills);
        if (elapsed > BestTime) PlayerPrefs.SetFloat(BestTimeKey, elapsed);
        PlayerPrefs.Save();
    }

    public static void ResetAll()
    {
        PlayerPrefs.DeleteKey(CoresKey);
        PlayerPrefs.DeleteKey(BestKillsKey);
        PlayerPrefs.DeleteKey(BestTimeKey);
        PlayerPrefs.Save();
    }
}
