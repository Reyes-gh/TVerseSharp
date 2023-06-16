using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioScripting : MonoBehaviour
{
    [Header("Control de audio respecto al mundo")]
    AudioSource audioFondo;
    AudioSource monoAudio;
    AudioSource[] audios;
    public MundoController mc;
    public IntroScript ic;
    bool isTuto;

    void Start() {
        /**
         * ? isTuto detecta si estamos en el tutorial, y reproduce la única canción que el tutorial necesita.
         */
        isTuto = SceneManager.GetActiveScene().buildIndex == 1;
        if (isTuto) monoAudio = mc.monoAudio;
    }

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
                if (isTuto)monoAudio.Play();
                foreach (AudioSource audio in audios) audio.Play();
            }
            if (audioFondo.volume < 0.6f)
            {
                StartCoroutine(volumeUp());
            } else {
                StopCoroutine(volumeUp());
            }

        } else {
            audioFondo.volume = 0f;
            if (isTuto)monoAudio.volume = 0f;
        }

        if (mc.isPaused)
        {
            foreach (AudioSource audio in audios) audio.Pause();
            audioFondo.Pause();
            if (isTuto) monoAudio.Pause();
            
        }
        else
        {
            foreach (AudioSource audio in audios) audio.UnPause();
            audioFondo.UnPause();
            if (isTuto) monoAudio.UnPause();
            
        }
    }

    IEnumerator volumeUp() {
        audioFondo.volume +=0.01f;
        if (isTuto) monoAudio.volume +=0.01f;
        yield return new WaitForSeconds(0.2f);
    }

    #endregion
}
