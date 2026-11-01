using System;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;

public class WavetableTxtLoader : MonoBehaviour
{
    [Header("Archivos de tablas de ondas")]
    [Tooltip("Asigna aquí los archivos TXT de las diferentes ondas.")]
    public TextAsset[] wavetableFiles;

    [HideInInspector]
    public float[] samples;

    public void LoadWavetable(int index)
    {
        // Si no hay archivos asignados, utiliza una onda senoidal.
        if (wavetableFiles == null ||
            index < 0 ||
            index >= wavetableFiles.Length ||
            wavetableFiles[index] == null)
        {
            Debug.LogWarning("Wavetable no encontrada. Se utilizará una onda senoidal.");
            GenerateSine();
            return;
        }

        string content = wavetableFiles[index].text;

        string[] values = content.Split(
            new char[] { ' ', '\t', '\n', '\r', ',', ';' },
            StringSplitOptions.RemoveEmptyEntries
        );

        List<float> loadedSamples = new List<float>();

        foreach (string value in values)
        {
            float sample;

            if (float.TryParse(
                value,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out sample))
            {
                loadedSamples.Add(sample);
            }
        }

        if (loadedSamples.Count < 2)
        {
            Debug.LogWarning("El archivo TXT no contiene suficientes muestras.");
            GenerateSine();
            return;
        }

        samples = loadedSamples.ToArray();

        Debug.Log("Wavetable cargada: " +
                  wavetableFiles[index].name +
                  " | Muestras: " + samples.Length);
    }

    private void GenerateSine()
    {
        int size = 2048;
        samples = new float[size];

        for (int i = 0; i < size; i++)
        {
            samples[i] = Mathf.Sin(2f * Mathf.PI * i / size);
        }
    }
}