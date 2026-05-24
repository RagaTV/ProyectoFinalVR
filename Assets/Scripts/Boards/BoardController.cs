using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BoardController : MonoBehaviour
{
    [Header("Referencias de UI")]
    public TextMeshProUGUI textoNombre;
    public TextMeshProUGUI textoDescripcion;
    public TextMeshProUGUI textoRecompensa;
    public TextMeshProUGUI textoNumCliente;
    public GameObject[] estrellasDificultad; // 3 objetos de estrella
    
    private int clientesAtendidosHoy = 0;
    [Header("Transform de Sonido")]
    public GameObject objetoGato; 
    void Start()
    {
        LimpiarPizarra();
    }

    public void LimpiarPizarra()
    {
        textoNombre.text = "Esperando jornada...";
        textoDescripcion.text = "Presiona el botón de Iniciar Día para recibir clientes.";
        textoRecompensa.text = "+0";
        textoNumCliente.text = "Cliente: #00";

        for (int i = 0; i < estrellasDificultad.Length; i++)
        {
            if (estrellasDificultad[i] != null)
                estrellasDificultad[i].SetActive(false);
        }
    }

    public void ResetearContadorClientes()
    {
        clientesAtendidosHoy = 0;
    }

    public void AsignarNuevaMision()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.recetaCompletada = false; 
        }
        
        SFXManager.Instance.PlaySFXAtPosition(SFXManager.Instance.maullidoGato, objetoGato.transform.position, 1f);
        clientesAtendidosHoy++;

        int diaHoy = 1; 
        if (SaveManager.Instance != null && SaveManager.Instance.datosActuales != null) {
            diaHoy = SaveManager.Instance.datosActuales.diaActual;
        }

        var todasLasRecetas = GameManager.Instance.todasLasRecetas;
        List<RecipeData> recetasPermitidas = new List<RecipeData>();

        foreach (RecipeData receta in todasLasRecetas) 
        {
            if (diaHoy <= 2) 
            {
                if (receta.clasificacion == DificultadReceta.Basica_2_Ingredientes)
                    recetasPermitidas.Add(receta);
            }
            else if (diaHoy <= 4) 
            {
                if (receta.clasificacion == DificultadReceta.Basica_2_Ingredientes || 
                    receta.clasificacion == DificultadReceta.Intermedia_3_Ingredientes)
                    recetasPermitidas.Add(receta);
            }
            else 
            {
                recetasPermitidas.Add(receta);
            }
        }

        if (recetasPermitidas.Count == 0) {
            Debug.LogWarning("<color=red>[BOARD]</color> Error de filtro: Lista vacía. Usando todas las recetas.");
            recetasPermitidas = todasLasRecetas;
        }

        RecipeData recetaElegida = recetasPermitidas[Random.Range(0, recetasPermitidas.Count)];

        int dificultadCalculada = recetaElegida.ingredientesRequeridos.Count;
        GameManager.Instance.recetaObjetivoActual = recetaElegida;
        textoNombre.text = "Receta: " + recetaElegida.nombrePocion;

        int suerte = Random.Range(0, 3);

        if (diaHoy <= 2)
        {
            suerte = 0;
        }

        Color colorEstrellas = Color.white;

        switch (suerte)
        {
            case 0: 
                Debug.Log("Pizarra: Mostrando descripción CLARA. Estrellas BLANCAS.");
                textoDescripcion.text = recetaElegida.descripcionClara;
                colorEstrellas = Color.white;
                break;
            case 1: // AMBIGUA
                Debug.Log("Pizarra: Mostrando descripción AMBIGUA. Estrellas AZULES.");
                textoDescripcion.text = recetaElegida.descripcionAmbigua;
                colorEstrellas = new Color(0f, 0.7f, 1f); 
                break;
            case 2: // CONFUSA
                Debug.Log("Pizarra: Mostrando descripción CONFUSA. Estrellas AMARILLAS.");
                textoDescripcion.text = recetaElegida.descripcionConfusa;
                colorEstrellas = Color.yellow; 
                break;
            default:
                textoDescripcion.text = recetaElegida.descripcionClara;
                colorEstrellas = Color.white;
                break;
        }

        int monedas = calcularMonedas(dificultadCalculada);
        bool esClienteVIP = false;

        if (SaveManager.Instance != null && SaveManager.Instance.datosActuales != null)
        {
            if (SaveManager.Instance.datosActuales.tieneImanEsencia)
            {
                if (Random.value <= 0.15f)
                {
                    esClienteVIP = true;
                    monedas *= 2;
                    Debug.Log("<color=yellow>[TIENDA]</color> ¡Imán de Esencia activado! Ha llegado un Cliente VIP.");
                }
            }
        }

        recetaElegida.recompensaMonedas = monedas;
        if (esClienteVIP) {
            textoRecompensa.text = "+" + monedas + " VIP";
        } else {
            textoRecompensa.text = "+" + monedas;
        }
        textoNumCliente.text = "Cliente: #" + clientesAtendidosHoy.ToString("00");

        actualizarEstrellas(dificultadCalculada, colorEstrellas);
    }

    public void SolicitarNuevaMision() 
    {
        DayNightCycle reloj = FindObjectOfType<DayNightCycle>();
        if (reloj == null || !reloj.jornadaActiva)
        {
            Debug.Log("<color=yellow>[INTERFAZ]</color> El local está cerrado. Inicia el día primero.");
            if (SFXManager.Instance != null) {
                SFXManager.Instance.PlaySFX(SFXManager.Instance.errorIngrediente, 0.5f);
            }
            return; 
        }

        if (GameManager.Instance.misionEnProgreso)
        {
            Debug.Log("<color=red>[BOARD]</color> Ya tienes una receta activa. ¡Termínala primero!");
            if (SFXManager.Instance != null) {
                SFXManager.Instance.PlaySFX(SFXManager.Instance.errorIngrediente, 0.5f);
            }
            return;
        }

        if (GameManager.Instance.HayMonedasPendientes())
        {
            Debug.Log("<color=orange>[BOARD]</color> Recoge tu pago antes de aceptar otro cliente.");
            if (SFXManager.Instance != null) {
                SFXManager.Instance.PlaySFX(SFXManager.Instance.errorIngrediente, 0.5f);
            }
            return;
        }

        GameManager.Instance.misionEnProgreso = true; 
        AsignarNuevaMision();
    }

    int calcularMonedas(int cantIngredientes) 
    {
        int monedasBase = 1;

        if (cantIngredientes == 2) monedasBase = 2;
        else if (cantIngredientes == 3) monedasBase = 3;
        else if (cantIngredientes >= 4) monedasBase = 5;

        if (SaveManager.Instance != null && SaveManager.Instance.datosActuales != null)
        {
            int nivelBono = SaveManager.Instance.datosActuales.nivelBonoMonedas;

            if (nivelBono == 1)      monedasBase += 1;
            else if (nivelBono == 2) monedasBase += 2;
        }

        return monedasBase;
    }

    void actualizarEstrellas(int cantIngredientes, Color colorElegido) {
        int numEstrellas = cantIngredientes - 1; 

        for (int i = 0; i < estrellasDificultad.Length; i++) {
            if (estrellasDificultad[i] != null) {
                estrellasDificultad[i].SetActive(i < numEstrellas);

                if (i < numEstrellas) {
                    Image imgEstrella = estrellasDificultad[i].GetComponent<Image>();
                    if (imgEstrella != null) {
                        imgEstrella.color = colorElegido;
                    }
                }
            }
        }
    }
}