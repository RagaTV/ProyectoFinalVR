using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cauldron : MonoBehaviour
{
    [Header("Memoria de la Poción")]
    public List<IngredientData> ingredientesEnElLíquido = new List<IngredientData>();

    [Header("Resistencia y Errores")]
    public int maxErroresPermitidos = 2; 
    public int erroresCometidos = 0;
    private bool calderoDestruido = false; 

    [Header("Referencias Visuales")]
    public MeshRenderer liquidoRenderer;
    public Transform modeloCaldero; 

    private bool estaTemblando = false;
    [Header("Efectos Visuales")]
    public GameObject prefabExplosionVFX;

    void Start()
    {
        SFXManager.Instance.PlayCalderoLoop(transform.position, 0.4f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (calderoDestruido) return;

        Ingredient ingredienteCayo = other.GetComponent<Ingredient>();
        
        if (ingredienteCayo != null && ingredienteCayo.data != null)
        {
            SFXManager.Instance.PlaySFXAtPosition(SFXManager.Instance.ingredienteAlCaldero, transform.position, 1f);
            
            if (!GameManager.Instance.misionEnProgreso)
            {
                Debug.Log($"<b><color=#FFD700>[CALDERO]</color></b> Jornada inactiva. Ingrediente desperdiciado, pero sin castigo de vida.");
                
                SFXManager.Instance.PlaySFXAtPosition(SFXManager.Instance.errorIngrediente, transform.position, 1f);
                if (!estaTemblando) StartCoroutine(EfectoTemblor());
                StartCoroutine(EfectoDisolver(ingredienteCayo.gameObject, false));
                
                return; 
            }

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
                erroresCometidos++;
                Debug.Log($"<b><color=#FF4500>[CALDERO]</color></b> Error cometido: {erroresCometidos} / {maxErroresPermitidos}");

                if (SaveManager.Instance != null && SaveManager.Instance.datosActuales != null)
                {
                    SaveManager.Instance.datosActuales.erroresAcumulados = erroresCometidos;
                }

                StartCoroutine(EfectoDisolver(ingredienteCayo.gameObject, false));

                if (erroresCometidos > maxErroresPermitidos)
                {
                    ExplotarCaldero();
                }
                else
                {
                    SFXManager.Instance.PlaySFXAtPosition(SFXManager.Instance.errorIngrediente, transform.position, 1f);
                    if (!estaTemblando) StartCoroutine(EfectoTemblor());
                }
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
            if (obj == null) yield break; 
            
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

    public void ExplotarCaldero()
    {
        calderoDestruido = true;
        
        SFXManager.Instance.PlaySFXAtPosition(SFXManager.Instance.calderoExplota, transform.position, 1f);
        
        Debug.Log("<b><color=#8B0000>[GAME OVER]</color></b> ¡EL CALDERO HA EXPLOTADO!");

        // Aquí apagamos el líquido para simular que se derramó o evaporó
        if (liquidoRenderer != null)
        {
            liquidoRenderer.gameObject.SetActive(false);
        }

        if (prefabExplosionVFX != null)
        {
            // Creamos las partículas en la posición del caldero
            GameObject explosion = Instantiate(prefabExplosionVFX, transform.position, Quaternion.identity);
            
            Destroy(explosion, 3f); 
        }
        GameManager.Instance.ForzarFinDeJornada(); 
    }

    public void CambiarColorFinal(Color nuevoColor)
    {
        if (liquidoRenderer != null && !calderoDestruido)
        {
            nuevoColor.a = 0.5f;
            StartCoroutine(TransicionColor(nuevoColor));
        }
    }

    IEnumerator TransicionColor(Color objetivo)
    {
        Material mat = liquidoRenderer.material;
        Color inicial = mat.color;
        float duracion = 2.0f; 
        float t = 0;

        while (t < duracion)
        {
            t += Time.deltaTime;
            Color colorResultante = Color.Lerp(inicial, objetivo, t / duracion);
            colorResultante.a = inicial.a; 
            mat.color = colorResultante;
            yield return null;
        }
    }

    public void ResetearCaldero()
    {
        ingredientesEnElLíquido.Clear();
        calderoDestruido = false;

        maxErroresPermitidos = 2; 

        if (SaveManager.Instance != null && SaveManager.Instance.datosActuales != null)
        {
            int nivel = SaveManager.Instance.datosActuales.nivelEstabilidad;
            if (nivel == 1) maxErroresPermitidos = 3;
            else if (nivel == 2) maxErroresPermitidos = 4;
            else if (nivel == 3) maxErroresPermitidos = 5;
            
            erroresCometidos = SaveManager.Instance.datosActuales.erroresAcumulados;
        }
        
        if (liquidoRenderer != null)
        {
            liquidoRenderer.gameObject.SetActive(true);
            liquidoRenderer.material.color = new Color(0.5f, 0f, 0.5f, 0.8f);
        }
    }
}