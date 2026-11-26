using UnityEngine;

/// <summary>
/// Grid de fundo com parallax e tint de arena. Cada camada acompanha a câmera
/// em passos de célula, mantendo o padrão alinhado ao mundo.
/// </summary>
public class BackgroundGrid : MonoBehaviour
{
    [SerializeField] Transform follow;
    [SerializeField] float cell = 4f;
    [Range(0f, 2f)]
    [SerializeField] float parallax = 1f;
    [SerializeField] Color bossColor = new Color(0.50f, 0.10f, 0.16f, 0.30f);

    SpriteRenderer _sprite;
    Color _baseColor;
    float _bossBlend;

    void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        if (_sprite != null) _baseColor = _sprite.color;
    }

    void LateUpdate()
    {
        if (follow == null) return;

        Vector3 position = follow.position * parallax;
        transform.position = new Vector3(
            Mathf.Round(position.x / cell) * cell,
            Mathf.Round(position.y / cell) * cell,
            transform.position.z);

        if (_sprite == null) return;

        float target = BossController.Active != null ? 1f : 0f;
        _bossBlend = Mathf.MoveTowards(_bossBlend, target, Time.unscaledDeltaTime * 1.8f);
        _sprite.color = Color.Lerp(_baseColor, bossColor, _bossBlend);
    }
}
