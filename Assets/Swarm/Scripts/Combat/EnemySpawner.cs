using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cria inimigos em um anel fora da tela.
/// A taxa de spawn cresce com o tempo; o boss tem um horário fixo.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] List<Enemy> templates = new List<Enemy>();

    [Header("Espaço")]
    [SerializeField] float spawnRadius = 13f;
    [SerializeField] int maxAlive = 300;

    [Header("Ritmo")]
    [Tooltip("Inimigos por segundo no início da run.")]
    [SerializeField] float baseRatePerSecond = 1.2f;
    [Tooltip("Quanto a taxa cresce por segundo de run.")]
    [SerializeField] float rateRampPerSecond = 0.028f;
    [SerializeField] float maxRatePerSecond = 18f;
    [Tooltip("A cada quantos inimigos/segundo o spawn vira um lote.")]
    [SerializeField] int batchDivisor = 6;

    [Header("Boss")]
    [SerializeField] Enemy bossTemplate;
    [SerializeField] float bossTime = 300f;

    float _timer;
    bool _bossSpawned;

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsRunning) return;
        if (target == null) return;

        float elapsed = GameManager.Instance.Elapsed;

        if (!_bossSpawned && elapsed >= bossTime)
        {
            _bossSpawned = true;
            SpawnBoss();
        }

        if (templates.Count == 0) return;

        _timer -= Time.deltaTime;
        if (_timer > 0f) return;

        float rate = Mathf.Min(maxRatePerSecond, baseRatePerSecond + elapsed * rateRampPerSecond);
        int batch = 1 + Mathf.FloorToInt(rate / Mathf.Max(1, batchDivisor));
        _timer = batch / Mathf.Max(0.1f, rate);

        for (int i = 0; i < batch; i++) SpawnOne(elapsed);
    }

    void SpawnBoss()
    {
        if (bossTemplate == null) return;

        Vector2 direction = Random.insideUnitCircle.normalized;
        Vector3 position = target.position + (Vector3)(direction * (spawnRadius + 2f));
        ObjectPool.Spawn(bossTemplate.gameObject, position, Quaternion.identity);
    }

    void SpawnOne(float elapsed)
    {
        if (Enemy.All.Count >= maxAlive) return;

        Enemy template = PickTemplate(elapsed);
        if (template == null) return;

        Vector2 direction = Random.insideUnitCircle.normalized;
        Vector3 position = target.position + (Vector3)(direction * spawnRadius);

        ObjectPool.Spawn(template.gameObject, position, Quaternion.identity);
    }

    Enemy PickTemplate(float elapsed)
    {
        float totalWeight = 0f;

        for (int i = 0; i < templates.Count; i++)
        {
            Enemy template = templates[i];
            if (!CanSpawn(template, elapsed)) continue;
            totalWeight += Mathf.Max(0.01f, template.Data.weight);
        }

        if (totalWeight <= 0f) return null;

        float roll = Random.value * totalWeight;

        for (int i = 0; i < templates.Count; i++)
        {
            Enemy template = templates[i];
            if (!CanSpawn(template, elapsed)) continue;

            roll -= Mathf.Max(0.01f, template.Data.weight);
            if (roll <= 0f) return template;
        }

        return null;
    }

    static bool CanSpawn(Enemy template, float elapsed)
    {
        if (template == null || template.Data == null) return false;
        if (template.Data.minTime > elapsed) return false;

        if (template.Data.maxAliveOfType > 0)
        {
            int alive = 0;
            for (int i = 0; i < Enemy.All.Count; i++)
            {
                Enemy other = Enemy.All[i];
                if (other != null && other.Data == template.Data) alive++;
            }

            if (alive >= template.Data.maxAliveOfType) return false;
        }

        return true;
    }
}
