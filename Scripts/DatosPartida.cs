using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DatosPartida : MonoBehaviour, DataPersistence
{
    float timeMillis;
    int currentLevel;
    GameObject textoTimer;
    private FileHandler fileHandler;
    public DatosManager datosManager;
    public IntroScript introScript;

    //Método que se encarga de cargar los datos de la partida.
    public void LoadData(DatosJuego datos)
    {
        this.timeMillis = datos.timeMillis;
        this.currentLevel = datos.currentLevel;
    }

    //Método que se encarga de guardar los datos en la partida.
    public void SaveData(ref DatosJuego datos)
    {
        datos.timeMillis = this.timeMillis;
        datos.currentLevel = this.currentLevel;
    }

    //Al instanciarse el objeto que tiene este script instancia un FileHandler al que se le cargan los datos.
    //Esto sirve para que el tiempo guardado se cargue al inicio de cada nivel.
    void Awake()
    {
        fileHandler = new FileHandler(Application.persistentDataPath, "data.tverse");
        this.timeMillis = fileHandler.Load().timeMillis;
    }

    //Al destruirse este objeto, el cual controla además de los datos de la partida el tiempo de la misma, se asegura
    //de guardar los datos hasta el momento, principalmente el tiempo.
    private void OnDestroy()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            DatosJuego newDatos = new DatosJuego();
            newDatos.currentLevel = SceneManager.GetActiveScene().buildIndex;
            newDatos.timeMillis = this.timeMillis;
            fileHandler.Save(newDatos);
        }
    }
    //Asignación de la variable textoTimer.
    void Start()
    {
        textoTimer = GameObject.FindGameObjectWithTag("timerPartida");
    }

    //En cada frame se actualiza el tiempo actual de la partida en la UI del juego.
    void Update()   
    {
        if (introScript!=null && introScript.isAnimacionOver) {
             timeMillis += Time.deltaTime;
        }

        currentLevel = SceneManager.GetActiveScene().buildIndex;
       
        int horas = TimeSpan.FromSeconds(timeMillis).Hours;
        int minutos = TimeSpan.FromSeconds(timeMillis).Minutes;
        int segundos = TimeSpan.FromSeconds(timeMillis).Seconds;

        if (textoTimer != null)
        {
            textoTimer.GetComponent<TextMeshProUGUI>().SetText(string.Format("{0:00}:{1:00}:{2:00}", horas, minutos, segundos));
        }
    }

}
