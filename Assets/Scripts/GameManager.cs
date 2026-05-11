using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    // Instancia estática para acceder desde otros scripts si es necesario
    public static GameManager Instance { get; private set; }

    [Header("Base de Datos de Alquimia")]
    public List<RecipeData> todasLasRecetas; 
    public RecipeData recetaObjetivoActual;

    [Header("Configuración de Rendimiento")]
    [SerializeField] private int targetFrameRate = 72;
    [SerializeField] private bool disableVSync = true;
    public GameObject monedaPrefab;
    public Transform puntoAparicionPlato;
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

    public bool ValidarIngrediente(IngredientData ingredienteCayo, List<IngredientData> listaCaldero)
    {
        // 1. ¿El ingrediente es parte de la receta activa?
        bool esParte = recetaObjetivoActual.ingredientesRequeridos.Contains(ingredienteCayo);

        if (!esParte)
        {
            // LOG DE ERROR: En rojo y negrita
            Debug.Log($"<b><color=#FF0000>[ALQUIMIA]</color></b> ¡ERROR! El ingrediente <b>{ingredienteCayo.nombreIngrediente}</b> no pertenece a la receta <i>{recetaObjetivoActual.nombrePocion}</i>.");
            return false; 
        }

        // 2. ¿Ya tenemos este ingrediente en el caldero? (Opcional, por si no quieres repetidos)
        if (listaCaldero.Contains(ingredienteCayo))
        {
            Debug.Log($"<b><color=#FFFF00>[ALQUIMIA]</color></b> El ingrediente <b>{ingredienteCayo.nombreIngrediente}</b> ya está en el caldero. (Duplicado)");
        }

        // LOG DE ACIERTO: En verde
        Debug.Log($"<b><color=#00FF00>[ALQUIMIA]</color></b> ¡BIEN! <b>{ingredienteCayo.nombreIngrediente}</b> añadido correctamente a la mezcla.");
        
        VerificarRecetaCompletada(listaCaldero, ingredienteCayo);
        return true;
    }

    private void VerificarRecetaCompletada(List<IngredientData> listaCaldero, IngredientData ultimoIngrediente)
    {
        List<IngredientData> listaPrueba = new List<IngredientData>(listaCaldero);
        listaPrueba.Add(ultimoIngrediente);

        var requeridos = recetaObjetivoActual.ingredientesRequeridos.OrderBy(i => i.name);
        var actuales = listaPrueba.OrderBy(i => i.name);

        if (requeridos.SequenceEqual(actuales))
        {
            string colorHex = ColorUtility.ToHtmlStringRGB(recetaObjetivoActual.colorFinal);
            Debug.Log($"<b><color=#{colorHex}>[SISTEMA]</color></b> ¡POCIÓN FINALIZADA! Has creado <b>{recetaObjetivoActual.nombrePocion}</b>.");

            BottleFiller filler = FindObjectOfType<BottleFiller>();
            if (filler != null) 
            {
                filler.HabilitarLlenado();
            }
            else 
            {
                Debug.LogError("No se encontró el script BottleFiller en la escena.");
            }

            Cauldron caldero = FindObjectOfType<Cauldron>();
            if (caldero != null)
            {
                caldero.CambiarColorFinal(recetaObjetivoActual.colorFinal);
            }
        }
    }


    [Header("Economía e Inventario")]
    public int dineroTotal = 0;
    public List<GameObject> monedasEnEscena = new List<GameObject>();

    // Reemplaza tu función EntregarPremio por esta Corrutina
    public IEnumerator SpawnMonedasConIntervalo(int cantidad)
    {
        for (int i = 0; i < cantidad; i++)
        {
            Vector3 desfase = new Vector3(Random.Range(-0.05f, 0.05f), 0.02f, Random.Range(-0.05f, 0.05f));
            GameObject nuevaMoneda = Instantiate(monedaPrefab, puntoAparicionPlato.position + desfase, Quaternion.identity);
            
            // La añadimos a nuestra lista de control
            monedasEnEscena.Add(nuevaMoneda);
            
            // Esperamos 0.1 segundos entre cada moneda para que los colliders no exploten
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void NotificarMonedaRecogida(GameObject moneda)
    {
        if (monedasEnEscena.Contains(moneda))
        {
            monedasEnEscena.Remove(moneda);
        }
    }

    // Esta es la función que usaremos para bloquear el progreso
    public bool HayMonedasPendientes()
    {
        Debug.Log($"Monedas restantes en lista: {monedasEnEscena.Count}"); // <-- Añade esto
        return monedasEnEscena.Count > 0;
    }

    public void SumarDinero(int cantidad)
    {
        dineroTotal += cantidad;
        Debug.Log($"<b><color=#FFD700>[BILLETERA]</color></b> +{cantidad}. Total: {dineroTotal}");
    }

    public void restarDinero(int cantidad)
    {
        dineroTotal -= cantidad;
        Debug.Log($"<b><color=#FFD700>[BILLETERA]</color></b> -{cantidad}. Total: {dineroTotal}");
    }
}