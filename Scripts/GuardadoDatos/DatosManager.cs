using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class DatosManager : MonoBehaviour
{

    [Header("Configuración Storage / Almacenamiento")]
    [SerializeField] private string fileName;
    private DatosJuego datosJuego;
    private List<DataPersistence> dataPersistences;
    private FileHandler fileHandler;
    public static DatosManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Hay más de un DatosManager en la escena");
        }

        instance = this;
    }

    private void Start()
    {
        this.fileHandler = new FileHandler(Application.persistentDataPath, fileName, true);
        this.dataPersistences = findAllDatas();
        LoadGame();
    }

    public void NewGame()
    {
        this.datosJuego = new DatosJuego();
    }

    public void LoadGame()
    {

        //Cargar datos
        this.datosJuego = fileHandler.Load();

        if (this.datosJuego == null)
        {
            Debug.Log("No se han encontrado datos. Creando partida nueva");
            NewGame();
        }

        foreach (DataPersistence datos in dataPersistences)
        {
            datos.LoadData(datosJuego);
        }

        Debug.Log("Cargado tiempo:" + datosJuego.timeMillis);
        Debug.Log("nivel:" + datosJuego.currentLevel);

    }

    public void SaveGame()
    {

        try
        {
            //Pasar datos a script
            foreach (DataPersistence datos in dataPersistences)
            {
                datos.SaveData(ref datosJuego);
            }

            Debug.Log("guardado tiempo:" + datosJuego.timeMillis);
            Debug.Log("guardado nivel:" + datosJuego.currentLevel);

        }
        catch (Exception e)
        {
            Debug.LogError("No hay datos de donde sacar referencia" + e);
        }

        //Hacer que script pase datos a archivo json.
        fileHandler.Save(datosJuego);
        return;
    }

    private void OnApplicationQuit()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0) SaveGame();
    }

    private List<DataPersistence> findAllDatas()
    {
        IEnumerable<DataPersistence> dataPersistences = FindObjectsOfType<MonoBehaviour>().OfType<DataPersistence>();
        return new List<DataPersistence>(dataPersistences);
    }

}
