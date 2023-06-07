using UnityEngine;
using UnityEngine.UI;

public class IntroScript : MonoBehaviour
{
    RawImage riNegro;
    RawImage riStripe;
    RawImage riText;
    AudioSource fondoM;
    float velocidadDespliegue;
    bool isStripeOver;
    public bool isAnimacionOver;
    GameObject refPoint;
    void Start()
    {
        refPoint = GameObject.FindGameObjectWithTag("refPoint");
        riNegro = GameObject.FindGameObjectWithTag("blackIntro").GetComponent<RawImage>();
        riStripe = GameObject.FindGameObjectWithTag("whiteStripe").GetComponent<RawImage>();
        riText = GameObject.FindGameObjectWithTag("textLevel").GetComponent<RawImage>();
        fondoM = GameObject.FindGameObjectWithTag("fondoM").GetComponent<AudioSource>();
        fondoM.volume = 0;
        fondoM.Stop();
        velocidadDespliegue = 0;
        isStripeOver = false;
        isAnimacionOver = false;
    }
    void Update()
    {

        if (!isStripeOver)
        {
            introStripe();

        }
        else
        {
            introNegro();
        }

    }

    void introText()
    {

        if (riText.transform.position.x > refPoint.transform.position.x - (Screen.width / 100) * 4 && riText.transform.position.x < refPoint.transform.position.x + (Screen.width / 100) * 4)
        {
            riText.transform.Translate(Screen.width * 0.1f * (Time.deltaTime), 0, 0);
        }
        else
        {
            riText.transform.Translate(Screen.width * 3f * (Time.deltaTime), 0, 0);
            velocidadDespliegue = 0;
        }

    }

    void introNegro()
    {

        riText.transform.position = new Vector3(9090, riText.transform.position.y, riText.transform.position.z);

        isAnimacionOver = true;

        //Añadimos dinamismo a la transición del fundido en negro para que no tenga una velocidad constante
        //y simulamos aceleración.

        if (riNegro.transform.position.x < refPoint.transform.position.x * 4)
        {

            riNegro.transform.Translate(Time.deltaTime * velocidadDespliegue, 0, 0);

            velocidadDespliegue += 32f;
        }

    }

    void introStripe()
    {

        if (riStripe.transform.position.x >= refPoint.transform.position.x && riText.transform.position.x < refPoint.transform.position.x * 1 * 2.3)
        {
            if (riText.transform.position.x < refPoint.transform.position.x * 1 * 2.3)
            {
                introText();
            }
            velocidadDespliegue = 0;
        }

        riStripe.transform.Translate(2.5f * (Screen.width / 2 * (Time.deltaTime * velocidadDespliegue)), 0, 0);
        velocidadDespliegue += Time.deltaTime * 2;

        if (riStripe.transform.position.x >= refPoint.transform.position.x * 4)
        {
            isStripeOver = true;
            velocidadDespliegue = 0;
        }



    }
}
