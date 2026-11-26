using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inimigo perseguidor. Registra-se numa lista estática para consulta rápida
/// (alvo da arma) sem FindObjectsOfType a cada frame.
///
/// Visual: o sprite fica num filho "Visual" para que o Dardo possa girar sem
/// girar o colisor. A animação é feita por SpriteAnimator (sem Animator).
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    public static readonly List<Enemy> All = new List<Enemy>();

    [SerializeField] EnemyData data;

    public EnemyData Data => data;

    /// <summary>Vida normalizada (0..1), usada pela barra do boss.</summary>
    public float Health01 => _maxHealth > 0f ? Mathf.Clamp01(_health / _maxHealth) : 0f;

    /// <summary>Avisa quando este inimigo morre (antes de voltar ao pool).</summary>
    public event System.Action Died;

    /// <summary>Quando true, outro componente (ex.: BossController) controla o movimento.</summary>
    [System.NonSerialized] public bool externalControl;

    /// <summary>Cooldown de dano do orbe orbital neste inimigo.</summary>
    [System.NonSerialized] public float orbitalCooldown;

    public bool IsDead => _dead;

    float _health;
    float _maxHealth;
    bool _dead;
    float _flash;
    Color _baseColor;
    Color _flashColor = Color.white;
    SpriteRenderer _sprite;
    SpriteRenderer _flashSprite;
    SpriteAnimator _animator;
    Transform _visual;
    Rigidbody2D _rb;
    Collider2D _collider;
    Transform _target;

    float _temporaryUntil;
    bool _temporary;
    float _hitUntil;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _visual = _sprite != null ? _sprite.transform : transform;

        if (_sprite != null)
        {
            _animator = _sprite.GetComponent<SpriteAnimator>();
            if (_animator == null) _animator = _sprite.gameObject.AddComponent<SpriteAnimator>();

            var flashObject = new GameObject("Flash");
            flashObject.transform.SetParent(_visual, false);
            _flashSprite = flashObject.AddComponent<SpriteRenderer>();
            _flashSprite.sharedMaterial = _sprite.sharedMaterial;
            _flashSprite.sortingOrder = _sprite.sortingOrder + 1;
            _flashSprite.enabled = false;
        }
    }

    void OnEnable()
    {
        All.Add(this);

        _dead = false;
        _flash = 0f;
        _flashColor = Color.white;
        _target = PlayerController.PlayerTransform;
        orbitalCooldown = 0f;
        _temporary = false;
        _temporaryUntil = 0f;
        _hitUntil = 0f;

        if (_visual != null) _visual.localRotation = Quaternion.identity;

        // A rotina de morte desliga o colisor; ao voltar do pool, ele precisa voltar.
        if (_collider != null) _collider.enabled = true;
        if (_rb != null) _rb.linearVelocity = Vector2.zero;

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
                ConfigureIdle();

                if (_flashSprite != null)
                {
                    _flashSprite.sprite = _sprite.sprite;
                    _flashSprite.color = new Color(1f, 1f, 1f, 0f);
                    _flashSprite.enabled = false;
                }
            }
        }
    }

    void OnDisable()
    {
        All.Remove(this);
        _temporary = false;
        _hitUntil = 0f;
        if (_flashSprite != null) _flashSprite.enabled = false;
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

        if (_flashSprite != null && _flashSprite.enabled && _sprite != null)
            _flashSprite.sprite = _sprite.sprite;

        if (_flash > 0f)
        {
            _flash -= Time.deltaTime * 6f;
            float alpha = Mathf.Clamp01(_flash);

            if (_sprite != null)
                _sprite.color = Color.Lerp(_baseColor, _flashColor, alpha);

            if (_flashSprite != null)
            {
                Color overlay = _flashColor;
                overlay.a = alpha;
                _flashSprite.color = overlay;
                _flashSprite.enabled = alpha > 0.01f;
            }
        }

        if (_dead) return;

        if (_temporary && Time.time >= _temporaryUntil)
            ConfigureIdle();

        if (_hitUntil > 0f && Time.time >= _hitUntil)
        {
            _hitUntil = 0f;
            ConfigureIdle();
        }
    }

    void LateUpdate()
    {
        if (_dead || data == null || !data.faceMovement || _visual == null || _rb == null) return;

        Vector2 velocity = _rb.linearVelocity;
        if (velocity.sqrMagnitude < 0.01f) return;

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        _visual.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void TakeDamage(float amount, bool critical)
    {
        if (_dead) return;

        _health -= amount;
        _flash = 1f;
        _flashColor = critical ? new Color(1f, 0.85f, 0.2f) : Color.white;

        if (data != null && data.hitFrames != null && data.hitFrames.Length > 0 && _animator != null)
        {
            _temporary = true;
            _temporaryUntil = Time.time + 0.09f;
            _hitUntil = _temporaryUntil;
            _animator.Play(data.hitFrames, 12f, false);
        }

        if (_flashSprite != null)
        {
            if (_sprite != null) _flashSprite.sprite = _sprite.sprite;
            _flashSprite.color = _flashColor;
            _flashSprite.enabled = true;
        }

        if (Sfx.Instance != null) Sfx.Instance.PlayHit();

        if (_health <= 0f) Die();
    }

    /// <summary>Troca a animação por um tempo (charge do Olho, ataque do Núcleo).</summary>
    public void PlayTemporary(Sprite[] frames, float fps, float duration)
    {
        if (_dead || frames == null || frames.Length == 0 || _animator == null) return;

        _temporary = true;
        _temporaryUntil = Time.time + Mathf.Max(0.05f, duration);
        _hitUntil = 0f;
        _animator.Play(frames, fps, true);
    }

    public void PlayIdle()
    {
        ConfigureIdle();
    }

    void ConfigureIdle()
    {
        _temporary = false;
        _hitUntil = 0f;

        if (data == null || _sprite == null) return;

        if (data.idleFrames != null && data.idleFrames.Length > 0)
        {
            if (_animator != null) _animator.Configure(data.idleFrames, data.animFps, true);
        }
        else if (data.sprite != null)
        {
            if (_animator != null) _animator.Stop();
            _sprite.sprite = data.sprite;
        }
    }

    void Die()
    {
        _dead = true;
        _temporary = false;
        _hitUntil = 0f;

        Died?.Invoke();

        if (GameManager.Instance != null) GameManager.Instance.RegisterKill();

        if (XpSystem.Instance != null && data != null)
            XpSystem.Instance.SpawnOrb(data.xp, transform.position);

        bool isBoss = GetComponent<BossController>() != null;
        float explosionScale = isBoss ? 3.4f : Mathf.Clamp(transform.localScale.x * 1.8f, 0.8f, 2.2f);

        if (DeathBurst.Instance != null)
        {
            DeathBurst.Instance.Spawn(transform.position, _baseColor, 0, explosionScale);
            if (isBoss) DeathBurst.Instance.SpawnCore(transform.position);
        }

        if (CameraFollow.Instance != null)
            CameraFollow.Instance.Shake(isBoss ? 0.35f : 0.03f, isBoss ? 0.55f : 0.08f);

        if (_flashSprite != null) _flashSprite.enabled = false;
        if (_rb != null) _rb.linearVelocity = Vector2.zero;
        if (_collider != null) _collider.enabled = false;

        if (data != null && data.deathFrames != null && data.deathFrames.Length > 0)
        {
            float duration = Mathf.Max(0.18f, data.deathFrames.Length / 12f);
            StartCoroutine(DeathRoutine(duration));
        }
        else
        {
            _flash = 0f;
            StartCoroutine(FallbackDeathRoutine());
        }
    }

    IEnumerator DeathRoutine(float duration)
    {
        if (_animator != null) _animator.Configure(data.deathFrames, 12f, false);
        yield return new WaitForSeconds(duration);
        ObjectPool.Despawn(gameObject);
    }

    IEnumerator FallbackDeathRoutine()
    {
        const float duration = 0.12f;

        float time = 0f;
        Vector3 startScale = transform.localScale;
        Color startColor = _sprite != null ? _sprite.color : _baseColor;

        while (time < duration)
        {
            time += Time.deltaTime;
            float k = Mathf.Clamp01(time / duration);

            transform.localScale = startScale * (1f + k * 0.25f);

            if (_sprite != null)
            {
                Color color = startColor;
                color.a = 1f - k;
                _sprite.color = color;
            }

            yield return null;
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
