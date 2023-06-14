using UnityEngine;

public class AudioScripting : MonoBehaviour
{
    [Header("Control de audio respecto al mundo")]
    AudioSource audioFondo;
    AudioSource[] audios;
    public MundoController mc;
    public IntroScript ic;

    #region Control de la música con respecto a la intro y la pausa.
    /*
    Esta clase controla que la música se reproduzca gradualmente una vez ha terminado la intro.
    También se encarga de pausar la música cuando se pausa la partida.
    */
    void Update()
    {
        audioFondo = mc.audioFondo;
        audios = mc.audios;

        if (ic.isAnimacionOver)
        {
            if (!audioFondo.isPlaying)
            {
                foreach (AudioSource audio in audios) audio.Play();
            }
            if (audioFondo.volume < 0.2f)
            {
                audioFondo.volume += (float)0.02;
            }
        }

        if (mc.isPaused)
        {
            foreach (AudioSource audio in audios) audio.Pause();
            audioFondo.Pause();
        }
        else
        {
            foreach (AudioSource audio in audios) audio.UnPause();
            audioFondo.UnPause();
        }
    }

    #endregion
}
