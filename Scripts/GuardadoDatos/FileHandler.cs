using System;
using System.IO;
using UnityEngine;

public class FileHandler
{

    private string dataDirPath = "";
    private string dataFileName = "";
    private bool encriptar = false;
    private readonly string encryptKeyWord = "alejandro";
    public FileHandler(string dataDirPath, string dataFileName, bool encriptar)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.encriptar = encriptar;
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

                //Desencripta los datos si así se requiere.
                if (encriptar)
                {
                    dataToLoad = encrypt(dataToLoad);
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

            //Encripta los datos si así se requiere.
            if (encriptar)
            {
                dataToStore = encrypt(dataToStore);
            }

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

    // ? Encriptación de los datos mediante XOR
    private string encrypt(string data)
    {
        string dataModificada = "";

        for (int i = 0; i < data.Length; i++)
        {
            dataModificada += (char)(data[i] ^ encryptKeyWord[i % encryptKeyWord.Length]);
        }
        return dataModificada;
    }
}
