using UnityEngine;

/// <summary>
/// Rastro do propulsor: partículas atrás da nave enquanto ela se move.
/// </summary>
public class ThrusterTrail : MonoBehaviour
{
    [SerializeField] Transform visual;
    [SerializeField] BurstParticle template;
    [SerializeField] float interval = 0.045f;
    [SerializeField] Color color = new Color(0.35f, 0.85f, 1f, 0.8f);

    float _timer;

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsRunning) return;
        if (template == null) return;

        Vector2 move = InputReader.Move;
        if (move.sqrMagnitude < 0.01f)
        {
            _timer = 0f;
            return;
        }

        _timer -= Time.deltaTime;
        if (_timer > 0f) return;
        _timer = interval;

        Vector2 backward = visual != null ? -(Vector2)visual.up : Vector2.down;
        Vector3 position = transform.position + (Vector3)(backward * 0.28f);

        GameObject instance = ObjectPool.Spawn(template.gameObject, position, Quaternion.identity);
        if (instance == null) return;

        var particle = instance.GetComponent<BurstParticle>();
        if (particle == null) return;

        Vector2 velocity = backward * Random.Range(0.8f, 1.6f) + Random.insideUnitCircle * 0.4f;
        particle.Init(velocity, Random.Range(0.2f, 0.35f), color, 0.12f, 8);
    }
}
