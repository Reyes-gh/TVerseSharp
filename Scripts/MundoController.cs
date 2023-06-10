using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MundoController : MonoBehaviour
{
    [Header("GameObjects para manipular y Arrays")]
    GameObject player;
    GameObject squareReset;
    GameObject fogLluvia;
    GameObject botonCambio;
    GameObject botonPausa;
    GameObject[] botonesPause;
    GameObject[] particulasArray;
    GameObject[] suelos;
    public DatosManager datosManager;
    [Header("Sprites")]
    public Sprite pulsarA;
    public Sprite pulsarW;
    public Sprite pulsarD;
    public Sprite pulsarS;
    public Sprite pausaSprite;
    public Sprite playSprite;
    [Header("Variables para control")]
    public string layerActual;
    public bool isPaused;
    bool startReset;
    bool resetLevel;
    float cooldown;
    float timer;
    int d0;
    int d1;
    int d2;
    int d3;
    float fogTimer;

    [System.Obsolete]
    void Start()
    {

        cooldown = 0;
        isPaused = false;
        fogTimer = 1;

        fogLluvia = GameObject.FindGameObjectWithTag("fog");
        particulasArray = GameObject.FindGameObjectsWithTag("particulas");
        squareReset = GameObject.FindGameObjectWithTag("squareReset");
        botonCambio = GameObject.FindGameObjectWithTag("btnCambio");
        botonPausa = GameObject.FindGameObjectWithTag("PausaJuego");
        botonesPause = GameObject.FindGameObjectsWithTag("botonesPause");
        player = GameObject.FindGameObjectWithTag("Player");
        suelos = GameObject.FindGameObjectsWithTag("suelo");
        GameObject fondoM = GameObject.FindGameObjectWithTag("fondoM");

        d3 = LayerMask.NameToLayer("Dimension3");
        d2 = LayerMask.NameToLayer("Dimension2");
        d1 = LayerMask.NameToLayer("Dimension1");
        d0 = LayerMask.NameToLayer("Dimension0");

        startReset = false;
        resetLevel = false;

        foreach (GameObject botones in botonesPause)
        {
            botones.GetComponent<Button>().enabled = false;
            botones.GetComponent<Image>().enabled = false;
        }
        botonPausa.GetComponent<Image>().color = new Color(1, 1, 1, 0);

        foreach (GameObject particulas in particulasArray)
        {
            particulas.GetComponent<ParticleSystem>().startColor = new Color(1, 1, 1, 0);
        }
        
        fondoM.GetComponent<AudioSource>().Play();

        //Ejecutamos el pulsarS al empezar para establecer todos los parámetros sin problema (se puede hacer con cualquier dimensión)
        botonCambio.GetComponent<Image>().sprite = pulsarS;
        layerActual = "Dimension0";
        Color colorD0 = new Color(181f / 255, 163f / 255, 47f / 255, 1f);
        toggleDimension(d0, colorD0);

    }
    #region pollómetro


    #endregion

    [System.Obsolete]
    void Update()
    {
        
        //Control de la niebla de la capa 0 / amarilla
        if (fogTimer > 0.3f)
        {
            fogTimer = 0.3f;
        }
        else if (fogTimer < 0)
        {
            fogTimer = 0f;
        }

        if (LayerMask.NameToLayer(layerActual) == d0)
        {
            fogLluvia.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, fogTimer);
            fogTimer += Time.deltaTime;
        }
        else
        {
            fogLluvia.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, fogTimer);
            fogTimer -= Time.deltaTime;
        }

        //Control de la transición al resetear el nivel

        if (startReset)
        {
            squareReset.transform.localScale += new Vector3(0.5f, 0.5f, 0.5f);

            if (resetLevel)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else
            {
                if (squareReset.transform.localScale.x >= 15)
                {
                    resetLevel = true;
                }
            }
        }

        //Control de la opacidad del botón de pause y la funcionalidad
        botonPausa.GetComponent<Image>().color = new Color(1, 1, 1, timer);
        timer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Escape))
        {

            if (isPaused == true)
            {

                foreach (GameObject botones in botonesPause)
                {
                    botones.GetComponent<Button>().enabled = false;
                    botones.GetComponent<Image>().enabled = false;
                }

                botonPausa.GetComponent<Image>().sprite = playSprite;
                botonPausa.GetComponent<Image>().enabled = true;
                timer = 1;
                isPaused = false;
                Time.timeScale = 1;

            }
            else
            {

                foreach (GameObject botones in botonesPause)
                {
                    botones.GetComponent<Button>().enabled = true;
                    botones.GetComponent<Image>().enabled = true;
                }
                botonPausa.GetComponent<Image>().sprite = pausaSprite;
                botonPausa.GetComponent<Image>().enabled = true;
                timer = 1;
                isPaused = true;
                Time.timeScale = 0;
            }

        }

        if (!isPaused)
        {

            cooldown -= Time.deltaTime;

            //Control del cambio de capa manejado con cooldown
            if (cooldown < 0)
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    botonCambio.GetComponent<Image>().sprite = pulsarS;
                    layerActual = "Dimension0";
                    Color colorD0 = new Color(181f / 255, 163f / 255, 47f / 255, 1f);
                    toggleDimension(d0, colorD0);
                }

                if (Input.GetKeyDown(KeyCode.A))
                {
                    botonCambio.GetComponent<Image>().sprite = pulsarA;
                    layerActual = "Dimension1";
                    Color colorD1 = new Color(47f / 255, 181f / 255, 52f / 255, 1f);
                    toggleDimension(d1, colorD1);
                }

                if (Input.GetKeyDown(KeyCode.W))
                {
                    botonCambio.GetComponent<Image>().sprite = pulsarW;
                    layerActual = "Dimension2";
                    Color colorD2 = new Color(47f / 255, 54f / 255, 181f / 255, 1f);
                    toggleDimension(d2, colorD2);
                }

                if (Input.GetKeyDown(KeyCode.D))
                {
                    botonCambio.GetComponent<Image>().sprite = pulsarD;
                    layerActual = "Dimension3";
                    Color colorD3 = new Color(181f / 255, 47f / 255, 67f / 255, 255f);
                    toggleDimension(d3, colorD3);
                }
            }

        }
        NextLevel();

    }

    //Control de reseteo de nivel
    public void ResetLevel()
    {
        datosManager.LoadGame();
        Time.timeScale = 1;
        startReset = true;
    }
    //Control de salida de nivel
    public void QuitLevel()
    {
        datosManager.SaveGame();
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    void NextLevel()
    {
        if (player.GetComponent<PlayerController>().isOnMeta == true)
        {
            LoadManager loadManager = new LoadManager();
            loadManager.nextLevel();
        }
    }


    //Función toggleDimensión la cual cambia las propiedades de cada dimensión en base a lo que se pulse pasado por parámetro.
    [System.Obsolete]
    void toggleDimension(int dis, Color newColor)
    {

        foreach (GameObject suelo in suelos)
        {

            if (suelo.layer == dis)
            {

                suelo.GetComponent<TilemapCollider2D>().enabled = true;
                TilemapRenderer rend = suelo.GetComponent<TilemapRenderer>();
                rend.sortingLayerName = "Default";
                rend.sortingOrder = 0;
                suelo.GetComponent<Tilemap>().color = newColor;

            }
            else
            {
                suelo.GetComponent<TilemapCollider2D>().enabled = false;
                shade(suelo);
            }
        }
    }

    [System.Obsolete]
    void shade(GameObject suelo)
    {

        foreach (GameObject particulas in particulasArray)
        {

            if (particulas.layer == LayerMask.NameToLayer(layerActual))
            {
                particulas.GetComponent<ParticleSystem>().startColor = new Color(1, 1, 1, 1);
            }
            else
            {
                particulas.GetComponent<ParticleSystem>().startColor = new Color(1, 1, 1, 0);
            }

        }

        //Cooldown para cambio de capa
        //Habilitado pero muy bajo (0.1)

        cooldown = 0.1f;

        TilemapRenderer rend2 = suelo.GetComponent<TilemapRenderer>();
        rend2.sortingLayerName = "background";
        rend2.sortingOrder = 0;

        if (suelo.layer == d1)
        {
            suelo.GetComponent<Tilemap>().color = new Color(47f / 255, 181f / 255, 52f / 255, 0.25f);
        }
        else if (suelo.layer == d2)
        {
            suelo.GetComponent<Tilemap>().color = new Color(47f / 255, 54f / 255, 181f / 255, 0.25f);
        }
        else if (suelo.layer == d0)
        {
            suelo.GetComponent<Tilemap>().color = new Color(181f / 255, 163f / 255, 47f / 255, 0.25f);
        }
        else if (suelo.layer == d3)
        {
            suelo.GetComponent<Tilemap>().color = new Color(181f / 255, 47f / 255, 67f / 255, 0.25f);
        }
    }

}
