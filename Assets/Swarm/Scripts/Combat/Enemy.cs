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

    float _health;
    bool _dead;
    float _flash;
    Color _baseColor;
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
        _target = PlayerController.PlayerTransform;

        float multiplier = GameManager.Instance != null ? GameManager.Instance.Difficulty : 1f;
        _health = data != null ? data.maxHealth * multiplier : 10f;

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
        if (_flash <= 0f) return;

        _flash -= Time.deltaTime * 6f;
        if (_sprite != null)
            _sprite.color = Color.Lerp(_baseColor, Color.white, Mathf.Clamp01(_flash));
    }

    public void TakeDamage(float amount, bool critical)
    {
        if (_dead) return;

        _health -= amount;
        _flash = 1f;

        if (Sfx.Instance != null) Sfx.Instance.PlayHit();

        if (_health <= 0f) Die();
    }

    void Die()
    {
        _dead = true;

        if (GameManager.Instance != null) GameManager.Instance.RegisterKill();

        if (XpSystem.Instance != null && data != null)
            XpSystem.Instance.SpawnOrb(data.xp, transform.position);

        if (CameraFollow.Instance != null)
        {
            bool big = data != null && data.scale > 1.4f;
            CameraFollow.Instance.Shake(big ? 0.12f : 0.03f, 0.08f);
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
