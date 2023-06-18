using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditController : MonoBehaviour
{
    public GameObject creditos;
    public GameObject refObject;
    public AudioSource musicaCreditos;
    Rigidbody2D rb2DCreditos;
    private void Start() {
        rb2DCreditos = creditos.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (rb2DCreditos.velocity.y > 22f) rb2DCreditos.velocity = new Vector2 (0, 22);
        if (rb2DCreditos.transform.position.y > refObject.transform.position.y) fadeBye();
    }

    void fadeBye() {

        rb2DCreditos.velocity = new Vector2(0, 0);
        if (!musicaCreditos.isPlaying) SceneManager.LoadScene(0);
    }
}
