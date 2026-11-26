using UnityEngine;

/// <summary>
/// Dados de um tipo de inimigo. Ajuste no Inspector, sem recompilar.
/// </summary>
[CreateAssetMenu(menuName = "SWARM/Enemy Data", fileName = "NovoInimigo")]
public class EnemyData : ScriptableObject
{
    public string displayName = "Drone";

    [Header("Combate")]
    public float maxHealth = 20f;
    public float moveSpeed = 2f;
    public float contactDamage = 6f;
    public int xp = 1;

    [Header("Spawner")]
    [Tooltip("Segundos de run antes deste inimigo poder aparecer.")]
    public float minTime = 0f;
    public float weight = 1f;
    [Tooltip("Máximo de inimigos deste tipo vivos ao mesmo tempo. 0 = sem limite. Use 1 para boss.")]
    public int maxAliveOfType = 0;

    [Header("Visual")]
    public Sprite sprite;
    public Color color = Color.red;
    public float scale = 0.7f;

    [Header("Animação")]
    public Sprite[] idleFrames;
    public Sprite[] moveFrames;
    public Sprite[] hitFrames;
    public Sprite[] deathFrames;
    public float animFps = 6f;
    [Tooltip("Gira o visual para a direção do movimento (Dardo).")]
    public bool faceMovement;
}
