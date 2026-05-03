using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Instancia estática para acceder desde otros scripts si es necesario
    public static GameManager Instance { get; private set; }

    [Header("Configuración de Rendimiento")]
    [SerializeField] private int targetFrameRate = 72;
    [SerializeField] private bool disableVSync = true;

    void Awake()
    {
        // Implementación de Singleton simple
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ConfigurarRendimiento();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void ConfigurarRendimiento()
    {
        // 1. Control de FPS
        // En VR, a veces el SDK (Oculus/OpenXR) ignora esto, 
        // pero para pruebas en editor y sistemas estándar es vital.
        Application.targetFrameRate = targetFrameRate;

        // 2. Control de VSync
        // 0 desactiva, 1 activa. Para que el targetFrameRate funcione, 
        // el VSync debe estar en 0.
        if (disableVSync)
        {
            QualitySettings.vSyncCount = 0;
        }

        Debug.Log($"Configuración aplicada: {targetFrameRate} FPS.");
    }
}