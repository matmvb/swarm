using UnityEngine;

/// <summary>
/// Propulsor em pixel art preso à nave. Substitui as partículas brancas
/// procedurais antigas: menos objetos por frame e acabamento coerente.
/// </summary>
public class ThrusterTrail : MonoBehaviour
{
    [SerializeField] Transform visual;
    [SerializeField] SpriteRenderer thruster;
    [SerializeField] Sprite[] frames;
    [SerializeField] float scale = 0.55f;

    SpriteAnimator _animator;
    bool _moving;

    void Start()
    {
        EnsureThruster();
    }

    public void SetFrames(Sprite[] newFrames)
    {
        frames = newFrames;
        EnsureThruster();
        _moving = false;

        if (_animator == null || frames == null || frames.Length == 0) return;
        _animator.Configure(frames, 12f, true);
    }

    void EnsureThruster()
    {
        if (visual == null) return;

        if (thruster == null)
        {
            var existing = visual.Find("Thruster");
            if (existing != null) thruster = existing.GetComponent<SpriteRenderer>();

            if (thruster == null)
            {
                var go = new GameObject("Thruster");
                go.transform.SetParent(visual, false);
                go.transform.localPosition = new Vector3(0f, -0.62f, 0f);
                go.transform.localScale = Vector3.one * scale;
                thruster = go.AddComponent<SpriteRenderer>();
                thruster.sortingOrder = 9;

                var playerSprite = visual.GetComponentInChildren<SpriteRenderer>();
                if (playerSprite != null) thruster.sharedMaterial = playerSprite.sharedMaterial;
            }
        }

        if (thruster == null) return;

        if (_animator == null)
        {
            _animator = thruster.GetComponent<SpriteAnimator>();
            if (_animator == null) _animator = thruster.gameObject.AddComponent<SpriteAnimator>();
        }

        if (frames != null && frames.Length > 0 && _animator != null)
            _animator.Configure(frames, 12f, true);

        thruster.enabled = false;
    }

    void Update()
    {
        if (thruster == null) return;

        if (GameManager.Instance == null || !GameManager.Instance.IsRunning)
        {
            thruster.enabled = false;
            return;
        }

        Vector2 move = InputReader.Move;
        bool moving = move.sqrMagnitude > 0.01f;

        if (moving != _moving)
        {
            _moving = moving;
            thruster.enabled = moving && frames != null && frames.Length > 0;
        }
    }
}
