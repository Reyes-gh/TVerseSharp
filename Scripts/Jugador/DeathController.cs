using UnityEngine;
using UnityEngine.UI;

public class DeathController : MonoBehaviour
{

    public MundoController mc;
    public PlayerController pc;
    GameObject player;
    GameObject refPoint;
    RawImage muerteScreen;
    AudioSource deathSound;
    Vector3 startPos;
    public bool muerto;
    int timesPlayed;
    SpriteRenderer cross;
    float velDespliegue;

    void Start()
    {
        cross = GameObject.FindGameObjectWithTag("explosion").GetComponent<SpriteRenderer>();
        deathSound = GameObject.FindGameObjectWithTag("audioMuerte").GetComponent<AudioSource>();
        refPoint = GameObject.FindGameObjectWithTag("refPoint");
        player = GameObject.FindGameObjectWithTag("Player");
        muerteScreen = GameObject.FindGameObjectWithTag("MuerteScreen").GetComponent<RawImage>();

        cross.enabled = false;
        timesPlayed = 1;
        muerto = false;
        startPos = muerteScreen.transform.position;
        Physics2D.IgnoreCollision(gameObject.GetComponent<BoxCollider2D>(), player.GetComponent<BoxCollider2D>());
    }
    #region Control del jugador y la cruz de muerte, además del sonido

    //Al ser muerto true nos aseguramos de que se reproduzca una vez el siguiente script, el cual hará al jugador invisible
    //por completo, teleportando la cruz a su posición en el momento de muerte y haciendola visible, además de reproducir el sonido de muerte,
    //A continuación se ejecuta respawn()
    void Update()
    {
        if (muerto)
        {
            if (timesPlayed == 1)
            {
                player.GetComponent<SpriteRenderer>().enabled = false;
                player.GetComponent<TrailRenderer>().enabled = false;
                cross.transform.position = player.transform.position;
                cross.enabled = true;
                deathSound.Play();
            }
            timesPlayed = 0;
            respawn();
        }
        else
        {
            velDespliegue = 0;
        }
    }

    #endregion

    //Al entrar la pequeña hitbox en colisión con alguna dimensión activará el bool muerto.
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(mc.layerActual)) muerto = true;
    }

    //Al entrar en los abismos de muerte (presentes, por ejemplo, en el nivel 2) también
    //moriremos.
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "pitMuerte") muerto = true;    
    }

    /** 
     * Respawn hace que la cortina negra atraviese la pantalla a velocidad gradual, controlando que en cierto punto (cuando cubre cierta parte de la pantalla)
     * teletransporte al jugador a la posición inicial, quitando la cruz y activando de nuevo al jugador, simulando una transición suave al inicio del nivel.*/
    void respawn()
    {

        if (muerteScreen.transform.position.x >= refPoint.transform.position.x - 100 && muerteScreen.transform.position.x <= refPoint.transform.position.x + 100)
        {
            player.transform.position = pc.startPos;
            player.GetComponent<TrailRenderer>().enabled = true;
            player.GetComponent<SpriteRenderer>().enabled = true;
            cross.enabled = false;

        }
        muerteScreen.transform.Translate(Screen.width * (float)velDespliegue * (Time.deltaTime), 0, 0);
        velDespliegue += (float)3 * Time.deltaTime;
        if (muerteScreen.transform.position.x > refPoint.transform.position.x * 3)
        {
            muerto = false;
            muerteScreen.transform.position = startPos;
            timesPlayed = 1;
        }
    }
}
