using UnityEngine;

/// <summary>
/// Dados de um piloto. Só números: a variedade vem de como eles se combinam.
/// </summary>
[CreateAssetMenu(menuName = "SWARM/Character", fileName = "NovoPiloto")]
public class CharacterData : ScriptableObject
{
    public string displayName = "PILOTO";

    [TextArea(1, 3)]
    public string description = "";

    public Color color = Color.white;

    [Tooltip("Sprite da nave usada por este piloto. Se vazio, mantém o sprite procedural.")]
    public Sprite shipSprite;

    [Header("Arte")]
    public Sprite portrait;
    public Sprite[] idleFrames;
    public Sprite[] hitFrames;
    public Sprite[] deathFrames;
    public Sprite[] thrusterFrames;

    [Header("Multiplicadores (1 = neutro)")]
    public float maxHealthMultiplier = 1f;
    public float moveSpeedMultiplier = 1f;
    public float damageMultiplier = 1f;
    public float fireRateMultiplier = 1f;
}
