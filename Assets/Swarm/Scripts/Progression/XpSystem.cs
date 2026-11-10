using System;
using UnityEngine;

/// <summary>
/// XP, nível e fila de level ups.
/// A curva é linear de propósito: 12 a 20 level ups por run.
/// </summary>
public class XpSystem : MonoBehaviour
{
    public static XpSystem Instance { get; private set; }

    [Header("Progressão")]
    [SerializeField] int level = 1;
    [SerializeField] int xp;
    [SerializeField] int xpToNext = 5;

    [Tooltip("Multiplicador do XP por nível. 1.3 = curva exponencial suave (evita level 58 em runs longas).")]
    [SerializeField] float xpGrowth = 1.3f;

    [Header("Referências")]
    [SerializeField] XpOrb orbPrefab;
    [SerializeField] UpgradeSystem upgrades;
    [SerializeField] HudController hud;

    public int Level => level;
    public int Xp => xp;
    public int XpToNext => xpToNext;

    public event Action OnChanged;

    int _pendingLevelUps;

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void SpawnOrb(int value, Vector3 position)
    {
        if (orbPrefab == null) return;

        GameObject instance = ObjectPool.Spawn(orbPrefab.gameObject, position, Quaternion.identity);
        if (instance == null) return;

        var orb = instance.GetComponent<XpOrb>();
        if (orb != null) orb.Setup(value);
    }

    public void AddXp(int amount)
    {
        if (amount <= 0) return;
        if (GameManager.Instance != null && !GameManager.Instance.IsRunning) return;

        xp += amount;

        int guard = 0;
        while (xp >= xpToNext && guard++ < 100)
        {
            xp -= xpToNext;
            level++;
            float growth = xpGrowth > 1f ? xpGrowth : 1.3f;
            xpToNext = Mathf.Max(xpToNext + 1, Mathf.RoundToInt(xpToNext * growth));
            _pendingLevelUps++;
        }

        OnChanged?.Invoke();

        if (_pendingLevelUps > 0 && hud != null && !hud.LevelUpOpen)
            hud.ShowLevelUp(upgrades != null ? upgrades.Roll(3) : null);
    }

    /// <summary>Chamado pela HUD depois que o jogador escolhe um upgrade.</summary>
    public void ConsumePendingLevelUp()
    {
        if (_pendingLevelUps > 0) _pendingLevelUps--;

        if (_pendingLevelUps > 0 && hud != null)
        {
            hud.ShowLevelUp(upgrades != null ? upgrades.Roll(3) : null);
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
