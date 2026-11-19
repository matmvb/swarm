using UnityEngine;

/// <summary>
/// Partícula de morte/impacto: voa, desacelera e some. Poolada.
/// </summary>
public class BurstParticle : MonoBehaviour
{
    Vector2 _velocity;
    float _life;
    float _maxLife;
    Color _color;
    SpriteRenderer _sprite;

    void Awake()
    {
        _sprite = GetComponentInChildren<SpriteRenderer>();
    }

    public void Init(Vector2 velocity, float lifetime, Color color, float scale = -1f, int sortingOrder = -1)
    {
        _velocity = velocity;
        _life = lifetime;
        _maxLife = Mathf.Max(0.01f, lifetime);
        _color = color;

        if (_sprite != null)
        {
            _sprite.color = color;
            if (sortingOrder >= 0) _sprite.sortingOrder = sortingOrder;
        }

        transform.localScale = Vector3.one * (scale > 0f ? scale : Random.Range(0.14f, 0.26f));
    }

    void Update()
    {
        _life -= Time.deltaTime;
        if (_life <= 0f)
        {
            ObjectPool.Despawn(gameObject);
            return;
        }

        transform.position += (Vector3)(_velocity * Time.deltaTime);
        _velocity = Vector2.Lerp(_velocity, Vector2.zero, Time.deltaTime * 6f);

        if (_sprite != null)
        {
            Color color = _color;
            color.a = _life / _maxLife;
            _sprite.color = color;
        }
    }
}
