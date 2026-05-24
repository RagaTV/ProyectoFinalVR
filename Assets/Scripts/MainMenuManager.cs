using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Necesario para interactuar con el componente Button
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    [SerializeField] private string nombreEscenaJuego = "Escena_Juego";

    [Header("Componentes de la Pizarra UI")]
    [SerializeField] private TextMeshProUGUI textoBotonContinuar;
    [SerializeField] private Button componenteBotonContinuar; // Para desactivar el clic nativo

    void Start()
    {
        ConfigurarMenu();
    }

    private void ConfigurarMenu()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.CargarProgreso();
            int diaActual = SaveManager.Instance.datosActuales.diaActual;

            // Si es una partida nueva o no hay datos válidos
            if (diaActual <= 1)
            {
                if (textoBotonContinuar != null)
                {
                    textoBotonContinuar.text = "Continuar Jornada\n(No hay datos)";
                    textoBotonContinuar.color = new Color(0.7f, 0.7f, 0.7f, 0.5f); // Texto grisáceo
                }
                
                // Desactivamos el botón por completo para que el láser lo ignore
                if (componenteBotonContinuar != null)
                {
                    componenteBotonContinuar.interactable = false;
                }
            }
            else
            {
                // Si va en el día 2 o más, muestra el día guardado en la pizarra
                if (textoBotonContinuar != null)
                {
                    textoBotonContinuar.text = $"Continuar Jornada\n(Día {diaActual})";
                }
                
                if (componenteBotonContinuar != null)
                {
                    componenteBotonContinuar.interactable = true;
                }
            }
        }
    }

    // ==========================================
    // ACCIONES QUE LLAMARÁN LOS LÁSERES
    // ==========================================

    public void NuevaPartida()
    {
        Debug.Log("<color=green>[MENÚ]</color> Reseteando datos para Nueva Partida...");
        
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.datosActuales = new DatosGuardado();
            SaveManager.Instance.datosActuales.diaActual = 1;
            SaveManager.Instance.datosActuales.monedasTotales = 0;
            SaveManager.Instance.datosActuales.erroresAcumulados = 0;
            
            SaveManager.Instance.GuardarProgreso();
        }

        CargarLaboratorio();
    }

    public void ContinuarJornada()
    {
        Debug.Log("<color=green>[MENÚ]</color> Cargando partida guardada.");
        CargarLaboratorio();
    }

    public void SalirDelJuego()
    {
        Debug.Log("<color=red>[MENÚ]</color> Saliendo de la aplicación...");
        Application.Quit();
    }

    private void CargarLaboratorio()
    {
        SceneManager.LoadScene(nombreEscenaJuego);
    }
}