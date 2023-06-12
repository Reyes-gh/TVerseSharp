using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HintScript : MonoBehaviour
{
    [Header("Lista de pistas y audio a reproducir")]
    public List<TilemapRenderer> hints;
    public AudioSource hintSound;

    [Header("Variables de control para parpadeo")]
    private float timer = 4;
    private int whichHint = 0;
    private bool isFlickerRunning = false;

    #region Start y Update
    void Start()
    {
        foreach (TilemapRenderer hint in hints)
        {
            hint.enabled = false;
        }
    }

    /*
    Resta constantemente Time.deltaTime al timer, no hace falta controlarlo de otra forma, ya que 
    en el método showPista() se establece un nuevo timer de duración para la misma y este 
    se va restando hasta sacar del while al método
    */
    void Update()
    {

        timer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isFlickerRunning) showPista();
        }
    }
    #endregion

    #region Métodos para el llamamiento a la Coroutine flickering y control de la misma.

    /*
    showPista() establece isFlickerRunning como true para que el jugador no ejecute de nuevo la coroutine de pista mientras
    una ya se está ejecutando, también establece el timer a 3 (los segundos que durará la pista) y comienza la coroutine.
    */
    void showPista()
    {
        isFlickerRunning = true;
        timer = 3;
        StartCoroutine(flickerHint(hints[whichHint]));
    }

    /*
    Coroutine que se repite constantemente hasta que se acabe el timer, esta lo que hace es provocar un parpadeo en
    la pista seleccionada por parámetro.

    Al finalizar el while se encarga de asegurarse que la flecha es invisible deshabilitando su sprite y se asegura
    de que la pista no supere el máximo de la Lista (por defecto 2, ya que tenemos 3 pistas por nivel; 0, 1 y 2).
    */
    IEnumerator flickerHint(TilemapRenderer tilemap)
    {

        while (timer > 0)
        {
            if (tilemap.enabled == false)
            {
                tilemap.enabled = true;
                hintSound.Play();
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                tilemap.enabled = false;
                yield return new WaitForSeconds(0.5f);
            }
        }

        tilemap.enabled = false;
        Debug.Log(whichHint);
        if (whichHint == 2)
        {
            whichHint = 0;
        }
        else
        {
            whichHint += 1;
        }
        isFlickerRunning = false;
    }
    #endregion
}
