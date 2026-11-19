using System.Collections;
using UnityEngine;

/// <summary>
/// Congelamento curtíssimo no impacto (hitstop). Usa tempo real e nunca
/// sobrescreve uma pausa de level up / game over.
/// </summary>
public class HitStop : MonoBehaviour
{
    public static HitStop Instance { get; private set; }

    Coroutine _routine;

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Play(float duration = 0.05f, float scale = 0.05f)
    {
        if (_routine != null) return;
        if (Time.timeScale <= 0.01f) return;

        _routine = StartCoroutine(Run(duration, scale));
    }

    IEnumerator Run(float duration, float scale)
    {
        float previous = Time.timeScale;
        Time.timeScale = scale;

        yield return new WaitForSecondsRealtime(duration);

        // Se um level up/game over pausou no meio, não restaura por cima.
        if (Time.timeScale > 0.01f) Time.timeScale = previous;

        _routine = null;
    }
}
