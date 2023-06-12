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

        #region Variables de control
        cooldown = 0;
        isPaused = false;
        fogTimer = 1;
        #endregion

        #region Imágenes UI / Fondo / Animaciones

        fogLluvia = GameObject.FindGameObjectWithTag("fog");
        particulasArray = GameObject.FindGameObjectsWithTag("particulas");
        squareReset = GameObject.FindGameObjectWithTag("squareReset");
        botonCambio = GameObject.FindGameObjectWithTag("btnCambio");
        botonPausa = GameObject.FindGameObjectWithTag("PausaJuego");
        botonesPause = GameObject.FindGameObjectsWithTag("botonesPause");
        GameObject fondoM = GameObject.FindGameObjectWithTag("fondoM");

        #endregion

        #region Jugador y Entorno

        player = GameObject.FindGameObjectWithTag("Player");
        suelos = GameObject.FindGameObjectsWithTag("suelo");

        #endregion

        d3 = LayerMask.NameToLayer("Dimension3");
        d2 = LayerMask.NameToLayer("Dimension2");
        d1 = LayerMask.NameToLayer("Dimension1");
        d0 = LayerMask.NameToLayer("Dimension0");

        startReset = false;
        resetLevel = false;

        #region Inicialización de objetos ingame como partículas o botones, música... etc.

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

        #endregion

        //Ejecutamos el pulsarS al empezar para establecer todos los parámetros sin problema (se puede hacer con cualquier dimensión)
        botonCambio.GetComponent<Image>().sprite = pulsarS;
        layerActual = "Dimension0";
        Color colorD0 = new Color(181f / 255, 163f / 255, 47f / 255, 1f);
        toggleDimension(d0, colorD0);

    }
    #region Eventos en tiempo real

    [System.Obsolete]
    void Update()
    {

        //Control de la niebla de la capa 0 / amarilla
        //Esto nos sirve para la niebla que se genera al cambiar a la capa
        //con lluvia, ya que si no controlamos esto, al ser gradual irá 
        //o incrementando por encima de lo que debe, o decrementando por debajo
        //de lo que debería.
        if (fogTimer > 0.3f)
        {
            fogTimer = 0.3f;
        }
        else if (fogTimer < 0)
        {
            fogTimer = 0f;
        }

        //Más control específico sobre las partículas de la capa 0 / amarilla

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
        //Al pulsar el botón de resetear el nivel se hará true el booleano
        //startReset, ejecutando la animación de reseteo y cargando de nuevo la escena.

        //Al ser el guardado de datos compatible con la carga de escena, se reseteará
        //sin fallos el timer.

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
        //Constantemente se baja la opacidad del botón de pausa, esto en la build no se nota
        //ya que al pausar detenemos el tiempo y el botón no cambia de opacidad, pero al continuar
        //jugando se quita automáticamente el botón gracias a esto, dejando la UI limpia para jugar.
        botonPausa.GetComponent<Image>().color = new Color(1, 1, 1, timer);
        timer -= Time.deltaTime;

        //Control del pause con ESC.
        //Simplemente desactivamos o no la UI dependiendo del pause, así como
        //detener el tiempo del juego.
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

        //Con este if controlamos que no esté pausado, pudiendo ejecutar las mecánicas
        //solo en dicho caso.
        if (!isPaused)
        {
            //La variable cooldown se resta constantemente, ya que es necesario.
            //(Se le asigna valor debajo, esto sirve como timer / cooldown para el cambio de capa)
            cooldown -= Time.deltaTime;

            //Control del cambio de capa manejado con cooldown
            if (cooldown < 0)
            {

                /**
                 * ! Depende de la tecla que pulsemos, se enviarán unos datos u otros al método toggleDimension()
                 * ! el cual se encarga de las propiedades de las capas (si se activan sus colisiones, si se le cambia el color)
                 * 
                 * Al pulsar cualquier tecla, se cambiará el sprite de los botones WASD situado abajo a la izquierda, también
                 * se cambiará la variable layerActual utilizada por otros scripts y este mismo, también se asignará a la capa
                 * correspondiente el color que necesite, y se enviarán datos al método como la capa actual y el color
                 * (ver método)*/
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
        //NextLevel se ejecuta constantemente, así cuando se pulse F se comprueba rápidamente que el
        //jugador esté en la meta o no, y pase al siguiente nivel o se quede.
        NextLevel();

    }
    #endregion
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

    //Controlado por el script PlayerController, al pulsar F se comprobará si estamos en la meta o no
    //dicho booleano se pasará aquí y se utilizará para pasar de nivel o no.
    void NextLevel()
    {
        if (player.GetComponent<PlayerController>().isOnMeta == true)
        {
            LoadManager loadManager = new LoadManager();
            loadManager.nextLevel();
        }
    }

    //Método específico para el tutorial, es igual a NextLevel pero solo se ejecuta en el tutorial
    //y su botón de "Saltar Tutorial"
    public void NextTuto()
    {
        LoadManager loadManager = new LoadManager();
        loadManager.nextLevel();
    }


    //Función toggleDimensión la cual cambia las propiedades de cada dimensión en base a lo que se pulse pasado por parámetro
    [System.Obsolete]
    void toggleDimension(int dis, Color newColor)
    {
        //Repasa todos los objetos de tipo suelo, y en caso de coincidir con la capa actual se le 
        //aplican las propiedades necesarias, como activar los colliders y cambiar la capa activa para
        //que se muestre por encima de los demás, además de asignarle el color requerido.
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
                //En caso de no ser de la capa actual, el collider del suelo se desactivará y
                //se activará el método shade() con dicho elemento de tipo suelo.
                //(ver shade())
                suelo.GetComponent<TilemapCollider2D>().enabled = false;
                shade(suelo);
            }
        }
    }

    [System.Obsolete]

    //Shade se encarga de mostrar las partículas de la capa actual y resetear el cooldown del cambio
    //además de oscurecer las demás capas no activas.
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

        //Creamos la nueva variable TilemapRenderer y la enviamos a una capa inferior.
        TilemapRenderer rend2 = suelo.GetComponent<TilemapRenderer>();
        rend2.sortingLayerName = "background";
        rend2.sortingOrder = 0;

        //Dependiendo de que capa no esté activa se le aplicará su color correspondiente.
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
