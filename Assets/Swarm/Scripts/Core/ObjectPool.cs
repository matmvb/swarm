using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pool genérico por prefab. Substitui Instantiate/Destroy em gameplay
/// para evitar picos de GC (o famoso "micro stutter").
/// </summary>
public static class ObjectPool
{
    static readonly Dictionary<GameObject, Queue<GameObject>> Pools =
        new Dictionary<GameObject, Queue<GameObject>>();

    static readonly Dictionary<GameObject, GameObject> Origin =
        new Dictionary<GameObject, GameObject>();

    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null) return null;

        if (!Pools.TryGetValue(prefab, out Queue<GameObject> queue))
        {
            queue = new Queue<GameObject>();
            Pools[prefab] = queue;
        }

        GameObject instance = null;

        while (queue.Count > 0)
        {
            GameObject candidate = queue.Dequeue();
            if (candidate != null)
            {
                instance = candidate;
                break;
            }
        }

        if (instance == null)
        {
            instance = Object.Instantiate(prefab);
            Origin[instance] = prefab;
        }

        instance.transform.SetPositionAndRotation(position, rotation);
        instance.SetActive(true);
        return instance;
    }

    public static void Despawn(GameObject instance)
    {
        if (instance == null) return;

        // Já foi devolvido (ex.: dois triggers no mesmo passo de física).
        if (!instance.activeSelf) return;

        if (!Origin.TryGetValue(instance, out GameObject prefab) || prefab == null)
        {
            Object.Destroy(instance);
            return;
        }

        instance.SetActive(false);

        if (!Pools.TryGetValue(prefab, out Queue<GameObject> queue))
        {
            queue = new Queue<GameObject>();
            Pools[prefab] = queue;
        }

        queue.Enqueue(instance);
    }

    /// <summary>Chamado no restart: os prefabs antigos morreram com a cena.</summary>
    public static void ClearAll()
    {
        Pools.Clear();
        Origin.Clear();
    }
}
