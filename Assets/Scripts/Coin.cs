using UnityEngine;

public class Coin : MonoBehaviour
{
    public int valorMoneda = 1;
    
    public void RecogerConClic()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SumarDinero(valorMoneda);
            GameManager.Instance.NotificarMonedaRecogida(this.gameObject);
            Destroy(gameObject);
        }
    }
}