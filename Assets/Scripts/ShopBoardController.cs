using UnityEngine;
using TMPro;

public class ShopBoardController : MonoBehaviour
{
    [Header("Control de Paneles")]
    public GameObject panelTiendaAbierta;
    public GameObject panelTiendaCerrada;

    [Header("UI Tienda Abierta (Mejoras)")]
    public TextMeshProUGUI textoEstabilidad;
    public TextMeshProUGUI textoBorrador;
    public TextMeshProUGUI textoBonoComercio;
    public TextMeshProUGUI textoImanEsencia;
    public TextMeshProUGUI textoStatusCaldero;

    [Header("UI Tienda Cerrada (Información)")]
    public TextMeshProUGUI textoBilletera;
    public TextMeshProUGUI textoHora;

    [Header("Precios de las Mejoras")]
    public int[] costosEstabilidad = { 10, 15, 20 }; 
    public int costoBorrador = 20;
    public int[] costosBono = { 25, 35 }; 
    public int costoImanEsencia = 40;

    private bool tiendaInicializada = false;

    void Start()
    {
        panelTiendaAbierta.SetActive(true);
        panelTiendaCerrada.SetActive(false);
        tiendaInicializada = false; 
    }

    void Update()
    {
        if (!tiendaInicializada)
        {
            if (SaveManager.Instance != null && SaveManager.Instance.datosActuales != null)
            {
                ActualizarInterfazTienda();
                tiendaInicializada = true;
                Debug.Log("<color=green>[TIENDA]</color> ¡JSON cargado con éxito! Textos actualizados.");
            }
        }

        DayNightCycle reloj = FindObjectOfType<DayNightCycle>();
        bool jornadaCorriendo = (reloj != null && reloj.jornadaActiva);

        if (jornadaCorriendo)
        {
            if (panelTiendaAbierta.activeSelf) panelTiendaAbierta.SetActive(false);
            if (!panelTiendaCerrada.activeSelf) panelTiendaCerrada.SetActive(true);

            if (GameManager.Instance != null && textoBilletera != null)
            {
                textoBilletera.text = $"Fondos: {GameManager.Instance.dineroTotal} Monedas";
            }

            if (reloj != null && textoHora != null)
            {
                textoHora.text = $"Hora: {reloj.horaActualTexto}"; 
            }
        }
        else
        {
            if (!panelTiendaAbierta.activeSelf) 
            {
                panelTiendaAbierta.SetActive(true);
                ActualizarInterfazTienda(); 
            }
            if (panelTiendaCerrada.activeSelf) panelTiendaCerrada.SetActive(false);
        }
    }


    public void ActualizarInterfazTienda()
    {
        // === MENSAJES DE DIAGNÓSTICO ===
        if (SaveManager.Instance == null) {
            Debug.LogError("<color=red>[ERROR CRÍTICO]</color> El SaveManager no existe en la escena. ¡Por eso no cambian los textos!");
            return;
        }
        if (SaveManager.Instance.datosActuales == null) {
            Debug.LogWarning("<color=orange>[ADVERTENCIA]</color> SaveManager está en la escena, pero datosActuales está vacío.");
            return;
        }
        // ===============================

        var datos = SaveManager.Instance.datosActuales;

        if (datos.nivelEstabilidad >= 3) {
            textoEstabilidad.text = "Estabilidad: MÁX (Nivel 3)";
        } else {
            int proximoCosto = costosEstabilidad[datos.nivelEstabilidad];
            textoEstabilidad.text = $"Estabilidad Caldero (Niv {datos.nivelEstabilidad})\n[ Coste: {proximoCosto} Monedas ]";
        }

        if (datos.nivelBonoMonedas >= 2) {
            textoBonoComercio.text = "Bono Comercio: MÁX (Nivel 2)";
        } else {
            int proximoCosto = costosBono[datos.nivelBonoMonedas];
            textoBonoComercio.text = $"Bono Comercio (Niv {datos.nivelBonoMonedas})\n[ Coste: {proximoCosto} Monedas ]";
        }

        if (datos.tieneImanEsencia) {
            textoImanEsencia.text = "Imán de Esencia: ADQUIRIDO";
        } else {
            textoImanEsencia.text = $"Imán de Esencia (VIP)\n[ Coste: {costoImanEsencia} Monedas ]";
        }

        Cauldron caldero = FindObjectOfType<Cauldron>();
        if (caldero != null)
        {
            textoStatusCaldero.text = $"ESTADO DEL CALDERO\nErrores: {datos.erroresAcumulados} / {caldero.maxErroresPermitidos}";
            
            if (datos.erroresAcumulados == 0) {
                textoBorrador.text = "Caldero Limpio\nNo requiere reparación";
            } else {
                textoBorrador.text = $"Remover Errores\n[ Coste: {costoBorrador} Monedas ]";
            }
        }
    }

    private bool EsJornadaActiva()
    {
        DayNightCycle reloj = FindObjectOfType<DayNightCycle>();
        if (reloj != null && reloj.jornadaActiva)
        {
            Debug.Log("<color=yellow>[TIENDA]</color> Intento de compra bloqueado: La tienda está cerrada.");
            if (SFXManager.Instance != null) {
                SFXManager.Instance.PlaySFX(SFXManager.Instance.errorIngrediente, 0.5f);
            }
            return true;
        }
        return false; 
    }

    // ==========================================
    // FUNCIONES DE BOTONES CON CHIVATOS DE VR
    // ==========================================

    public void ComprarEstabilidad()
    {
        Debug.Log("<color=magenta>[VR CLICK]</color> Botón presionado: COMPRAR ESTABILIDAD");
        
        if (EsJornadaActiva()) return;
        var datos = SaveManager.Instance.datosActuales;
        if (datos.nivelEstabilidad >= 3) return; 

        int coste = costosEstabilidad[datos.nivelEstabilidad];
        if (GameManager.Instance.dineroTotal >= coste)
        {
            GameManager.Instance.restarDinero(coste);
            datos.nivelEstabilidad++;
            datos.monedasTotales = GameManager.Instance.dineroTotal;
            SaveManager.Instance.GuardarProgreso();

            Cauldron caldero = FindObjectOfType<Cauldron>();
            if (caldero != null) caldero.ResetearCaldero();

            SFXManager.Instance.PlaySFX(SFXManager.Instance.aciertoReceta, 1f);
            ActualizarInterfazTienda();
        }
        else {
            Debug.Log("<color=orange>[TIENDA]</color> No tienes dinero suficiente para Estabilidad.");
            SFXManager.Instance.PlaySFX(SFXManager.Instance.errorIngrediente, 0.5f);
        }
    }

    public void ComprarBorradorErrores()
    {
        Debug.Log("<color=magenta>[VR CLICK]</color> Botón presionado: BORRAR ERRORES");

        if (EsJornadaActiva()) return;
        var datos = SaveManager.Instance.datosActuales;
        if (datos.erroresAcumulados == 0) {
            Debug.Log("<color=cyan>[TIENDA]</color> El caldero ya está limpio.");
            return; 
        }

        if (GameManager.Instance.dineroTotal >= costoBorrador)
        {
            GameManager.Instance.restarDinero(costoBorrador);
            datos.erroresAcumulados = 0;
            datos.monedasTotales = GameManager.Instance.dineroTotal;
            SaveManager.Instance.GuardarProgreso();

            Cauldron caldero = FindObjectOfType<Cauldron>();
            if (caldero != null) caldero.ResetearCaldero();

            SFXManager.Instance.PlaySFX(SFXManager.Instance.aciertoReceta, 1f); 
            ActualizarInterfazTienda();
        }
        else {
            Debug.Log("<color=orange>[TIENDA]</color> No tienes dinero suficiente para el Borrador.");
            SFXManager.Instance.PlaySFX(SFXManager.Instance.errorIngrediente, 0.5f);
        }
    }

    public void ComprarBonoComercio()
    {
        Debug.Log("<color=magenta>[VR CLICK]</color> Botón presionado: BONO DE COMERCIO");

        if (EsJornadaActiva()) return;
        var datos = SaveManager.Instance.datosActuales;
        if (datos.nivelBonoMonedas >= 2) return;

        int coste = costosBono[datos.nivelBonoMonedas];
        if (GameManager.Instance.dineroTotal >= coste)
        {
            GameManager.Instance.restarDinero(coste);
            datos.nivelBonoMonedas++;
            datos.monedasTotales = GameManager.Instance.dineroTotal;
            SaveManager.Instance.GuardarProgreso();

            SFXManager.Instance.PlaySFX(SFXManager.Instance.aciertoReceta, 1f);
            ActualizarInterfazTienda();
        }
        else {
            Debug.Log("<color=orange>[TIENDA]</color> No tienes dinero suficiente para el Bono.");
            SFXManager.Instance.PlaySFX(SFXManager.Instance.errorIngrediente, 0.5f);
        }
    }

    public void ComprarImanEsencia()
    {
        Debug.Log("<color=magenta>[VR CLICK]</color> Botón presionado: IMÁN DE ESENCIA");

        if (EsJornadaActiva()) return;
        var datos = SaveManager.Instance.datosActuales;
        if (datos.tieneImanEsencia) return;

        if (GameManager.Instance.dineroTotal >= costoImanEsencia)
        {
            GameManager.Instance.restarDinero(costoImanEsencia);
            datos.tieneImanEsencia = true;
            datos.monedasTotales = GameManager.Instance.dineroTotal;
            SaveManager.Instance.GuardarProgreso();

            SFXManager.Instance.PlaySFX(SFXManager.Instance.aciertoReceta, 1f);
            ActualizarInterfazTienda();
        }
        else {
            Debug.Log("<color=orange>[TIENDA]</color> No tienes dinero suficiente para el Imán.");
            SFXManager.Instance.PlaySFX(SFXManager.Instance.errorIngrediente, 0.5f);
        }
    }
}