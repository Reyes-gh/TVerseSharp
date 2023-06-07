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
        miniHB = GameObject.FindGameObjectWithTag("miniHitbox");

    }

    void Update()
    {

        if (introScript.isAnimacionOver)
        {

            miniHB.transform.position = gameObject.transform.position;

            if (!mc.isPaused)
            {
                activeLayer = mc.layerActual;

                if (Input.GetKey("left"))
                {
                    rb2D.AddForce(new Vector2(-370000f * Time.deltaTime, 0));
                    sprite.flipX = true;
                }
                else
                {

                    if (Input.GetKey("right"))
                    {
                        rb2D.AddForce(new Vector2(370000f * Time.deltaTime, 0));
                        sprite.flipX = false;
                    }

                }

                if (Input.GetKeyDown("up"))
                {
                    jump();
                }

                raycasting();
                ComportamientoJugadorEntorno();
                animacion();
            }
        }
    }

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

    void raycasting()
    {

        LayerMask layerActualNoPlayer = 1 << LayerMask.NameToLayer(activeLayer);

        hitDown1 = Physics2D.Raycast(transform.position - (new Vector3(6.4f, 9)), Vector2.down, 5, layerActualNoPlayer);
        hitDown2 = Physics2D.Raycast(transform.position - (new Vector3(-6.4f, 9)), Vector2.down, 5, layerActualNoPlayer);

        hitLeft = Physics2D.Raycast(transform.position - (new Vector3(0, 0)), Vector2.left, 9, layerActualNoPlayer);
        hitRight = Physics2D.Raycast(transform.position - (new Vector3(0, 0)), Vector2.right, 9, layerActualNoPlayer);

        if ((hitDown1.collider != null || hitDown2.collider != null))
        {
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

        if (hitLeft.collider != null || hitRight.collider != null)
        {
            canWallJump = true;
        }
        else
        {
            canWallJump = false;
        }

        Debug.DrawRay(transform.position - (new Vector3(0, 0)), (new Vector2(11, 0)), Color.red);
        Debug.DrawRay(transform.position - (new Vector3(0, 0)), (new Vector2(-11, 0)), Color.blue);
        Debug.DrawRay(transform.position - (new Vector3(6.4f, 9)), (new Vector2(0, -15)), Color.green);
        Debug.DrawRay(transform.position - (new Vector3(-6.4f, 9)), (new Vector2(0, -15)), Color.green);
    }

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

    void jump()
    {

        if (canWallJump && ((Input.GetKey("left")) || Input.GetKey("right")))
        {
            canWallJump = false;

            if (Input.GetKey("left") && hitLeft.collider != null)
            {
                rb2D.AddForce(new Vector2(87000f, 110000f));
                audioWalljump.Play();
            }
            else
            {
                if (Input.GetKey("right") && hitRight.collider != null)
                {
                    rb2D.AddForce(new Vector2(-87000f, 110000f));
                    audioWalljump.Play();
                }
            }

            return;
        }

        if (canJump)
        {
            audioJump.Play();
            canJump = false;
            rb2D.AddForce(new Vector2(0, 91000f));
            return;
        }
    }

    void animacion()
    {

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

    void ComportamientoJugadorEntorno()
    {

        var velocity = rb2D.velocity;

        if (velocity.x > maxHorizontalSpeed) velocity.x = maxHorizontalSpeed;
        if (velocity.x < -maxHorizontalSpeed) velocity.x = -maxHorizontalSpeed;

        if (velocity.y > maxVerticalSpeed) velocity.y = maxVerticalSpeed;
        if (velocity.y < -maxVerticalSpeed) velocity.y = -maxVerticalSpeed;

        rb2D.velocity = velocity;

        //Frenado por deceleración

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

        //Frenado en seco
        /*
            if (!(Input.GetKey("left") || (Input.GetKey("right")))) {
                    rb2D.velocity = new Vector2(0, rb2D.velocity.y);
            }
        */

        //Wall clinging

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

        //Si dejas de pulsar la tecla de salto se frenará el salto del personaje, dando lugar a un
        //salto más controlado.
        if (Input.GetKeyUp("up"))
        {
            if (rb2D.velocity.y > 0)
            {
                rb2D.velocity = new Vector2(rb2D.velocity.x, rb2D.velocity.y / 3);
            }
        }
    }

}
