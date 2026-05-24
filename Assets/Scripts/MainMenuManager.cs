using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class MainMenuManager : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    [SerializeField] private string nombreEscenaJuego = "MainLevel";

    [Header("Referencias de la Pizarra (UI)")]
    [SerializeField] private TextMeshProUGUI textoBotonContinuar;
    [SerializeField] private Collider colisionadorContinuar;

    [Header("Ajustes Visuales (Sin Datos)")]
    [SerializeField] private float tamanoLetraConDatos = 36f;   
    [SerializeField] private float tamanoLetraSinDatos = 24f;   
    [SerializeField] private Color colorDesactivado = new Color(0.5f, 0.5f, 0.5f, 0.6f); 

    private string rutaArchivo;

    void Awake()
    {
        rutaArchivo = Application.persistentDataPath + "/ProgresoAlquimia.json";
    }

    void Start()
    {
        StartCoroutine(SecuenciaVerificacionInicial());

        if (SFXManager.Instance != null && SFXManager.Instance.musicaAmbiente != null)
        {
            SFXManager.Instance.PlayAmbientMusic(SFXManager.Instance.musicaAmbiente, 0.6f);
        }
    }

    private System.Collections.IEnumerator SecuenciaVerificacionInicial()
    {
        yield return new WaitForEndOfFrame();

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.CargarProgreso();
        }

        ConfigurarTextoContinuar();
    }

    private void ConfigurarTextoContinuar()
    {
        bool existeArchivo = File.Exists(rutaArchivo);
        int diaActual = 1;

        if (SaveManager.Instance != null && SaveManager.Instance.datosActuales != null)
        {
            diaActual = SaveManager.Instance.datosActuales.diaActual;
        }

        if (!existeArchivo || diaActual <= 1)
        {
            if (textoBotonContinuar != null)
            {
                textoBotonContinuar.text = "Continuar Jornada\n<color=#555555>(No hay datos)</color>";
                textoBotonContinuar.color = colorDesactivado;
                textoBotonContinuar.fontSize = tamanoLetraSinDatos; 
            }

            if (colisionadorContinuar != null)
            {
                colisionadorContinuar.enabled = false; 
            }
            
            Debug.Log($"<color=gray>[MENÚ INTERNO]</color> Bloqueado: No se encontró archivo o el día es {diaActual}.");
        }
        else
        {
            if (textoBotonContinuar != null)
            {
                textoBotonContinuar.text = $"Continuar Jornada\n<color=green>(Día {diaActual})</color>";
                textoBotonContinuar.color = Color.white; 
                textoBotonContinuar.fontSize = tamanoLetraConDatos; 
            }

            if (colisionadorContinuar != null)
            {
                colisionadorContinuar.enabled = true; 
            }

            Debug.Log($"<color=green>[MENÚ INTERNO]</color> ¡Acceso Concedido! Datos detectados correctamente para el Día {diaActual}.");
        }
    }

    public void SeleccionarNuevaPartida()
    {
        ReproducirClickYDetenerMusica();
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
        ReproducirClickYDetenerMusica();
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
        ReproducirClickYDetenerMusica();
        Debug.Log("<color=red>[MENÚ]</color> Cerrando aplicación...");
        Application.Quit();
    }

    private void CargarLaboratorio()
    {
        SceneManager.LoadScene(nombreEscenaJuego);
    }

    private void ReproducirClickYDetenerMusica()
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayBotonUI();
            SFXManager.Instance.DetenerMusicaAmbiente();
        }
    }
}