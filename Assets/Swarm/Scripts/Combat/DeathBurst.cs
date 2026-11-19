using UnityEngine;

/// <summary>
/// Explosão de partículas pooladas na morte de um inimigo.
/// </summary>
public class DeathBurst : MonoBehaviour
{
    public static DeathBurst Instance { get; private set; }

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

    public void Spawn(Vector3 position, Color color, int amount = 0)
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
            particle.Init(direction * Random.Range(2f, 5f), Random.Range(0.25f, 0.45f), color);
        }
    }
}
