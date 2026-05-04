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
        Application.targetFrameRate = targetFrameRate;

        if (disableVSync)
        {
            QualitySettings.vSyncCount = 0;
        }
        Debug.Log($"Configuración aplicada: {targetFrameRate} FPS.");
    }
}