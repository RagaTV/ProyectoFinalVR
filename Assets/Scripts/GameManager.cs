using UnityEngine;
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
        // Creamos una copia de la lista actual + el último que acaba de caer para la comparación
        List<IngredientData> listaPrueba = new List<IngredientData>(listaCaldero);
        listaPrueba.Add(ultimoIngrediente);

        var requeridos = recetaObjetivoActual.ingredientesRequeridos.OrderBy(i => i.name);
        var actuales = listaPrueba.OrderBy(i => i.name);

        if (requeridos.SequenceEqual(actuales))
        {
            // LOG DE VICTORIA: Con el color final de la poción
            string colorHex = ColorUtility.ToHtmlStringRGB(recetaObjetivoActual.colorFinal);
            Debug.Log($"<b><color=#{colorHex}>[SISTEMA]</color></b> ¡POCIÓN FINALIZADA! Has creado una <b>{recetaObjetivoActual.nombrePocion}</b> con éxito.");
        }

        if (requeridos.SequenceEqual(actuales))
        {
            string colorHex = ColorUtility.ToHtmlStringRGB(recetaObjetivoActual.colorFinal);
            Debug.Log($"<b><color=#{colorHex}>[SISTEMA]</color></b> ¡POCIÓN FINALIZADA! Has creado una <b>{recetaObjetivoActual.nombrePocion}</b> con éxito.");

            // Buscamos el caldero en la escena y le mandamos el color
            FindObjectOfType<Cauldron>().CambiarColorFinal(recetaObjetivoActual.colorFinal);
        }
    }
}