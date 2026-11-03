using UnityEngine;

// Lista de todos los efectos del juego.
public enum Sfx
{
    UIClick,
    UIBack,
    Jump,
    JumpPad,
    PowerGrow,
    PowerShrink,
    PowerPickup,
    DoorOpen,
    DoorClose,
    ButtonPress,
    ButtonRelease,
    SawHit,
    Win,
    Lose,
    DoorEnter,
    Fall
}

public class ProceduralSfx : MonoBehaviour
{
    private const double TwoPi = 2.0 * System.Math.PI;
    private const int PoolSize = 8;

    private static readonly float[] Pure = { 1f };
    private static readonly float[] Two = { 1f, 0.48f };
    private static readonly float[] Soft3 = { 1f, 0.25f, 0.10f };
    private static readonly float[] Saw5 = { 1.00f, 0.48f, 0.30f, 0.23f, 0.19f };

    public static ProceduralSfx Instance { get; private set; }

    [Range(0f, 1f)]
    public float masterVolume = 1f;

    private const float ClipGain = 2.8f;

    private AudioSource[] _pool;
    private int _next;
    private AudioClip[] _clips;

    private static uint _noiseState = 22222;

    // ------------------------------------------------------------------
    // Inicializacion
    // ------------------------------------------------------------------

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null) return;

        GameObject go = new GameObject("ProceduralSfx");
        go.AddComponent<ProceduralSfx>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _pool = new AudioSource[PoolSize];
        for (int i = 0; i < PoolSize; i++)
        {
            AudioSource s = gameObject.AddComponent<AudioSource>();
            s.playOnAwake = false;
            s.loop = false;
            s.spatialBlend = 0.5f;
            _pool[i] = s;
        }

        Generate();
    }

    private void Generate()
    {
        int sr = AudioSettings.outputSampleRate;
        Sfx[] all = (Sfx[])System.Enum.GetValues(typeof(Sfx));
        _clips = new AudioClip[all.Length];

        foreach (Sfx s in all)
        {
            float[] data = Render(s, sr);
            AudioClip clip = AudioClip.Create(s.ToString(), data.Length, 1, sr, false);
            clip.SetData(data, 0);
            _clips[(int)s] = clip;
        }
    }

    // ------------------------------------------------------------------
    // API publica
    // ------------------------------------------------------------------

    public static void Play(Sfx sfx, float volume = 1f, float pitchRandom = 0.05f)
    {
        if (Instance == null || Instance._clips == null) return;

        AudioSource src = Instance._pool[Instance._next];
        Instance._next = (Instance._next + 1) % PoolSize;

        src.clip = Instance._clips[(int)sfx];
        src.volume = Mathf.Clamp01(volume * Instance.masterVolume);
        src.pitch = 1f + Random.Range(-pitchRandom, pitchRandom);
        src.Play();
    }

    public static AudioClip GetClip(Sfx sfx)
    {
        if (Instance == null || Instance._clips == null) return null;
        return Instance._clips[(int)sfx];
    }

    // ------------------------------------------------------------------
    // Diseño de cada sonido (tonalidad base: Sol / G, para que combine con la musica)
    // ------------------------------------------------------------------

    private static float[] Render(Sfx s, int sr)
    {
        switch (s)
        {
            case Sfx.UIClick: return RenderUIClick(sr);
            case Sfx.UIBack: return RenderUIBack(sr);
            case Sfx.Jump: return RenderJump(sr);
            case Sfx.JumpPad: return RenderJumpPad(sr);
            case Sfx.PowerGrow: return RenderPowerGrow(sr);
            case Sfx.PowerShrink: return RenderPowerShrink(sr);
            case Sfx.PowerPickup: return RenderPowerPickup(sr);
            case Sfx.DoorOpen: return RenderDoorOpen(sr);
            case Sfx.DoorClose: return RenderDoorClose(sr);
            case Sfx.ButtonPress: return RenderButtonPress(sr);
            case Sfx.ButtonRelease: return RenderButtonRelease(sr);
            case Sfx.SawHit: return RenderSawHit(sr);
            case Sfx.Win: return RenderWin(sr);
            case Sfx.Lose: return RenderLose(sr);
            case Sfx.DoorEnter: return RenderDoorEnter(sr);
            case Sfx.Fall: return RenderFall(sr);
            default: return new float[sr / 10];
        }
    }

    // Click de boton
    private static float[] RenderUIClick(int sr)
    {
        float[] d = new float[sr / 4];
        Tone(d, sr, 587.33f, 587.33f, 0.0, 0.09, 0.003f, 0.07f, 0f, 0.02f, 0.35f, Two);
        Tone(d, sr, 1174.66f, 1174.66f, 0.0, 0.05, 0.002f, 0.04f, 0f, 0.01f, 0.10f, Pure);
        return Finish(d);
    }

    // Botón atras / cerrar
    private static float[] RenderUIBack(int sr)
    {
        float[] d = new float[sr / 4];
        Tone(d, sr, 440f, 329.63f, 0.0, 0.11, 0.003f, 0.09f, 0f, 0.02f, 0.35f, Two);
        return Finish(d);
    }

    // Salto normal
    private static float[] RenderJump(int sr)
    {
        float[] d = new float[sr / 3];
        Tone(d, sr, 196f, 392f, 0.0, 0.16, 0.005f, 0.08f, 0.5f, 0.04f, 0.65f, Two);
        Tone(d, sr, 392f, 784f, 0.0, 0.16, 0.005f, 0.08f, 0.5f, 0.04f, 0.20f, Pure);
        return Finish(d);
    }

    // Trampolin
    private static float[] RenderJumpPad(int sr)
    {
        float[] d = new float[sr / 2 + sr / 4];
        Tone(d, sr, 130f, 520f, 0.0, 0.25, 0.004f, 0.20f, 0.2f, 0.05f, 0.35f, Saw5);
        Tone(d, sr, 260f, 1040f, 0.0, 0.25, 0.004f, 0.20f, 0.1f, 0.05f, 0.12f, Pure);
        Tone(d, sr, 783.99f, 783.99f, 0.18, 0.30, 0.005f, 0.25f, 0f, 0.05f, 0.12f, Two);
        Echo(d, 0.09, 0.25, sr);
        return Finish(d);
    }

    // Power-up "crecer"
    private static float[] RenderPowerGrow(int sr)
    {
        float[] d = new float[(int)(sr * 1.3f)];
        float[] notes = { 392.00f, 493.88f, 587.33f, 783.99f };
        for (int i = 0; i < notes.Length; i++)
            Tone(d, sr, notes[i], notes[i], i * 0.07, 0.35, 0.004f, 0.25f, 0.1f, 0.10f, 0.25f, Two);
        Tone(d, sr, 392f, 1568f, 0.0, 0.40, 0.01f, 0.30f, 0.1f, 0.10f, 0.07f, Pure);
        Echo(d, 0.14, 0.30, sr);
        return Finish(d);
    }

    // Power-up "encoger"
    private static float[] RenderPowerShrink(int sr)
    {
        float[] d = new float[(int)(sr * 1.1f)];
        float[] notes = { 783.99f, 587.33f, 493.88f, 392.00f };
        for (int i = 0; i < notes.Length; i++)
            Tone(d, sr, notes[i], notes[i], i * 0.065, 0.28, 0.004f, 0.20f, 0.05f, 0.08f, 0.25f, Pure);
        Tone(d, sr, 1568f, 392f, 0.0, 0.35, 0.01f, 0.25f, 0.1f, 0.10f, 0.07f, Pure);
        Echo(d, 0.12, 0.25, sr);
        return Finish(d);
    }

    // Power-ups
    private static float[] RenderPowerPickup(int sr)
    {
        float[] d = new float[sr];
        Tone(d, sr, 783.99f, 783.99f, 0.00, 0.20, 0.003f, 0.15f, 0.05f, 0.05f, 0.30f, Two);
        Tone(d, sr, 1174.66f, 1174.66f, 0.08, 0.40, 0.003f, 0.30f, 0.05f, 0.10f, 0.30f, Two);
        Echo(d, 0.10, 0.25, sr);
        return Finish(d);
    }

    // Puerta abriendose
    private static float[] RenderDoorOpen(int sr)
    {
        float[] d = new float[(int)(sr * 1.5f)];
        Tone(d, sr, 98f, 196f, 0.03, 0.17, 0.03f, 0.10f, 0.3f, 0.05f, 0.14f, Saw5);
        NoiseBurst(d, sr, 0.22, 0.30, 0.01f, 0.20f, 0.3f, 0.08f, 0.34f, 4500f, false, 12f, 0.7f);
        Tone(d, sr, 196f, 196f, 0.23, 0.24, 0.01f, 0.15f, 0.4f, 0.06f, 0.45f, Saw5);
        Tone(d, sr, 98f, 98f, 0.23, 0.22, 0.01f, 0.15f, 0.4f, 0.06f, 0.20f, Saw5);
        Tone(d, sr, 392f, 392f, 0.24, 0.16, 0.01f, 0.12f, 0.2f, 0.05f, 0.12f, Two);
        NoiseBurst(d, sr, 0.43, 0.07, 0.001f, 0.06f, 0f, 0.01f, 0.90f, 2500f, true);
        Tone(d, sr, 784f, 784f, 0.43, 0.12, 0.001f, 0.10f, 0f, 0.02f, 0.14f, Two);
        Tone(d, sr, 1568f, 1568f, 0.43, 0.14, 0.001f, 0.12f, 0f, 0.02f, 0.12f, Pure);
        NoiseBurst(d, sr, 0.46, 0.55, 0.02f, 0.40f, 0.2f, 0.15f, 0.025f, 1500f, false, 12f, 0.7f);
        Tone(d, sr, 196f, 165f, 0.46, 0.55, 0.02f, 0.40f, 0.25f, 0.15f, 0.14f, Saw5);
        NoiseBurst(d, sr, 0.73, 0.06, 0.001f, 0.05f, 0f, 0.01f, 0.30f, 1200f, true);
        Tone(d, sr, 392f, 392f, 0.73, 0.12, 0.001f, 0.10f, 0f, 0.02f, 0.12f, Two);
        NoiseBurst(d, sr, 0.88, 0.07, 0.001f, 0.06f, 0f, 0.01f, 0.30f, 1500f, true);
        Tone(d, sr, 392f, 392f, 0.88, 0.12, 0.001f, 0.10f, 0f, 0.02f, 0.12f, Two);
        Tone(d, sr, 784f, 784f, 0.88, 0.12, 0.001f, 0.10f, 0f, 0.02f, 0.08f, Pure);
        Tone(d, sr, 196f, 98f, 0.95, 0.40, 0.05f, 0.25f, 0.1f, 0.10f, 0.03f, Two);

        return Finish(d);
    }

    // Puerta cerrandose
    private static float[] RenderDoorClose(int sr)
    {
        float[] d = new float[(int)(sr * 1.2f)];
        NoiseBurst(d, sr, 0.0, 0.30, 0.02f, 0.20f, 0.3f, 0.08f, 0.30f, 4000f, false, 12f, 0.7f);
        Tone(d, sr, 196f, 165f, 0.0, 0.30, 0.02f, 0.20f, 0.3f, 0.08f, 0.40f, Saw5);
        Tone(d, sr, 98f, 82f, 0.0, 0.30, 0.02f, 0.20f, 0.3f, 0.08f, 0.16f, Saw5);
        Tone(d, sr, 196f, 98f, 0.32, 0.28, 0.003f, 0.22f, 0f, 0.05f, 0.40f, Saw5);
        NoiseBurst(d, sr, 0.32, 0.12, 0.002f, 0.10f, 0f, 0.02f, 0.30f, 1500f, false);
        NoiseBurst(d, sr, 0.32, 0.07, 0.001f, 0.06f, 0f, 0.01f, 0.80f, 2500f, true);
        Tone(d, sr, 784f, 784f, 0.32, 0.12, 0.001f, 0.10f, 0f, 0.02f, 0.14f, Two);
        Tone(d, sr, 1568f, 1568f, 0.32, 0.14, 0.001f, 0.12f, 0f, 0.02f, 0.12f, Pure);
        NoiseBurst(d, sr, 0.55, 0.06, 0.001f, 0.05f, 0f, 0.01f, 0.28f, 1200f, true);
        Tone(d, sr, 392f, 392f, 0.55, 0.12, 0.001f, 0.10f, 0f, 0.02f, 0.12f, Two);
        NoiseBurst(d, sr, 0.68, 0.07, 0.001f, 0.06f, 0f, 0.01f, 0.26f, 1500f, true);
        Tone(d, sr, 392f, 392f, 0.68, 0.12, 0.001f, 0.10f, 0f, 0.02f, 0.12f, Two);
        Tone(d, sr, 784f, 784f, 0.68, 0.12, 0.001f, 0.10f, 0f, 0.02f, 0.08f, Pure);
        Tone(d, sr, 196f, 98f, 0.60, 0.45, 0.05f, 0.25f, 0.1f, 0.10f, 0.03f, Two);

        return Finish(d);
    }

    // La pelota entra por la puerta
    private static float[] RenderDoorEnter(int sr)
    {
        float[] d = new float[(int)(sr * 0.7f)];

        // Glissando descendente Re6 -> Re4 (se va "tragando" la pelota)
        Tone(d, sr, 1174.66f, 293.66f, 0.0, 0.36, 0.005f, 0.28f, 0.2f, 0.06f, 0.35f, Two);
        Tone(d, sr, 2349.32f, 587.33f, 0.0, 0.30, 0.005f, 0.24f, 0.1f, 0.05f, 0.10f, Pure);

        // Aire / roce rapido
        NoiseBurst(d, sr, 0.0, 0.34, 0.02f, 0.25f, 0.1f, 0.07f, 0.20f, 2500f, true, 14f, 0.5f);

        // Golpecito suave al llegar al centro (Sol3 + Sol4)
        Tone(d, sr, 196f, 147f, 0.32, 0.22, 0.003f, 0.18f, 0f, 0.04f, 0.35f, Two);
        Tone(d, sr, 392f, 392f, 0.32, 0.18, 0.003f, 0.14f, 0f, 0.04f, 0.18f, Pure);
        Echo(d, 0.08, 0.20, sr);

        return Finish(d);
    }

    // Caida al vacio
    private static float[] RenderFall(int sr)
    {
        float[] d = new float[(int)(sr * 1.1f)];

        Tone(d, sr, 1568f, 196f, 0.0, 0.90, 0.01f, 0.30f, 0.60f, 0.15f, 0.65f, Two);
        Tone(d, sr, 1600f, 200f, 0.0, 0.90, 0.01f, 0.30f, 0.60f, 0.15f, 0.40f, Pure);
        NoiseBurst(d, sr, 0.0, 0.80, 0.02f, 0.30f, 0.30f, 0.15f, 0.15f, 2500f, true, 9f, 0.5f);

        Echo(d, 0.09, 0.20, sr);
        return Finish(d);
    }

    // Boton del piso al pisarlo
    private static float[] RenderButtonPress(int sr)
    {
        float[] d = new float[sr / 2];
        NoiseBurst(d, sr, 0.0, 0.03, 0.001f, 0.025f, 0f, 0.005f, 0.30f, 1800f, true);
        Tone(d, sr, 196f, 130f, 0.0, 0.12, 0.002f, 0.10f, 0f, 0.02f, 0.40f, Pure);
        Tone(d, sr, 392f, 392f, 0.02, 0.22, 0.003f, 0.18f, 0f, 0.04f, 0.12f, Two);
        return Finish(d);
    }

    // Boton al soltarlo
    private static float[] RenderButtonRelease(int sr)
    {
        float[] d = new float[sr / 3];
        NoiseBurst(d, sr, 0.0, 0.02, 0.001f, 0.015f, 0f, 0.005f, 0.18f, 2200f, true);
        Tone(d, sr, 293.66f, 220f, 0.0, 0.07, 0.002f, 0.06f, 0f, 0.01f, 0.22f, Pure);
        return Finish(d);
    }

    // Pelota contra la sierra
    private static float[] RenderSawHit(int sr)
    {
        float[] d = new float[(int)(sr * 1.1f)];
        NoiseBurst(d, sr, 0.0, 0.06, 0.001f, 0.05f, 0f, 0.01f, 0.40f, 400f, false);
        NoiseBurst(d, sr, 0.0, 0.30, 0.002f, 0.20f, 0.10f, 0.10f, 0.35f, 1500f, true, 85f, 0.7f);
        Tone(d, sr, 220f, 180f, 0.0, 0.22, 0.003f, 0.18f, 0.1f, 0.05f, 0.15f, Saw5);

        float[] partials = { 1830f, 2740f, 3910f, 5230f };
        float[] levels = { 0.12f, 0.10f, 0.08f, 0.05f };
        for (int i = 0; i < partials.Length; i++)
            Tone(d, sr, partials[i], partials[i], 0.0, 0.35, 0.001f, 0.30f, 0f, 0.06f, levels[i], Pure);
        NoiseBurst(d, sr, 0.12, 0.70, 0.002f, 0.45f, 0.05f, 0.20f, 0.55f, 1200f, false);
        Tone(d, sr, 196f, 49f, 0.12, 0.45, 0.002f, 0.35f, 0f, 0.08f, 0.45f, Saw5);
        NoiseBurst(d, sr, 0.12, 0.30, 0.001f, 0.20f, 0f, 0.08f, 0.25f, 3500f, true, 30f, 0.6f);

        Echo(d, 0.07, 0.20, sr);
        return Finish(d);
    }

    // Ganar
    private static float[] RenderWin(int sr)
    {
        float[] d = new float[sr * 3];

        float[] notes = { 392.00f, 493.88f, 587.33f, 783.99f };
        for (int i = 0; i < notes.Length; i++)
            Tone(d, sr, notes[i], notes[i], i * 0.12, 0.40, 0.004f, 0.30f, 0.1f, 0.10f, 0.25f, Two);

        float[] chord = { 392.00f, 493.88f, 587.33f, 783.99f, 987.77f };
        for (int i = 0; i < chord.Length; i++)
            Tone(d, sr, chord[i], chord[i], 0.60, 1.60, 0.02f, 0.60f, 0.30f, 0.60f, 0.14f, Soft3);

        Echo(d, 0.18, 0.35, sr);
        return Finish(d);
    }

    // Perder
    private static float[] RenderLose(int sr)
    {
        float[] d = new float[(int)(sr * 2.4f)];

        Tone(d, sr, 392.00f, 392.00f, 0.00, 0.32, 0.01f, 0.20f, 0.4f, 0.10f, 0.28f, Two);
        Tone(d, sr, 369.99f, 369.99f, 0.30, 0.32, 0.01f, 0.20f, 0.4f, 0.10f, 0.28f, Two);
        Tone(d, sr, 349.23f, 349.23f, 0.60, 0.32, 0.01f, 0.20f, 0.4f, 0.10f, 0.28f, Two);
        Tone(d, sr, 329.63f, 233.08f, 0.90, 1.10, 0.02f, 0.40f, 0.5f, 0.50f, 0.28f, Saw5);

        Echo(d, 0.16, 0.30, sr);
        return Finish(d);
    }

    // ------------------------------------------------------------------
    // Herramientas de sintesis
    // ------------------------------------------------------------------

    private static void Tone(float[] data, int sr, float f0, float f1, double start, double duration,
                             float attack, float decay, float sustain, float release,
                             float level, float[] harmonics)
    {
        int first = (int)(start * sr);
        int last = System.Math.Min(data.Length, (int)((start + duration) * sr));

        float norm = 0f;
        for (int h = 0; h < harmonics.Length; h++) norm += harmonics[h];
        if (norm <= 0f) norm = 1f;

        double ratio = f1 / (double)f0;
        double phase = 0.0;

        for (int i = first; i < last; i++)
        {
            double t = (i - first) / (double)sr;
            double k = t / duration;
            double freq = f0 * System.Math.Pow(ratio, k);

            phase += TwoPi * freq / sr;

            float env = Adsr(t, duration, attack, decay, sustain, release);

            float sample = 0f;
            for (int h = 0; h < harmonics.Length; h++)
                sample += harmonics[h] * (float)System.Math.Sin(phase * (h + 1));

            data[i] += level * env * sample / norm;
        }
    }

    // Rafaga de ruido blanco filtrado (pasa-bajos o pasa-altos), con modulacion de amplitud opcional
    private static void NoiseBurst(float[] data, int sr, double start, double duration,
                                   float attack, float decay, float sustain, float release,
                                   float level, float cutoff, bool highpass,
                                   float amRate = 0f, float amDepth = 0f)
    {
        int first = (int)(start * sr);
        int last = System.Math.Min(data.Length, (int)((start + duration) * sr));

        float alpha = 1f - (float)System.Math.Exp(-TwoPi * cutoff / sr);
        float gain = highpass ? 1f : Mathf.Min(6f, Mathf.Sqrt((2f - alpha) / alpha));
        float lp = 0f;

        for (int i = first; i < last; i++)
        {
            double t = (i - first) / (double)sr;

            float n = NextNoise();
            lp += alpha * (n - lp);
            float x = highpass ? (n - lp) : lp * gain;

            float env = Adsr(t, duration, attack, decay, sustain, release);

            float am = 1f;
            if (amRate > 0f)
                am = 1f - amDepth * (0.5f + 0.5f * (float)System.Math.Sin(TwoPi * amRate * t));

            data[i] += level * env * x * am;
        }
    }

    private static float Adsr(double t, double duration, float a, float d, float s, float r)
    {
        if (t < 0.0 || t >= duration) return 0f;

        double total = a + d + r;
        double k = total > duration ? duration / total : 1.0;
        double aa = a * k, dd = d * k, rr = r * k;

        if (t < aa) return aa > 0.0 ? (float)(t / aa) : 1f;
        t -= aa;

        if (t < dd) return Mathf.Lerp(1f, s, dd > 0.0 ? (float)(t / dd) : 1f);
        t -= dd;

        double sustainLength = duration - aa - dd - rr;
        if (t < sustainLength) return s;
        t -= sustainLength;

        return rr > 0.0 ? s * (1f - (float)(t / rr)) : 0f;
    }

    // Eco simple (retroalimentado) aplicado sobre el mismo buffer
    private static void Echo(float[] data, double delaySeconds, double feedback, int sr)
    {
        int delay = (int)(delaySeconds * sr);
        float fb = (float)feedback;
        for (int i = delay; i < data.Length; i++)
            data[i] += data[i - delay] * fb;
    }

    private static float NextNoise()
    {
        _noiseState ^= _noiseState << 13;
        _noiseState ^= _noiseState >> 17;
        _noiseState ^= _noiseState << 5;
        return (_noiseState / (float)uint.MaxValue) * 2f - 1f;
    }

    // Limitador suave para evitar saturacion 
    private static float[] Finish(float[] data)
    {
        for (int i = 0; i < data.Length; i++)
            data[i] = (float)System.Math.Tanh(data[i] * ClipGain);
        return data;
    }
}