using System;
using UnityEngine;

/// <summary>
/// Vida genérica com eventos. Não conhece HUD nem GameManager:
/// apenas avisa quem estiver inscrito.
/// </summary>
public class Health : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;

    /// <summary>1 = dano cheio. A armadura dos upgrades reduz este valor.</summary>
    public float damageTakenMultiplier = 1f;

    public float Current { get; private set; }
    public float Max => maxHealth;
    public bool IsDead => Current <= 0f;

    /// <summary>(atual, máximo)</summary>
    public event Action<float, float> Changed;

    public event Action Died;

    void Awake()
    {
        Current = maxHealth;
    }

    void Start()
    {
        Changed?.Invoke(Current, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (IsDead || amount <= 0f) return;

        Current = Mathf.Max(0f, Current - amount * damageTakenMultiplier);
        Changed?.Invoke(Current, maxHealth);

        if (Current <= 0f) Died?.Invoke();
    }

    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f) return;

        Current = Mathf.Min(maxHealth, Current + amount);
        Changed?.Invoke(Current, maxHealth);
    }

    public void AddMaxHealth(float amount, bool healToo = true)
    {
        if (amount <= 0f) return;

        maxHealth += amount;
        if (healToo) Current = Mathf.Min(maxHealth, Current + amount);
        Changed?.Invoke(Current, maxHealth);
    }

    /// <summary>Aplica o multiplicador do piloto escolhido no início da run.</summary>
    public void ScaleMaxHealth(float multiplier)
    {
        if (multiplier <= 0f) return;

        maxHealth = Mathf.Max(1f, maxHealth * multiplier);
        Current = maxHealth;
        Changed?.Invoke(Current, maxHealth);
    }
}
