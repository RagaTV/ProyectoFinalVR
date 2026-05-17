using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        SFXManager.Instance.PlaySFXAtPosition(SFXManager.Instance.maullidoGato, objetoGato.transform.position, 1f);
        clientesAtendidosHoy++;

        var recetas = GameManager.Instance.todasLasRecetas;
        RecipeData recetaElegida = recetas[Random.Range(0, recetas.Count)];

        int dificultadCalculada = recetaElegida.ingredientesRequeridos.Count;
        GameManager.Instance.recetaObjetivoActual = recetaElegida;
        textoNombre.text = "Receta: " + recetaElegida.nombrePocion;

        int suerte = Random.Range(0, 3);
        Color colorEstrellas = Color.white;

        switch (suerte)
        {
            case 0: // CLARA / Normal
                Debug.Log("Pizarra: Mostrando descripción CLARA. Estrellas BLANCAS.");
                textoDescripcion.text = recetaElegida.descripcionClara;
                colorEstrellas = Color.white; // Blanco brillante estándar
                break;
            case 1: // AMBIGUA
                Debug.Log("Pizarra: Mostrando descripción AMBIGUA. Estrellas AZULES.");
                textoDescripcion.text = recetaElegida.descripcionAmbigua;
                colorEstrellas = new Color(0f, 0.7f, 1f); // Un Cian/Azul brillante muy visible
                break;
            case 2: // CONFUSA
                Debug.Log("Pizarra: Mostrando descripción CONFUSA. Estrellas AMARILLAS.");
                textoDescripcion.text = recetaElegida.descripcionConfusa;
                colorEstrellas = Color.yellow; // Amarillo intenso
                break;
            default:
                textoDescripcion.text = recetaElegida.descripcionClara;
                colorEstrellas = Color.white;
                break;
        }

        textoDescripcion.text = elegirDescripcion(recetaElegida);

        int monedas = calcularMonedas(dificultadCalculada);
        recetaElegida.recompensaMonedas = monedas;
        textoRecompensa.text = "+" + monedas;
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

    int calcularMonedas(int cantIngredientes) {
        if(cantIngredientes == 2) return 2;
        if(cantIngredientes == 3) return 3;
        if(cantIngredientes >= 4) return 5;
        return 1;
    }

    string elegirDescripcion(RecipeData r) 
    {
        // Generamos un número aleatorio entre 0 y 2
        int suerte = Random.Range(0, 3);

        switch (suerte)
        {
            case 0:
                Debug.Log("Pizarra: Mostrando descripción CLARA");
                return r.descripcionClara;
            case 1:
                Debug.Log("Pizarra: Mostrando descripción AMBIGUA");
                return r.descripcionAmbigua;
            case 2:
                Debug.Log("Pizarra: Mostrando descripción CONFUSA");
                return r.descripcionConfusa;
            default:
                return r.descripcionClara;
        }
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