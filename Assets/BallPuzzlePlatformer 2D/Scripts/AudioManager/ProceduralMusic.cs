
using UnityEngine;


public enum DivisionRitmica
{
    Redonda,       // 4 tiempos
    Blanca,        // 2 tiempos
    Negra,         // 1 tiempo
    Corchea,       // 0.5 tiempos
    Semicorchea    // 0.25 tiempos
}


public enum NotaMusical
{
    Do = 0,
    DoSostenido = 1,
    Re = 2,
    ReSostenido = 3,
    Mi = 4,
    Fa = 5,
    FaSostenido = 6,
    Sol = 7,
    SolSostenido = 8,
    La = 9,
    LaSostenido = 10,
    Si = 11
}



[System.Serializable]
public class SeccionRitmica
{
    [Header("Duración de la sección")]
    [Min(1)]
    public int compases = 1;

    [Header("Subdivisión")]
    public DivisionRitmica division = DivisionRitmica.Negra;

    [Header("Patrón")]
    public int[] patron = {0};
}


[System.Serializable]
public class EnvolventeInstrumento
{
    [Header("ADSR")]
    [Min(0f)] public float attackMs = 10f;
    [Min(0f)] public float decayMs = 50f;

    [Range(0f, 1f)]
    public float sustainLevel = 0.5f;

    [Min(0f)] public float sustainMs = 100f;
    [Min(0f)] public float releaseMs = 20f;
}

[System.Serializable]
public class ConfiguracionInstrumento
{
    [Header("Estado")]
    public bool activo = true;

    [Header("Afinación musical")]
    public NotaMusical nota = NotaMusical.La;

    [Range(0, 8)]
    public int octava = 4;

    public float frecuencia
    {
        get
        {
            int midi = (octava + 1) * 12 + (int)nota;

            return 440f * Mathf.Pow(
                2f,
                (midi - 69) / 12f
            );
        }
    }

    [Header("Segundo sonido opcional")]
    [Min(0f)] public float frecuenciaSecundaria = 0f;

    [Range(0f, 1f)]
    public float volumenSecundario = 0.1f;

    public EnvolventeInstrumento envolventeSecundaria =
        new EnvolventeInstrumento();

    [Range(0f, 1f)]
    public float volumen = 0.2f;

    [Range(0f, 1f)]
    public float nivelRuido = 0f;

    [Range(0f, 1f)]
    public float reverberacion = 0f;

    [Header("Envolvente")]
    public EnvolventeInstrumento envolvente =
        new EnvolventeInstrumento();

     [Header("Secuencia rítmica")]
    public SeccionRitmica[] seccionesRitmicas =
        new SeccionRitmica[]
        {
            new SeccionRitmica()
        };
}

[RequireComponent(typeof(AudioSource))]
public class ProceduralMusic : MonoBehaviour
{


    [Header("Metrónomo")]
public bool metronomoActivo = true;

[Range(0f, 1f)]
public float volumenMetronomo = 0.3f;

public float frecuenciaPulsoFuerte = 1500f;
public float frecuenciaPulsoDebil = 1000f;

private double tiempoUltimoPulso = -1;
private double faseMetronomo;



private float GenerarClick(
    double tiempo,
    float frecuencia,
    float duracion = 0.035f)
{
    if (tiempo < 0 || tiempo >= duracion)
        return 0f;

    float envolvente =
        1f - (float)(tiempo / duracion);

    envolvente *= envolvente;

    float sonido =
        (float)System.Math.Sin(faseMetronomo);

    faseMetronomo +=
        TwoPi * frecuencia / sampleRate;

    if (faseMetronomo > TwoPi)
        faseMetronomo -= TwoPi;

    return sonido * envolvente * volumenMetronomo;
}



    [Header("Wavetable")]
    public WavetableTxtLoader wavetableLoader;

    [Header("Música")]
    [Range(60f, 500f)]
    public float tempo = 160f;

    [Header("Compás")]
    [Range(2, 7)]
    public int tiemposPorCompas = 4;

    [Header("Instrumentos Samba")]

    public ConfiguracionInstrumento agogo =
        new ConfiguracionInstrumento
        {
            nota = (NotaMusical)9,
            octava = 4,
            volumen = 0.1f,

            envolvente = new EnvolventeInstrumento
            {
                attackMs = 20f,
                decayMs = 63f,
                sustainLevel = 0f,
                sustainMs = 160f,
                releaseMs = 0f
            },

            frecuenciaSecundaria = 2100f,
            volumenSecundario = 0.05f,

            envolventeSecundaria = new EnvolventeInstrumento
            {
                attackMs = 10f,
                decayMs = 80f,
                sustainLevel = 0.05f,
                sustainMs = 160f,
                releaseMs = 180f
            },

            seccionesRitmicas = new SeccionRitmica[]
            {
                // Primera sección: un compás de negras
                new SeccionRitmica
                {
                    compases = 1,
                    division = DivisionRitmica.Negra,
                    patron = new int[]
                    {
                        0,0,0
                    }
                }

            }
        };

    public ConfiguracionInstrumento surdo =
        new ConfiguracionInstrumento
        {
            nota = (NotaMusical)9,
            octava = 3,
            volumen = 0.5f,
            nivelRuido = 0.05f,
            reverberacion = 0.9f,

            envolvente = new EnvolventeInstrumento
            {
                attackMs = 10f,
                decayMs = 36f,
                sustainLevel = 0.2f,
                sustainMs = 210f,
                releaseMs = 0f
            },

            seccionesRitmicas = new SeccionRitmica[]
            {
                // Primera sección: un compás de negras
                new SeccionRitmica
                {
                    compases = 1,
                    division = DivisionRitmica.Negra,
                    patron = new int[]
                    {
                       0,0,0
                    }
                }

            }
        };

    public ConfiguracionInstrumento repique =
        new ConfiguracionInstrumento
        {
            nota = (NotaMusical)9,
            octava = 4,
            volumen = 0.7f,
            nivelRuido = 0.05f,
            reverberacion = 0.4f,

            envolvente = new EnvolventeInstrumento
            {
                attackMs = 4f,
                decayMs = 94f,
                sustainLevel = 0.1f,
                sustainMs = 70f,
                releaseMs = 34f
            },

            seccionesRitmicas = new SeccionRitmica[]
            {
                // Primera sección: un compás de negras
                new SeccionRitmica
                {
                    compases = 1,
                    division = DivisionRitmica.Negra,
                    patron = new int[]
                    {
                        0,0,0
                    }
                }

            }
        };

    public ConfiguracionInstrumento tamborim =
        new ConfiguracionInstrumento
        {
            nota = (NotaMusical)6,
            octava = 4,
            volumen = 0.2f,
            nivelRuido = 0.05f,
            reverberacion = 0.4f,

            envolvente = new EnvolventeInstrumento
            {
                attackMs = 4f,
                decayMs = 94f,
                sustainLevel = 0.1f,
                sustainMs = 70f,
                releaseMs = 34f
            },

            seccionesRitmicas = new SeccionRitmica[]
            {
                // Primera sección: un compás de negras
                new SeccionRitmica
                {
                    compases = 1,
                    division = DivisionRitmica.Negra,
                    patron = new int[]
                    {
                        0,0,0
                    }
                }

            }
        };

    public ConfiguracionInstrumento cuica =
        new ConfiguracionInstrumento
        {
            nota = NotaMusical.La,
            octava = 5,
            volumen = 0.2f,
            reverberacion = 0.4f,

            envolvente = new EnvolventeInstrumento
            {
                attackMs = 75f,
                decayMs = 51f,
                sustainLevel = 0.5f,
                sustainMs = 0f,
                releaseMs = 77f
            },

            seccionesRitmicas = new SeccionRitmica[]
            {
                // Primera sección: un compás de negras
                new SeccionRitmica
                {
                    compases = 1,
                    division = DivisionRitmica.Negra,
                    patron = new int[]
                    {
                        0,0,0
                    }
                }

            }
        };

    public ConfiguracionInstrumento silbato =
        new ConfiguracionInstrumento
        {
            nota = (NotaMusical)11,
            octava = 6,
            volumen = 0.2f,

            envolvente = new EnvolventeInstrumento
            {
                attackMs = 11f,
                decayMs = 4f,
                sustainLevel = 0.6f,
                sustainMs = 151f,
                releaseMs = 91f
            },

            seccionesRitmicas = new SeccionRitmica[]
            {
                // Primera sección: un compás de negras
                new SeccionRitmica
                {
                    compases = 1,
                    division = DivisionRitmica.Negra,
                    patron = new int[]
                    {
                        0,0,0
                    }
                }

            }
        };

    public ConfiguracionInstrumento caixa =
        new ConfiguracionInstrumento
        {
            nota = (NotaMusical)4,
            octava = 4,
            volumen = 0.2f,
            nivelRuido = 0.02f,
            reverberacion = 0.4f,

            envolvente = new EnvolventeInstrumento
            {
                attackMs = 2f,
                decayMs = 60f,
                sustainLevel = 0f,
                sustainMs = 190f,
                releaseMs = 0f
            },

            seccionesRitmicas = new SeccionRitmica[]
            {
                // Primera sección: un compás de negras
                new SeccionRitmica
                {
                    compases = 1,
                    division = DivisionRitmica.Negra,
                    patron = new int[]
                    {
                        0,0,0
                    }
                }

            }
        };

    public ConfiguracionInstrumento chocalho =
        new ConfiguracionInstrumento
        {
            nota = (NotaMusical)11,
            octava = 6,
            volumen = 1f,
            nivelRuido = 1f,
            reverberacion = 0.2f,

            envolvente = new EnvolventeInstrumento
            {
                attackMs = 5f,
                decayMs = 43f,
                sustainLevel = 0.1f,
                sustainMs = 18f,
                releaseMs = 20f
            },

            seccionesRitmicas = new SeccionRitmica[]
            {
                // Primera sección: un compás de negras
                new SeccionRitmica
                {
                    compases = 1,
                    division = DivisionRitmica.Negra,
                    patron = new int[]
                    {
                        0,0,0
                    }
                }

            }
        };

    // INTRO

    [Header("Intro")]
    [Range(0f, 10f)]
    public float introSeconds = 2.5f;

    [Range(0f, 2f)]
    public float introVolume = 1.2f;

    [Range(0f, 2f)]
    public float introReverb = 0.4f;

    [Range(20f, 2000f)]
    public float introBaseFrequency = 440f;

    [Range(0f, 20f)]
    public float introFlutterRate = 5f;

    [Range(0f, 1f)]
    public float introFlutterDepth = 0.05f;

    public bool useSongStructure = true;

    // WAVETABLE

    [Header("Forma de onda")]
    [Range(0, 3)]
    public int wavetableIndex = 0;

    [Range(1, 10)]
    public int harmonicCount = 6;

    [Range(0f, 1f)]
    public float[] harmonicLevels =
    {
        1f, 0.5f, 0.25f, 0.15f, 0.1f,
        0.08f, 0.05f, 0.04f, 0.03f, 0.02f
    };

    // PERCUSIÓN GENERAL

    [Header("Percusión general")]
    [Range(0f, 1f)]
    public float kickVolume = 0.30f;

    [Range(0f, 1f)]
    public float hatVolume = 0.035f;


    // ARMÓNICOS DE LA CAIXA

    private static readonly float[] CaixaHarmonics =
    {
        1.000000f,
        0.161716f,
        0.148355f,
        0.129068f,
        0.166834f,
        0.132854f,
        0.111406f,
        0.114621f,
        0.057086f,
        0.118569f
    };

    // CONFIGURACIÓN DE LA INTRO

    private const float IntroTempo = 140f;
    private const int IntroNotesPerBeat = 4;

    private const double IntroGridStart = -0.0741;
    private const float IntroScale = 1.5f / 4000f;
    private const double IntroFadeOut = 0.40;
    private const double IntroTail = 8.0;

    private static readonly byte[][] IntroLevels =
    {
        new byte[] { 10, 62, 32, 20, 30, 64, 87, 100, 88, 47, 21, 16, 32, 59, 59, 40, 20, 7, 3, 9, 26, 49, 50, 42, 32, 17, 9, 10, 22, 39, 40, 33, 23, 11, 5, 6, 13, 20, 18, 12 },

        new byte[] { 18, 97, 37, 20, 17, 28, 55, 66, 64, 74, 38, 22, 12, 10, 24, 39, 50, 61, 33, 21, 17, 25, 44, 45, 39, 39, 17, 11, 8, 10, 17, 17, 14, 13, 5, 3, 3, 4, 6, 7 },

        new byte[] { 22, 39, 53, 39, 22, 10, 19, 61, 118, 124, 125, 69, 34, 21, 8, 14, 18, 15, 21, 16, 8, 5, 6, 11, 15, 14, 15, 9, 5, 3, 3, 4, 5, 5, 5, 3, 1, 1, 0, 0 },

        new byte[] { 32, 78, 98, 122, 92, 31, 18, 12, 16, 35, 26, 31, 41, 21, 6, 7, 6, 3, 2, 6, 5, 2, 1, 1, 1, 2, 2, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0 },

        new byte[] { 20, 35, 45, 36, 58, 48, 11, 8, 12, 13, 10, 15, 17, 10, 3, 2, 1, 2, 3, 2, 2, 2, 1, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },

        new byte[] { 8, 18, 30, 39, 25, 12, 14, 8, 3, 3, 3, 4, 4, 3, 3, 1, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },

        new byte[] { 6, 20, 18, 15, 10, 7, 6, 10, 4, 4, 3, 2, 2, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },

        new byte[] { 4, 28, 6, 10, 4, 1, 2, 3, 3, 6, 3, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
    };

    private const int IntroSteps = 40;

    private static readonly float[] IntroGate =
    {
        1.09f, 1.46f, 1.73f, 1.71f,
        1.40f, 1.07f, 1.02f, 1.07f,
        0.87f, 0.49f, 0.37f, 0.40f,
        0.58f, 0.88f, 0.95f, 0.91f
    };

    // REVERBERACIÓN

    private static readonly int[] CombBase =
    {
        1116, 1188, 1277, 1356
    };

    private static readonly int[] AllpassBase =
    {
        556, 441
    };

    private const float CombFeedback = 0.90f;
    private const float CombDamp = 0.3f;

    // CONSTANTES GENERALES

    private const int TableSize = 2048;
    private const int TableMask = TableSize - 1;
    private const double TwoPi = 2.0 * System.Math.PI;

    // ESTADO GENERAL

    private float sampleRate;

    private double elapsedTime;
    private double beatPos;
    private double introElapsed;

    private uint noiseState = 22222;

    // FASES DE LOS INSTRUMENTOS

    private double agogoPhase;
    private double agogoSecondPhase;

    private double surdoPhase;
    private double repiquePhase;
    private double tamborimPhase;
    private double cuicaPhase;
    private double silbatoPhase;
    private double silbatoLfoPhase;

    private double silbatoVibratoPhase;
private double silbatoMultiplyPhase;


    private double caixaPhase;
    private double chocalhoPhase;


    // Pequeños choques del chocalho
private const int ChocalhoGrainCount = 20;

private float[] chocalhoGrainDelay =
    new float[ChocalhoGrainCount];

private float[] chocalhoGrainDuration =
    new float[ChocalhoGrainCount];

private float[] chocalhoGrainAmplitude =
    new float[ChocalhoGrainCount];

    // ESTADO DE REVERBERACIÓN

    private float[][] combBuf;
    private float[][] apBuf;

    private float[] combFilt;
    private int[] combIdx;
    private int[] apIdx;

    // WAVETABLE

    private int loadedWavetableIndex = -1;

    private float[] harmonicCos = new float[10];
    private float[] harmonicSin = new float[10];

    private volatile float[] waveTable;

    private int wavetableVersion;
    private int lastBuildHash = int.MinValue;

    // INICIALIZACIÓN

    private void Start()
    {
        sampleRate = AudioSettings.outputSampleRate;

        InitReverb();

        AudioSource source = GetComponent<AudioSource>();

        source.clip = null;
        source.loop = false;
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.volume = 1f;

        if (wavetableLoader == null)
            wavetableLoader = GetComponent<WavetableTxtLoader>();

        if (wavetableLoader != null)
        {
            wavetableLoader.LoadWavetable(wavetableIndex);
            AnalyzeWavetable();
        }
        else
        {
            harmonicSin[0] = 1f;
            wavetableVersion++;
        }

        loadedWavetableIndex = wavetableIndex;

        RebuildTableIfNeeded();

        source.Play();
    }

    // INICIALIZAR REVERBERACIÓN

    private void InitReverb()
    {
        float scale = sampleRate / 44100f;

        combBuf = new float[CombBase.Length][];
        combIdx = new int[CombBase.Length];
        combFilt = new float[CombBase.Length];

        for (int c = 0; c < CombBase.Length; c++)
        {
            combBuf[c] = new float[
                Mathf.Max(8, (int)(CombBase[c] * scale))
            ];
        }

        apBuf = new float[AllpassBase.Length][];
        apIdx = new int[AllpassBase.Length];

        for (int a = 0; a < AllpassBase.Length; a++)
        {
            apBuf[a] = new float[
                Mathf.Max(8, (int)(AllpassBase[a] * scale))
            ];
        }
    }

    // PROCESAMIENTO DE REVERBERACIÓN

    private float ProcessReverb(float input)
    {
        float x = input * 0.15f;
        float sum = 0f;

        for (int c = 0; c < combBuf.Length; c++)
        {
            float[] buf = combBuf[c];

            int idx = combIdx[c];
            float y = buf[idx];

            combFilt[c] =
                y * (1f - CombDamp) +
                combFilt[c] * CombDamp;

            buf[idx] = x + combFilt[c] * CombFeedback;

            if (++idx >= buf.Length)
                idx = 0;

            combIdx[c] = idx;
            sum += y;
        }

        sum *= 0.25f;

        for (int a = 0; a < apBuf.Length; a++)
        {
            float[] buf = apBuf[a];

            int idx = apIdx[a];
            float b = buf[idx];

            float o = -sum + b;

            buf[idx] = sum + b * 0.5f;

            if (++idx >= buf.Length)
                idx = 0;

            apIdx[a] = idx;
            sum = o;
        }

        return sum;
    }

    // ACTUALIZACIÓN

    private void Update()
    {
        if (wavetableLoader != null &&
            wavetableIndex != loadedWavetableIndex)
        {
            wavetableLoader.LoadWavetable(wavetableIndex);

            AnalyzeWavetable();

            loadedWavetableIndex = wavetableIndex;
        }

        RebuildTableIfNeeded();
    }

    // ANALIZAR WAVETABLE

    private void AnalyzeWavetable()
    {
        float[] samples = wavetableLoader.samples;

        if (samples == null || samples.Length < 2)
            return;

        int count = samples.Length;
        int maxHarmonics = Mathf.Min(10, count / 2);

        float[] newCos = new float[10];
        float[] newSin = new float[10];

        for (int h = 1; h <= maxHarmonics; h++)
        {
            double a = 0.0;
            double b = 0.0;

            for (int n = 0; n < count; n++)
            {
                double angle = TwoPi * h * n / count;

                a += samples[n] * System.Math.Cos(angle);
                b += samples[n] * System.Math.Sin(angle);
            }

            newCos[h - 1] = (float)(2.0 * a / count);
            newSin[h - 1] = (float)(2.0 * b / count);
        }

        harmonicCos = newCos;
        harmonicSin = newSin;

        wavetableVersion++;
    }

    // CONSTRUIR WAVETABLE

    private void RebuildTableIfNeeded()
    {
        int hash = wavetableVersion * 7919 + harmonicCount;

        if (harmonicLevels != null)
        {
            for (int i = 0; i < harmonicLevels.Length; i++)
            {
                hash = hash * 31 +
                    (int)(harmonicLevels[i] * 1000f);
            }
        }

        if (hash == lastBuildHash && waveTable != null)
            return;

        lastBuildHash = hash;

        int count = Mathf.Clamp(harmonicCount, 1, 10);

        float[] t = new float[TableSize];
        float peak = 0f;

        for (int i = 0; i < TableSize; i++)
        {
            double theta = TwoPi * i / TableSize;
            double s = 0.0;

            for (int h = 0; h < count; h++)
            {
                float level =
                    harmonicLevels != null &&
                    h < harmonicLevels.Length
                    ? Mathf.Clamp01(harmonicLevels[h])
                    : 1f;

                double a = theta * (h + 1);

                s += (
                    harmonicCos[h] * System.Math.Cos(a) +
                    harmonicSin[h] * System.Math.Sin(a)
                ) * level;
            }

            t[i] = (float)s;

            peak = Mathf.Max(peak, Mathf.Abs(t[i]));
        }

        if (peak < 1e-4f)
        {
            for (int i = 0; i < TableSize; i++)
            {
                t[i] = (float)System.Math.Sin(
                    TwoPi * i / TableSize
                );
            }
        }
        else
        {
            float inv = 1f / peak;

            for (int i = 0; i < TableSize; i++)
                t[i] *= inv;
        }

        waveTable = t;
    }

    // OSCILADOR

    private static float Osc(double phase, float[] table)
    {
        if (table == null)
            return (float)System.Math.Sin(phase);

        double pos = phase * (TableSize / TwoPi);

        int i0 = (int)pos;

        float frac = (float)(pos - i0);

        i0 &= TableMask;

        return table[i0] * (1f - frac) +
               table[(i0 + 1) & TableMask] * frac;
    }

    // ENVOLVENTE ADSR DE LOS INSTRUMENTOS

    private static float AdsrInstrumento(
        double t,
        EnvolventeInstrumento env)
    {
        if (env == null || t < 0.0)
            return 0f;

        double a = env.attackMs / 1000.0;
        double d = env.decayMs / 1000.0;
        double s = env.sustainMs / 1000.0;
        double r = env.releaseMs / 1000.0;

        double total = a + d + s + r;

        if (t >= total)
            return 0f;

        // Ataque

        if (t < a)
            return a > 0.0 ? (float)(t / a) : 1f;

        t -= a;

        // Decaimiento

        if (t < d)
        {
            float progress =
                d > 0.0 ? (float)(t / d) : 1f;

            return Mathf.Lerp(
                1f,
                env.sustainLevel,
                progress
            );
        }

        t -= d;

        // Sostenimiento

        if (t < s)
            return env.sustainLevel;

        t -= s;

        // Liberación

        if (t < r)
        {
            float progress =
                r > 0.0 ? (float)(t / r) : 1f;

            return env.sustainLevel * (1f - progress);
        }

        return 0f;
    }

    // GENERADOR DE RUIDO BLANCO

    private float Noise()
    {
        noiseState ^= noiseState << 13;
        noiseState ^= noiseState >> 17;
        noiseState ^= noiseState << 5;

        return (noiseState / (float)uint.MaxValue) * 2f - 1f;
    }



private void GenerarChoquesChocalho()
{
    for (int i = 0; i < ChocalhoGrainCount; i++)
    {
        // Aparición aleatoria entre 0 y 45 milisegundos
        chocalhoGrainDelay[i] =
            (Noise() * 0.5f + 0.5f) * 0.045f;

        // Duración aleatoria entre 2 y 8 milisegundos
        chocalhoGrainDuration[i] =
            0.002f + (Noise() * 0.5f + 0.5f) * 0.006f;

        // Intensidad aleatoria de cada choque
        chocalhoGrainAmplitude[i] =
            0.3f + (Noise() * 0.5f + 0.5f) * 0.7f;
    }
}
    // MOTOR DE AUDIO


   private static float ObtenerDuracionSubdivision(
    DivisionRitmica division)
    {
        switch (division)
        {
            case DivisionRitmica.Redonda:
                return 4f;
            case DivisionRitmica.Blanca:
                return 2f;
            case DivisionRitmica.Negra:
                return 1f;
            case DivisionRitmica.Corchea:
                return 0.5f;
            case DivisionRitmica.Semicorchea:
                return 0.25f;
            default:
                return 1f;
        }
    }


    private bool ObtenerEventoRitmico(
    ConfiguracionInstrumento instrumento,
    double posicionBeat,
    double beatDuration,
    out double tiempoEvento)
    {
        tiempoEvento = 0.0;

        if (instrumento == null ||
            instrumento.seccionesRitmicas == null ||
            instrumento.seccionesRitmicas.Length == 0)
            return false;

        double beatLocal = posicionBeat;

        foreach (SeccionRitmica seccion in instrumento.seccionesRitmicas)
        {
            if (seccion == null)
                continue;

            double duracionSeccion =
                Mathf.Max(1, seccion.compases) * tiemposPorCompas;

            if (beatLocal < duracionSeccion)
            {
                if (seccion.patron == null ||
                    seccion.patron.Length == 0)
                    return false;

                double subdivision =
                    ObtenerDuracionSubdivision(seccion.division);

                double posicionSubdivision =
                    beatLocal / subdivision;

                int indice =
                    (int)System.Math.Floor(posicionSubdivision);

                int indicePatron =
                    indice % seccion.patron.Length;

                if (seccion.patron[indicePatron] == 0)
                    return false;

                double fraccion =
                    posicionSubdivision -
                    System.Math.Floor(posicionSubdivision);

                tiempoEvento =
                    fraccion * subdivision * beatDuration;

                return true;
            }

            beatLocal -= duracionSeccion;
        }

        // Repetir la secuencia desde el principio
        return ObtenerEventoRitmico(
            instrumento,
            beatLocal,
            beatDuration,
            out tiempoEvento
        );
    }


    // Aplica una configuración seleccionada por MusicDirector.
    // Los parámetros tímbricos de cada instrumento se conservan; solo cambia
    // tempo, compás y secuencias rítmicas.
    public void AplicarConfiguracionMusical(ConfiguracionMusical config)
    {
        if (config == null) return;

        tempo = Mathf.Clamp(config.bpm, 60f, 500f);
        tiemposPorCompas = Mathf.Clamp(config.tiemposPorCompas, 2, 7);

        CopiarPatrones(config.agogo, agogo);
        CopiarPatrones(config.surdo, surdo);
        CopiarPatrones(config.repique, repique);
        CopiarPatrones(config.tamborim, tamborim);
        CopiarPatrones(config.cuica, cuica);
        CopiarPatrones(config.silbato, silbato);
        CopiarPatrones(config.caixa, caixa);
        CopiarPatrones(config.chocalho, chocalho);

        // La introducción instrumental está incorporada como primera sección
        // de cada instrumento. Desactivamos el efecto de intro pregrabado.
        useSongStructure = false;
        introSeconds = 0f;
        beatPos = 0.0;
        introElapsed = 0.0;
        elapsedTime = 0.0;
        tiempoUltimoPulso = -1;
    }

    private void CopiarPatrones(PatronInstrumento origen,
                                ConfiguracionInstrumento destino)
    {
        if (origen == null || destino == null) return;
        destino.activo = origen.activo;

        if (origen.secciones == null || origen.secciones.Length == 0)
        {
            destino.seccionesRitmicas = new SeccionRitmica[0];
            return;
        }

        SeccionRitmica[] copia = new SeccionRitmica[origen.secciones.Length];
        for (int i = 0; i < origen.secciones.Length; i++)
        {
            SeccionRitmica s = origen.secciones[i];
            if (s == null) continue;
            copia[i] = new SeccionRitmica
            {
                compases = Mathf.Max(1, s.compases),
                division = s.division,
                patron = s.patron != null ? (int[])s.patron.Clone() : new int[0]
            };
        }
        destino.seccionesRitmicas = copia;
    }

    private void OnValidate()
{
    AjustarPatrones(agogo);
    AjustarPatrones(surdo);
    AjustarPatrones(repique);
    AjustarPatrones(tamborim);
    AjustarPatrones(cuica);
    AjustarPatrones(silbato);
    AjustarPatrones(caixa);
    AjustarPatrones(chocalho);
}

private void AjustarPatrones(
    ConfiguracionInstrumento instrumento)
{
    if (instrumento == null ||
        instrumento.seccionesRitmicas == null)
        return;

    foreach (SeccionRitmica seccion
        in instrumento.seccionesRitmicas)
    {
        if (seccion == null)
            continue;

        // Duración de cada posición rítmica
        float duracionSubdivision =
            ObtenerDuracionSubdivision(seccion.division);

        // Duración total de la sección en tiempos
        float duracionTotal =
            Mathf.Max(1, seccion.compases) * tiemposPorCompas;

        // Cantidad de posiciones necesarias
        int cantidad =
            Mathf.CeilToInt(
                duracionTotal / duracionSubdivision
            );

        // Crear un nuevo patrón del tamaño correcto
        int[] nuevoPatron = new int[cantidad];

        // Conservar los valores que ya existían
        if (seccion.patron != null)
        {
            int limite = Mathf.Min(
                seccion.patron.Length,
                cantidad
            );

            for (int i = 0; i < limite; i++)
            {
                nuevoPatron[i] =
                    Mathf.Clamp(seccion.patron[i], 0, 1);
            }
        }

        // Los espacios nuevos quedan en cero
        seccion.patron = nuevoPatron;
    }
}

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (sampleRate <= 0f || channels <= 0 || combBuf == null)
            return;

        float[] table = waveTable;

        double beatDuration =
            60.0 / Mathf.Clamp(tempo, 60f, 500f);

        double dBeat = 1.0 / (sampleRate * beatDuration);
        double dt = 1.0 / sampleRate;

        bool structure = useSongStructure;

        double introSecs = structure
            ? Mathf.Max(0f, introSeconds)
            : 0.0;

        float introVol = Mathf.Clamp(introVolume, 0f, 2f);
        float introRev = Mathf.Max(0f, introReverb);

        for (int i = 0; i < data.Length; i += channels)
        {
            // INTRO

            bool inIntro = structure && introElapsed < introSecs;

            double beatTime =
                (beatPos - System.Math.Floor(beatPos))
                * beatDuration;


float metronomoSound = 0f;

if (metronomoActivo && !inIntro)
{
    double beatActual = beatPos;

    int pulso = (int)System.Math.Floor(beatActual);

    if (pulso != tiempoUltimoPulso)
    {
        tiempoUltimoPulso = pulso;

        float frecuencia =
            pulso % tiemposPorCompas == 0
            ? frecuenciaPulsoFuerte
            : frecuenciaPulsoDebil;

        metronomoSound = GenerarClick(
            0,
            frecuencia
        );
    }
}


            // AGOGO

            float agogoSound = 0f;

            if (agogo.activo && !inIntro &&
                ObtenerEventoRitmico(
                    agogo,
                    beatPos,
                    beatDuration,
                    out double tiempoAgogo))
            {
                float envPrincipal = AdsrInstrumento(
                    tiempoAgogo,
                    agogo.envolvente
                );

                float envSecundaria = AdsrInstrumento(
                    tiempoAgogo,
                    agogo.envolventeSecundaria
                );

                float principal =
                    Osc(agogoPhase, table) *
                    agogo.volumen *
                    envPrincipal;

                float secundario =
                    Osc(agogoSecondPhase, table) *
                    agogo.volumenSecundario *
                    envSecundaria;

                agogoSound = principal + secundario;
            }

            // SURDO

            float surdoSound = 0f;

            if (surdo.activo && !inIntro)
            {
                double tiempoSurdo;

                if (ObtenerEventoRitmico(
                    surdo,
                    beatPos,
                    beatDuration,
                    out tiempoSurdo))
                {
                    float env = AdsrInstrumento(
                        (float)tiempoSurdo,
                        surdo.envolvente
                    );

                    float tono =
                        (float)System.Math.Sin(surdoPhase) *
                        surdo.volumen *
                        env;

                    float dry = tono;
                    float wet = ProcessReverb(dry);

                    surdoSound = dry +
                        wet * surdo.reverberacion;
                }
            }

            // REPIQUE

            float repiqueSound = 0f;

            if (repique.activo && !inIntro)
            {
                double tiempoRepique;

                if (ObtenerEventoRitmico(
                    repique,
                    beatPos,
                    beatDuration,
                    out tiempoRepique))
                {
                    float env = AdsrInstrumento(
                        (float)tiempoRepique,
                        repique.envolvente
                    );

                    float tono =
                        (float)System.Math.Sin(repiquePhase) *
                        repique.volumen *
                        env;

                    float ruido =
                        Noise() *
                        repique.nivelRuido *
                        env;

                    float dry = tono + ruido;
                    float wet = ProcessReverb(dry);

                    repiqueSound = dry +
                        wet * repique.reverberacion;
                }
            }

            // TAMBORIM

            float tamborimSound = 0f;

            if (tamborim.activo && !inIntro)
            {
                double tiempoTamborim;

                if (ObtenerEventoRitmico(
                    tamborim,
                    beatPos,
                    beatDuration,
                    out tiempoTamborim))
                {
                    float env = AdsrInstrumento(
                        (float)tiempoTamborim,
                        tamborim.envolvente
                    );

                    float tono =
                        (float)System.Math.Sin(tamborimPhase) *
                        tamborim.volumen *
                        env;

                    float ruido =
                        Noise() *
                        tamborim.nivelRuido *
                        env;

                    float dry = tono + ruido;
                    float wet = ProcessReverb(dry);

                    tamborimSound = dry +
                        wet * tamborim.reverberacion;
                }
            }

            // CUICA

            float cuicaSound = 0f;
        
float frecuenciaCuicaActual = cuica.frecuencia;

            if (cuica.activo && !inIntro)
            {
                double tiempoCuica;

                if (ObtenerEventoRitmico(
                    cuica,
                    beatPos,
                    beatDuration,
                    out tiempoCuica))
                {
                    float env = AdsrInstrumento(
                        (float)tiempoCuica,
                        cuica.envolvente
                    );

                    float rangoNotas = 12f; // Una octava
                    float velocidad = 5f;

                    // Oscila entre 0 y 12 semitonos
                    float desplazamiento =
                        (Mathf.Sin((float)(tiempoCuica * velocidad * TwoPi)) + 1f)
                        * 0.5f * rangoNotas;

                    // Convierte semitonos a frecuencia
                    frecuenciaCuicaActual =
                        cuica.frecuencia * Mathf.Pow(2f, desplazamiento / 12f);

                    float tono =
                        (float)System.Math.Sin(cuicaPhase) *
                        cuica.volumen *
                        env;

                    float wet = ProcessReverb(tono);

                    cuicaSound = tono +
                        wet * cuica.reverberacion;
                }
            }   

            // SILBATO

            // SILBATO

            float silbatoSound = 0f;

            if (silbato.activo && !inIntro)
            {
                double tiempoSilbato;

                if (ObtenerEventoRitmico(
                    silbato,
                    beatPos,
                    beatDuration,
                    out tiempoSilbato))
                {
                    float env = AdsrInstrumento(
                        (float)tiempoSilbato,
                        silbato.envolvente
                    );

                
                    // LFO 2: VIBRATO - máxima profundidad
                    float vibrato =
                        (float)System.Math.Sin(silbatoVibratoPhase);

                    float profundidadVibrato = 1f;

                    float frecuenciaActual =
                        silbato.frecuencia *
                        (1f + vibrato * profundidadVibrato);

                    // LFO 3: MULTIPLY - máxima intensidad
                    float multiply =
                        1f *
                        (float)System.Math.Sin(silbatoMultiplyPhase);

                    // Oscilador con frecuencia modulada
                    float tono =
                        (float)System.Math.Sin(silbatoPhase) *
                        silbato.volumen *
                        env;

                    silbatoSound =
                        tono *
                        multiply;
                }
            }

            // CAIXA

            float caixaSound = 0f;

            if (caixa.activo && !inIntro)
            {
                double tiempoCaixa;

                if (ObtenerEventoRitmico(
                    caixa,
                    beatPos,
                    beatDuration,
                    out tiempoCaixa))
                {
                    float env = AdsrInstrumento(
                        (float)tiempoCaixa,
                        caixa.envolvente
                    );

                    float tono = 0f;
                    float sumaArmonicos = 0f;

                    for (int h = 0; h < CaixaHarmonics.Length; h++)
                    {
                        int numeroArmonico = h + 1;

                        tono += CaixaHarmonics[h] *
                            (float)System.Math.Sin(
                                caixaPhase * numeroArmonico
                            );

                        sumaArmonicos += CaixaHarmonics[h];
                    }

                    tono /= sumaArmonicos;
                    tono *= caixa.volumen * env;

                    float ruido =
                        Noise() *
                        caixa.nivelRuido *
                        env;

                    float dry = tono + ruido;
                    float wet = ProcessReverb(dry);

                    caixaSound = dry +
                        wet * caixa.reverberacion;
                }
            }

            // CHOCALHO

            // CHOCALHO
            float chocalhoSound = 0f;

            if (chocalho.activo && !inIntro)
            {
                double tiempoChocalho;

                if (ObtenerEventoRitmico(
                    chocalho,
                    beatPos,
                    beatDuration,
                    out tiempoChocalho))
                {
                    // Generar las partículas al comenzar cada golpe
                    if (tiempoChocalho < dt * 1.5)
                    {
                        GenerarChoquesChocalho();
                    }

                    float env = AdsrInstrumento(
                        (float)tiempoChocalho,
                        chocalho.envolvente
                    );

                    // Mezclar los pequeños choques
                    float ruidoParticulas = 0f;

                    for (int p = 0; p < ChocalhoGrainCount; p++)
                    {
                        // Esperar hasta que aparezca la partícula
                        if (chocalhoGrainDelay[p] > 0f)
                        {
                            chocalhoGrainDelay[p] -= (float)dt;
                        }
                        else if (chocalhoGrainDuration[p] > 0f)
                        {
                            float envParticula =
                                chocalhoGrainDuration[p] / 0.008f;

                            ruidoParticulas +=
                                Noise() *
                                chocalhoGrainAmplitude[p] *
                                envParticula;

                            chocalhoGrainDuration[p] -= (float)dt;
                        }
                    }

                    float variacion =
                        0.7f + 0.3f *
                        (float)System.Math.Sin(chocalhoPhase);

                    float dry =
                        ruidoParticulas *
                        0.25f *
                        chocalho.nivelRuido *
                        chocalho.volumen *
                        env *
                        variacion;

                    float wet = ProcessReverb(dry);

                    chocalhoSound = dry +
                        wet * chocalho.reverberacion;
                }
            }

            
            // INTRO MUSICAL

            float introOut = 0f;

            if (structure && introElapsed < introSecs + IntroTail)
            {
                double ti = introElapsed;

                float bells = 0f;

                if (ti < introSecs)
                {
                    double stepDur =
                        60.0 / IntroTempo / IntroNotesPerBeat;

                    double pos = (ti - IntroGridStart) / stepDur;
                    double cen = pos - 0.5;

                    int i0 = (int)System.Math.Floor(cen);

                    float fr = (float)(cen - i0);

                    int ia = Mathf.Clamp(i0, 0, IntroSteps - 1);
                    int ib = Mathf.Clamp(i0 + 1, 0, IntroSteps - 1);

                    int gl = IntroGate.Length;

                    double gp =
                        (pos - System.Math.Floor(pos)) * gl - 0.5;

                    int gi = (int)System.Math.Floor(gp);

                    float gw = (float)(gp - gi);

                    float gate = Mathf.Lerp(
                        IntroGate[(gi + gl) % gl],
                        IntroGate[(gi + 1 + gl) % gl],
                        gw
                    );

                    float flutter =
                        1f + introFlutterDepth *
                        (float)System.Math.Sin(
                            TwoPi * introFlutterRate * ti
                        );

                    float sum = 0f;

                    for (int h = 0; h < IntroLevels.Length; h++)
                    {
                        float level =
                            Mathf.Lerp(
                                IntroLevels[h][ia],
                                IntroLevels[h][ib],
                                fr
                            ) * IntroScale;

                        if (level <= 0f)
                            continue;

                        sum += level *
                            (float)System.Math.Sin(
                                TwoPi *
                                introBaseFrequency *
                                (h + 1) *
                                ti
                            );
                    }

                    float fade = Mathf.Clamp01(
                        (float)((introSecs - ti) / IntroFadeOut)
                    );

                    bells = sum * gate * flutter * fade * introVol;
                }

                introOut =
                    bells + ProcessReverb(bells) * introRev;
            }

            // MEZCLA FINAL

            float output = (float)System.Math.Tanh(
    (
        agogoSound +
        surdoSound +
        repiqueSound +
        tamborimSound +
        cuicaSound +
        silbatoSound +
        caixaSound +
        chocalhoSound +
        introOut +
        metronomoSound
    ) * 1.5f
);

            for (int c = 0; c < channels; c++)
                data[i + c] = output;

            // AVANCE DEL TIEMPO

            if (introElapsed < introSecs + IntroTail + 1.0)
                introElapsed += dt;

            if (!inIntro)
                beatPos += dBeat;

            elapsedTime += dt;

            // AVANCE DE FASES

            // Agogó
            agogoPhase +=
                TwoPi * agogo.frecuencia / sampleRate;

            if (agogoPhase > TwoPi)
                agogoPhase -= TwoPi;

            if (agogo.frecuenciaSecundaria > 0f)
            {
                agogoSecondPhase +=
                    TwoPi * agogo.frecuenciaSecundaria / sampleRate;

                if (agogoSecondPhase > TwoPi)
                    agogoSecondPhase -= TwoPi;
            }

            // Surdo
            surdoPhase +=
                TwoPi * surdo.frecuencia / sampleRate;

            if (surdoPhase > TwoPi)
                surdoPhase -= TwoPi;

            // Repique
            repiquePhase +=
                TwoPi * repique.frecuencia / sampleRate;

            if (repiquePhase > TwoPi)
                repiquePhase -= TwoPi;

            // Tamborim
            tamborimPhase +=
                TwoPi * tamborim.frecuencia / sampleRate;

            if (tamborimPhase > TwoPi)
                tamborimPhase -= TwoPi;

            // Cuica
            cuicaPhase +=
                TwoPi * cuica.frecuencia / sampleRate;

            if (cuicaPhase > TwoPi)
                cuicaPhase -= TwoPi;

            // SILBATO: OSCILADOR PRINCIPAL
            silbatoPhase +=
                TwoPi * silbato.frecuencia / sampleRate;

            if (silbatoPhase > TwoPi)
                silbatoPhase -= TwoPi;

            // TREMOLO: 5 Hz
            silbatoLfoPhase +=
                TwoPi * 5f / sampleRate;

            if (silbatoLfoPhase > TwoPi)
                silbatoLfoPhase -= TwoPi;

            // VIBRATO: 8 Hz
            silbatoVibratoPhase +=
                TwoPi * 8f / sampleRate;

            if (silbatoVibratoPhase > TwoPi)
                silbatoVibratoPhase -= TwoPi;

            // MULTIPLY: 12 Hz
            silbatoMultiplyPhase +=
                TwoPi * 12f / sampleRate;

            if (silbatoMultiplyPhase > TwoPi)
                silbatoMultiplyPhase -= TwoPi;

            // Caixa
            caixaPhase +=
                TwoPi * caixa.frecuencia / sampleRate;

            if (caixaPhase > TwoPi)
                caixaPhase -= TwoPi;

            // Chocalho
            chocalhoPhase +=
                TwoPi * 8f / sampleRate;

            if (chocalhoPhase > TwoPi)
                chocalhoPhase -= TwoPi;

        }
    }
}
