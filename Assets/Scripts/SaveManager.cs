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
        
        // Al entrar al juego simplemente cargamos lo que haya (Día 1 si es nuevo, o el día guardado)
        CargarProgreso(); 
    }

    public void EliminarProgresoExistente()
    {
        // 1. Borrado físico absoluto en el disco
        if (File.Exists(rutaArchivo))
        {
            File.Delete(rutaArchivo);
            Debug.Log("<color=red>[SAVE SYSTEM]</color> JSON borrado físicamente del disco.");
        }

        // 2. LIMPIEZA INMEDIATA DE LA RAM: Forzamos datos limpios en la variable activa
        datosActuales = new DatosGuardado();
        datosActuales.diaActual = 1;
        datosActuales.monedasTotales = 0;
        datosActuales.erroresAcumulados = 0;

        // 3. Escribimos el archivo en blanco de inmediato
        string contenidoJson = JsonUtility.ToJson(datosActuales, true);
        File.WriteAllText(rutaArchivo, contenidoJson);

        Debug.Log("<color=cyan>[SAVE SYSTEM]</color> RAM y disco sincronizados en el Día 1.");
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

    public string ObtenerRutaArchivo()
    {
        return rutaArchivo;
    }
}