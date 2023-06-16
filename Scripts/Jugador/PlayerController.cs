using UnityEngine;

public class PlayerController : MonoBehaviour
{
    bool canJump;
    bool canWallJump;
    private Rigidbody2D rb2D;
    private Animator animator;
    private SpriteRenderer sprite;
    public float maxHorizontalSpeed = 80;
    public float maxVerticalSpeed = 180;
    int d1;
    int d2;
    public IntroScript introScript;
    AudioSource audioJump;
    AudioSource audioWalljump;
    string activeLayer;
    public MundoController mc;
    RaycastHit2D hitDown1;
    RaycastHit2D hitDown2;
    RaycastHit2D hitLeft;
    RaycastHit2D hitRight;
    GameObject miniHB;
    public Vector3 startPos;
    public bool isOnMeta;
    void Start()
    {
        startPos = gameObject.transform.position;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        rb2D = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        rb2D.freezeRotation = true;

        GameObject jumpSound = GameObject.FindGameObjectWithTag("audioJump");
        audioJump = jumpSound.GetComponent<AudioSource>();

        GameObject walljumpSound = GameObject.FindGameObjectWithTag("audioWalljump");
        audioWalljump = walljumpSound.GetComponent<AudioSource>();

        //Necesario para que la miniHitbox siga al player.
        //la miniHitbox o miniHB se encarga de controlar la muerte, básicamente es una hitbox cuadrada
        //similar a la del jugador, solo que esta va en su centro y es mucho más pequeña.
        //Sirve como "zona de peligro" ya que si esta hitbox pequeñita toca algun collider del mapa
        //provocará la muerte del jugador.
        miniHB = GameObject.FindGameObjectWithTag("miniHitbox");

    }

    void Update()
    {

        //este if controla que la animación de intro se haya acabado para poder mover al personaje.
        if (introScript.isAnimacionOver)
        {
            //Hacemos que miniHB se mueva junto con el jugador para que esté en su centro.
            miniHB.transform.position = gameObject.transform.position;

            //este if controla que el juego no esté pausado.
            if (!mc.isPaused)
            {
                //almacenamos la capa actual enviada por MundoController
                activeLayer = mc.layerActual;

                //! Para el movimiento se utilizarán fuerzas vectoriales multiplicadas por Time.deltaTime para que vayan
                //! ligadas al framerate del jugador, como muchas otras cosas del juego.
                //! Esto hará que si el usuario tiene mal framerate la ejecución ocurra ligada a ello, siendo más lenta.
                //! Es por esto que limitamos los frames a 60, como muchos otros juegos profesionales (Dark souls, Devil May Cry... etc)
                //! Normalmente acelerar el framerate de un juego rompe mucho su código, esto pasa también en muchos juegos famosos como
                //! Zelda Ocarina of Time.
                if (Input.GetKey("left"))
                {
                    rb2D.AddForce(new Vector2(-370000f * Time.deltaTime, 0));
                    sprite.flipX = true;
                }
                else if (Input.GetKey("right"))
                {
                    rb2D.AddForce(new Vector2(370000f * Time.deltaTime, 0));
                    sprite.flipX = false;
                }

                if (Input.GetKeyDown("up"))
                {
                    jump();
                }

                if (Input.GetKey(KeyCode.Q)) {
                    animator.SetBool("dancing", true);
                } else {
                    animator.SetBool("dancing", false);
                }

                //Cada frame se llama a estos métodos, explicados en su debido lugar en el código.
                raycasting();
                ComportamientoJugadorEntorno();
                animacion();
            }
        }
    }

    //OnTrigger se ejecutará cada vez que el jugador entre en contacto con una colisión de tipo trigger.
    //Si estamos en la meta y pulsamos F, el jugador tendrá el status "isOnMeta" el cual se pasará por
    //parametro a MundoController, que lo comprueba constantemente y pasaremos de nivel.
    //Si no estamos en ningún trigger meta, el jugador no tendrá la posibilidad de cambiar el estado pulsando F.
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Meta")
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                isOnMeta = true;
            }
        }
        else
        {
            isOnMeta = false;
        }
    }

    //Raycasting es principalmente como funciona las mecánicas del jugador, se explicará línea por línea.
    void raycasting()
    {
        //Primero creamos una LayerMask, a la que le pasaremos la Layer actual exceptuando al jugador (de ahí el bitshift)
        LayerMask layerActualNoPlayer = 1 << LayerMask.NameToLayer(activeLayer);

        //Asignamos a los dos rayos que van hacia abajo la capa objetivo con la que van a colisionar, y desde qué coordenadas lo harán.
        hitDown1 = Physics2D.Raycast(transform.position - (new Vector3(6.4f, 9)), Vector2.down, 5, layerActualNoPlayer);
        hitDown2 = Physics2D.Raycast(transform.position - (new Vector3(-6.4f, 9)), Vector2.down, 5, layerActualNoPlayer);

        //Hacemos lo mismo que los rayos de abajo pero para los lados, esto controlará las colisiones con paredes.
        hitLeft = Physics2D.Raycast(transform.position - (new Vector3(0, 0)), Vector2.left, 9, layerActualNoPlayer);
        hitRight = Physics2D.Raycast(transform.position - (new Vector3(0, 0)), Vector2.right, 9, layerActualNoPlayer);

        /**
         * ! Controlamos que alguno de los dos rayos orientados hacia abajo esté tocando un suelo, si está cerca del suelo
         * ! la variable canJump será true, permitiendo al jugador saltar en cualquier superficie.
         * 
         * ! Esto ayuda mucho a la hora de generalizar las físicas, ya que el comportamiento del jugador no va atado a un objeto
         * ! concreto, sino a las colisiones del mismo las cuales suelen ser generales para todo y se pueden configurar de manera
         * ! muy versátil.
         * */
        if ((hitDown1.collider != null || hitDown2.collider != null))
        {
            //! Es importante que ambas estén por encima de 1f, ya que puede ocurrir que el jugador esté en un borde de
            //! plataforma, en este caso solo uno de los rayos estaría activo, si controlamos que ambos no estén tocando el
            //! suelo nos estaremos asegurando de que el jugador está en el aire, impidiéndole saltar en ese caso con
            //! canJump = false;
            if ((hitDown1.distance > 1f && hitDown2.distance > 1f))
            {
                canJump = false;
            }
            else
            {
                canJump = true;
            }
        }
        else
        {
            canJump = false;
        }

        // Si alguno de los raycasts toca una pared (distancia designada en el propio rayo) se activará la
        // variable canWallJump, con la que se trabaja más abajo para el salto en pared.

        if (hitLeft.collider != null || hitRight.collider != null)
        {
            canWallJump = true;
        }
        else
        {
            canWallJump = false;
        }

        //Esto son rayos que podemos ver en el editor, simulando los generados para las colisiones
        //Así podemos ver cuando estamos tocando las colisiones que queremos.
        Debug.DrawRay(transform.position - (new Vector3(0, 0)), (new Vector2(11, 0)), Color.red);
        Debug.DrawRay(transform.position - (new Vector3(0, 0)), (new Vector2(-11, 0)), Color.blue);
        Debug.DrawRay(transform.position - (new Vector3(6.4f, 9)), (new Vector2(0, -15)), Color.green);
        Debug.DrawRay(transform.position - (new Vector3(-6.4f, 9)), (new Vector2(0, -15)), Color.green);
    }

    //Este método es específico para calcular cuando se ejecutará la animación de caer al suelo.
    bool isNearFloorAnimation()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, ~(1 << LayerMask.NameToLayer("Player")) | 1 << LayerMask.NameToLayer(activeLayer));

        if (hit.collider != null)
        {
            if ((hit.distance < 21f) && animator.GetBool("falling"))
            {
                animator.SetBool("falling", false);
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    //Método jump, que controlará como se ejecutan los saltos.
    void jump()
    {
        //Primero le daremos prioridad al salto en pared, si el jugador pulsa cualquier dirección estando en la pared
        //y la variable WallJump es true, depende de en qué dirección estemos presionando ejecutará una fuerza vectorial
        //u otra, además de reproducir el sonido correspondiente al salto en pared.

        if (canWallJump && Input.GetKey("left") && hitLeft.collider != null)
        {
            rb2D.AddForce(new Vector2(87000f, 110000f));
            audioWalljump.Play();
        // Nos aseguramos de hacer return para que no se ejecute código sin querer.
        return;
        }

        if (canWallJump && Input.GetKey("right") && hitRight.collider != null)
        {
                rb2D.AddForce(new Vector2(-87000f, 110000f));
                audioWalljump.Play();
        // Nos aseguramos de hacer return para que no se ejecute código sin querer.
        return;
        }
        
        //Como los cálculos se hacen en el propio raycast(), aqui solo basta con hacer el bool canJump falso
        //y ejecutar la fuerza vectorial, además de reproducir el sonido correspondiente.
        if (canJump)
        {
            audioJump.Play();
            canJump = false;
            rb2D.AddForce(new Vector2(0, 91000f));
            return;
        }
    }

    #region Control de estado para la animación
    //Este método se encarga de actualizar en qué estado se encuentra el jugador, trabajando con el Animator de unity para
    //darle valor a los estados y reproducir la animación que toca en cada momento (Salto, parado, corriendo, en caída, habiendo caido, etc.)
    void animacion()
    {
        //Es importante controlar aquí donde deben ir los else y donde no, así como los GetKey, GetKeyUp y GetKeyDown,
        //dependiendo de como queremos que se reproduzca nuestra animación.
        if (Input.GetKeyDown("left") || (Input.GetKeyDown("right")))
        {
            animator.SetBool("moving", true);
        }
        if (!(Input.GetKey("left") || (Input.GetKey("right"))))
        {
            animator.SetBool("moving", false);
        }
        if (!canJump || !canWallJump)
        {
            animator.SetBool("landing", false);
            if (rb2D.velocity.y > 1)
            {
                animator.SetBool("jumping", true);
                animator.SetBool("falling", false);
            }
            //Gracias a isNearFloorAnimation podemos pasar al estado "landing" su opción correspondiente mediante el calculo hecho
            //en el método con los rayos vectoriales.
            animator.SetBool("landing", isNearFloorAnimation());
        }
        else
        {
            animator.SetBool("landing", true);
            animator.SetBool("falling", false);
            animator.SetBool("jumping", false);
        }
        if (rb2D.velocity.y < 0)
        {
            animator.SetBool("jumping", false);
            animator.SetBool("falling", true);
        }
    }
    #endregion

    //Este método servirá para decirle al jugador como se debe comportar respecto al entorno en términos de fricción
    //y gravedad.
    void ComportamientoJugadorEntorno()
    {
        //Al estar sumando constantemente fuerza vectorial al jugador para simular la aceleración, debemos controlar la velocidad
        //máxima a la que puede llegar, tanto vertical como horizontalmente.
        var velocity = rb2D.velocity;

        if (velocity.x > maxHorizontalSpeed) velocity.x = maxHorizontalSpeed;
        if (velocity.x < -maxHorizontalSpeed) velocity.x = -maxHorizontalSpeed;

        if (velocity.y > maxVerticalSpeed) velocity.y = maxVerticalSpeed;
        if (velocity.y < -maxVerticalSpeed) velocity.y = -maxVerticalSpeed;

        rb2D.velocity = velocity;

        //En el juego hay dos tipos de formas de frenar y/o moverse, con deceleración o en seco.
        //la principal diferencia entre ambas es la precisión que se consigue con cada una.
        //Si activamos el frenado en seco, tendremos una precisión mucho mayor a la hora de saltar y movernos
        //sintiéndose mucho más fluido y facil.
        //Sin embargo, si activamos el frenado por deceleración se calculará el frenado de otra forma, haciendo parecer
        //que el jugador pesa más y se resbala un poco más.

        #region Frenado por deceleración
        /*
            if (!(Input.GetKey("left") || (Input.GetKey("right"))))
            {
                if ((rb2D.velocity.x < -1 || rb2D.velocity.x > 1))
                {
                    rb2D.AddForce(new Vector2(-(rb2D.velocity.x) * 7200 * Time.deltaTime, 0));
                }
                else
                {
                    rb2D.velocity = new Vector2(0, rb2D.velocity.y);
                }
            }
        */
        #endregion

        #region  Frenado en seco

        //El frenado en seco detecta cuando se deja de pulsar alguna de las dos teclas direccionales, y establece
        //la velocidad a 0, haciendo que el cambiar de dirección sea mas directo y preciso, pero manteniendo la pequeña
        //aceleración vectorial del movimiento.

        if ((Input.GetKeyUp("left") || (Input.GetKeyUp("right"))))
        {

            rb2D.velocity = new Vector2(0, rb2D.velocity.y);
        }

        #endregion

        #region WallClinging / Engancharse a la pared

        //Al estar enganchados a la pared se reducirá la velocidad de caida del personaje en un tercio
        //Se controla también si la dirección en la que se está pulsando es la dirección en la que está la pared, para
        //no frenar de forma innecesaria.

        //Al no ser posible controlar las animaciones de ello en el animator por necesitar cálculos, se hace aquí para
        //tener un poco más ordenado el código.
        if (canWallJump && (Input.GetKey("left") || Input.GetKey("right")))
        {
            if (hitLeft.collider != null && (Input.GetKey("left")) || hitRight.collider != null && (Input.GetKey("right")))
            {
                rb2D.velocity = new Vector2(rb2D.velocity.x, rb2D.velocity.y / 3);
                animator.SetBool("stick", true);
            }
        }
        else
        {
            animator.SetBool("stick", false);
        }

        #endregion

        //Si se deja de pulsar la tecla de salto se frenará el salto del personaje, dando lugar a un
        //salto más controlado (cuanto mas se pulse más salto llegará).
        if (Input.GetKeyUp("up"))
        {
            if (rb2D.velocity.y > 0)
            {
                rb2D.velocity = new Vector2(rb2D.velocity.x, rb2D.velocity.y / 3);
            }
        }
    }

}
