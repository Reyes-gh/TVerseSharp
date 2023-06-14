using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class MainMenuController : MonoBehaviour
{
    GameObject btnLoad;
    GameObject btnQuit;
    GameObject btnNew;
    GameObject fondoM;
    GameObject videoIntro;
    GameObject textSprite;
    bool startCurtain = false;
    float timer = 0.0f;
    int seconds;
    int opc;
    float creceDecrece;
    float speedFade;
    AudioSource soundBtns;
    AudioSource audioBack;
    LoadManager loadManager;
    void Start()
    {
        //instanciamos el LoadManager
        this.loadManager = new LoadManager();

        //Desactivamos la sincronización vertical y forzamos el juego a correr a 60FPS.
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        //Debe estar siempre en 1 para que las opacidades funcionen bien, para cambiar la velocidad
        //debemos cambiar valores como el dividendo del Time.DeltaTime/X
        timer = 1;
        startCurtain = false;
        creceDecrece = 0.3f;
        speedFade = 1.5f;

        textSprite = GameObject.FindGameObjectWithTag("textSprite");
        videoIntro = GameObject.FindGameObjectWithTag("videoIntro");
        fondoM = GameObject.FindGameObjectWithTag("fondoM");
        soundBtns = GameObject.FindGameObjectWithTag("select").GetComponent<AudioSource>();
        audioBack = GameObject.FindGameObjectWithTag("audioBack").GetComponent<AudioSource>();

        btnNew = GameObject.FindGameObjectWithTag("btnNew");
        btnLoad = GameObject.FindGameObjectWithTag("btnLoad");
        btnQuit = GameObject.FindGameObjectWithTag("btnPause");

        if (File.Exists(Path.Combine(Application.persistentDataPath, "data.tverse"))) {
            btnLoad.SetActive(true);
        } else {
            btnLoad.SetActive(false);
        }

    }

    void Update()
    {

        //La variable "curtain" es la que inciará el código para cargar una escena, ver StartFade();
        //se pone curtain = true al invocar los flickers.

        moverTitulo();

        //Cortina al entrar al menú

        if (startCurtain)
        {
            timer += Time.deltaTime / speedFade;
            startFade();
        }
        else if (startCurtain == false)
        {

            if (!audioBack.isPlaying)
            {
                audioBack.Play();
                audioBack.loop = true;
            }

            if (timer > 0)
            {
                timer -= Time.deltaTime / speedFade;
                fondoM.GetComponent<Image>().color = new Color(0, 0, 0, timer);
            }
        }
    }

    //Función que se encarga únicamente de aumentar y disminuir el titulo de forma suave para crear la animación vista
    //en el menú.
    void moverTitulo()
    {

        //Es necesario controlar la velocidad a la que aumenta y disminuye el texto, ya que si no lo hacemos
        //Aumentará y disminuirá cada vez más hasta romperse.
        if (textSprite.transform.localScale.x >= 21)
        {
            creceDecrece -= 0.01f;
        }
        else if (textSprite.transform.localScale.x <= 18)
        {
            creceDecrece += 0.01f;
        }

        if (creceDecrece > 0.3f)
        {
            creceDecrece = 0.3f;
        }
        else if (creceDecrece < -0.3f)
        {
            creceDecrece = -0.3f;
        }
        textSprite.transform.localScale = new Vector3(textSprite.transform.localScale.x + (creceDecrece), textSprite.transform.localScale.y + (creceDecrece), 1);
    }

    //Función para el botón NEW, activa la animación de la cortina y llama al flickering, activa la opción 0 (ver startFade())
    public void NewGame()
    {
        startCurtain = true;
        opc = 0;
        StartCoroutine(flickering(btnNew));
        soundBtns.Play();
    }

    //Función para el botón LOAD, activa la animación de la cortina y llama al flickering, activa la opción 1 (ver startFade())
    public void LoadGame()
    {
        startCurtain = true;
        StartCoroutine(flickering(btnLoad));
        opc = 1;
        soundBtns.Play();
    }

    //Función para el botón QUIT, activa la animación de la cortina y llama al flickering, activa la opción 2 (ver startFade())

    public void QuitGame()
    {
        startCurtain = true;
        opc = 2;
        StartCoroutine(flickering(btnQuit));
        soundBtns.Play();
    }

    //Simple coroutine para hacer que el botón clickado parpadee.
    IEnumerator flickering(GameObject btnPressed)
    {
        while (true)
        {
            if (btnPressed.GetComponent<Image>().enabled == true)
            {
                btnPressed.GetComponent<Image>().enabled = false;
            }
            else
            {
                btnPressed.GetComponent<Image>().enabled = true;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    //Gradualmente se aumenta la opacidad de la cortinilla y se disminuye el volumen para una transición suave
    //a los niveles.
    void startFade()
    {

        fondoM.GetComponent<Image>().color = new Color(0, 0, 0, timer);
        audioBack.volume -= Time.deltaTime / speedFade;

        //Nos guiaremos por la variable opc, depende de que botón pulsemos ocurrirá una cosa u otra.
        //Opc 0 - Nuevo
        //Opc 1 - Cargar
        //Opc 2 - Salir

        if (audioBack.volume == 0 && opc == 0)
        {
            loadManager.nuevaPartida();
        }
        else if (audioBack.volume == 0 && opc == 1)
        {
            loadManager.cargarPartida();
        }
        else if (audioBack.volume == 0 && opc == 2)
        {
            Application.Quit();
        }
    }
}
