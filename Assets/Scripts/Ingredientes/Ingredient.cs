using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit; // Necesario para detectar la mano

public class Ingredient : MonoBehaviour
{
    [Header("Configuración de Datos")]
    public IngredientData data;

    private Coroutine rutinaDestruccion;
    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(AlSerAgarrado);
        }
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(AlSerAgarrado);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            if (rutinaDestruccion == null) 
            {
                rutinaDestruccion = StartCoroutine(CuentaRegresivaDestruccion());
                Debug.Log($"<color=grey>[LIMPIEZA]</color> {data.nombreIngrediente} cayó al suelo. Iniciando cuenta regresiva de 5s...");
            }
        }
    }

    private void AlSerAgarrado(SelectEnterEventArgs args)
    {
        if (rutinaDestruccion != null)
        {
            StopCoroutine(rutinaDestruccion);
            rutinaDestruccion = null;
            Debug.Log($"<color=green>[LIMPIEZA]</color> {data.nombreIngrediente} fue salvado del suelo.");
        }
    }

    private IEnumerator CuentaRegresivaDestruccion()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}