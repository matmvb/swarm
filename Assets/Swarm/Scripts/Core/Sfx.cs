using UnityEngine;

/// <summary>
/// Sons 100% gerados por código (sem assets). Troque por samples reais depois:
/// a interface pública (PlayHit, PlayShoot...) não muda.
/// </summary>
public class Sfx : MonoBehaviour
{
    public static Sfx Instance { get; private set; }

    [Range(0f, 1f)]
    [SerializeField] float masterVolume = 0.35f;

    AudioSource _source;
    AudioSource _musicSource;
    AudioClip _shoot;
    AudioClip _hit;
    AudioClip _pickup;
    AudioClip _levelUp;
    AudioClip _death;
    AudioClip _warning;

    float _lastHitTime;
    float _lastPickupTime;

    void Awake()
    {
        Instance = this;

        _source = gameObject.AddComponent<AudioSource>();
        _source.playOnAwake = false;
        _source.spatialBlend = 0f;
        _source.loop = false;

        _shoot = BuildTone("sfx_shoot", 880f, 0.06f, 40f, 0f);
        _hit = BuildTone("sfx_hit", 180f, 0.09f, 30f, 0.6f);
        _pickup = BuildTone("sfx_pickup", 1320f, 0.06f, 38f, 0f);
        _levelUp = BuildArpeggio("sfx_levelup", new[] { 523f, 659f, 784f, 1046f }, 0.085f);
        _death = BuildTone("sfx_death", 110f, 0.55f, 6f, 0.3f);
        _warning = BuildTone("sfx_warning", 150f, 0.35f, 5f, 0.15f);

        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.playOnAwake = false;
        _musicSource.loop = true;
        _musicSource.spatialBlend = 0f;
        _musicSource.volume = 0.22f;
        _musicSource.clip = BuildMusic();
        _musicSource.Play();
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void PlayShoot()
    {
        Play(_shoot, 0.5f, Random.Range(0.95f, 1.06f));
    }

    public void PlayPickup()
    {
        if (Time.unscaledTime - _lastPickupTime < 0.035f) return;
        _lastPickupTime = Time.unscaledTime;
        Play(_pickup, 0.3f, Random.Range(0.97f, 1.04f));
    }

    public void PlayHit()
    {
        if (Time.unscaledTime - _lastHitTime < 0.045f) return;
        _lastHitTime = Time.unscaledTime;
        Play(_hit, 0.45f, Random.Range(0.9f, 1.12f));
    }

    public void PlayLevelUp()
    {
        Play(_levelUp, 0.8f, 1f);
    }

    public void PlayDeath()
    {
        Play(_death, 0.9f, 1f);
    }

    public void PlayWarning()
    {
        Play(_warning, 0.55f, 1f);
    }

    public void PlayBossDown()
    {
        Play(_levelUp, 0.9f, 0.75f);
    }

    void Play(AudioClip clip, float volume, float pitch)
    {
        if (clip == null || _source == null) return;
        _source.pitch = pitch;
        _source.PlayOneShot(clip, volume * masterVolume);
    }

    /// <summary>
    /// Loop ambiente de 16 s: pad lento (Am-F-C-G), drone grave e arpejo esparso.
    /// Gerado por código para não depender de asset nem licença.
    /// </summary>
    static AudioClip BuildMusic()
    {
        const int sampleRate = 22050;
        const float chordDuration = 4f;
        const float beat = 0.5f;

        float[][] chords =
        {
            new[] { 110.00f, 261.63f, 329.63f }, // Am
            new[] { 87.31f, 220.00f, 261.63f },  // F
            new[] { 130.81f, 329.63f, 392.00f }, // C
            new[] { 98.00f, 246.94f, 293.66f }   // G
        };

        int samplesPerChord = Mathf.RoundToInt(sampleRate * chordDuration);
        int length = samplesPerChord * chords.Length;
        var data = new float[length];

        for (int c = 0; c < chords.Length; c++)
        {
            int offset = c * samplesPerChord;
            float[] chord = chords[c];

            for (int i = 0; i < samplesPerChord; i++)
            {
                float t = i / (float)sampleRate;
                float attack = Mathf.Clamp01(t / 1f);
                float release = Mathf.Clamp01((chordDuration - t) / 1f);
                float envelope = Mathf.Min(attack, release);

                float pad = 0f;
                for (int n = 0; n < chord.Length; n++)
                    pad += Mathf.Sin(2f * Mathf.PI * chord[n] * t) * 0.12f;

                float drone = Mathf.Sin(2f * Mathf.PI * chord[0] * 0.5f * t) * 0.10f;

                float beatTime = t % beat;
                int beatIndex = Mathf.FloorToInt(t / beat);
                float note = chord[beatIndex % chord.Length] * 2f;
                float arp = Mathf.Sin(2f * Mathf.PI * note * beatTime) * Mathf.Exp(-beatTime * 6f) * 0.06f;

                data[offset + i] = (pad * envelope + drone * envelope + arp) * 0.9f;
            }
        }

        int fade = Mathf.RoundToInt(sampleRate * 0.02f);
        for (int i = 0; i < fade; i++)
        {
            float k = i / (float)fade;
            data[i] *= k;
            data[length - 1 - i] *= k;
        }

        var clip = AudioClip.Create("swarm_music", length, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    static AudioClip BuildTone(string name, float frequency, float duration, float decay, float noise)
    {
        const int sampleRate = 44100;
        int length = Mathf.Max(1, Mathf.RoundToInt(sampleRate * duration));
        var data = new float[length];

        for (int i = 0; i < length; i++)
        {
            float t = i / (float)sampleRate;
            float envelope = Mathf.Exp(-t * decay);
            float sweep = Mathf.Sin(2f * Mathf.PI * frequency * t * (1f - t * 1.5f));
            float hiss = Random.Range(-1f, 1f);
            data[i] = (sweep * (1f - noise) + hiss * noise) * envelope * 0.5f;
        }

        var clip = AudioClip.Create(name, length, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    static AudioClip BuildArpeggio(string name, float[] notes, float noteDuration)
    {
        const int sampleRate = 44100;
        int perNote = Mathf.RoundToInt(sampleRate * noteDuration);
        int length = perNote * notes.Length;
        var data = new float[length];

        for (int n = 0; n < notes.Length; n++)
        {
            for (int i = 0; i < perNote; i++)
            {
                float t = i / (float)sampleRate;
                float envelope = Mathf.Exp(-t * 14f);
                data[n * perNote + i] = Mathf.Sin(2f * Mathf.PI * notes[n] * t) * envelope * 0.5f;
            }
        }

        var clip = AudioClip.Create(name, length, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
