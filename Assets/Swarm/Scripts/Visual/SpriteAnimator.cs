using UnityEngine;

/// <summary>
/// Animação de sprite sheets já fatiadas no importador.
/// Não usa Animator/AnimatorController: em hordas grandes, um update simples
/// compartilhado é mais barato e previsível.
/// </summary>
[DisallowMultipleComponent]
public class SpriteAnimator : MonoBehaviour
{
    [SerializeField] SpriteRenderer target;
    [SerializeField] Sprite[] frames;
    [SerializeField] float fps = 8f;
    [SerializeField] bool loop = true;
    [SerializeField] bool playOnEnable = true;

    Sprite[] _defaultFrames;
    float _defaultFps;
    bool _defaultLoop;

    float _clock;
    int _frame;
    int _direction = 1;
    bool _playing;

    public SpriteRenderer Renderer => target;
    public bool IsPlaying => _playing;
    public Sprite[] Frames => frames;

    void Awake()
    {
        if (target == null) target = GetComponent<SpriteRenderer>();
        if (target == null) target = GetComponentInChildren<SpriteRenderer>();
    }

    void OnEnable()
    {
        if (playOnEnable && frames != null && frames.Length > 0) Play(frames, fps, loop);
    }

    /// <summary>Define a animação padrão e já a coloca em loop.</summary>
    public void Configure(Sprite[] newFrames, float newFps, bool newLoop = true)
    {
        _defaultFrames = newFrames;
        _defaultFps = Mathf.Max(0.01f, newFps);
        _defaultLoop = newLoop;

        if (newFrames != null && newFrames.Length > 0)
        {
            ApplyFirstFrame(newFrames);
            Play(newFrames, newFps, newLoop);
        }
        else
        {
            _playing = false;
        }
    }

    public void Play(Sprite[] newFrames, float newFps, bool newLoop = true)
    {
        if (newFrames == null || newFrames.Length == 0)
        {
            _playing = false;
            return;
        }

        frames = newFrames;
        fps = Mathf.Max(0.01f, newFps);
        loop = newLoop;
        _clock = 0f;
        _frame = 0;
        _direction = 1;
        _playing = true;
        ApplyFirstFrame(frames);
    }

    public void PlayDefault()
    {
        if (_defaultFrames != null && _defaultFrames.Length > 0)
            Play(_defaultFrames, _defaultFps, _defaultLoop);
    }

    public void Stop()
    {
        _playing = false;
    }

    void Update()
    {
        if (!_playing || frames == null || frames.Length == 0 || target == null) return;

        _clock += Time.deltaTime * fps;

        while (_clock >= 1f)
        {
            _clock -= 1f;

            if (frames.Length == 1)
            {
                ApplyFrame(0);
                continue;
            }

            int next = _frame + _direction;

            if (next >= frames.Length || next < 0)
            {
                if (!loop)
                {
                    ApplyFrame(Mathf.Clamp(_frame, 0, frames.Length - 1));
                    _playing = false;
                    return;
                }

                _direction = -_direction;
                next = _frame + _direction;
                next = Mathf.Clamp(next, 0, frames.Length - 1);
            }

            _frame = next;
            ApplyFrame(_frame);
        }
    }

    void ApplyFirstFrame(Sprite[] newFrames)
    {
        if (target == null) target = GetComponent<SpriteRenderer>();
        if (target == null || newFrames == null || newFrames.Length == 0) return;
        target.sprite = newFrames[0];
    }

    void ApplyFrame(int index)
    {
        if (target == null || frames == null || frames.Length == 0) return;
        target.sprite = frames[Mathf.Clamp(index, 0, frames.Length - 1)];
    }
}
