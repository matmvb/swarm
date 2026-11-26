using UnityEngine;

/// <summary>
/// Inimigo de longo alcance: mantém distância e atira projéteis no jogador.
/// Assume o movimento do Enemy via externalControl.
/// </summary>
[RequireComponent(typeof(Enemy), typeof(Rigidbody2D))]
public class RangedEnemy : MonoBehaviour
{
    [SerializeField] EnemyProjectile projectilePrefab;
    [SerializeField] float range = 8f;
    [SerializeField] float keepDistance = 5.5f;
    [SerializeField] float fireInterval = 2.2f;
    [SerializeField] float projectileSpeed = 5.5f;
    [SerializeField] float damage = 6f;
    [SerializeField] Sprite[] chargeFrames;
    [SerializeField] float chargeLead = 0.45f;

    Enemy _enemy;
    Rigidbody2D _rb;
    Transform _player;
    float _cooldown;
    bool _charging;

    void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        _enemy.externalControl = true;
        _player = PlayerController.PlayerTransform;
        _cooldown = Random.Range(0.8f, 1.8f);
        _charging = false;
    }

    void OnDisable()
    {
        if (_enemy != null)
        {
            _enemy.externalControl = false;
            _enemy.PlayIdle();
        }
    }

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsRunning)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        if (_player == null)
        {
            _player = PlayerController.PlayerTransform;
            if (_player == null) return;
        }

        Vector2 toPlayer = (Vector2)_player.position - (Vector2)transform.position;
        float distance = toPlayer.magnitude;
        Vector2 direction = distance > 0.01f ? toPlayer / distance : Vector2.up;

        float speed = _enemy.Data != null ? _enemy.Data.moveSpeed : 1.4f;

        if (distance > keepDistance + 0.5f) _rb.linearVelocity = direction * speed;
        else if (distance < keepDistance - 0.5f) _rb.linearVelocity = -direction * speed;
        else _rb.linearVelocity = new Vector2(-direction.y, direction.x) * (speed * 0.5f);

        _cooldown -= Time.deltaTime;

        // Antecipação: o Olho troca para a sheet de carga antes de atirar.
        if (!_charging && _cooldown <= chargeLead && distance <= range && _cooldown > 0f)
        {
            _charging = true;
            _enemy.PlayTemporary(chargeFrames, 10f, Mathf.Max(0.1f, _cooldown));
        }

        if (_cooldown > 0f || distance > range || projectilePrefab == null) return;

        _cooldown = fireInterval;
        _charging = false;
        _enemy.PlayIdle();

        GameObject instance = ObjectPool.Spawn(projectilePrefab.gameObject, transform.position, Quaternion.identity);
        if (instance == null) return;

        var projectile = instance.GetComponent<EnemyProjectile>();
        if (projectile != null)
            projectile.Init(direction, projectileSpeed, damage, 4f,
                CombatPalette.EnemyProjectile, CombatPalette.EnemyProjectileAccent);
    }
}
