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

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(mc.layerActual)) muerto = true;
    }

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
