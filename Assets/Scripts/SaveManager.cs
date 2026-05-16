using UnityEngine;
using System.IO; // Necesario para leer y escribir archivos en el disco duro

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public DatosGuardado datosActuales;
    private string rutaArchivo;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); 

        rutaArchivo = Application.persistentDataPath + "/ProgresoAlquimia.json";
        
        CargarProgreso(); 
    }

    public void GuardarProgreso()
    {
        string contenidoJson = JsonUtility.ToJson(datosActuales, true);
        
        File.WriteAllText(rutaArchivo, contenidoJson);
        
        Debug.Log("<color=cyan>[SAVE SYSTEM]</color> Progreso guardado con éxito en: " + rutaArchivo);
    }

    public void CargarProgreso()
    {
        if (File.Exists(rutaArchivo))
        {
            string contenidoJson = File.ReadAllText(rutaArchivo);
            datosActuales = JsonUtility.FromJson<DatosGuardado>(contenidoJson);
            Debug.Log("<color=cyan>[SAVE SYSTEM]</color> Progreso cargado exitosamente. Día actual: " + datosActuales.diaActual);
        }
        else
        {
            datosActuales = new DatosGuardado();
            Debug.Log("<color=cyan>[SAVE SYSTEM]</color> No hay guardado previo. Iniciando nueva partida desde el Día 1.");
        }
    }
}