using System;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// Grava o resultado de cada run em CSV para balanceamento.
/// Arquivo: %USERPROFILE%\AppData\LocalLow\{company}\{product}\swarm-runs.csv
/// </summary>
public class RunLogger : MonoBehaviour
{
    [SerializeField] XpSystem xp;
    [SerializeField] UpgradeSystem upgrades;

    static string FilePath => Path.Combine(Application.persistentDataPath, "swarm-runs.csv");

    void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnRunEnded += LogRun;
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnRunEnded -= LogRun;
    }

    void LogRun()
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager == null) return;

        try
        {
            bool isNew = !File.Exists(FilePath);

            var builder = new StringBuilder();
            if (isNew)
                builder.AppendLine("timestamp,version,duration_s,kills,level,victory,boss_time,build");

            string build = upgrades != null ? upgrades.BuildSummary() : string.Empty;
            string bossTime = "-1";

            if (BossController.SpawnedThisRun)
            {
                bossTime = BossController.LastFightWon
                    ? BossController.LastFightDuration.ToString("F1", CultureInfo.InvariantCulture)
                    : "0";
            }

            builder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)).Append(',')
                   .Append(Application.version).Append(',')
                   .Append(gameManager.Elapsed.ToString("F1", CultureInfo.InvariantCulture)).Append(',')
                   .Append(gameManager.Kills).Append(',')
                   .Append(xp != null ? xp.Level : 0).Append(',')
                   .Append(gameManager.LastRunVictory ? 1 : 0).Append(',')
                   .Append(bossTime).Append(',')
                   .Append('"').Append(build.Replace("\"", "'")).Append('"')
                   .AppendLine();

            File.AppendAllText(FilePath, builder.ToString(), new UTF8Encoding(false));
            Debug.Log("[SWARM] Run registrada: " + FilePath);
        }
        catch (Exception exception)
        {
            Debug.LogWarning("[SWARM] Falha ao gravar a run: " + exception.Message);
        }
    }
}
