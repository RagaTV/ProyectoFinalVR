using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cauldron : MonoBehaviour
{
    [Header("Memoria de la Poción")]
    public List<IngredientData> ingredientesEnElLíquido = new List<IngredientData>();

    [Header("Referencias Visuales")]
    public MeshRenderer liquidoRenderer;
    public Transform modeloCaldero; // Arrastra aquí el objeto del Caldero para que tiemble

    private bool estaTemblando = false;

    private void OnTriggerEnter(Collider other)
    {
        Ingredient ingredienteCayo = other.GetComponent<Ingredient>();

        if (ingredienteCayo != null && ingredienteCayo.data != null)
        {
            Debug.Log($"<b><color=#6495ED>[CALDERO]</color></b> Detectado: {ingredienteCayo.data.nombreIngrediente}. Validando con GameManager...");
            
            bool esCorrecto = GameManager.Instance.ValidarIngrediente(ingredienteCayo.data, ingredientesEnElLíquido);

            if (esCorrecto)
            {
                ingredientesEnElLíquido.Add(ingredienteCayo.data);
                StartCoroutine(EfectoDisolver(ingredienteCayo.gameObject, true));
            }
            else
            {
                Debug.Log($"<b><color=#FF4500>[CALDERO]</color></b> Rechazando ingrediente incorrecto. Iniciando secuencia de temblor.");
                if (!estaTemblando) StartCoroutine(EfectoTemblor());
                StartCoroutine(EfectoDisolver(ingredienteCayo.gameObject, false));
            }
        }
    }

    // El booleano 'exito' sirve para decidir si cambiamos el color o no
    IEnumerator EfectoDisolver(GameObject obj, bool exito)
    {
        // 1. Congelar el objeto en el líquido
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null) { rb.isKinematic = true; }
        obj.GetComponent<Collider>().enabled = false;

        // 2. Encoger el objeto
        Vector3 escalaInicial = obj.transform.localScale;
        float duracion = 0.8f;
        float t = 0;

        while (t < duracion)
        {
            t += Time.deltaTime;
            obj.transform.localScale = Vector3.Lerp(escalaInicial, Vector3.zero, t / duracion);
            yield return null;
        }

        // 3. Si fue un ingrediente exitoso y completó la receta, el GameManager se encarga del color final
        // (Por ahora el Destroy es suficiente)
        Destroy(obj);
    }

    IEnumerator EfectoTemblor()
    {
        estaTemblando = true;
        Vector3 posOriginal = modeloCaldero.localPosition;
        float duracion = 0.5f;
        float fuerza = 0.05f;
        float t = 0;

        while (t < duracion)
        {
            t += Time.deltaTime;
            // Movimiento aleatorio rápido para simular vibración
            modeloCaldero.localPosition = posOriginal + (Random.insideUnitSphere * fuerza);
            yield return null;
        }

        modeloCaldero.localPosition = posOriginal;
        estaTemblando = false;
    }

    // Dentro de Cauldron.cs
    public void CambiarColorFinal(Color nuevoColor)
    {
        if (liquidoRenderer != null)
        {
            // Usamos una Corrutina para que el cambio de color sea suave y no de golpe
            StartCoroutine(TransicionColor(nuevoColor));
        }
    }

    IEnumerator TransicionColor(Color objetivo)
    {
        Material mat = liquidoRenderer.material;
        Color inicial = mat.color;
        float duracion = 2.0f; // 2 segundos de transición
        float t = 0;

        while (t < duracion)
        {
            t += Time.deltaTime;
            // Mezclamos del color actual al objetivo respetando el Alpha que ya tenías
            Color colorResultante = Color.Lerp(inicial, objetivo, t / duracion);
            colorResultante.a = inicial.a; 
            mat.color = colorResultante;
            yield return null;
        }
    }
}