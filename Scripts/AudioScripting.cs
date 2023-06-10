using UnityEngine;

public class AudioScripting : MonoBehaviour
{
    [Header("Control de audio respecto al mundo")]
    AudioSource audioFondo;
    public MundoController mc;
    public IntroScript ic;

    #region Control de la música con respecto a la intro y la pausa.
    void Start()
    {
        audioFondo = GameObject.FindGameObjectWithTag("fondoM").GetComponent<AudioSource>();
    }

    /*
    Esta clase controla que la música se reproduzca gradualmente una vez ha terminado la intro.
    También se encarga de pausar la música cuando se pausa la partida.
    */
    void Update()
    {

        if (ic.isAnimacionOver) {
            if (!audioFondo.isPlaying) {
                audioFondo.Play();
            }
            if (audioFondo.volume < 0.4) {
                audioFondo.volume+=(float)0.02;
            }
        }

        if (mc.isPaused) {
            audioFondo.Pause();
        } else {
            audioFondo.UnPause();
        }
    }

    #endregion
}
