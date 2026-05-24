using UnityEngine;
using System.Collections.Generic;

public enum DificultadReceta {
    Basica_2_Ingredientes,
    Intermedia_3_Ingredientes,
    Avanzada_4_Ingredientes
}

[CreateAssetMenu(fileName = "NuevaReceta", menuName = "Alquimia/Receta")]
public class RecipeData : ScriptableObject
{
    [Header("Información Básica")]
    public string nombrePocion;
    public Color colorFinal = Color.green; 

    [Header("Ingredientes Necesarios")]
    [Tooltip("Arrastra aquí los IngredientData que forman esta receta")]
    public List<IngredientData> ingredientesRequeridos;

    [Header("Textos para la Pizarra / Cliente")]
    [TextArea(2, 3)] 
    public string descripcionClara;
    
    [TextArea(2, 3)] 
    public string descripcionAmbigua;
    
    [TextArea(2, 3)] 
    public string descripcionConfusa;
    [Header("Economía")]
    public int recompensaMonedas = 1;
    
    [Header("Configuración de Progreso")]
    public DificultadReceta clasificacion;
}