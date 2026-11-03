using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
    private void Start()
    {
        MusicDirector director = MusicDirector.Instancia;

        if (director == null)
        {
            Debug.LogError("No se encontró MusicDirector en Main Menu.");
            return;
        }

        director.ReproducirMain();
    }
}