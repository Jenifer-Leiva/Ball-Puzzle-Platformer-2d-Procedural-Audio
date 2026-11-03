using System.Collections.Generic;
using System.Collections;
using UnityEngine;


[RequireComponent(typeof(ProceduralMusic))]
public class MusicDirector : MonoBehaviour
{
    [Header("Referencias")]
    public ProceduralMusic motorMusical;
    [Header("Reproducción")]
    public bool reproducirMainAlIniciar = false;

    private Dictionary<int, ConfiguracionMusical> niveles;
    private ConfiguracionMusical main;


[Header("Control de audio")]
[SerializeField] private float volumenBase = 1f;

private AudioSource fuenteAudio;
private Coroutine fadeCoroutine;
   


    private static MusicDirector instancia;

    
private void Awake()
{
    if (instancia != null && instancia != this)
    {
        Destroy(gameObject);
        return;
    }

    instancia = this;

    if (motorMusical == null)
        motorMusical = GetComponent<ProceduralMusic>();



        fuenteAudio = motorMusical.GetComponent<AudioSource>();

if (fuenteAudio == null)
{
    Debug.LogError("MusicDirector: no se encontró AudioSource.");
}
else
{
    volumenBase = fuenteAudio.volume;
}

    ConstruirConfiguraciones();

    DontDestroyOnLoad(gameObject);

    Debug.Log("MusicDirector activo. Motor: " +
              (motorMusical != null ? motorMusical.gameObject.name : "NO ENCONTRADO"));
}



    public static MusicDirector Instancia
    {
        get { return instancia; }
    }

public void ReproducirMain()
{
    if (main == null || motorMusical == null) return;

    PrepararReproduccion();
    motorMusical.AplicarConfiguracionMusical(main);

    Debug.Log("Reproduciendo música Main");
}

public void ReproducirNivelPorIndice(int indiceCeroBased)
{
    if (motorMusical == null || niveles == null) return;

    int numeroNivel = indiceCeroBased + 1;

    if (niveles.TryGetValue(numeroNivel, out ConfiguracionMusical config))
    {
        PrepararReproduccion();
        motorMusical.AplicarConfiguracionMusical(config);

        Debug.Log("Reproduciendo: " + config.nombre);
    }
    else
    {
        Debug.LogWarning("No existe configuración musical para el nivel " + numeroNivel);
    }
}

private void PrepararReproduccion()
{
    if (fuenteAudio == null) return;

    if (fadeCoroutine != null)
    {
        StopCoroutine(fadeCoroutine);
        fadeCoroutine = null;
    }

    fuenteAudio.volume = volumenBase;

    if (fuenteAudio.isPlaying)
        fuenteAudio.UnPause();
    else
        fuenteAudio.Play();
}


public void FadeOutYDetener(float duracion = 1f)
{
    if (fuenteAudio == null) return;

    if (fadeCoroutine != null)
        StopCoroutine(fadeCoroutine);

    fadeCoroutine = StartCoroutine(
        Co_FadeOutYDetener(duracion)
    );
}

private IEnumerator Co_FadeOutYDetener(float duracion)
{
    float volumenInicial = fuenteAudio.volume;
    float tiempo = 0f;

    if (duracion <= 0f)
    {
        fuenteAudio.volume = 0f;
    }
    else
    {
        while (tiempo < duracion)
        {
            tiempo += Time.unscaledDeltaTime;

            float porcentaje = Mathf.Clamp01(tiempo / duracion);

            fuenteAudio.volume = Mathf.Lerp(
                volumenInicial,
                0f,
                porcentaje
            );

            yield return null;
        }
    }

    fuenteAudio.volume = 0f;
    fuenteAudio.Pause();

    fadeCoroutine = null;

    Debug.Log("Música pausada después del Fade Out.");
}

    // Alternativa si GameControl ya maneja números de nivel desde 1 hasta 10.
    public void ReproducirNivel(int numeroNivel)
    {
        if (motorMusical == null || niveles == null) return;
        if (niveles.TryGetValue(numeroNivel, out ConfiguracionMusical config))
            motorMusical.AplicarConfiguracionMusical(config);
        else
            Debug.LogWarning("No existe configuración musical para el nivel " + numeroNivel);
    }

    private void ConstruirConfiguraciones()
    {
        niveles = new Dictionary<int, ConfiguracionMusical>();
        main = Nueva("Main", 100, 4);
        main.surdo = Instrumento(S("1000 1000 1000 1000", DivisionRitmica.Semicorchea));
        main.chocalho = Instrumento(S("1010 1010 1010 1010", DivisionRitmica.Semicorchea));
        main.tamborim = Instrumento(S("0100 1010 1010 0101", DivisionRitmica.Semicorchea));

        // Niveles 1-2: un compás de intro instrumental seguido de la música.
        ConfiguracionMusical l1 = Nueva("Nivel 1", 80, 4);
        l1.agogo = Instrumento(
            S("1000 0000 0000 1110", DivisionRitmica.Semicorchea));
        l1.surdo = Instrumento(
            S("1000 0001 0101 0011", DivisionRitmica.Semicorchea));
        l1.repique = Instrumento(
            S("00 00 11 11", DivisionRitmica.Corchea),
            S("0 0 1 1", DivisionRitmica.Negra));
        niveles[1] = l1;

         ConfiguracionMusical l2 = Nueva("Nivel 2", 80, 4);
        l2.agogo = Instrumento(
            S("1000 0000 0000 1110", DivisionRitmica.Semicorchea));
        l2.surdo = Instrumento(
            S("1000 0001 0101 0011", DivisionRitmica.Semicorchea));
        l2.repique = Instrumento(
            S("00 00 11 11", DivisionRitmica.Corchea),
            S("0 0 1 1", DivisionRitmica.Negra));
        l2.caixa = Instrumento(
            S("11 01 11 01", DivisionRitmica.Corchea));
        l2.chocalho = Instrumento(
            S("1111 1111 0000 0000", DivisionRitmica.Semicorchea));
        niveles[2] = l2;


        ConfiguracionMusical l3 = Nueva("Nivel 3", 85, 4);
        l3.agogo = Instrumento(S("1001 0001 0000 1100", DivisionRitmica.Semicorchea));
        l3.surdo = Instrumento(S("1010 1010 1001 1101", DivisionRitmica.Semicorchea));
        l3.repique = Instrumento(
            S("1 0 0 1", DivisionRitmica.Negra),
            S("10 01 01 00", DivisionRitmica.Corchea));
        niveles[3] = l3;


        ConfiguracionMusical l4 = Nueva("Nivel 4", 85, 4);
        l4.agogo = Instrumento(S("1001 0001 0000 1100", DivisionRitmica.Semicorchea));
        l4.surdo = Instrumento(S("1010 1010 1001 1101", DivisionRitmica.Semicorchea));
        l4.repique = Instrumento(
            S("1 0 0 1", DivisionRitmica.Negra),
            S("10 01 01 00", DivisionRitmica.Corchea));
        l4.tamborim = Instrumento(S("11 01 11 01", DivisionRitmica.Corchea));
        l4.chocalho = Instrumento(S("1111 1111 1000 0000", DivisionRitmica.Semicorchea));
        niveles[4] = l4;



        ConfiguracionMusical l5 = Nueva("Nivel 5", 90, 4);
        l5.agogo = Instrumento(
            S("0001 0110 1110 1111", DivisionRitmica.Semicorchea),
            S("10 10 10 10", DivisionRitmica.Corchea));
        l5.surdo = Instrumento(S("10 10 10 10", DivisionRitmica.Corchea));
        l5.repique = Instrumento(S("1001 0010 0100 1100", DivisionRitmica.Semicorchea));
        niveles[5] = l5;

        ConfiguracionMusical l6 = Nueva("Nivel 6", 90, 4);
        l6.agogo = Instrumento(
            S("0001 0110 1110 1111", DivisionRitmica.Semicorchea),
            S("10 10 10 10", DivisionRitmica.Corchea));
        l6.surdo = Instrumento(S("10 10 10 10", DivisionRitmica.Corchea));
        l6.repique = Instrumento(S("1001 0010 0100 1100", DivisionRitmica.Semicorchea));
        l6.tamborim = Instrumento(S("11 01 11 01", DivisionRitmica.Corchea));
        l6.chocalho = Instrumento(S("1111 1111 1111 1111", DivisionRitmica.Semicorchea));
        niveles[6] = l6;

        ConfiguracionMusical l7 = Nueva("Nivel 7", 100, 4);
        l7.agogo = Instrumento(S("1100 1101 0000 0111", DivisionRitmica.Semicorchea));
        l7.surdo = Instrumento(S("0000 1111 0011 0011", DivisionRitmica.Semicorchea));
        l7.repique = Instrumento(S("0010 1010 0000 0000", DivisionRitmica.Semicorchea));
        l7.tamborim = Instrumento(S("0101 0101 0101 0101", DivisionRitmica.Semicorchea));
        niveles[7] = l7;

        ConfiguracionMusical l8 = Nueva("Nivel 8", 100, 4);
        l8.agogo = Instrumento(S("1100 1101 0000 0111", DivisionRitmica.Semicorchea));
        l8.surdo = Instrumento(S("0000 1111 0011 0011", DivisionRitmica.Semicorchea));
        l8.repique = Instrumento(S("0010 1010 0000 0000", DivisionRitmica.Semicorchea));
        l8.tamborim = Instrumento(S("0101 0101 0101 0101", DivisionRitmica.Semicorchea));
        l8.caixa = Instrumento(S("0011 0011 0011 0011", DivisionRitmica.Semicorchea));
        l8.chocalho = Instrumento(S("1111 1111 0000 0000", DivisionRitmica.Semicorchea));
        niveles[8] = l8;

        ConfiguracionMusical l9 = Nueva("Niveles 9", 115, 3);
        l9.surdo = Instrumento(
            S("0001 1000 1101", DivisionRitmica.Semicorchea),
            S("1010 1010 1101", DivisionRitmica.Semicorchea));
        l9.caixa = Instrumento(S("1111 0110 1111", DivisionRitmica.Semicorchea));
        l9.repique = Instrumento(S("1101 1101 1101", DivisionRitmica.Semicorchea));
        l9.tamborim = Instrumento(S("1111 0110 1111", DivisionRitmica.Semicorchea));
        niveles[9] = l9;

        ConfiguracionMusical l10 = Nueva("Niveles 9", 115, 3);
        l10.surdo = Instrumento(
            S("0001 1000 1101", DivisionRitmica.Semicorchea),
            S("1010 1010 1101", DivisionRitmica.Semicorchea));
        l10.caixa = Instrumento(S("1111 0110 1111", DivisionRitmica.Semicorchea));
        l10.repique = Instrumento(S("1101 1101 1101", DivisionRitmica.Semicorchea));
        l10.tamborim = Instrumento(S("1111 0110 1111", DivisionRitmica.Semicorchea));
        l10.agogo = Instrumento(S("1010 1001 0010", DivisionRitmica.Semicorchea));
        l10.chocalho = Instrumento(S("10 10 10", DivisionRitmica.Corchea));
        niveles[10] = l10;
    }

    private static ConfiguracionMusical Nueva(string nombre, float bpm, int compas)
    {
        return new ConfiguracionMusical { nombre = nombre, bpm = bpm, tiemposPorCompas = compas };
    }

    private static SeccionRitmica S(string filas, DivisionRitmica division)
    {
        string[] grupos = filas.Split(' ');
        List<int> valores = new List<int>();
        foreach (string grupo in grupos)
            foreach (char c in grupo)
                if (c == '0' || c == '1') valores.Add(c == '1' ? 1 : 0);
        return new SeccionRitmica { compases = 1, division = division, patron = valores.ToArray() };
    }

    private static PatronInstrumento Instrumento(params SeccionRitmica[] secciones)
    {
        return new PatronInstrumento { activo = true, secciones = secciones };
    }

    private static ConfiguracionMusical Clonar(ConfiguracionMusical original, string nombre)
    {
        ConfiguracionMusical c = Nueva(nombre, original.bpm, original.tiemposPorCompas);
        c.agogo = Copia(original.agogo); c.surdo = Copia(original.surdo);
        c.repique = Copia(original.repique); c.tamborim = Copia(original.tamborim);
        c.cuica = Copia(original.cuica); c.silbato = Copia(original.silbato);
        c.caixa = Copia(original.caixa); c.chocalho = Copia(original.chocalho);
        return c;
    }

    private static PatronInstrumento Copia(PatronInstrumento p)
    {
        if (p == null) return new PatronInstrumento { activo = false };
        SeccionRitmica[] ss = new SeccionRitmica[p.secciones.Length];
        for (int i = 0; i < ss.Length; i++)
            ss[i] = new SeccionRitmica { compases = p.secciones[i].compases,
                division = p.secciones[i].division,
                patron = (int[])p.secciones[i].patron.Clone() };
        return new PatronInstrumento { activo = p.activo, secciones = ss };
    }
}
