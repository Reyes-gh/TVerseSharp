using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    private DatosJuego datosJuego = new DatosJuego();
    private FileHandler fileHandler = new FileHandler(Application.persistentDataPath, "data.tverse");
    private DatosManager datosManager;


    public void nuevaPartida()
    {
        this.datosJuego.timeMillis = 0;
        this.datosJuego.currentLevel = 1;
        fileHandler.Save(datosJuego);
        SceneManager.LoadScene(datosJuego.currentLevel);
    }

    public void cargarPartida()
    {
        SceneManager.LoadScene(fileHandler.Load().currentLevel);
    }

    public void nextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
