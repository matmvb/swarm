using UnityEngine;

/// <summary>
/// Projétil do boss. Dano no jogador, não em inimigos. Poolado.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyProjectile : MonoBehaviour
{
    Rigidbody2D _rb;
    SpriteRenderer _sprite;
    SpriteRenderer _core;
    Color _tint = Color.white;
    float _damage;
    float _life;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;
        _sprite = GetComponent<SpriteRenderer>();

        Transform core = transform.Find("Core");
        if (core != null) _core = core.GetComponent<SpriteRenderer>();
    }

    public void Init(Vector2 direction, float speed, float damage, float lifetime, Color tint, Color coreTint)
    {
        _damage = damage;
        _life = lifetime;
        _tint = tint;

        if (_sprite != null) _sprite.color = tint;

        if (_core != null)
        {
            _core.color = coreTint;
            _core.enabled = true;
        }

        Vector2 normalized = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.up;
        _rb.linearVelocity = normalized * speed;
        transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(normalized.y, normalized.x) * Mathf.Rad2Deg);
    }

    void Update()
    {
        _life -= Time.deltaTime;
        if (_life <= 0f) ObjectPool.Despawn(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player == null) return;

        if (player.HealthComponent != null) player.HealthComponent.TakeDamage(_damage);
        if (CameraFollow.Instance != null) CameraFollow.Instance.Shake(0.12f, 0.12f);
        if (Sfx.Instance != null) Sfx.Instance.PlayHit();
        if (DeathBurst.Instance != null) DeathBurst.Instance.SpawnHit(transform.position, _tint, 0.28f);

        ObjectPool.Despawn(gameObject);
    }
}
