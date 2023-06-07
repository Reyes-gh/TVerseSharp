using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    GameObject btnLoad;
    GameObject btnQuit;
    GameObject btnNew;
    bool flickerNew = false;
    bool flickerQuit = false;
    bool flickerLoad = false;
    float timer = 0.0f;
    int seconds;
    AudioSource soundBtns;
    AudioSource audioBack;
    int opc;
    float creceDecrece;
    GameObject fondoM;
    bool startCurtain = false;
    GameObject videoIntro;
    bool isVideoOver;
    GameObject textSprite;
    float speedFade;
    LoadManager loadManager;
    void Start()
    {
        this.loadManager = new LoadManager();

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        //Debe estar siempre en 1 para que las opacidades funcionen bien, para cambiar la velocidad
        //debemos cambiar valores como el dividendo del Time.DeltaTime/X
        timer = 1;
        startCurtain = false;
        isVideoOver = false;
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

    void moverTitulo()
    {

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
        //textSprite.transform.rotation = Quaternion.Euler(1, 1, -textSprite.transform.localScale.x/2.5f+Time.deltaTime*creceDecrece*20);
        textSprite.transform.localScale = new Vector3(textSprite.transform.localScale.x + (creceDecrece), textSprite.transform.localScale.y + (creceDecrece), 1);
    }

    public void NewGame()
    {
        startCurtain = true;
        opc = 0;
        StartCoroutine(flickering(btnNew));
        soundBtns.Play();
    }

    public void LoadGame()
    {
        startCurtain = true;
        StartCoroutine(flickering(btnLoad));
        opc = 1;
        soundBtns.Play();
    }

    public void QuitGame()
    {
        startCurtain = true;
        opc = 2;
        StartCoroutine(flickering(btnQuit));
        soundBtns.Play();
    }

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
