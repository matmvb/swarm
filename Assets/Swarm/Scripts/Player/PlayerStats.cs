using UnityEngine;

/// <summary>
/// Todos os números do jogador em um só lugar.
/// Upgrades alteram estes campos em tempo de execução.
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [Header("Movimento")]
    public float moveSpeed = 5f;

    [Header("Arma")]
    public float damage = 10f;
    [Tooltip("Tiros por segundo.")]
    public float fireRate = 2.5f;
    public float range = 7.5f;
    public int projectiles = 1;
    public int pierce = 0;
    public float projectileSpeed = 16f;
    public float projectileLifetime = 1.2f;

    [Header("Coleta")]
    public float pickupRadius = 2.5f;

    [Header("Crítico")]
    [Range(0f, 1f)]
    public float critChance = 0.05f;
    public float critMultiplier = 2f;

    [Header("Orbe Orbital")]
    [Tooltip("Quantidade de orbes girando ao redor do jogador (upgrade).")]
    public int orbitals = 0;
    public float orbitalDamage = 1f;

    [Header("Defesa")]
    [Range(0f, 0.6f)]
    public float armor = 0f;
    public float regen = 0f;
}
