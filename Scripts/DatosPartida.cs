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

    public void LoadData(DatosJuego datos)
    {
        this.timeMillis = datos.timeMillis;
        this.currentLevel = datos.currentLevel;
    }

    public void SaveData(ref DatosJuego datos)
    {
        datos.timeMillis = this.timeMillis;
        datos.currentLevel = this.currentLevel;
    }

    void Awake()
    {
        fileHandler = new FileHandler(Application.persistentDataPath, "data.tverse");
        this.timeMillis = fileHandler.Load().timeMillis;
    }

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
    void Start()
    {
        textoTimer = GameObject.FindGameObjectWithTag("timerPartida");
    }

    void Update()
    {

        currentLevel = SceneManager.GetActiveScene().buildIndex;
        timeMillis += Time.deltaTime;
        int horas = TimeSpan.FromSeconds(timeMillis).Hours;
        int minutos = TimeSpan.FromSeconds(timeMillis).Minutes;
        int segundos = TimeSpan.FromSeconds(timeMillis).Seconds;

        if (textoTimer != null)
        {
            textoTimer.GetComponent<TextMeshProUGUI>().SetText(string.Format("{0:00}:{1:00}:{2:00}", horas, minutos, segundos));
        }


    }

}
