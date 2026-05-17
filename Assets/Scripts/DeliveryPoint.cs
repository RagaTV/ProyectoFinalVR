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
        if (fillerScript != null && fillerScript.socketEntrega.hasSelection)
        {
            Debug.Log("<color=green>[ENTREGA]</color> Procesando entrega...");

            if (parteRoja != null) StartCoroutine(AnimacionBoton());

            IXRSelectInteractable botellaInteractable = fillerScript.socketEntrega.interactablesSelected[0];
            GameObject botella = botellaInteractable.transform.gameObject;

            if (efectoEntrega != null) Instantiate(efectoEntrega, botella.transform.position, Quaternion.identity);
            
            Destroy(botella);
            GameManager.Instance.misionEnProgreso = false;
            int recompensa = GameManager.Instance.recetaObjetivoActual.recompensaMonedas;
            GameManager.Instance.StartCoroutine(GameManager.Instance.SpawnMonedasConIntervalo(recompensa));

            fillerScript.socketEntrega.socketActive = false;
        }
        else
        {
            Debug.Log("<color=yellow>[SISTEMA]</color> No hay ninguna botella lista para entregar.");
        }
    }

    IEnumerator AnimacionBoton()
    {
        Vector3 posOriginal = parteRoja.transform.localPosition;
        parteRoja.transform.localPosition = posOriginal + new Vector3(0, -0.02f, 0); 
        yield return new WaitForSeconds(0.15f);
        parteRoja.transform.localPosition = posOriginal;
    }
}