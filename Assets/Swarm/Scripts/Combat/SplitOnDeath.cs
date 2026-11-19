using UnityEngine;

/// <summary>
/// Ao morrer, gera fragmentos (usa um template de inimigo comum).
/// </summary>
[RequireComponent(typeof(Enemy))]
public class SplitOnDeath : MonoBehaviour
{
    [SerializeField] Enemy fragmentTemplate;
    [SerializeField] int count = 2;
    [SerializeField] float spread = 1.1f;

    Enemy _enemy;

    void Awake()
    {
        _enemy = GetComponent<Enemy>();
    }

    void OnEnable()
    {
        if (_enemy != null) _enemy.Died += Split;
    }

    void OnDisable()
    {
        if (_enemy != null) _enemy.Died -= Split;
    }

    void Split()
    {
        if (fragmentTemplate == null) return;

        for (int i = 0; i < count; i++)
        {
            Vector2 direction = Random.insideUnitCircle.normalized;
            Vector3 position = transform.position + (Vector3)(direction * spread);
            ObjectPool.Spawn(fragmentTemplate.gameObject, position, Quaternion.identity);
        }
    }
}
