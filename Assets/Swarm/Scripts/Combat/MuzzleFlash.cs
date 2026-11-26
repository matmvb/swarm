using UnityEngine;

/// <summary>
/// Flash curto no cano a cada disparo (poolado, sprite sheet de pixel art).
/// </summary>
public class MuzzleFlash : MonoBehaviour
{
    public static MuzzleFlash Instance { get; private set; }

    [SerializeField] GameObject template;

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
        Spawn(position, color, Quaternion.identity);
    }

    public void Spawn(Vector3 position, Color color, Quaternion rotation)
    {
        if (template == null) return;

        GameObject instance = ObjectPool.Spawn(template, position, rotation);
        if (instance == null) return;

        var fx = instance.GetComponent<FxOneShot>();
        if (fx != null) fx.Init(0.42f, color, 11);
    }
}
