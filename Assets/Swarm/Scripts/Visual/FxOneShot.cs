using UnityEngine;

/// <summary>
/// Efeito de sprite sheet que toca uma vez e volta ao pool.
/// A duração é dividida entre os frames, então o efeito termina exatamente
/// no último frame (sem sobra de tempo com a imagem parada).
/// </summary>
[DisallowMultipleComponent]
public class FxOneShot : MonoBehaviour
{
    [SerializeField] SpriteRenderer target;
    [SerializeField] Sprite[] frames;
    [SerializeField] float lifetime = 0.24f;
    [SerializeField] bool despawnOnFinish = true;

    float _time;
    int _index = -1;
    bool _running;

    void Awake()
    {
        if (target == null) target = GetComponent<SpriteRenderer>();
        if (target == null) target = GetComponentInChildren<SpriteRenderer>();
    }

    void OnEnable()
    {
        _time = 0f;
        _index = -1;
        _running = false;
    }

    public void Init(float scale, Color color, int sortingOrder = -1)
    {
        transform.localScale = Vector3.one * Mathf.Max(0.01f, scale);

        if (target != null)
        {
            target.color = color;
            if (sortingOrder >= 0) target.sortingOrder = sortingOrder;
        }

        _time = 0f;
        _index = -1;
        _running = true;
        ApplyFrame(0);
    }

    void Update()
    {
        if (!_running) return;

        _time += Time.deltaTime;

        if (_time >= lifetime)
        {
            _running = false;
            if (despawnOnFinish) ObjectPool.Despawn(gameObject);
            return;
        }

        if (frames == null || frames.Length == 0) return;

        int frame = Mathf.Min(frames.Length - 1, Mathf.FloorToInt(_time / lifetime * frames.Length));
        if (frame != _index) ApplyFrame(frame);
    }

    void ApplyFrame(int index)
    {
        if (target == null || frames == null || frames.Length == 0) return;
        _index = Mathf.Clamp(index, 0, frames.Length - 1);
        target.sprite = frames[_index];
    }
}
