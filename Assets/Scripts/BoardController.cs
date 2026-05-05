using UnityEngine;
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

    void Start()
    {
        // Esto llamará a la función automáticamente al empezar
        AsignarNuevaMision();
    }

    public void AsignarNuevaMision()
    {
        clientesAtendidosHoy++;
        var recetas = GameManager.Instance.todasLasRecetas;
        RecipeData recetaElegida = recetas[Random.Range(0, recetas.Count)];
        int dificultadCalculada = recetaElegida.ingredientesRequeridos.Count;

        // 1. Lógica del nombre (¿Ya está hecha o comprada?)
        // Por ahora lo ponemos con ??? como dijiste
        textoNombre.text = "Receta: ???"; 

        // 2. Lógica de descripción según dificultad (usando tus campos del ScriptableObject)
        textoDescripcion.text = elegirDescripcion(recetaElegida);

        // 3. Mostrar recompensa y cliente
        int monedas = calcularMonedas(dificultadCalculada);
        textoRecompensa.text = "+" + monedas;
        textoNumCliente.text = "Cliente: #" + clientesAtendidosHoy.ToString("00");

        // 4. Actualizar estrellas
        actualizarEstrellas(dificultadCalculada);
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

    void actualizarEstrellas(int cantIngredientes) {
        int numEstrellas = cantIngredientes - 1; 

        for (int i = 0; i < estrellasDificultad.Length; i++) {
            estrellasDificultad[i].SetActive(i < numEstrellas);
        }
    }
}