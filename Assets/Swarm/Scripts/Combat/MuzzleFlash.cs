using UnityEngine;

/// <summary>
/// Flash curto no cano a cada disparo (poolado).
/// </summary>
public class MuzzleFlash : MonoBehaviour
{
    public static MuzzleFlash Instance { get; private set; }

    [SerializeField] BurstParticle template;

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Spawn(Vector3 position, Color color)
    {
        if (template == null) return;

        GameObject instance = ObjectPool.Spawn(template.gameObject, position, Quaternion.identity);
        if (instance == null) return;

        var particle = instance.GetComponent<BurstParticle>();
        if (particle != null) particle.Init(Vector2.zero, 0.07f, color, 0.38f, 11);
    }
}
