using System.Collections;
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

    /// <summary>Vida normalizada (0..1), usada pela barra do boss.</summary>
    public float Health01 => _maxHealth > 0f ? Mathf.Clamp01(_health / _maxHealth) : 0f;

    /// <summary>Avisa quando este inimigo morre (antes de voltar ao pool).</summary>
    public event System.Action Died;

    /// <summary>Quando true, outro componente (ex.: BossController) controla o movimento.</summary>
    [System.NonSerialized] public bool externalControl;

    /// <summary>Cooldown de dano do orbe orbital neste inimigo.</summary>
    [System.NonSerialized] public float orbitalCooldown;

    float _health;
    float _maxHealth;
    bool _dead;
    float _flash;
    Color _baseColor;
    Color _flashColor = Color.white;
    SpriteRenderer _sprite;
    SpriteRenderer _flashSprite;
    Rigidbody2D _rb;
    Collider2D _collider;
    Transform _target;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();

        if (_sprite != null)
        {
            var flashObject = new GameObject("Flash");
            flashObject.transform.SetParent(transform, false);
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

        if (_flash <= 0f) return;

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

    public void TakeDamage(float amount, bool critical)
    {
        if (_dead) return;

        _health -= amount;
        _flash = 1f;
        _flashColor = critical ? new Color(1f, 0.85f, 0.2f) : Color.white;

        if (_flashSprite != null)
        {
            if (_sprite != null) _flashSprite.sprite = _sprite.sprite;
            _flashSprite.color = _flashColor;
            _flashSprite.enabled = true;
        }

        if (Sfx.Instance != null) Sfx.Instance.PlayHit();

        if (_health <= 0f) Die();
    }

    void Die()
    {
        _dead = true;

        Died?.Invoke();

        if (GameManager.Instance != null) GameManager.Instance.RegisterKill();

        if (XpSystem.Instance != null && data != null)
            XpSystem.Instance.SpawnOrb(data.xp, transform.position);

        if (DeathBurst.Instance != null)
            DeathBurst.Instance.Spawn(transform.position, _baseColor);

        if (CameraFollow.Instance != null)
        {
            bool big = GetComponent<BossController>() != null;
            CameraFollow.Instance.Shake(big ? 0.25f : 0.03f, big ? 0.40f : 0.08f);
        }

        if (_flashSprite != null) _flashSprite.enabled = false;

        if (_rb != null) _rb.linearVelocity = Vector2.zero;

        if (_collider != null) _collider.enabled = false;

        bool isBoss = GetComponent<BossController>() != null;
        if (isBoss)
        {
            ObjectPool.Despawn(gameObject);
            return;
        }

        _flash = 0f;
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        const float duration = 0.12f;

        float time = 0f;
        Vector3 startScale = transform.localScale;
        Color startColor = _sprite != null ? _sprite.color : _baseColor;

        while (time < duration)
        {
            time += Time.deltaTime;
            float k = Mathf.Clamp01(time / duration);

            transform.localScale = startScale * (1f + k * 0.35f);

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
