using UnityEngine;

/// <summary>
/// FX de combate poolados: explosão de morte, faísca de impacto e partículas.
/// A explosão recebe a cor da classe e é disparada depois da death sheet.
/// </summary>
public class DeathBurst : MonoBehaviour
{
    public static DeathBurst Instance { get; private set; }

    [SerializeField] GameObject explosionTemplate;
    [SerializeField] GameObject hitTemplate;
    [SerializeField] GameObject coreTemplate;
    [SerializeField] BurstParticle template;
    [SerializeField] int count = 6;
    [SerializeField] float explosionLifetime = 0.40f;

    public float ExplosionDuration => Mathf.Max(0.05f, explosionLifetime);

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    /// <summary>Compatibilidade: explosão + partículas no mesmo ponto.</summary>
    public void Spawn(Vector3 position, Color color, int amount = 0, float scale = 1f)
    {
        SpawnDeath(position, color, scale, amount);
    }

    /// <summary>Explosão final + partículas coloridas pela classe do inimigo.</summary>
    public void SpawnDeath(Vector3 position, Color color, float scale, int amount = 0)
    {
        SpawnExplosion(position, color, scale);
        SpawnParticles(position, color, amount);
    }

    public void SpawnHit(Vector3 position, Color color)
    {
        SpawnHit(position, color, 0.28f);
    }

    public void SpawnHit(Vector3 position, Color color, float scale)
    {
        if (hitTemplate == null) return;

        GameObject instance = ObjectPool.Spawn(hitTemplate, position, Quaternion.identity);
        if (instance == null) return;

        var fx = instance.GetComponent<FxOneShot>();
        if (fx != null) fx.Init(scale, color, 9);
    }

    public void SpawnCore(Vector3 position)
    {
        if (coreTemplate == null) return;

        GameObject instance = ObjectPool.Spawn(coreTemplate, position, Quaternion.identity);
        if (instance == null) return;

        var fx = instance.GetComponent<FxOneShot>();
        if (fx != null) fx.Init(1.2f, Color.white, 8);
    }

    void SpawnParticles(Vector3 position, Color color, int amount)
    {
        if (template == null) return;

        int total = amount > 0 ? amount : count;

        for (int i = 0; i < total; i++)
        {
            GameObject instance = ObjectPool.Spawn(template.gameObject, position, Quaternion.identity);
            if (instance == null) continue;

            var particle = instance.GetComponent<BurstParticle>();
            if (particle == null) continue;

            Vector2 direction = Random.insideUnitCircle.normalized;
            Color particleColor = CombatPalette.Highlight(color);
            particle.Init(direction * Random.Range(2f, 5f), Random.Range(0.25f, 0.45f), particleColor, -1f, 7);
        }
    }

    void SpawnExplosion(Vector3 position, Color color, float scale)
    {
        if (explosionTemplate == null) return;

        GameObject instance = ObjectPool.Spawn(explosionTemplate, position, Quaternion.identity);
        if (instance == null) return;

        var fx = instance.GetComponent<FxOneShot>();
        if (fx != null) fx.Init(scale, color, 6);
    }
}
