[System.Serializable]
public class DatosJuego
{

    public float timeMillis;
    public int currentLevel;

    public DatosJuego()
    {

        this.timeMillis = -4;
        //El nivel 0 equivale al menú principal, es por ello que empezamos por la escena 1
        this.currentLevel = 1;
    }

}
