using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Configuración de Tiempo")]
    public bool jornadaActiva = false;
    public float segundosRealesPorDia = 600f; // Cuánto dura la jornada (ej. 2 min)
    [Header("Luces del Proyecto")]
    [SerializeField] private Light sunLight;          // Tu Directional Light
    [SerializeField] public Light dayPointLight;    // El DayPointLight 
    [Header("Registro de Datos (Lectura)")]
    public int diaActual = 1;
    public string horaActualTexto;
    public float progresoJornada; // De 0 a 1

    private float horaInicio = 10f; // 10 AM
    private float horaFin = 24f;    // 12 AM (Medianoche)
    private float tiempoTranscurrido;

    void Start()
    {
        tiempoTranscurrido = 0f;
        progresoJornada = 0f;
        CalcularReloj();
        ActualizarRotacionSol();
        ActualizarLuces();
    }
    void Update()
    {
        if (!jornadaActiva) return;

        if (tiempoTranscurrido < segundosRealesPorDia)
        {
            tiempoTranscurrido += Time.deltaTime;
            progresoJornada = tiempoTranscurrido / segundosRealesPorDia;
            
            CalcularReloj();
            ActualizarRotacionSol();
            ActualizarLuces();
        }
        else
        {
            FinalizarJornada();
        }
    }

    public void IniciarNuevaJornada()
    {
        tiempoTranscurrido = 0f;
        progresoJornada = 0f;
        jornadaActiva = true;
        Debug.Log("<color=cyan>[TIEMPO]</color> ¡La jornada del día " + diaActual + " ha comenzado!");
    }

    void CalcularReloj()
    {
        // Interpola entre las 10 y las 24 horas
        float horaDecimal = Mathf.Lerp(horaInicio, horaFin, progresoJornada);
        int horas = Mathf.FloorToInt(horaDecimal);
        int minutos = Mathf.FloorToInt((horaDecimal - horas) * 60);
        
        // Formato para el UI futuro
        horaActualTexto = string.Format("{0:00}:{1:00}", horas, minutos);
    }

    void ActualizarLuces()
    {
        if (progresoJornada > 0.8f) 
        {
            float tX = (progresoJornada - 0.8f) / 0.2f; // Normalizamos el último 20% del tiempo

            // Bajamos la luz del sol de 1 a 0
            if(sunLight != null) sunLight.intensity = Mathf.Lerp(1f, 0f, tX);

            // Bajamos la luz de tu cuarto de 2.5 a 0
            if(dayPointLight != null) dayPointLight.intensity = Mathf.Lerp(2.5f, 0f, tX);
        }
        else
        {
            // Mantenemos intensidades normales durante el día
            if(sunLight != null) sunLight.intensity = 1f;
            if(dayPointLight != null) dayPointLight.intensity = 2.5f;
        }
    }

    void ActualizarRotacionSol()
    {
        // Eje Y: de -80 a 80
        float rotY = Mathf.Lerp(-80f, 80f, progresoJornada);
        
        // Eje X: se mantiene en 10, pero baja a -30 al final del trayecto
        float rotX = 10f;
        if (progresoJornada > 0.8f) // Empieza a bajar el sol al 80% del tiempo
        {
            float tX = (progresoJornada - 0.8f) / 0.2f;
            rotX = Mathf.Lerp(10f, -30f, tX);
            
            // Bajamos la intensidad para que oscurezca
            sunLight.intensity = Mathf.Lerp(1f, 0f, tX);
        }

        transform.localRotation = Quaternion.Euler(rotX, rotY, 0);
    }

    void FinalizarJornada()
    {
        jornadaActiva = false; 
        GameManager.Instance.misionEnProgreso = false; 
        
        if (SFXManager.Instance != null && SFXManager.Instance.musicaGanar != null)
        {
            SFXManager.Instance.PlaySFX(SFXManager.Instance.musicaGanar, 0.6f);
        }

        StartCoroutine(GameManager.Instance.TransicionNuevoDia());
    }

    public void SiguienteDia()
    {
        diaActual++;
        tiempoTranscurrido = 0;
    }
}