using UnityEngine;

// Definimos las categorías exactas de tu jerarquía
public enum TipoIngrediente
{
    Planta,
    Hongo,
    Mineral,
    Hueso,
    Animal
}

[CreateAssetMenu(fileName = "NuevoIngrediente", menuName = "Alquimia/Ingrediente")]
public class IngredientData : ScriptableObject
{
    public string nombreIngrediente;
    
    // Ahora aparecerá como una lista desplegable en el Inspector
    public TipoIngrediente tipo; 

    [Header("Configuración de Color")]
    public Color colorPrincipal = Color.white;
}