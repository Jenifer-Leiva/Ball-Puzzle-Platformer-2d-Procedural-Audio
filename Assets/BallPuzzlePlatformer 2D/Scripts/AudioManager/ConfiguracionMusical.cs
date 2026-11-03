using System;
using UnityEngine;

// Cada instrumento contiene secciones ordenadas. Las secciones se reproducen
// una detrás de otra y el conjunto vuelve a empezar al terminar.
[Serializable]
public class PatronInstrumento
{
    public bool activo = true;
    public SeccionRitmica[] secciones = new SeccionRitmica[0];
}

[Serializable]
public class ConfiguracionMusical
{
    public string nombre = "Música";
    [Range(60f, 500f)] public float bpm = 100f;
    [Range(2, 7)] public int tiemposPorCompas = 4;

    [Header("Patrones por instrumento")]
    public PatronInstrumento agogo = new PatronInstrumento();
    public PatronInstrumento surdo = new PatronInstrumento();
    public PatronInstrumento repique = new PatronInstrumento();
    public PatronInstrumento tamborim = new PatronInstrumento();
    public PatronInstrumento cuica = new PatronInstrumento();
    public PatronInstrumento silbato = new PatronInstrumento();
    public PatronInstrumento caixa = new PatronInstrumento();
    public PatronInstrumento chocalho = new PatronInstrumento();
}
