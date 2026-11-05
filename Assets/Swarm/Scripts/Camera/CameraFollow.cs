using UnityEngine;

/// <summary>
/// Segue o jogador com suavização e aplica camera shake.
/// O shake usa tempo não escalado: não treme durante o level up (timeScale = 0).
/// </summary>
public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }

    [SerializeField] Transform target;
    [SerializeField] float smoothTime = 0.12f;

    Vector3 _basePosition;
    Vector3 _velocity;

    float _shakeTime;
    float _shakeDuration;
    float _shakeAmplitude;

    void Awake()
    {
        Instance = this;
        _basePosition = transform.position;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Shake(float amplitude, float duration)
    {
        if (amplitude <= 0f || duration <= 0f) return;

        // Não deixa um shake fraco cancelar um forte em andamento.
        if (_shakeTime > 0f && amplitude < _shakeAmplitude) return;

        _shakeAmplitude = amplitude;
        _shakeDuration = duration;
        _shakeTime = duration;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desired = new Vector3(target.position.x, target.position.y, _basePosition.z);
            _basePosition = Vector3.SmoothDamp(_basePosition, desired, ref _velocity, smoothTime);
        }

        Vector3 position = _basePosition;

        if (_shakeTime > 0f)
        {
            _shakeTime -= Time.unscaledDeltaTime;
            float strength = _shakeDuration > 0f ? Mathf.Clamp01(_shakeTime / _shakeDuration) : 0f;
            position += (Vector3)(Random.insideUnitCircle * (_shakeAmplitude * strength));
        }

        transform.position = position;
    }
}
