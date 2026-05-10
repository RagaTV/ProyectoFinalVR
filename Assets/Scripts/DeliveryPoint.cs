using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class DeliveryPoint : MonoBehaviour
{
    public BottleFiller fillerScript; 
    public GameObject parteRoja; 
    public GameObject efectoEntrega; 

    public void EntregarPedido()
    {
        // 1. Verificamos si hay una botella en el socket de entrega
        if (fillerScript != null && fillerScript.socketEntrega.hasSelection)
        {
            Debug.Log("<color=green>[ENTREGA]</color> Procesando entrega...");

            if (parteRoja != null) StartCoroutine(AnimacionBoton());

            IXRSelectInteractable botellaInteractable = fillerScript.socketEntrega.interactablesSelected[0];
            GameObject botella = botellaInteractable.transform.gameObject;

            if (efectoEntrega != null) Instantiate(efectoEntrega, botella.transform.position, Quaternion.identity);
            
            Destroy(botella);

            fillerScript.socketEntrega.socketActive = false;
            
            BoardController pizarra = FindObjectOfType<BoardController>();
            if (pizarra != null) pizarra.AsignarNuevaMision();

        }
        else
        {
            Debug.Log("<color=yellow>[SISTEMA]</color> No hay ninguna botella lista para entregar.");
        }
    }

    IEnumerator AnimacionBoton()
    {
        Vector3 posOriginal = parteRoja.transform.localPosition;
        // Baja un poco en el eje Y (ajusta el -0.02f según el tamaño de tu botón)
        parteRoja.transform.localPosition = posOriginal + new Vector3(0, -0.02f, 0); 
        yield return new WaitForSeconds(0.15f);
        parteRoja.transform.localPosition = posOriginal;
    }
}