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
    [Tooltip("Passos por unidade para alinhar a câmera à grade de pixel. 0 desliga.")]
    [SerializeField] float pixelSnap = 40f;

    Vector3 _basePosition;
    Vector3 _velocity;
    Camera _camera;
    float _baseSize;

    float _shakeTime;
    float _shakeDuration;
    float _shakeAmplitude;

    float _zoomTime;
    float _zoomDuration;
    float _zoomAmount;

    void Awake()
    {
        Instance = this;
        _basePosition = transform.position;
        _camera = GetComponent<Camera>();
        if (_camera != null) _baseSize = _camera.orthographicSize;
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

    /// <summary>Zoom de impacto: fecha e volta (orthographicSize - amount).</summary>
    public void ZoomPunch(float amount, float duration)
    {
        if (_camera == null || amount <= 0f || duration <= 0f) return;

        _zoomAmount = amount;
        _zoomDuration = duration;
        _zoomTime = duration;
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

        if (pixelSnap > 0f)
        {
            Vector3 snapped = transform.position;
            snapped.x = Mathf.Round(snapped.x * pixelSnap) / pixelSnap;
            snapped.y = Mathf.Round(snapped.y * pixelSnap) / pixelSnap;
            transform.position = snapped;
        }

        if (_camera != null)
        {
            if (_zoomTime > 0f)
            {
                _zoomTime -= Time.unscaledDeltaTime;
                float k = _zoomDuration > 0f ? Mathf.Clamp01(_zoomTime / _zoomDuration) : 0f;
                _camera.orthographicSize = _baseSize - _zoomAmount * Mathf.Sin(k * Mathf.PI);
            }
            else
            {
                _camera.orthographicSize = _baseSize;
            }
        }
    }
}
