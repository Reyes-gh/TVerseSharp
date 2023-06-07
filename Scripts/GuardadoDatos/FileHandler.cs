using System;
using System.IO;
using UnityEngine;

public class FileHandler
{

    private string dataDirPath = "";
    private string dataFileName = "";

    public FileHandler(string dataDirPath, string dataFileName)
    {

        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;

    }

    public DatosJuego Load()
    {

        //Path completo al archivo, se usa Path.Combine() para que distintos sistemas operativos puedan usarlo.
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        DatosJuego partidaCargada = null;

        if (File.Exists(fullPath))
        {

            try
            {

                //Leer los datos serializados desde el JSON
                string dataToLoad = "";

                using (FileStream fs = new FileStream(fullPath, FileMode.Open))
                {

                    using (StreamReader sr = new StreamReader(fs))
                    {

                        dataToLoad = sr.ReadToEnd();
                    }

                }

                //Deserializar el archivo JSON
                partidaCargada = JsonUtility.FromJson<DatosJuego>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("No se ha podido leer la partida: " + fullPath + "\n" + e);
            }

        }

        return partidaCargada;

    }

    /**
    
    Método para guardar partida en un archivo local almacenado en el juego.

    **/
    public void Save(DatosJuego datosJuego)
    {

        //Path completo al archivo, se usa Path.Combine() para que distintos sistemas operativos puedan usarlo.
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {

            //Creación del archivo de guardado si no existe.
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //Serialización de datos a JSON
            string dataToStore = JsonUtility.ToJson(datosJuego, true);

            using (FileStream fs = new FileStream(fullPath, FileMode.Create))
            {

                using (StreamWriter sm = new StreamWriter(fs))
                {

                    sm.Write(dataToStore);

                }
            }

        }
        catch (Exception e)
        {
            Debug.LogError("Error al guardar el archivo: " + fullPath + "\n" + e);
        }
    }
}
