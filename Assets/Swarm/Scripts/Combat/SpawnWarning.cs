using UnityEngine;

/// <summary>
/// Aviso antes do inimigo nascer: um anel que cresce no ponto de spawn
/// e, ao terminar, cria o inimigo. Poolado.
/// </summary>
public class SpawnWarning : MonoBehaviour
{
    [SerializeField] Color color = new Color(1f, 0.72f, 0.30f, 0.72f);

    SpriteRenderer _sprite;
    SpriteAnimator _animator;
    Enemy _template;
    Vector3 _position;
    float _timer;
    float _duration;

    void Awake()
    {
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponentInChildren<SpriteAnimator>();
    }

    public void Begin(Enemy template, Vector3 position, float delay)
    {
        _template = template;
        _position = position;
        _duration = Mathf.Max(0.05f, delay);
        _timer = _duration;

        transform.position = position;
        transform.localScale = Vector3.one * 0.4f;

        if (_animator != null) _animator.PlayDefault();
        if (_sprite != null) _sprite.color = new Color(color.r, color.g, color.b, 0.2f);
    }

    void Update()
    {
        _timer -= Time.deltaTime;

        float progress = 1f - Mathf.Clamp01(_timer / _duration);
        transform.localScale = Vector3.one * Mathf.Lerp(0.4f, 1.05f, progress);

        if (_sprite != null)
        {
            Color value = color;
            value.a = Mathf.Lerp(0.25f, 0.9f, progress);
            _sprite.color = value;
        }

        if (_timer > 0f) return;

        // Durante o duelo do boss a horda não recebe reforços.
        if (BossController.Active != null)
        {
            ObjectPool.Despawn(gameObject);
            return;
        }

        if (_template != null)
            ObjectPool.Spawn(_template.gameObject, _position, Quaternion.identity);

        ObjectPool.Despawn(gameObject);
    }
}
