using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class IngredientDispenser : MonoBehaviour
{
    [Header("Configuración del Estante")]
    public GameObject prefabIngrediente; // Arrastra aquí el Prefab del hongo/flor
    public float tiempoDeReparicion = 2f;

    private GameObject ingredienteActual;

    void Start()
    {
        AparecerIngrediente();
    }

    void AparecerIngrediente()
    {
        ingredienteActual = Instantiate(prefabIngrediente, transform.position, transform.rotation);
        
        XRGrabInteractable grab = ingredienteActual.GetComponent<XRGrabInteractable>();
        if (grab != null)
        {
            grab.selectEntered.AddListener(AlSerAgarrado);
        }
    }

    void AlSerAgarrado(SelectEnterEventArgs args)
    {
        // 1. Nos desuscribimos para no crear errores
        XRGrabInteractable grab = ingredienteActual.GetComponent<XRGrabInteractable>();
        if (grab != null)
        {
            grab.selectEntered.RemoveListener(AlSerAgarrado);
        }

        // 2. Olvidamos el ingrediente. Ahora le pertenece a la mano del jugador o al caldero.
        ingredienteActual = null;

        // 3. Iniciamos la cuenta regresiva para crear uno nuevo en el estante
        StartCoroutine(RutinaReparicion());
    }

    IEnumerator RutinaReparicion()
    {
        yield return new WaitForSeconds(tiempoDeReparicion);
        AparecerIngrediente();
    }
}