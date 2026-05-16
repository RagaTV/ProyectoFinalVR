using UnityEngine;

public class SueloLimpiador : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Ingredient basura = other.GetComponent<Ingredient>();
        
        if (basura != null)
        {
            Destroy(other.gameObject);
            Debug.Log("<color=grey>[LIMPIEZA]</color> Objeto caído al piso destruido para liberar RAM.");
        }
    }
}