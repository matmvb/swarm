using UnityEngine;

/// <summary>
/// Projétil do boss. Dano no jogador, não em inimigos. Poolado.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyProjectile : MonoBehaviour
{
    Rigidbody2D _rb;
    float _damage;
    float _life;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;
    }

    public void Init(Vector2 direction, float speed, float damage, float lifetime)
    {
        _damage = damage;
        _life = lifetime;

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
        if (DeathBurst.Instance != null) DeathBurst.Instance.SpawnHit(transform.position, UIPalette.Danger);

        ObjectPool.Despawn(gameObject);
    }
}
