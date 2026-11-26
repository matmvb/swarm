using UnityEngine;

/// <summary>
/// Arma automática: acha o inimigo mais próximo no alcance e dispara.
/// O jogador nunca aperta "atirar" — isso é design, não limitação.
/// </summary>
public class Weapon : MonoBehaviour
{
    [SerializeField] PlayerStats stats;
    [SerializeField] Transform muzzle;
    [SerializeField] Projectile projectilePrefab;

    [Header("Spread")]
    [Tooltip("Ângulo entre projéteis quando há mais de um.")]
    [SerializeField] float spreadDegrees = 10f;

    float _cooldown;

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsRunning) return;
        if (stats == null || projectilePrefab == null) return;

        _cooldown -= Time.deltaTime;
        if (_cooldown > 0f) return;

        Enemy target = FindTarget();
        if (target == null) return;

        _cooldown = 1f / Mathf.Max(0.05f, stats.fireRate);
        Fire(target.transform.position);
    }

    /// <summary>Prioriza o boss quando ele está no alcance; senão, o mais próximo.</summary>
    Enemy FindTarget()
    {
        BossController boss = BossController.Active;
        if (boss != null && boss.Enemy != null)
        {
            float distance = Vector2.Distance(transform.position, boss.transform.position);
            if (distance <= stats.range) return boss.Enemy;
        }

        return Enemy.FindNearest(transform.position, stats.range);
    }

    void Fire(Vector3 targetPosition)
    {
        Vector3 origin = muzzle != null ? muzzle.position : transform.position;
        Vector2 baseDirection = ((Vector2)(targetPosition - origin)).normalized;

        int count = Mathf.Max(1, stats.projectiles);
        int pierce = Mathf.Max(0, stats.pierce);

        for (int i = 0; i < count; i++)
        {
            float offset = count == 1 ? 0f : (i - (count - 1) * 0.5f) * spreadDegrees;
            Vector2 direction = Rotate(baseDirection, offset);

            GameObject instance = ObjectPool.Spawn(projectilePrefab.gameObject, origin, Quaternion.identity);
            if (instance == null) continue;

            var projectile = instance.GetComponent<Projectile>();
            if (projectile == null) continue;

            bool critical = Random.value < stats.critChance;
            float damage = stats.damage * (critical ? stats.critMultiplier : 1f);

            projectile.Init(direction, damage, pierce, stats.projectileSpeed, stats.projectileLifetime, critical);
        }

        if (Sfx.Instance != null) Sfx.Instance.PlayShoot();
        if (MuzzleFlash.Instance != null)
        {
            float angle = Mathf.Atan2(baseDirection.y, baseDirection.x) * Mathf.Rad2Deg;
            MuzzleFlash.Instance.Spawn(origin, Color.white, Quaternion.Euler(0f, 0f, angle));
        }
    }

    static Vector2 Rotate(Vector2 vector, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        return new Vector2(vector.x * cos - vector.y * sin, vector.x * sin + vector.y * cos);
    }
}
