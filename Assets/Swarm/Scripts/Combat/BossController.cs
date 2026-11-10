using UnityEngine;

/// <summary>
/// Cérebro do boss Núcleo: persegue, telegrafa e alterna rajada radial com investida.
/// Assume o movimento do Enemy via externalControl.
/// </summary>
[RequireComponent(typeof(Enemy), typeof(Rigidbody2D))]
public class BossController : MonoBehaviour
{
    public static BossController Active { get; private set; }

    /// <summary>Duração da última luta vencida (0 se não matou o boss).</summary>
    public static float LastFightDuration { get; private set; }
    public static bool LastFightWon { get; private set; }

    [Header("Rajada radial")]
    [SerializeField] EnemyProjectile projectilePrefab;
    [SerializeField] int burstCount = 14;
    [SerializeField] float burstSpeed = 6.5f;
    [SerializeField] float burstDamage = 8f;
    [SerializeField] float burstLifetime = 4.5f;
    [SerializeField] float burstEvery = 5f;
    [SerializeField] float burstTelegraph = 0.75f;

    [Header("Investida")]
    [SerializeField] float chargeEvery = 11f;
    [SerializeField] float chargeSpeed = 10f;
    [SerializeField] float chargeDuration = 0.5f;
    [SerializeField] float chargeTelegraph = 0.65f;

    [Header("Ritmo")]
    [SerializeField] float recoverTime = 0.8f;
    [SerializeField] float firstBurstDelay = 2.5f;
    [SerializeField] float firstChargeDelay = 7f;

    enum State
    {
        Chase,
        TelegraphBurst,
        TelegraphCharge,
        Charge,
        Recover
    }

    Enemy _enemy;
    Rigidbody2D _rb;
    SpriteRenderer _sprite;
    SpriteRenderer _telegraph;
    Transform _player;
    State _state;
    float _stateEnd;
    float _nextBurst;
    float _nextCharge;
    float _spawnTime;
    Vector2 _chargeDirection;
    float _baseScale = 1f;

    public float Health01 => _enemy != null ? _enemy.Health01 : 1f;

    public Enemy Enemy => _enemy;

    public static void ResetRunStats()
    {
        LastFightWon = false;
        LastFightDuration = 0f;
    }

    void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _rb = GetComponent<Rigidbody2D>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
    }

    void OnEnable()
    {
        Active = this;
        _enemy.externalControl = true;
        _player = PlayerController.PlayerTransform;
        _baseScale = _enemy.Data != null ? _enemy.Data.scale : 1f;

        _state = State.Chase;
        _spawnTime = Time.time;
        _nextBurst = Time.time + firstBurstDelay;
        _nextCharge = Time.time + firstChargeDelay;

        LastFightWon = false;
        transform.localScale = Vector3.one * _baseScale;

        EnsureTelegraph();
        HideTelegraph();

        if (Sfx.Instance != null) Sfx.Instance.PlayWarning();
    }

    void OnDisable()
    {
        if (Active == this) Active = null;
        if (_enemy != null) _enemy.externalControl = false;

        if (_enemy != null && _enemy.Health01 <= 0f)
        {
            LastFightWon = true;
            LastFightDuration = Time.time - _spawnTime;
        }

        HideTelegraph();
        transform.localScale = Vector3.one * _baseScale;
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

        switch (_state)
        {
            case State.Chase:
                TickChase();
                break;

            case State.TelegraphBurst:
                _rb.linearVelocity = Vector2.zero;
                AnimateTelegraph();
                if (Time.time >= _stateEnd) Burst();
                break;

            case State.TelegraphCharge:
                _rb.linearVelocity = Vector2.zero;
                AnimateTelegraph();
                if (Time.time >= _stateEnd) BeginCharge();
                break;

            case State.Charge:
                _rb.linearVelocity = _chargeDirection * chargeSpeed;
                if (Time.time >= _stateEnd)
                {
                    _nextCharge = Time.time + chargeEvery;
                    EnterRecover();
                }
                break;

            case State.Recover:
                _rb.linearVelocity = Vector2.zero;
                if (Time.time >= _stateEnd)
                {
                    transform.localScale = Vector3.one * _baseScale;
                    _state = State.Chase;
                }
                break;
        }
    }

    void TickChase()
    {
        transform.localScale = Vector3.one * _baseScale;

        Vector2 toPlayer = (Vector2)_player.position - (Vector2)transform.position;
        Vector2 direction = toPlayer.sqrMagnitude > 0.01f ? toPlayer.normalized : Vector2.up;
        float speed = _enemy.Data != null ? _enemy.Data.moveSpeed : 1f;
        _rb.linearVelocity = direction * speed;

        if (Time.time >= _nextBurst) Telegraph(State.TelegraphBurst, burstTelegraph);
        else if (Time.time >= _nextCharge) Telegraph(State.TelegraphCharge, chargeTelegraph);
    }

    void Telegraph(State state, float duration)
    {
        _state = state;
        _stateEnd = Time.time + duration;
        _rb.linearVelocity = Vector2.zero;

        ShowTelegraph();
        if (Sfx.Instance != null) Sfx.Instance.PlayWarning();
    }

    void Burst()
    {
        HideTelegraph();

        if (projectilePrefab != null)
        {
            float offset = Random.Range(0f, 360f);

            for (int i = 0; i < burstCount; i++)
            {
                float angle = offset + (360f / burstCount) * i;
                float radians = angle * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
                Vector3 position = transform.position + (Vector3)(direction * (_baseScale * 0.6f));

                GameObject instance = ObjectPool.Spawn(projectilePrefab.gameObject, position, Quaternion.identity);
                if (instance == null) continue;

                var projectile = instance.GetComponent<EnemyProjectile>();
                if (projectile != null)
                    projectile.Init(direction, burstSpeed, burstDamage, burstLifetime);
            }
        }

        _nextBurst = Time.time + burstEvery;
        EnterRecover();
    }

    void BeginCharge()
    {
        HideTelegraph();

        Vector2 toPlayer = (Vector2)_player.position - (Vector2)transform.position;
        _chargeDirection = toPlayer.sqrMagnitude > 0.01f ? toPlayer.normalized : Vector2.up;

        _state = State.Charge;
        _stateEnd = Time.time + chargeDuration;
    }

    void EnterRecover()
    {
        HideTelegraph();
        _state = State.Recover;
        _stateEnd = Time.time + recoverTime;
        _rb.linearVelocity = Vector2.zero;
    }

    // ─────────────────────────────────────────────────────────────
    // TELEGRAFO (halo vermelho pulsando + som)
    // ─────────────────────────────────────────────────────────────

    void EnsureTelegraph()
    {
        if (_telegraph != null || _sprite == null || _sprite.sprite == null) return;

        var go = new GameObject("TelegraphHalo");
        go.transform.SetParent(transform, false);
        go.transform.localScale = Vector3.one * 1.7f;

        var sprite = go.AddComponent<SpriteRenderer>();
        sprite.sprite = _sprite.sprite;
        sprite.sharedMaterial = _sprite.sharedMaterial;
        sprite.color = new Color(1f, 0.15f, 0.15f, 0.3f);
        sprite.sortingOrder = _sprite.sortingOrder - 1;

        _telegraph = sprite;
        go.SetActive(false);
    }

    void ShowTelegraph()
    {
        if (_telegraph == null) return;
        _telegraph.gameObject.SetActive(true);
    }

    void AnimateTelegraph()
    {
        float pulse = 1f + Mathf.Sin(Time.time * 22f) * 0.1f;
        transform.localScale = Vector3.one * (_baseScale * pulse);

        if (_telegraph == null) return;

        float wave = Mathf.PingPong(Time.time * 3f, 1f);
        _telegraph.color = new Color(1f, 0.15f, 0.15f, Mathf.Lerp(0.15f, 0.55f, wave));
        _telegraph.transform.localScale = Vector3.one * Mathf.Lerp(1.6f, 2.05f, wave);
    }

    void HideTelegraph()
    {
        if (_telegraph != null) _telegraph.gameObject.SetActive(false);
    }
}
