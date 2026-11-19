using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inimigo perseguidor. Registra-se numa lista estática para consulta rápida
/// (alvo da arma) sem FindObjectsOfType a cada frame.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    public static readonly List<Enemy> All = new List<Enemy>();

    [SerializeField] EnemyData data;

    public EnemyData Data => data;

    /// <summary>Vida normalizada (0..1), usada pela barra do boss.</summary>
    public float Health01 => _maxHealth > 0f ? Mathf.Clamp01(_health / _maxHealth) : 0f;

    /// <summary>Quando true, outro componente (ex.: BossController) controla o movimento.</summary>
    [System.NonSerialized] public bool externalControl;

    /// <summary>Cooldown de dano do orbe orbital neste inimigo.</summary>
    [System.NonSerialized] public float orbitalCooldown;

    float _health;
    float _maxHealth;
    bool _dead;
    float _flash;
    Color _baseColor;
    Color _flashColor = Color.white;
    SpriteRenderer _sprite;
    Rigidbody2D _rb;
    Transform _target;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;
        _sprite = GetComponentInChildren<SpriteRenderer>();
    }

    void OnEnable()
    {
        All.Add(this);

        _dead = false;
        _flash = 0f;
        _flashColor = Color.white;
        _target = PlayerController.PlayerTransform;

        float multiplier = GameManager.Instance != null ? GameManager.Instance.Difficulty : 1f;
        _health = data != null ? data.maxHealth * multiplier : 10f;
        _maxHealth = _health;

        if (data != null)
        {
            transform.localScale = Vector3.one * data.scale;

            if (_sprite != null)
            {
                _baseColor = data.color;
                _sprite.color = _baseColor;
            }
        }
    }

    void OnDisable()
    {
        All.Remove(this);
    }

    void FixedUpdate()
    {
        if (_dead || data == null) return;
        if (externalControl) return;

        if (GameManager.Instance != null && !GameManager.Instance.IsRunning)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        if (_target == null)
        {
            _target = PlayerController.PlayerTransform;
            if (_target == null) return;
        }

        Vector2 direction = ((Vector2)_target.position - (Vector2)transform.position).normalized;
        _rb.linearVelocity = direction * data.moveSpeed;
    }

    void Update()
    {
        if (orbitalCooldown > 0f) orbitalCooldown -= Time.deltaTime;

        if (_flash <= 0f) return;

        _flash -= Time.deltaTime * 6f;
        if (_sprite != null)
            _sprite.color = Color.Lerp(_baseColor, _flashColor, Mathf.Clamp01(_flash));
    }

    public void TakeDamage(float amount, bool critical)
    {        if (_dead) return;

        _health -= amount;
        _flash = 1f;
        _flashColor = critical ? new Color(1f, 0.85f, 0.2f) : Color.white;

        if (Sfx.Instance != null) Sfx.Instance.PlayHit();

        if (_health <= 0f) Die();
    }

    void Die()
    {
        _dead = true;

        if (GameManager.Instance != null) GameManager.Instance.RegisterKill();

        if (XpSystem.Instance != null && data != null)
            XpSystem.Instance.SpawnOrb(data.xp, transform.position);

        if (DeathBurst.Instance != null)
            DeathBurst.Instance.Spawn(transform.position, _baseColor);

        if (CameraFollow.Instance != null)
        {
            bool big = data != null && data.scale > 1.4f;
            CameraFollow.Instance.Shake(big ? 0.25f : 0.03f, big ? 0.40f : 0.08f);
        }

        ObjectPool.Despawn(gameObject);
    }

    /// <summary>Inimigo vivo mais próximo dentro do alcance (ou null).</summary>
    public static Enemy FindNearest(Vector2 point, float maxRange)
    {
        Enemy best = null;
        float bestDistance = maxRange * maxRange;

        for (int i = 0; i < All.Count; i++)
        {
            Enemy candidate = All[i];
            if (candidate == null || candidate._dead) continue;

            float distance = ((Vector2)candidate.transform.position - point).sqrMagnitude;
            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = candidate;
            }
        }

        return best;
    }
}
