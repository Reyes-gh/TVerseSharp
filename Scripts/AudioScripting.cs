using UnityEngine;

public class AudioScripting : MonoBehaviour
{
    AudioSource audioFondo;
    public MundoController mc;
    public IntroScript ic;
    void Start()
    {
        audioFondo = GameObject.FindGameObjectWithTag("fondoM").GetComponent<AudioSource>();
    }

    // Update is called once per frame
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
}
