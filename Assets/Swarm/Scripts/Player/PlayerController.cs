using UnityEngine;

/// <summary>
/// Movimento por física + dano de contato.
/// O jogador é trigger: inimigos não o empurram, mas ainda causam dano.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    /// <summary>Referência global usada por inimigos e orbes (evita Find a cada frame).</summary>
    public static Transform PlayerTransform { get; private set; }

    [SerializeField] PlayerStats stats;
    [SerializeField] Health health;

    [Header("Contato")]
    [SerializeField] float contactInterval = 0.4f;

    public PlayerStats Stats => stats;
    public Health HealthComponent => health;

    Rigidbody2D _rb;
    float _contactTimer;

    void Awake()
    {
        Instance = this;
        PlayerTransform = transform;

        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;
    }

    void Start()
    {
        if (health != null) health.Died += HandleDeath;
    }

    void OnDestroy()
    {
        if (health != null) health.Died -= HandleDeath;

        if (Instance == this)
        {
            Instance = null;
            PlayerTransform = null;
        }
    }

    void FixedUpdate()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsRunning)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        _rb.linearVelocity = InputReader.Move * stats.moveSpeed;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsRunning) return;

        var enemy = other.GetComponent<Enemy>();
        if (enemy == null || enemy.Data == null) return;
        if (Time.time - _contactTimer < contactInterval) return;

        _contactTimer = Time.time;
        if (health != null) health.TakeDamage(enemy.Data.contactDamage);
        if (CameraFollow.Instance != null) CameraFollow.Instance.Shake(0.1f, 0.12f);
        if (Sfx.Instance != null) Sfx.Instance.PlayHit();
    }

    void HandleDeath()
    {
        if (Sfx.Instance != null) Sfx.Instance.PlayDeath();
        if (GameManager.Instance != null) GameManager.Instance.EndRun();
    }
}
