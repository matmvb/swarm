using UnityEngine;

/// <summary>
/// Cérebro do boss Núcleo: persegue, telegrafa e alterna rajada radial com investida.
/// Assume o movimento do Enemy via externalControl.
/// </summary>
[RequireComponent(typeof(Enemy), typeof(Rigidbody2D))]
public class BossController : MonoBehaviour
{
    public static BossController Active { get; private set; }

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
    Transform _player;
    State _state;
    float _stateEnd;
    float _nextBurst;
    float _nextCharge;
    Vector2 _chargeDirection;
    float _baseScale = 1f;

    public float Health01 => _enemy != null ? _enemy.Health01 : 1f;

    void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        Active = this;
        _enemy.externalControl = true;
        _player = PlayerController.PlayerTransform;
        _baseScale = _enemy.Data != null ? _enemy.Data.scale : 1f;

        _state = State.Chase;
        _nextBurst = Time.time + firstBurstDelay;
        _nextCharge = Time.time + firstChargeDelay;
        transform.localScale = Vector3.one * _baseScale;
    }

    void OnDisable()
    {
        if (Active == this) Active = null;
        if (_enemy != null) _enemy.externalControl = false;
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
                Pulse();
                if (Time.time >= _stateEnd) Burst();
                break;

            case State.TelegraphCharge:
                _rb.linearVelocity = Vector2.zero;
                Pulse();
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
    }

    void Pulse()
    {
        float pulse = 1f + Mathf.Sin(Time.time * 28f) * 0.07f;
        transform.localScale = Vector3.one * (_baseScale * pulse);
    }

    void Burst()
    {
        transform.localScale = Vector3.one * _baseScale;

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
        transform.localScale = Vector3.one * _baseScale;

        Vector2 toPlayer = (Vector2)_player.position - (Vector2)transform.position;
        _chargeDirection = toPlayer.sqrMagnitude > 0.01f ? toPlayer.normalized : Vector2.up;

        _state = State.Charge;
        _stateEnd = Time.time + chargeDuration;
    }

    void EnterRecover()
    {
        _state = State.Recover;
        _stateEnd = Time.time + recoverTime;
        _rb.linearVelocity = Vector2.zero;
    }
}
