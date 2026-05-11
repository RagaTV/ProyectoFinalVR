using UnityEngine;
using System.Collections.Generic;

// Esto nos crea el atajo en el menú de click derecho de Unity
[CreateAssetMenu(fileName = "NuevaReceta", menuName = "Alquimia/Receta")]
public class RecipeData : ScriptableObject
{
    [Header("Información Básica")]
    public string nombrePocion;
    public Color colorFinal = Color.green; // El color que tomará el agua al triunfar

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
}