using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class MainMenuManager : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    [SerializeField] private string nombreEscenaJuego = "Escena_Juego";

    [Header("Referencias de la Pizarra (UI)")]
    [SerializeField] private TextMeshProUGUI textoBotonContinuar;
    [SerializeField] private Button componenteBotonContinuar;

    [Header("Ajustes Visuales (Sin Datos)")]
    [SerializeField] private float tamanoLetraConDatos = 36f;   
    [SerializeField] private float tamanoLetraSinDatos = 24f;   
    [SerializeField] private Color colorDesactivado = new Color(0.5f, 0.5f, 0.5f, 0.6f); 

    private string rutaArchivo;

    void Awake()
    {
        // Definimos la ruta en el Awake para tenerla lista inmediatamente
        rutaArchivo = Application.persistentDataPath + "/ProgresoAlquimia.json";
    }

    void Start()
    {
        // En lugar de verificar en seco, iniciamos una pequeña secuencia de espera asíncrona
        StartCoroutine(SecuenciaVerificacionInicial());
    }

    private System.Collections.IEnumerator SecuenciaVerificacionInicial()
    {
        // Esperamos un frame completo para garantizar que SaveManager inicializó su Awake y leyó el disco
        yield return new WaitForEndOfFrame();

        // Forzamos la carga del progreso de manera segura una vez que todo existe en la escena
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.CargarProgreso();
        }

        // Una vez asegurados los datos en memoria, configuramos el estado real del botón
        ConfigurarTextoContinuar();
    }

    private void ConfigurarTextoContinuar()
    {
        // Comprobación física directa y rigurosa en el disco duro
        bool existeArchivo = File.Exists(rutaArchivo);
        int diaActual = 1;

        if (SaveManager.Instance != null && SaveManager.Instance.datosActuales != null)
        {
            diaActual = SaveManager.Instance.datosActuales.diaActual;
        }

        // CONDICIÓN CORREGIDA: Solo se bloquea si REALMENTE el archivo no existe en el disco duro
        // O si el archivo existe pero los datos internos apuntan a que es el Día 1 (Partida nueva/vía reseteo)
        if (!existeArchivo || diaActual <= 1)
        {
            if (textoBotonContinuar != null)
            {
                textoBotonContinuar.text = "Continuar Jornada\n<color=#555555>(No hay datos)</color>";
                textoBotonContinuar.color = colorDesactivado;
                textoBotonContinuar.fontSize = tamanoLetraSinDatos; 
            }

            if (componenteBotonContinuar != null)
            {
                componenteBotonContinuar.interactable = false; // El láser lo ignora por completo
                
                var colores = componenteBotonContinuar.colors;
                colores.disabledColor = colorDesactivado;
                componenteBotonContinuar.colors = colores;
                
                // Forzamos la actualización visual en la UI de Unity
                componenteBotonContinuar.gameObject.SetActive(false);
                componenteBotonContinuar.gameObject.SetActive(true);
            }
            
            Debug.Log($"<color=gray>[MENÚ INTERNO]</color> Bloqueado: No se encontró archivo o el día es {diaActual}.");
        }
        else
        {
            // SI HAY REGISTROS (Día 2 o más detectado con éxito)
            if (textoBotonContinuar != null)
            {
                textoBotonContinuar.text = $"Continuar Jornada\n<color=green>(Día {diaActual})</color>";
                textoBotonContinuar.color = Color.white; 
                textoBotonContinuar.fontSize = tamanoLetraConDatos; // Letra grande normal
            }

            if (componenteBotonContinuar != null)
            {
                componenteBotonContinuar.interactable = true; // Habilitado para los gatillos/tecla G
            }

            Debug.Log($"<color=green>[MENÚ INTERNO]</color> ¡Acceso Concedido! Datos detectados correctamente para el Día {diaActual}.");
        }
    }

    public void SeleccionarNuevaPartida()
    {
        Debug.Log("<b><color=red>[MENÚ]</color></b> Iniciando Nueva Partida. Forzando reseteo total...");
        
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.EliminarProgresoExistente();
        }

        StartCoroutine(SecuenciaCargaLimpia());
    }

    private System.Collections.IEnumerator SecuenciaCargaLimpia()
    {
        yield return new WaitForEndOfFrame();

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.CargarProgreso();
        }

        SceneManager.LoadScene(nombreEscenaJuego);
    }

    public void SeleccionarContinuarJornada()
    {
        bool existeArchivo = File.Exists(rutaArchivo);
        if (!existeArchivo || SaveManager.Instance == null || SaveManager.Instance.datosActuales.diaActual <= 1)
        {
            Debug.LogWarning("[MENÚ] Clic bloqueado. No hay datos reales que cargar.");
            return; 
        }

        Debug.Log($"<b><color=green>[MENÚ]</color></b> Cargando laboratorio con el progreso intacto en el Día: {SaveManager.Instance.datosActuales.diaActual}");
        CargarLaboratorio();
    }

    public void SeleccionarSalir()
    {
        Debug.Log("<color=red>[MENÚ]</color> Cerrando aplicación...");
        Application.Quit();
    }

    private void CargarLaboratorio()
    {
        SceneManager.LoadScene(nombreEscenaJuego);
    }
}