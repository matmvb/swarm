using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Dono do estado da run: tempo, kills, dificuldade, fim e restart.
/// Único ponto do jogo que decide "a run acabou".
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Dificuldade")]
    [Tooltip("Quanto o HP dos inimigos cresce por minuto. 0.35 = +35% por minuto.")]
    [SerializeField] float hpRampPerMinute = 0.35f;

    public bool IsRunning { get; private set; }
    public float Elapsed { get; private set; }
    public int Kills { get; private set; }

    /// <summary>Multiplicador de HP aplicado quando um inimigo nasce.</summary>
    public float Difficulty => 1f + (Elapsed / 60f) * hpRampPerMinute;

    /// <summary>Disparado uma única vez, quando o jogador morre.</summary>
    public event Action OnRunEnded;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Time.timeScale = 1f;
    }

    void Start()
    {
        Elapsed = 0f;
        Kills = 0;
        IsRunning = true;
    }

    void OnDestroy()
    {
        if (Instance != this) return;

        Instance = null;
        ObjectPool.ClearAll();
    }

    void Update()
    {
        if (IsRunning) Elapsed += Time.deltaTime;
    }

    public void RegisterKill()
    {
        Kills++;
    }

    public void EndRun()
    {
        if (!IsRunning) return;

        IsRunning = false;
        Time.timeScale = 0f;

        int cores = Mathf.FloorToInt(Kills / 10f) + Mathf.FloorToInt(Elapsed / 60f);
        SaveSystem.LastCoresEarned = cores;
        SaveSystem.AddCores(cores);
        SaveSystem.ReportRun(Kills, Elapsed);

        OnRunEnded?.Invoke();
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
