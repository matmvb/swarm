using UnityEngine;

/// <summary>
/// Orbe de XP com coleta magnética. Sem colisor: usa distância.
/// </summary>
public class XpOrb : MonoBehaviour
{
    [SerializeField] float collectDistance = 0.55f;
    [SerializeField] float magnetSpeed = 14f;

    int _value;
    SpriteRenderer _sprite;

    void Awake()
    {
        _sprite = GetComponentInChildren<SpriteRenderer>();
    }

    public void Setup(int value)
    {
        _value = Mathf.Max(1, value);

        if (_sprite == null) return;

        float weight = Mathf.Clamp01(_value / 10f);
        _sprite.color = Color.Lerp(new Color(0.4f, 1f, 0.5f), new Color(1f, 0.9f, 0.2f), weight);
        transform.localScale = Vector3.one * Mathf.Lerp(0.3f, 0.45f, weight);
    }

    void Update()
    {
        Transform player = PlayerController.PlayerTransform;
        if (player == null) return;

        float distance = Vector2.Distance(player.position, transform.position);

        if (distance <= collectDistance)
        {
            if (XpSystem.Instance != null) XpSystem.Instance.AddXp(_value);
            if (Sfx.Instance != null) Sfx.Instance.PlayPickup();
            ObjectPool.Despawn(gameObject);
            return;
        }

        float radius = 2.5f;
        if (PlayerController.Instance != null && PlayerController.Instance.Stats != null)
            radius = PlayerController.Instance.Stats.pickupRadius;

        if (distance <= radius)
            transform.position = Vector2.MoveTowards(transform.position, player.position, magnetSpeed * Time.deltaTime);
    }
}
