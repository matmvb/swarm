using UnityEngine;

/// <summary>
/// FX de combate poolados: explosão de morte, faísca de impacto e partículas.
/// </summary>
public class DeathBurst : MonoBehaviour
{
    public static DeathBurst Instance { get; private set; }

    [SerializeField] GameObject explosionTemplate;
    [SerializeField] GameObject hitTemplate;
    [SerializeField] GameObject coreTemplate;
    [SerializeField] BurstParticle template;
    [SerializeField] int count = 6;

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Spawn(Vector3 position, Color color, int amount = 0, float scale = 1f)
    {
        SpawnExplosion(position, scale);

        if (template == null) return;

        int total = amount > 0 ? amount : count;

        for (int i = 0; i < total; i++)
        {
            GameObject instance = ObjectPool.Spawn(template.gameObject, position, Quaternion.identity);
            if (instance == null) continue;

            var particle = instance.GetComponent<BurstParticle>();
            if (particle == null) continue;

            Vector2 direction = Random.insideUnitCircle.normalized;
            particle.Init(direction * Random.Range(2f, 5f), Random.Range(0.25f, 0.45f), color, -1f, 7);
        }
    }

    public void SpawnHit(Vector3 position, Color color)
    {
        if (hitTemplate == null) return;

        GameObject instance = ObjectPool.Spawn(hitTemplate, position, Quaternion.identity);
        if (instance == null) return;

        var fx = instance.GetComponent<FxOneShot>();
        if (fx != null) fx.Init(0.38f, color, 9);
    }

    public void SpawnCore(Vector3 position)
    {
        if (coreTemplate == null) return;

        GameObject instance = ObjectPool.Spawn(coreTemplate, position, Quaternion.identity);
        if (instance == null) return;

        var fx = instance.GetComponent<FxOneShot>();
        if (fx != null) fx.Init(1.2f, Color.white, 8);
    }

    void SpawnExplosion(Vector3 position, float scale)
    {
        if (explosionTemplate == null) return;

        GameObject instance = ObjectPool.Spawn(explosionTemplate, position, Quaternion.identity);
        if (instance == null) return;

        var fx = instance.GetComponent<FxOneShot>();
        if (fx != null) fx.Init(scale, Color.white, 6);
    }
}
