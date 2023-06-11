[System.Serializable]
public class DatosJuego
{
    public float timeMillis;
    public int currentLevel;

    /**
     * * Datos a guardar
     * ? timeMillis guarda el tiempo.
     * ? currentLevel guarda el nivel actual.
     * */
    public DatosJuego()
    {
        this.timeMillis = 0;
        //El nivel 0 equivale al menú principal, es por ello que empezamos por la escena 1
        this.currentLevel = 1;
    }

}
