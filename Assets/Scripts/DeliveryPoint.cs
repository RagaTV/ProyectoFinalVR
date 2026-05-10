using UnityEngine;

public class DeliveryPoint : MonoBehaviour
{
    public BottleFiller fillerScript; 
    public GameObject efectoEntrega; 

    public void EntregarPedido()
    {
        if (fillerScript.socketEntrega.hasSelection)
        {
            Debug.Log("<color=green>[ENTREGA]</color> Pedido entregado con éxito.");

            GameObject botella = fillerScript.socketEntrega.interactablesSelected[0].transform.gameObject;
            
            // Opcional: Crear partículas de "humo" o éxito
            if(efectoEntrega != null) Instantiate(efectoEntrega, botella.transform.position, Quaternion.identity);

            Destroy(botella); 

            fillerScript.socketEntrega.socketActive = false; 

            FindObjectOfType<BoardController>().AsignarNuevaMision();
            
        }
        else
        {
            Debug.Log("<color=yellow>[SISTEMA]</color> No hay nada que entregar aún.");
        }
    }

}