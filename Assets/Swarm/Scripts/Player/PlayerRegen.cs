using UnityEngine;

/// <summary>
/// Regeneração passiva de vida (upgrade Nanorrobôs). Cura em ticks de 1 s
/// para não atualizar a HUD a cada frame.
/// </summary>
[RequireComponent(typeof(PlayerStats), typeof(Health))]
public class PlayerRegen : MonoBehaviour
{
    PlayerStats _stats;
    Health _health;
    float _timer;

    void Awake()
    {
        _stats = GetComponent<PlayerStats>();
        _health = GetComponent<Health>();
    }

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsRunning) return;
        if (_stats == null || _health == null || _stats.regen <= 0f) return;

        _timer += Time.deltaTime;
        if (_timer < 1f) return;

        _health.Heal(_stats.regen * _timer);
        _timer = 0f;
    }
}
