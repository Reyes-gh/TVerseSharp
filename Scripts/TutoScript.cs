using System.Collections.Generic;
using UnityEngine;

public class TutoScript : MonoBehaviour
{
    public Camera cam;
    public GameObject player;
    bool running;
    private List<Vector3> posis;
    private GameObject fogLluvia;
    public GameObject particulas;
    public IntroScript introScript;
    public GameObject uiTuto;
    bool isTutoAsking;
    public MundoController mc;
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("tutoCam").GetComponent<Camera>();
        fogLluvia = GameObject.FindGameObjectWithTag("fog");
        isTutoAsking = true;
    }


    //Hacemos que las partículas y la cámara sigan al jugador.

    //También pausamos el juego hasta que el jugador responda a la pregunta de
    //saltarse el tutorial.
    void Update()
    {
        cam.transform.position = player.transform.position;
        particulas.transform.position = player.transform.position;
        fogLluvia.transform.position = player.transform.position;

        if (!introScript.isAnimacionOver && isTutoAsking) {
            Time.timeScale = 0;
        }
    }

    //Si se salta el tutorial se llama al MundoController y se ejecuta
    //NextTuto, un método específico para el tutorial.
    public void tutoSkip() {
        isTutoAsking = false;
        Time.timeScale = 1;
        mc.NextTuto();
    }

    //Si no se salta el tutorial enviamos los botones fuera del alcance del jugador
    //y hacemos que el tiempo continúe.
    public void tutoPlay() {
        isTutoAsking = false;
        Time.timeScale = 1;
        uiTuto.transform.position = new Vector3(99999, 99999, 9999);
    }
}
