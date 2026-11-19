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
    TrailRenderer _trail;
    float _damage;
    int _pierce;
    float _speed;
    float _life;
    bool _critical;

    static readonly Color NormalColor = new Color(0.6f, 1f, 1f);
    static readonly Color CriticalColor = new Color(1f, 0.9f, 0.3f);

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _trail = GetComponent<TrailRenderer>();
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

        if (_sprite != null) _sprite.color = critical ? CriticalColor : NormalColor;

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

        if (_pierce > 0) _pierce--;
        else ObjectPool.Despawn(gameObject);
    }
}
