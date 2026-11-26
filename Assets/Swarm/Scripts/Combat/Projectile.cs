using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Projétil com perfuração. Poolado: nunca é destruído durante a run.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    readonly HashSet<Enemy> _hits = new HashSet<Enemy>();

    Rigidbody2D _rb;
    SpriteRenderer _sprite;
    SpriteRenderer _core;
    TrailRenderer _trail;
    float _damage;
    int _pierce;
    float _speed;
    float _life;
    bool _critical;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;
        _sprite = GetComponent<SpriteRenderer>();
        _trail = GetComponent<TrailRenderer>();

        Transform core = transform.Find("Core");
        if (core != null) _core = core.GetComponent<SpriteRenderer>();
    }

    public void Init(Vector2 direction, float damage, int pierce, float speed, float lifetime, bool critical)
    {
        _damage = damage;
        _pierce = pierce;
        _speed = speed;
        _life = lifetime;
        _critical = critical;

        _hits.Clear();
        if (_trail != null) _trail.Clear();

        if (_sprite != null) _sprite.color = critical ? CombatPalette.PlayerCritical : CombatPalette.Player;

        if (_core != null)
        {
            _core.color = CombatPalette.PlayerCore;
            _core.enabled = true;
        }

        Vector2 normalized = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.up;
        _rb.linearVelocity = normalized * _speed;
        transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(normalized.y, normalized.x) * Mathf.Rad2Deg);
    }

    void Update()
    {
        _life -= Time.deltaTime;
        if (_life <= 0f) ObjectPool.Despawn(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var enemy = other.GetComponent<Enemy>();
        if (enemy == null || !_hits.Add(enemy)) return;

        enemy.TakeDamage(_damage, _critical);

        if (DeathBurst.Instance != null)
        {
            Color impact = _critical ? CombatPalette.PlayerCritical : CombatPalette.Impact;
            DeathBurst.Instance.SpawnHit(transform.position, impact, _critical ? 0.32f : 0.26f);
        }

        if (_pierce > 0) _pierce--;
        else ObjectPool.Despawn(gameObject);
    }
}
