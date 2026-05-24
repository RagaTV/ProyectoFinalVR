using UnityEngine;

public class BotonEmergencia : MonoBehaviour
{
    public void ReiniciarInstanciaDia()
    {
        // 1. Reproducimos el sonido del botón
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySFX(SFXManager.Instance.errorIngrediente, 1f);
        }

        // 2. Le ordenamos al GameManager que haga el trabajo pesado de resetear los datos y la escena
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReiniciarJornadaPorEmergencia();
        }
        else
        {
            Debug.LogError("<color=red>[ERROR]</color> No se encontró el GameManager.");
        }
    }
}