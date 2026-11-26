using UnityEngine;

/// <summary>
/// Orienta a nave para o inimigo mais próximo (ou para a direção do movimento).
/// Rotaciona só o filho "Visual", então o colisor/corpo do jogador não gira.
/// O cano (Muzzle) é filho do Visual e acompanha a ponta do triângulo.
/// </summary>
public class PlayerVisual : MonoBehaviour
{
    [SerializeField] Transform visual;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] SpriteAnimator animator;
    [SerializeField] float rotateSpeed = 720f;
    [SerializeField] float facingRange = 30f;

    float _angle;

    void Awake()
    {
        if (visual == null) return;
        if (sprite == null) sprite = visual.GetComponentInChildren<SpriteRenderer>();
        if (animator == null && sprite != null) animator = sprite.GetComponent<SpriteAnimator>();
    }

    /// <summary>Aplica a nave do piloto escolhido (idle animado em pixel art).</summary>
    public void SetShip(CharacterData character)
    {
        if (character == null || sprite == null) return;

        sprite.color = Color.white;

        if (animator == null) animator = sprite.GetComponent<SpriteAnimator>();

        if (character.idleFrames != null && character.idleFrames.Length > 0 && animator != null)
        {
            animator.Configure(character.idleFrames, 6f, true);
            return;
        }

        if (animator != null) animator.Stop();
        if (character.shipSprite != null) sprite.sprite = character.shipSprite;
    }

    void LateUpdate()
    {
        if (visual == null) return;

        Vector2 direction = Vector2.zero;

        PlayerStats stats = PlayerController.Instance != null ? PlayerController.Instance.Stats : null;
        float range = stats != null ? stats.range : facingRange;

        // Mesma prioridade da arma: boss no alcance, senão o mais próximo no alcance.
        Enemy target = null;
        if (BossController.Active != null && BossController.Active.Enemy != null)
        {
            float bossDistance = Vector2.Distance(transform.position, BossController.Active.transform.position);
            if (bossDistance <= range) target = BossController.Active.Enemy;
        }

        if (target == null) target = Enemy.FindNearest(transform.position, range);

        if (target != null)
        {
            direction = (Vector2)target.transform.position - (Vector2)transform.position;
        }
        else
        {
            Vector2 move = InputReader.Move;
            if (move.sqrMagnitude > 0.01f) direction = move;
        }

        if (direction.sqrMagnitude > 0.0001f)
        {
            // O sprite aponta para cima; -90 converte o ângulo para a rotação do sprite.
            float desired = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            _angle = Mathf.MoveTowardsAngle(_angle, desired, rotateSpeed * Time.deltaTime);
        }

        visual.rotation = Quaternion.Euler(0f, 0f, _angle);
    }
}
