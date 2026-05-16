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

    void Start()
    {
        SFXManager.Instance.PlayCalderoLoop(transform.position, 0.4f);
    }

    private void OnTriggerEnter(Collider other)
    {
        Ingredient ingredienteCayo = other.GetComponent<Ingredient>();
        SFXManager.Instance.PlaySFXAtPosition(SFXManager.Instance.ingredienteAlCaldero, transform.position, 1f);

        if (ingredienteCayo != null && ingredienteCayo.data != null)
        {
            Debug.Log($"<b><color=#6495ED>[CALDERO]</color></b> Detectado: {ingredienteCayo.data.nombreIngrediente}. Validando con GameManager...");
            
            bool esCorrecto = GameManager.Instance.ValidarIngrediente(ingredienteCayo.data, ingredientesEnElLíquido);

            if (esCorrecto)
            {
                ingredientesEnElLíquido.Add(ingredienteCayo.data);
                SFXManager.Instance.PlaySFXAtPosition(SFXManager.Instance.aciertoReceta, transform.position, 1f);
                StartCoroutine(EfectoDisolver(ingredienteCayo.gameObject, true));
            }
            else
            {
                Debug.Log($"<b><color=#FF4500>[CALDERO]</color></b> Rechazando ingrediente incorrecto. Iniciando secuencia de temblor.");
                SFXManager.Instance.PlaySFXAtPosition(SFXManager.Instance.errorIngrediente, transform.position, 1f);
                if (!estaTemblando) StartCoroutine(EfectoTemblor());
                StartCoroutine(EfectoDisolver(ingredienteCayo.gameObject, false));
            }
        }
    }

    IEnumerator EfectoDisolver(GameObject obj, bool exito)
    {
        yield return new WaitForSeconds(0.15f);
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null) { rb.isKinematic = true; }
        obj.GetComponent<Collider>().enabled = false;

        Vector3 escalaInicial = obj.transform.localScale;
        float duracion = 0.8f;
        float t = 0;

        while (t < duracion)
        {
            t += Time.deltaTime;
            obj.transform.localScale = Vector3.Lerp(escalaInicial, Vector3.zero, t / duracion);
            yield return null;
        }

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
            modeloCaldero.localPosition = posOriginal + (Random.insideUnitSphere * fuerza);
            yield return null;
        }

        modeloCaldero.localPosition = posOriginal;
        estaTemblando = false;
    }

    /*public void ExplotarCaldero()
    {
        // Sonido de explosión 3D justo en el caldero
        SFXManager.Instance.PlaySFXAtPosition(SFXManager.Instance.calderoExplota, transform.position, 0.8f);
        
        // Aquí continuaría tu código para ocultar el caldero o poner humo...
    }*/

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